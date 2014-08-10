using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MusicCollection.Implementation;
using MusicCollection.Fundation;
using System.Threading;

namespace MusicCollection.FileImporter
{
    internal class MusicImporter : MusicImporterAbstract
    {
        private string[] _Music = null;
        private List<string> _Im;

        internal MusicImporter(List<string> Mus, List<string> Im, IImportHelper NameClue)
            : base(NameClue)
        {
            _Music = Mus.ToArray();
            _Im = Im;
        }

        protected override IEnumerable<string> InFiles
        {
            get { return _Music.Concat(_Im); }
        }


        protected override IEnumerable<string> Images { get { return _Im; } }

        protected override IEnumerable<Track> GetTracks(IEventListener iel, CancellationToken iCancellationToken)
        {

            foreach (string Mus in _Music)
            {
                TrackStatus Tr = Track.GetTrackFromPath(Mus, _NameClue, Context);

                Visit(Mus, Tr);

                if (Tr != null)
                {
                    if (Tr.Continue)
                        yield return Tr.Found;
                }        
            }
        }

    }
}
