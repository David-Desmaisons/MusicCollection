using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;


namespace MusicCollection.FileImporter
{
    internal abstract class MusicImporterAbstract : FinalizerConverterAbstract, IImporter
    {
       // private List<string> _Image = null;
        protected IImportHelper _NameClue = null;

        internal MusicImporterAbstract(
            //List<string> Im, 
            IImportHelper NameClue)
        {
            _NameClue = NameClue;
            //_Image = Im;
        }


        protected abstract IEnumerable<Track> GetTracks(IEventListener iel);

        protected abstract IEnumerable<string> Images{get;}

        override protected bool ExpectedNotNullResult
        {
            get { return false; }
        }


        protected override ImporterConverterAbstract GetNext(IEventListener iel)
        {
            iel.OnProgress(new ImportProgessEventArgs(_NameClue.DisplayName));

            List<Track> LocalTrack = GetTracks(iel).ToList();


            if (LocalTrack.Count == 0)
            {
                ImportEnded();
                return null;
            }         

            List<string> Pictures = (from s in Images
                                     let n = Path.GetFileNameWithoutExtension(s).ToLower()
                                     orderby (n.Contains("cover") || n.Contains("front") ? 0 : 1), n
                                     select s).ToList<string>();

            //bool alc = true;

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

                    AM.Commit(true);
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
