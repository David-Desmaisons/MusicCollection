using MusicCollection.Fundation;
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
        public PlayerViewModel(IMusicPlayer iMusicPlayer, IPlayListFactory iPlayListFactory)
        {
            _IMusicPlayer = iMusicPlayer;
            _IPlayListFactory = iPlayListFactory;

            _IMusicPlayer.TrackEvent += OnTrackEvent;
            _IMusicPlayer.TrackPlaying += TrackPlaying;

            _PlayList = _IPlayListFactory.CreateAlbumPlayList("Memory PlayList");
            _IMusicPlayer.PlayList = _PlayList;

            Play = RelayCommand.Instanciate(()=>_IMusicPlayer.Mode = PlayMode.Play);
            Pause = RelayCommand.Instanciate(() => _IMusicPlayer.Mode = PlayMode.Paused);
            VolumeUp = RelayCommand.Instanciate(() => _IMusicPlayer.Volume += 0.1);
            VolumeDown = RelayCommand.Instanciate(() => _IMusicPlayer.Volume -= 0.1);
            Like = RelayCommand.Instanciate(DoLike);
        }

        private void OnTrackEvent(object sender, MusicTrackEventArgs TrackEvent)
        {
            switch (TrackEvent.What)
            {
                case TrackPlayingEvent.Loading:
                    EndInMilliSeconds = TrackEvent.Track.Duration.TotalMilliseconds;
                    CurentInMilliSeconds = 0;
                    break;

                case TrackPlayingEvent.BeginPlay:
                    EndInMilliSeconds = _IMusicPlayer.MaxPosition.TotalMilliseconds;
                    CurentInMilliSeconds = _IMusicPlayer.Position.TotalMilliseconds;
                    break;

                case TrackPlayingEvent.EndPlay:
                    CurentInMilliSeconds = 0;
                    break;

                case TrackPlayingEvent.Broken:
                    Trace.WriteLine("Media failed");
                    CurentInMilliSeconds = 0;
                    break;
            }
        }

        private void TrackPlaying(object sender, MusicTrackPlayingEventArgs TrackEvent)
        {
            CurentInMilliSeconds = TrackEvent.Position.TotalMilliseconds;
            EndInMilliSeconds = TrackEvent.MaxPosition.TotalMilliseconds;
        }

        public IAlbum CurrentPlaying { get { return this.Get<PlayerViewModel, IAlbum>(() => el => el._PlayList.CurrentAlbumItem); } }

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
            set { Set(ref _CurentInMilliSeconds, value); }
        }


        public double Volume
        {
            get { return _IMusicPlayer.Volume; }
            set { _IMusicPlayer.Volume = value; }
        }

        public IList<IAlbum> Albums { get; private set; }

        #region Command

        public ICommand Play { get; private set; }

        public ICommand Pause { get; private set; }

        public ICommand VolumeUp { get; private set; }

        public ICommand VolumeDown { get; private set; }

        public ICommand Like { get; private set; }


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

            IAlbum last = ialls.First();
   
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


    } 
}
