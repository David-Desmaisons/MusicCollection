using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        private IAlbum _Album;
        private int _ImageCount = 0;

        public AlbumViewModel(IAlbum iAlbum)
        {
            _Album = iAlbum;
            InitImage();
            _Album.Images.CollectionChanged+=Images_CollectionChanged;

            PreviousImage = RelayCommand.Instanciate(DoPreviewImage);
            NextImage = RelayCommand.Instanciate(DoNextImage);
        }

        private void DoNextImage()
        {
            if ( _ImageCount < _Album.Images.Count-1)
            {
                _ImageCount++;
                CurrentImage = _Album.Images[_ImageCount];
            }
        }

        private void DoPreviewImage()
        {
            if (_ImageCount > 0)
            {
                _ImageCount--;
                CurrentImage = _Album.Images[_ImageCount];
            }
        }

        private void InitImage()
        {
            _IAlbumPicture = (_Album.Images.Count > 0) ? _Album.Images[0] : null;
            _ImageCount = (_IAlbumPicture==null)? -1 :0;
        }

        private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = (_IAlbumPicture!=null) ? _Album.Images.IndexOf(_IAlbumPicture) : -1;
            if ( index!=-1)
            {
                _ImageCount = index;
                return;
            }
 
            InitImage();
        }

        public IAlbum Album { get { return _Album; } }

        private IAlbumPicture _IAlbumPicture;
        public IAlbumPicture CurrentImage
        {
            get { return _IAlbumPicture; }
            set { IsInTransition = true; Set(ref _IAlbumPicture, value); IsInTransition = false; }
        }

        private bool _IsInTransition=false;
        public bool IsInTransition
        {
            get { return _IsInTransition; }
            set { Set(ref _IsInTransition, value); }
        }

        public override void Dispose()
        {
            _Album.Images.CollectionChanged -= Images_CollectionChanged;
            base.Dispose();
        }

        public ICommand PreviousImage { get; private set; }

        public ICommand NextImage { get; private set; }
    }
}
