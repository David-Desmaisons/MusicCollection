﻿using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Infra;

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

        private AplicationViewModel _ApplicationMain;

        public FindItemsControlViewModel(ISessionEntityFinder finder, AplicationViewModel iAplicationViewModel)
        {
            _ApplicationMain = iAplicationViewModel;
            _AlbumFinder = finder.AlbumFinder;
            _IArtistFinder = finder.ArtistFinder;
            _TrackFinder = finder.TrackFinder;

            Commit = RelayCommand.Instanciate(DoCommit);
            Reset = RelayCommand.Instanciate(DoReset);
            Activate = RelayCommand.Instanciate(DoActivate);

            InitLists();
        }

        private bool _Tracking = false;

        public FilterType FilteringEntity 
        {
            get { return Get<FindItemsControlViewModel, FilterType>(() => (t) => Convert(t.FilteringObject)); } 
        }

        private FilterType Convert(IMusicObject iob)
        {
            if (iob is IArtist)
                return FilterType.Artist;

            if (iob is IAlbum)
                return FilterType.Name;

            if (iob is ITrack)
                return FilterType.Track;

            return FilterType.All;
        }

        private void TrackChanges()
        {
            if (_Tracking)
                return;

            _Tracking = true;
            _AlbumFinder.OnUpdate += OnUpdate_Album;
            _TrackFinder.OnUpdate += OnUpdate_Track;
            _IArtistFinder.OnUpdate += OnUpdate_Artist;
        }

        private void UnTrackChanges()
        {
            if (!_Tracking)
                return;

            _Tracking = false;
            _AlbumFinder.OnUpdate -= OnUpdate_Album;
            _TrackFinder.OnUpdate -= OnUpdate_Track;
            _IArtistFinder.OnUpdate -= OnUpdate_Artist;
        }

        private void OnUpdate_Artist(object sender, EventArgs e)
        {
            Artists = _IArtistFinder.Search(Search).ToList().LiveWhere(ar => ar.Albums.Count > 0);
        }

        private void OnUpdate_Track(object sender, EventArgs e)
        {
            Tracks = _TrackFinder.Search(Search).ToList();
        }

        private void OnUpdate_Album(object sender, EventArgs e)
        {
            Albums = _AlbumFinder.Search(Search).ToList();
        }


        private void InitLists()
        {
            UnTrackChanges();
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
            get { return this.Get<FindItemsControlViewModel, string>(() => (t) => t.RealSearch ?? ((t.FilteringObject != null) ? t.FilteringObject.Name : string.Empty)); }
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
            Albums = _AlbumFinder.Search(nv).OrderBy(it=>it.Name).ToList();
            Artists = _IArtistFinder.Search(nv).OrderBy(it => it.Name).ToList().LiveWhere(ar => ar.Albums.Count > 0);
            Tracks = _TrackFinder.Search(nv).OrderBy(it => it.Name).ToList();
            RealSearch = nv;
            TrackChanges();
        }

        private void Clean()
        {
            InitLists();
            FilteringObject = null;
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

            if (_FilteringEntity == null)
            {
                if (Albums == null)
                    return;

                if (Albums.Count + Artists.Count + Tracks.Count != 1)
                    return;

                FilteringObject = this.Albums.Cast<IMusicObject>().Concat(this.Artists).Concat(this.Tracks).FirstOrDefault();
            }

            RealSearch = null;

            _PreventChanges = true;
            InitLists();
            _PreventChanges = false;

            DisplayInfo = false;
        }

        private bool _PreventChanges=false;
        private IMusicObject _FilteringEntity;
        public IMusicObject FilteringObject
        {
            get { return _FilteringEntity; }
            set
            {
                if (_PreventChanges)
                    return;

                if (Set(ref _FilteringEntity, value))
                {
                    _ApplicationMain.SetFilter(_FilteringEntity);
                }
            }
        }
    }
}
