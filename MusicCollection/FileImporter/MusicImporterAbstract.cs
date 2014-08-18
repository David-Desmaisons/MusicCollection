using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using System.Threading;
using MusicCollection.ToolBox;


namespace MusicCollection.FileImporter
{
    internal abstract class MusicImporterAbstract : FinalizerConverterAbstract, IImporter
    {
        protected IImportHelper _NameClue = null;

        internal MusicImporterAbstract(IImportHelper NameClue)
        {
            _NameClue = NameClue;
        }

        protected abstract IEnumerable<Track> GetTracks(IEventListener iel, CancellationToken iCancellationToken);

        protected abstract IEnumerable<string> Images{get;}

        override protected bool ExpectedNotNullResult
        {
            get { return false; }
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            iel.Report(new ImportProgessEventArgs(_NameClue.DisplayName));

            List<Track> LocalTrack = GetTracks(iel, iCancellationToken).CancelableToList(iCancellationToken);        

            if (iCancellationToken.IsCancellationRequested)
            {
                RawImportEnded(KOEndImport());
                return null;
            } 
            
            if (LocalTrack.Count == 0)
            {
                ImportEnded();
                return null;
            }

            List<string> Pictures = Images.OrderBy( imp => 
                                        {   
                                            string ifn = Path.GetFileNameWithoutExtension(imp).ToLower(); 
                                            return (ifn.Contains("cover") || ifn.Contains("front"))? 0 : 1;
                                        } ).ToList();

            foreach (Album Al in (from r in LocalTrack select r.Owner).Distinct<Album>())
            {
                using (IModifiableAlbum AM = Al.GetModifiableAlbum(true, Context))
                {
                    if (AM.FrontImage != null)
                        continue;

                    int i = 0;
                    foreach (string apic in Pictures)
                    {
                        IAlbumPicture res = AM.AddAlbumPicture(apic, i);
                        if (res != null)
                            i++;
                    }

                    if (iCancellationToken.IsCancellationRequested)
                    {
                        RawImportEnded(KOEndImport());
                        return null;
                    } 

                    AM.Commit();
                }
            }

            ImportEnded();

            return null;
        }

        public override ImportType Type
        {
            get { return ImportType.Import; }
        }

        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
        }
    }
}
