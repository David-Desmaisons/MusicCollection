using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.ViewModel
{
    public class ArtistSearchableFactory : ISearchableFactory
    {
        private IMusicSession _IMS;
        private IEntityFinder<IArtist> _IArtistFinder;

        public ArtistSearchableFactory(IMusicSession ims)
        {
            _IMS = ims;
            _IArtistFinder = _IMS.EntityFinder.ArtistFinder;
                //new EntityFinder(_IMS).ArtistFinder;
        }

        public IList PossibilitiesFromClue(string clue)
        {
            return _IArtistFinder.SearchOrdered(clue).ToList();
        }

        public object CreateFromName(string iArtistName)
        {
            return _IMS.CreateArtist(iArtistName);
        }

        public void Dispose()
        {
            //_IArtistFinder.Dispose();
        }
    }
}
