using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using MusicCollection.Implementation;
using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;
using MusicCollection.WebServices;
using MusicCollection.Infra;

//DEM changes TR

namespace MusicCollection.FileConverter
{
    internal class CDImporter : ImporterConverterAbstract, IImporter
    {
        private char _Driver;
        private List<ITrackDescriptor> _TDs = new List<ITrackDescriptor>();
        private bool _OpenCDDoorOnComplete;
        private IMusicConverter _IMusicConverter;

        internal CDImporter(IMusicConverter iit, char Driver, bool iOpenCDDoorOnComplete)
        {
            _IMusicConverter = iit;
            _Driver = Driver;
            _OpenCDDoorOnComplete = iOpenCDDoorOnComplete;
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            int CDnumber = _IMusicConverter.GetDriverNumber(_Driver);

            if (!CDHelper.CanOpenCDDoor(CDnumber))
            {
                //ca pue un lecteur de cd qu'on ne peut pas ouvrir
                //je ne fair rien dans ce cas (cle 3G?)
                Trace.WriteLine("Problem due to device detected as CD driver");
                return null;
            }

            using (CDLocker cdl = CDLocker.GetLocker(CDnumber))
            {
                if (!cdl.IsOK)
                {
                    iel.Report(new NoCDInsertedArgs());
                    Trace.WriteLine("CD driver not ready");
                    return null;
                }

                if (!CDHelper.IsCDAudio(CDnumber))
                {
                    iel.Report(new NoCDAudioInsertedArgs());
                    System.Diagnostics.Trace.WriteLine("CD not audio");
                    return null;
                }

                iel.Report(new CDIndentifyingProgessEventArgs());

                CDInfoHandler cih = new CDInfoHandler(CDnumber);

                IEnumerable<MatchAlbum> albums = Context.FindFromHashes(cih.IDs.RawHash);
                if (albums.Any())
                {
                    OtherAlbumsConfirmationNeededEventArgs error = new OtherAlbumsConfirmationNeededEventArgs(albums);
                    iel.Report(error);

                    System.Diagnostics.Trace.WriteLine("CD potentially aleady imported");

                    if (!error.Continue)
                    {
                        System.Diagnostics.Trace.WriteLine("stopping import");
                        return null;
                    }
                }

                IWebQuery webq = Context.Session.WebQueryFactory.FromCDInfo(cih);
                webq.NeedCoverArt = false;
                IInternetFinder ifn = Context.Session.GetInternetFinder(webq);
                ifn.Compute(iCancellationToken,null);

                AmbigueousCDInformationArgs acfi = new AmbigueousCDInformationArgs(ifn.Result.Found, AlbumDescriptor.CreateBasicFromCD(cih, Context));

                iel.Report(acfi);

                if (!acfi.Continue)
                    return null;

                AlbumDescriptor ifad = acfi.SelectedInfo as AlbumDescriptor;

                iel.Report(new CDImportingProgessEventArgs(ifad.Name));

                ifad.MergeIDsFromCDInfos(cih);

                IMusicfilesConverter IMC = _IMusicConverter.GetCDMusicConverter(ifad, Context.Folders.File, false, CDnumber);

                if (IMC == null)
                {
                    TimeSpan ts = TimeSpan.FromSeconds(7);
                    int repeat = 0;
                    int Maxtent = 3;
                    while ((IMC == null) && (repeat < Maxtent))
                    {
                        bool Forcebute = repeat == (Maxtent - 1);
                        Thread.Sleep(ts);
                        IMC = _IMusicConverter.GetCDMusicConverter(ifad, Context.Folders.File, Forcebute, CDnumber);
                        repeat++;
                        Trace.WriteLine(string.Format("Trial {0} to get CDMusicConverter, Forcebute {1} success {2}", repeat, Forcebute, (IMC == null)));
                        ts = TimeSpan.FromSeconds(ts.TotalSeconds * 3);
                    }

                    if (IMC == null)
                    {
                        System.Diagnostics.Trace.WriteLine("no importer returned");
                        iel.Report(new CDInUse());
                        return null;
                    }
                }

                Action ParrallelCoverLoading = null;

                IList<WebMatch<IFullAlbumDescriptor>> resultwithimage = null;

                if (acfi.PreprocessedWebInfo != null)
                {
                    resultwithimage = acfi.PreprocessedWebInfo;
                    ParrallelCoverLoading = () => acfi.PreprocessedWebInfo.Apply(wr => wr.FindItem.LoadImages());
                }
                else
                {
                    IWebQuery webqim = new WebQueryFactory(Context.Session).FromAlbumDescriptor(ifad);
                    webqim.NeedCoverArt = true;

                    IInternetFinder ifni = Context.Session.GetInternetFinder(webqim);

                    resultwithimage = new List<WebMatch<IFullAlbumDescriptor>>();

                    if (ifad.HasImage())
                    {
                        resultwithimage.Add(new WebMatch<IFullAlbumDescriptor>(ifad, MatchPrecision.Suspition, acfi.Provider));
                    }

                    ParrallelCoverLoading = () => { ifad.LoadImages(); ifni.Compute(iCancellationToken,null); resultwithimage.AddCollection(ifni.Result.Found); };
                }

                int TN = ifad.TrackDescriptors.Count;

                IAsyncResult ias = ParrallelCoverLoading.BeginInvoke(null, null);

                bool feedbacknegative = false;

                IProgress<TrackConverted> progress = new SimpleProgress<TrackConverted>
                ( (e) =>
                    {
                        iel.Report(new ConvertProgessEventArgs(ifad.Name, (int)e.Track.TrackNumber, TN));
                        if (e.OK)
                        {
                            _TDs.Add(e.Track);
                        }
                        else
                        {
                            feedbacknegative = true;
                            iel.OnFactorisableError<UnableToConvertFile>(e.Track.Name);
                        }
                    }
                );

                bool convres = IMC.ConvertTomp3(progress, iCancellationToken);

                if ((convres == false) && (_TDs.Count == 0) && (feedbacknegative == false))
                {
                    iel.Report(new CDUnknownErrorArgs());
                    return null;
                }

                if (iCancellationToken.IsCancellationRequested)
                {
                    iel.Report(new CancelledImportEventArgs());
                    return null;
                }

                if (!ias.IsCompleted)
                {
                    iel.Report(new CDImportingProgessAdditionalCoverInfoEventArgs(ifad));
                }

                if (_OpenCDDoorOnComplete)
                    CDHelper.OpenCDDoor();

                bool okfound = ias.AsyncWaitHandle.WaitOne(TimeSpan.FromMinutes(3), false);
                //j'attends pas plus de 1 minute apres avoir grave le cd pour trouver
                //les pochettes sur internet
 
                Trace.WriteLine(string.Format("CD import cover from internet time out result (false:timedout): {0}!", okfound));

                if (resultwithimage != null)
                {             
                    int tracknumber = ifad.RawTrackDescriptors.Count;           
                    var images = resultwithimage.Where(wr => ((wr.FindItem.MatchTrackNumberOnDisk(tracknumber)) && (wr.FindItem.Images != null) && (wr.FindItem.Images.Count > 0))).ToList();

                    if (images.Count > 0)
                    {
                        Trace.WriteLine(string.Format("{0} Image(s) found!!", images.Count));

                        CDCoverInformationArgs cdfi = new CDCoverInformationArgs(images, ifad);
                        iel.Report(cdfi);
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("Information found but no matching image found for CD:{0} !",ifad));
                    }
                }
                else
                {
                    Trace.WriteLine("No image found for CD!");
                }

                Trace.WriteLine("Import CD OK");

                return new MusicWithMetadaImporter(_TDs.ToArray(), new List<string>(), new ICDInfosHelperAdpt(ifad));
            }
        }


        public override ImportType Type
        {
            get { return ImportType.CDImport; }
        }


        protected override bool ExpectedNotNullResult
        {
            get { return true; }
        }

        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
            switch(EI.State)
            {
                case ImportState.OK:
                    return;

                case ImportState.Partial:
                    if (EI.FilesNotimported.Any())
                    {
                        Context.Folders.GetFileCleanerFromFiles( OutFilesFiles.Where( t => EI.FilesNotimported.Contains(t)), n => false, false).Remove();
                    }
                    return;

                case ImportState.KO:
                case ImportState.NotFinalized:
                    Context.Folders.GetFileCleanerFromFiles(OutFilesFiles, n => false, false).Remove();
                    break;
            }
        }

        protected override IEnumerable<string> InFiles
        {
            get { return Enumerable.Empty<string>(); }
        }

        protected override IEnumerable<string> OutFilesFiles
        {
            get { return _TDs.Select( t => t.Path); }
        }
    }
}
