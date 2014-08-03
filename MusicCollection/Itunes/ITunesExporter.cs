using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using iTunesLib;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using System.Threading.Tasks;
using System.Threading;

namespace MusicCollection.Itunes
{
    internal class ITunesExporter :  IItunesExporter
    {
        private IImportContext _IIC;
        private IInternalMusicSession _MSI;

        internal ITunesExporter(IInternalMusicSession MSI)
        {
            _IIC = MSI.GetNewSessionContext();
            //_IIC.Error += ((o, e) => OnError(e));
            _MSI = MSI;
        }

        public Task SynchronizeAsync(bool DeleteBrokenItunes, IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken)
        {
            return Task.Factory.StartNew(
            () =>
            {
                PrivateSynchronize(DeleteBrokenItunes,iIImportExportProgress, iCancellationToken.HasValue ? iCancellationToken.Value : CancellationToken.None);
            },
             CancellationToken.None,
             TaskCreationOptions.LongRunning,
             TaskScheduler.Default);
        }

        private IEnumerable<IAlbum>  _AlbumToExport;
        public IEnumerable<IAlbum> AlbumToExport
        {
            set { _AlbumToExport = value; }
            get { return _AlbumToExport; }
        }

        private bool _Exporttoipod;
        public bool Exporttoipod
        {
            set { _Exporttoipod = value; }
            get { return _Exporttoipod; }
        }

        public void Export(IImportExportProgress iIImportExportProgress)
        {
            PrivateExportToItunes(iIImportExportProgress, CancellationToken.None);
        }

        public Task ExportAsync(IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken)
        {
            return Task.Factory.StartNew(
              () =>
              {
                  PrivateExportToItunes(iIImportExportProgress, iCancellationToken.HasValue ? iCancellationToken.Value : CancellationToken.None);
              },
               CancellationToken.None,
               TaskCreationOptions.LongRunning,
               TaskScheduler.Default);
        }

        private void TrackError(IImportExportProgress iIImportExportProgress)
        {
            _IIC.Error += ((o, e) => iIImportExportProgress.SafeReport(e));
        }


        private void PrivateSynchronize(bool DeleteBrokenItunes, IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken)
        {
            TrackError(iIImportExportProgress);

            try
            {
                var MyMusicCollect = (from al in _MSI.Albums from t in al.Tracks let fi = new FileInfo(t.Path) where fi.Exists select fi.FullName ).ToList();

                iTunesApp iTunesApp = new iTunesApp();

                if (iTunesApp == null)
                    return;

                IITLibraryPlaylist mainLibrary = iTunesApp.LibraryPlaylist;

                //OnProgress(new ITunesIdentifyingProgessEventArgs());
                iIImportExportProgress.SafeReport(new ITunesIdentifyingProgessEventArgs());

                var ItunesCollect = (from track in mainLibrary.Tracks.Cast<IITTrack>()
                                     let filetrack = track as IITFileOrCDTrack
                                     where ((filetrack != null) && (filetrack.Kind == ITTrackKind.ITTrackKindFile))
                                     select new { Location = filetrack.Location, TunesTrack = filetrack }).ToList();

                var cleanitunescollect = (from tunetrack in ItunesCollect where tunetrack.Location != null select tunetrack.Location).ToHashSet();
                var hashedcollect = MyMusicCollect.ToHashSet();

                var ToAdd = MyMusicCollect.Where(path => (!cleanitunescollect.Contains(path)) && (Path.GetExtension(path) != ".wma"));
 
                var ToRemove = from tunetrack in ItunesCollect
                               let rem = (DeleteBrokenItunes ? ((tunetrack.Location == null) || !(hashedcollect.Contains(tunetrack.Location))) : ((tunetrack.Location != null) && !(hashedcollect.Contains(tunetrack.Location))))
                               where rem
                               select tunetrack.TunesTrack;

                //OnProgress(new ITunesExportingProgessEventArgs(0));
                iIImportExportProgress.SafeReport(new ITunesExportingProgessEventArgs(0));

                foreach (string res in ToAdd)
                {
                    try
                    {
                        mainLibrary.AddFile(res);
                    }
                    catch(Exception e)
                    {
                        Trace.WriteLine(string.Format("Export error to itunes {0}", e));
                        _IIC.OnFactorisableError<ExportFileError>(res);
                    }
                }

                foreach (IITFileOrCDTrack res in ToRemove)
                {
                    res.Delete();
                }

                //OnProgress(new ITunesExportingProgessEventArgs(100));
                iIImportExportProgress.SafeReport(new ITunesExportingProgessEventArgs(100));

                //OnProgress(new EndExport("Collection Synchronised with iTunes"));
                iIImportExportProgress.SafeReport(new EndExport("Collection Synchronised with iTunes"));
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem to connect to iPod "+e.ToString());
                iIImportExportProgress.SafeReport(new ITunesNotResponding());
                //OnError(new ITunesNotResponding());

            }
        }

        private void PrivateExportToItunes(IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken)
        {
            TrackError(iIImportExportProgress);

            try
            {
                iTunesApp iTunesApp = new iTunesApp();

                if (iTunesApp == null)
                    return;

                IITLibraryPlaylist mainLibrary = iTunesApp.LibraryPlaylist;

                List<IITTrack> list = new List<IITTrack>();

                foreach (string p in from al in AlbumToExport from tr in al.Tracks select tr.Path)
                {
                    IITOperationStatus iops = mainLibrary.AddFile(p);
                    list.AddCollection(iops.Tracks.Cast<IITTrack>());
                }

                if (!Exporttoipod)
                    return;

                bool Transfer = false;

                IITSourceCollection _sources = iTunesApp.Sources;
                foreach (IITSource s in _sources)
                {
                    if (s.Kind == ITSourceKind.ITSourceKindIPod)
                    {
                        Transfer = true;

                        IITPlaylist l = s.Playlists.get_ItemByName(s.Name);

                        IITLibraryPlaylist lpl = l as IITLibraryPlaylist;

                        foreach (IITTrack track in list)
                        {
                            IITTrack itt = lpl.AddTrack(track);
                            if (itt == null)
                                _IIC.OnFactorisableError<ImpossibleToTransferMusicToIPod>(track.Name);
                        }

                        IITIPodSource ipod = s as IITIPodSource;
                        if (ipod != null)
                            ipod.UpdateIPod();

                        break;
                    }
                }

                if (Transfer == false)
                {
                    //OnError(new iPodNotFound());
                    iIImportExportProgress.SafeReport(new iPodNotFound());
                    return;
                }

                _IIC.FireFactorizedEvents();
                //OnProgress(new EndExport(AlbumToExport));
                iIImportExportProgress.SafeReport(new EndExport(AlbumToExport));
            }
            catch (Exception e)
            {
                
                Trace.WriteLine("Problem to connect to iPod " + e.ToString());

                if (e.Message == "The playlist is not modifiable.")
                    //OnError(new ITunesIPodPlaylistreadonly());
                    iIImportExportProgress.SafeReport(new ITunesIPodPlaylistreadonly());
                else
                    //OnError(new ITunesNotResponding());
                    iIImportExportProgress.SafeReport(new ITunesNotResponding());
            }

        }
    }
}
