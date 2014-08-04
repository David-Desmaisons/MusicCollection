using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;

namespace MusicCollection.FileConverter
{
    class MusicWithMetadaImporter : MusicImporterAbstract
    {
        private ITrackDescriptor[] _Listtracks = null;
        private List<string> _Image;

        internal MusicWithMetadaImporter(ITrackDescriptor[] tracks, List<string> Image, IImportHelper ClueName)
            : base(ClueName)
        {
            _Listtracks = tracks;
            _Image = Image;
        }

        protected override IEnumerable<string> Images { get { return _Image?? Enumerable.Empty<string>(); } }

        protected override IEnumerable<string> InFiles
        {
            get { return Images.Concat(from t in _Listtracks select t.Path); }
        }

     
        protected override IEnumerable<Track> GetTracks(IEventListener iel)
        {
            foreach (ITrackDescriptor Mus in _Listtracks)
            {
                TrackStatus res = Track.GetTrackFromTrackDescriptor(Mus, true, Context,true);

                Visit(Mus.Path, res);

                if (res != null) 
                {
                    if (res.Continue)
                        yield return res.Found;
                }
  
            }

            yield break;

        }



    }
}
