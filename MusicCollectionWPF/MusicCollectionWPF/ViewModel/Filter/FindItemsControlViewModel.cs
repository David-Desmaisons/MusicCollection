using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class FindItemsControlViewModel : ViewModelBase
    {
        private static readonly List<IAlbum> _EmptyAlbum = new List<IAlbum>();
        private static readonly List<IArtist> _EmptyArtist = new List<IArtist>();
        private static readonly List<ITrack> _EmptyTrack = new List<ITrack>();

        private IEntityFinder<IAlbum> _AlbumFinder;
        private IEntityFinder<IArtist> _IArtistFinder;
        private IEntityFinder<ITrack> _TrackFinder;

        private FilterBuilder _FB;

        public FindItemsControlViewModel(ISessionEntityFinder finder, FilterView fv)
        {
            _AlbumFinder = finder.AlbumFinder;
            _IArtistFinder = finder.ArtistFinder;
            _TrackFinder = finder.TrackFinder;

            _FB = new FilterBuilder() { Filter = fv };

            Commit = RelayCommand.Instanciate(DoCommit);
            Reset = RelayCommand.Instanciate(DoReset);
            Activate = RelayCommand.Instanciate(DoActivate);

            InitLists();
        }


        private void InitLists()
        {
            Albums = _EmptyAlbum;
            Artists = _EmptyArtist;
            Tracks = _EmptyTrack;
        }

        private IList<IArtist> _Artists;
        public IList<IArtist> Artists
        {
            get { return _Artists; }
            private set { Set(ref _Artists, value); }
        }


        private IList<IAlbum> _Albums;
        public IList<IAlbum> Albums
        {
            get { return _Albums; }
            private set { Set(ref _Albums, value); }
        }

        private IList<ITrack> _Tracks;
        public IList<ITrack> Tracks
        {
            get { return _Tracks; }
            private set { Set(ref _Tracks, value); }
        }

        private bool _DisplayInfo=false;
        public bool DisplayInfo
        {
            get { return _DisplayInfo; }
            private set { Set(ref _DisplayInfo, value); }
        }

        public string _Search = string.Empty;
        public string Search
        {
            get { return this.Get<FindItemsControlViewModel, string>(() => (t) => t.RealSearch ?? t._FB.Filter.FilteringDisplay); }
            set { RealSearch = value; UpdatePopUpInfo(value); }
        }

        private string _RealSearch;
        private string RealSearch
        {
            get { return _RealSearch; }
            set { Set(ref _RealSearch, value); }
        }

        public ICommand Commit { get; private set; }

        public ICommand Reset { get; private set; }

        public ICommand Activate { get; private set; }


        private void UpdatePopUpInfo(string nv)
        {
            if ((string.IsNullOrEmpty(nv)) || (nv.Length < _AlbumFinder.MinimunLengthForSearch))
            {
                Clean();
                return;
            }

            DisplayInfo = true;
            Albums = _AlbumFinder.Search(nv).ToList();
            Artists = _IArtistFinder.Search(nv).Where(ar => ar.Albums.Count > 0).ToList();
            Tracks = _TrackFinder.Search(nv).ToList();
        }

        private void Clean()
        {
            InitLists();
            _FB.FilterEntity = new MusicCollectionWPF.ViewModel.Filter.NoFilter();
            DisplayInfo = false;
        }

        private void DoReset()
        {
            Clean();
            Search = null;
        }

        private void DoActivate()
        {
            if (!string.IsNullOrEmpty(Search))
            {
                UpdatePopUpInfo(Search);
                DisplayInfo = true;
            }
            else
                DoReset();
        }

        private void DoCommit()
        {
            if (_FB.FilterObject == null)
            {
                if (Albums == null)
                    return;

                if (Albums.Count + Artists.Count + Tracks.Count != 1)
                    return;

                _FB.FilterObject = this.Albums.Cast<object>().Concat(this.Artists).Concat(this.Tracks).FirstOrDefault();
            }

            var filter = _FB.FilterObject;

            RealSearch = null;
            InitLists();
            _FB.FilterObject = filter;
            DisplayInfo = false;
        }

        private IMusicObject _FilteringEntity;
        public IMusicObject FilteringObject
        {
            get { return _FilteringEntity; }
            set
            {
                if (Set(ref _FilteringEntity, value))
                    _FB.FilterObject = value;
            }
        }
    }
}
