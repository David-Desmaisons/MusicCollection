﻿using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private IMusicPlayer _IMusicPlayer;
        private IAlbumPlayList _PlayList;
        private IPlayListFactory _IPlayListFactory;
        private CollectionWithDetailVM<IAlbum> _PlayedAlbums;

        public PlayerViewModel(IMusicPlayer iMusicPlayer, IPlayListFactory iPlayListFactory)
        {
            _IMusicPlayer = iMusicPlayer;
            _IPlayListFactory = iPlayListFactory;

            _IMusicPlayer.TrackEvent += OnTrackEvent;
            _IMusicPlayer.TrackPlaying += TrackPlaying;

            _PlayList = _IPlayListFactory.CreateAlbumPlayList("Memory PlayList");
            _IMusicPlayer.PlayList = _PlayList; 
            _PlayedAlbums = new CollectionWithDetailVM<IAlbum>(_PlayList.Albums);

            Play = RelayCommand.Instanciate(()=>_IMusicPlayer.Mode = PlayMode.Play);
            Pause = RelayCommand.Instanciate(() => _IMusicPlayer.Mode = PlayMode.Paused);
            VolumeUp = RelayCommand.Instanciate(() => _IMusicPlayer.Volume += 0.1);
            VolumeDown = RelayCommand.Instanciate(() => _IMusicPlayer.Volume -= 0.1);
            Like = RelayCommand.Instanciate(DoLike);
            SeeNextAlbum = _PlayedAlbums.Next;
            SeePreviousAlbum = _PlayedAlbums.Previous;
            PlayAlbum = RelayCommand.Instanciate(DoPlayAlbum);

            //call  to activate listeners
            var pa = this.PlayingAlbum;
        }

        public bool AlbumNavigating
        {
            get { return this.Get<PlayerViewModel, bool>(() => el => el._PlayedAlbums.IsInTransition); }
        }

        private void DoPlayAlbum()
        {
            var displayed = _PlayedAlbums.Current;
            if (_IMusicPlayer.AlbumPlayList.CurrentAlbumItem == displayed)
                return;

            _IMusicPlayer.AlbumPlayList.CurrentAlbumItem = displayed;
        }

        protected override void OnChanged(string iPropertyName, object old)
        {
           if ((iPropertyName=="PlayingAlbum") && (old==CurrentPlaying.Album) && (PlayingAlbum!=null))
           {
                _PlayedAlbums.Current = PlayingAlbum;
           }
        }

        private void OnTrackEvent(object sender, MusicTrackEventArgs TrackEvent)
        {
            switch (TrackEvent.What)
            {
                case TrackPlayingEvent.Loading:
                    EndInMilliSeconds = TrackEvent.Track.Duration.TotalMilliseconds;
                    InternalSetCurentInMilliSeconds(0);
                    break;

                case TrackPlayingEvent.BeginPlay:
                    EndInMilliSeconds = _IMusicPlayer.MaxPosition.TotalMilliseconds;
                    InternalSetCurentInMilliSeconds(_IMusicPlayer.Position.TotalMilliseconds);
                    break;

                case TrackPlayingEvent.EndPlay:
                    InternalSetCurentInMilliSeconds(0);
                    break;

                case TrackPlayingEvent.Broken:
                    Trace.WriteLine("Media failed");
                    InternalSetCurentInMilliSeconds(0);
                    break;
            }
        }

        public bool ShoulBePlayed
        {
            get { return Get<PlayerViewModel, bool>(() => t => t.MusicPlayer.PlayList.ReadOnlyTracks.Count>0); }
        }

        private void TrackPlaying(object sender, MusicTrackPlayingEventArgs TrackEvent)
        {
            InternalSetCurentInMilliSeconds(TrackEvent.Position.TotalMilliseconds);
            EndInMilliSeconds = TrackEvent.MaxPosition.TotalMilliseconds;
        }

        public AlbumViewModel CurrentPlaying 
        { 
            get 
            { 
                return this.Get<PlayerViewModel, AlbumViewModel>(() => el => el.Create((el._PlayedAlbums.Current))); 
            } 
        }

        public IAlbum PlayingAlbum
        {
            get
            {
                return this.Get<PlayerViewModel, IAlbum>(() => el => el._PlayList.CurrentAlbumItem);
            }
        }

        public ITrack CurrentTrack
        {
            get { return this.Get<PlayerViewModel, ITrack>(() => el => el._PlayList.CurrentTrack); }
            set { _PlayList.CurrentTrack = value; }
        }


        public PlayMode Mode 
        {
            get { return this.Get<PlayerViewModel, PlayMode>(() => el => el._IMusicPlayer.Mode); }
            set { _IMusicPlayer.Mode = value; }
        }

        private AlbumViewModel _AlbumViewModel;
        private AlbumViewModel  Create(IAlbum ialbum)
        {
            if (_AlbumViewModel!=null)
            {
                _AlbumViewModel.Dispose();
                _AlbumViewModel = null;
            }
            return (ialbum==null) ? null : (_AlbumViewModel = new AlbumViewModel(ialbum));
        }

        public bool IsPlaying { get { return this.Get<PlayerViewModel, bool>(() => el => el._IMusicPlayer.Mode== PlayMode.Play); } }
  
        private double? _EndInMilliSeconds;
        public double? EndInMilliSeconds
        {
            get { return _EndInMilliSeconds; }
            set { Set(ref _EndInMilliSeconds, value); }
        }

        private double? _CurentInMilliSeconds;
        public double? CurentInMilliSeconds
        {
            get { return _CurentInMilliSeconds; }
            set 
            { 
                if (Set(ref _CurentInMilliSeconds, value) && (value.HasValue))  
                    _IMusicPlayer.Position = TimeSpan.FromMilliseconds(value.Value); 
            }
        }

        private void InternalSetCurentInMilliSeconds(double value)
        {
            Set(ref _CurentInMilliSeconds, value,"CurentInMilliSeconds");
        }

        public double Volume
        {
            get { return this.Get<PlayerViewModel, double>(() => el => el._IMusicPlayer.Volume); }
            set {  _IMusicPlayer.Volume = value;}
        }

        public IObservableCollection<IAlbum> PlayingAlbums 
        {
            get { return _PlayList.Albums; } 
        }

        public IMusicPlayer MusicPlayer { get { return _IMusicPlayer; } }

        #region Command

        public ICommand Play { get; private set; }

        public ICommand Pause { get; private set; }

        public ICommand VolumeUp { get; private set; }

        public ICommand VolumeDown { get; private set; }

        public ICommand Like { get; private set; }

        public ICommand SeeNextAlbum { get; private set; }

        public ICommand SeePreviousAlbum { get; private set; }

        public ICommand PlayAlbum { get; private set; }

        public ICommand NextAlbumCover
        {
            get { return Get<PlayerViewModel, ICommand>(() => t => (t.CurrentPlaying==null)? null : t.CurrentPlaying.NextImage); }
        }

        public ICommand PreviousAlbumCover
        {
            get { return Get<PlayerViewModel,ICommand>(() => t => (t.CurrentPlaying==null)? null : t.CurrentPlaying.PreviousImage);}
        }

        private void DoLike()
        {
            if (_IMusicPlayer.Mode == PlayMode.Stopped)
                return;

            ITrack track = _IMusicPlayer.PlayList.CurrentTrack;
            if (track == null)
                return;

            track.Rating = 5;
        }


        #endregion

        public void StopPlay()
        {
            _IMusicPlayer.Mode = PlayMode.Paused;
        }

        private void AddAlbum(IEnumerable<IAlbum> Al)
        {
            Al.Apply(al => _PlayList.AddAlbum(al));
        }

        public void AddAlbumAndPlay(IEnumerable<IAlbum> ialls)
        {
            bool emptybefore = _PlayList.Albums.Count == 0;
            AddAlbum(ialls);

            IAlbum last = ialls.FirstOrDefault();

            if (last == null)
                return;

            _PlayedAlbums.Current = last;
            _PlayList.CurrentAlbumItem = last;
            

            _IMusicPlayer.Mode = PlayMode.Play;
        }

        public void AddAlbumAndPlay(IEnumerable<ITrack> trcs)
        {
            bool emptybefore = _PlayList.Albums.Count == 0;
            var tr = trcs.Distinct();
            var res = tr.Select(tra => tra.Album).Distinct();

            AddAlbum(res);

            var trlast = tr.First();
            var lastalb = trlast.Album;

            _PlayList.CurrentTrack = trlast;

            _IMusicPlayer.Mode = PlayMode.Play;
        }

        public override void Dispose()
        {
            if (_AlbumViewModel!=null)
                _AlbumViewModel.Dispose();
            base.Dispose();
        }


    } 
}
