using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionWPF.ViewModel.Element
{
    public class AlbumDistanceComparerFactory
    {
        private IMusicSession _Session;
        public AlbumDistanceComparerFactory(IMusicSession iSession)
        {
            _Session = iSession;
        }

        public IUpdatableComparer<IAlbum> GetComparer(IAlbum iCentered)
        {
            return new ElementCenteredComparer<IAlbum>(_Session.AllAlbums, new AlbumDistanceComparer(iCentered));
        }
    }


}
