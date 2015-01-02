using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        private IAlbum _Album;
        private CollectionWithDetailVM<IAlbumPicture> _ImagesVM;
        private IExtendedOrderedObservableCollection<ITrack> _OrderedTracks;

        public AlbumViewModel(IAlbum iAlbum)
        {
            _Album = iAlbum;
            _ImagesVM = new CollectionWithDetailVM<IAlbumPicture>(_Album.Images);
            _OrderedTracks = Register(Album.Tracks.LiveOrderBy(t => t.TrackNumber));
        }

        public bool ShouldGroup
        {
            get
            {
                return Get<AlbumViewModel, bool>(() =>
                  t => (t._Album.Tracks.Any(tr => tr.DiscNumber != t._Album.Tracks[0].DiscNumber)));
            }
        }

        public IList<ITrack> Tracks {get{return _OrderedTracks;}}
  
        public IAlbum Album { get { return _Album; } }

        public IAlbumPicture CurrentImage
        {
            get { return Get<AlbumViewModel, IAlbumPicture>(()=>t=>t._ImagesVM.Current); }
        }

        public bool IsInTransition
        {
            get { return Get<AlbumViewModel, bool>(() => t => t._ImagesVM.IsInTransition); }
        }

        public override void Dispose()
        {
            _ImagesVM.Dispose();
            base.Dispose();
        }

        public ICommand PreviousImage { get { return _ImagesVM.Previous; } }

        public ICommand NextImage { get { return _ImagesVM.Next; } }
    }
}
