using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Implementation;
using MusicCollection.PlayList;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;

namespace MusicCollection.MusicPlayer
{
    internal class MusicPlayer : NotifyCompleteListenerObject, IInternalMusicPlayer, IMusicPlayer, INotifyPropertyChanged, IInternalPlayerListener
    {
        #region PlayFoto 

        private class PlayFoto
        {
            internal ITrack Track { get; private set; }

            internal bool PlayOrPaused { get; private set; }

            internal TimeSpan Position { get; set; }

            internal PlayFoto(ITrack iTrack, TimeSpan iPosition, bool Play)
            {
                Track = iTrack;
                Position = iPosition;
                PlayOrPaused = Play;
            }
        }

        #endregion

        private const string _MusicTrackSourceProperty = "MusicTrackSource";
        private const string _ModeProperty = "Mode";
        //private const string _VolumeProperty = "Volume";

        private IMusicFactory _IMusicFactory;
        private PlayMode _PlayMode = PlayMode.Stopped;
        private PlayFoto _Foto = null;
        private bool _LockEvent = false;
        private IInternalPlayer _Player;

        internal MusicPlayer(IMusicFactory imf)
        {
            _TrackEvent = new UISafeEvent<MusicTrackEventArgs>(this);
            _TrackPlaying = new UISafeEvent<MusicTrackPlayingEventArgs>(this);
            _IMusicFactory = imf;
        } 

        private IInternalPlayer CurrentPlayer
        {
            get
            {
                if (_Player == null)
                {
                    _Player = _IMusicFactory.GetInternalPlayer();
                    _Player.Listener = this;
                    _Volume = _Player.Volume;
                }

                return _Player;
            }
        }

        #region Events

        private class Silenter : IDisposable
        {
            private MusicPlayer _Father;

            internal Silenter(MusicPlayer Father)
            {
                _Father = Father;
                _Father._LockEvent = true; ;
            }

            public void Dispose()
            {
                _Father._LockEvent = false;
            }
        }

        protected IDisposable Silent()
        {
            return new Silenter(this);
        }

        private UISafeEvent<MusicTrackEventArgs> _TrackEvent;
        private UISafeEvent<MusicTrackPlayingEventArgs> _TrackPlaying;

        public event EventHandler<MusicTrackEventArgs> TrackEvent
        {
            add { _TrackEvent.Event += value; }
            remove { _TrackEvent.Event -= value; }
        }

        private event EventHandler<MusicTrackEventArgs> PrivateTrackEvent;

        public event EventHandler<MusicTrackPlayingEventArgs> TrackPlaying
        {
            add { _TrackPlaying.Event += value; }
            remove { _TrackPlaying.Event -= value; }
        }

        void IInternalPlayerListener.OnBroken()
        {
            PrivateMode = PlayMode.Stopped;
            SendTrackEvent(TrackPlayingEvent.Broken);
        }

        void IInternalPlayerListener.OnTrackLoadedPlay()
        {
            SendTrackEvent(TrackPlayingEvent.BeginPlay);
        }

        void IInternalPlayerListener.OnTrackLoadingForPlay()
        {
            SendTrackEvent(TrackPlayingEvent.Loading);
        }

        private void SendTrackEvent(TrackPlayingEvent tpe)
        {
            bool Lock = _LockEvent;

            MusicTrackEventArgs tre = null;

            if (PrivateTrackEvent != null)
            {
                tre = (tpe == TrackPlayingEvent.BeginPlay) ? new MusicTrackEventArgs(_TrackSource, tpe, MaxPosition) : new MusicTrackEventArgs(_TrackSource, tpe);
                PrivateTrackEvent(this,tre);
            }

            if (tre == null)
            {
                tre = (tpe == TrackPlayingEvent.BeginPlay) ? new MusicTrackEventArgs(_TrackSource, tpe, MaxPosition) : new MusicTrackEventArgs(_TrackSource, tpe);
            }

            if (!Lock)
                _TrackEvent.Fire(tre, true);
        }

        public void OnTrackPlayingEvent(TimeSpan Position, TimeSpan MaxPoistion)
        {
            if (_LockEvent)
                return;

            _TrackPlaying.Fire(new MusicTrackPlayingEventArgs(_TrackSource, Position, MaxPoistion), false);
        }

        private void PlayListtransition()
        {
            if (_PlayList == null)
                return;


            if (!_PlayList.Transition())
            {
                using (TrackChanger(PlayMode.Stopped))
                {
                    RawClose();
                }
            }
        }


        void IInternalPlayerListener.OnTrackEnd()
        {
            if (_TrackSource == null)
                return;

            SendTrackEvent(TrackPlayingEvent.EndPlay);

            if (_TrackSource != null)
            {
                _TrackSource.RemovePlayer(this);
                _TrackSource = null; 
                
                PlayListtransition();
            }
          
        }


        //void IInternalPlayerListener.OnVolumeChange()
        //{
        //    //if (!_LockEvent)
        //    //    PropertyHasChanged(_VolumeProperty);
        //}


        private void PlayFromFoto()
        {
            if (_Foto == null)
                return;

            string path = _Foto.Track.Path;
            if (FileSource != path)
                FileSource = path;

            TimeSpan cPosition = _Foto.Position;
            ITrack tr = _Foto.Track;
            _Foto = null;

            //bool Done = false;
            IDisposable sil = Silent();
            EventHandler<MusicTrackEventArgs> timeshifter = null;
            double vol = Volume;
            InternalVolume = 0;

            timeshifter = (o, ev) =>
            {
                if ((ev.What == TrackPlayingEvent.BeginPlay) && (object.ReferenceEquals(_TrackSource, tr))) RawPosition = cPosition; InternalVolume = vol; sil.Dispose(); PrivateTrackEvent -= timeshifter;
            };
            PrivateTrackEvent += timeshifter;

            PrivateMode = PlayMode.Play;
            RawPlay();
        }

        void IInternalMusicPlayer.OnLockEvent(object sender, ObjectStateChangeArgs e)
        {
            IInternalTrack ts = sender as IInternalTrack;
            if (ts != _TrackSource)
                throw new Exception("Lock management");

            PlayMode currentmode = Mode;

            switch (e.NewState)
            {

                case ObjectState.Available:
                    if (e.OldState != ObjectState.UnderEdit)
                        break;

                    //liberation de lock
                    if (_Foto == null)
                        return;

                    if ((_Foto.Track != ts) || (currentmode != PlayMode.Paused))
                        throw new Exception("Lock management");
                        
                    bool play = _Foto.PlayOrPaused;
                    if (play)
                    {
                        PlayFromFoto();
                    }
                  
                    break;


                case ObjectState.UnderEdit:

                    //Lock de track
                    switch (currentmode)
                    {
                        case PlayMode.Stopped:
                            RawClose();
                            break;

                        case PlayMode.Play:
                            RawPause();
                            _Foto = new PlayFoto(_TrackSource, Position, true);
                            using (Silent())
                            {
                                RawStop();
                                RawClose();
                            }
                            PrivateMode = PlayMode.Paused;
                            break;


                        case PlayMode.Paused:
                            _Foto = new PlayFoto(_TrackSource, Position, false);
                            using (Silent())
                            {
                                RawStop();
                                RawClose();
                            }
                            break;
                    }
                    break;

                case ObjectState.FileNotAvailable:

                    PlayListtransition();
                    break;
            }

        }

        #endregion

        #region Delegate to letter

        private string FileSource
        {
            set { CurrentPlayer.FileSource = value; }
            get { return CurrentPlayer.FileSource; }
        }

        private void RawStop()
        {
            CurrentPlayer.Stop();
        }

        private void RawPlay()
        {
            CurrentPlayer.Play();
        }

        private void RawPause()
        {
            CurrentPlayer.Pause();
        }

        private void RawClose()
        {
            CurrentPlayer.Close();
        }

        public TimeSpan RawPosition
        {
            get { return CurrentPlayer.Position; }
            set { CurrentPlayer.Position = value; }
        }

        private double _Volume=0;
        public double Volume
        {
            get { return CurrentPlayer.Volume; }
            set 
            {
                if (value > 1) 
                    value = 1;
                else if  (value < 0)
                    value = 0;

                CurrentPlayer.Volume = value;
                Set(ref _Volume, value);
            }
        }

        public double InternalVolume
        {
            get { return CurrentPlayer.Volume; }
            set  
            {
                if (value > 1) 
                    value = 1;
                else if  (value < 0)
                    value = 0;

                CurrentPlayer.Volume = value;
            }
        }



        public TimeSpan MaxPosition
        {
            get { return CurrentPlayer.MaxPosition; }
        }

        #endregion

        #region IMusicPlayer

        public void Play()
        {
            if ((_TrackSource == null) && (_PlayList != null))
            {
                IReadOnlyPlayList Rpl = _PlayList;
                ITrack track = Rpl.CurrentTrack;
                if (track == null)
                {
                    Rpl.Init();
                    track = Rpl.CurrentTrack;
                }
                MusicRawTrack = track as IInternalTrack;
            }

            if (_TrackSource == null)
                return;

            if (_TrackSource.IsBroken)
            {

                PrivateMode = PlayMode.Stopped;
                SendTrackEvent(TrackPlayingEvent.Broken);
                return;
            }


            if (_TrackSource.InternalState == ObjectState.UnderEdit)
            {
                PrivateMode = (_Foto != null) ? PlayMode.Paused : PlayMode.Stopped;
                return;
            }

            if (_Foto!=null)
            {
                PlayFromFoto();
                return;
            }

            using (TrackChanger(PlayMode.Play))
            {
                RawPlay();
            }
        }

        public void Pause()
        {
            if ((_TrackSource == null) || (_TrackSource.IsBroken))
                return;

            if (_TrackSource.InternalState == ObjectState.UnderEdit)
                return;

            using (TrackChanger(PlayMode.Paused))
            {
                RawPause();
            }

        }

        public void Stop()
        {
            _Foto = null;

            if ((_TrackSource == null) || (_TrackSource.IsBroken))
                return;

            if (_TrackSource.InternalState == ObjectState.UnderEdit)
            {
                PrivateMode = PlayMode.Stopped;
                return;
            }

            using (TrackChanger(PlayMode.Stopped))
            {
                RawStop();
            }
        }

        public TimeSpan Position
        {
            get { if (_Foto != null) return _Foto.Position; return RawPosition; }
            set { if (_Foto != null) { _Foto.Position = value; return; } RawPosition = value; }
        }

        public PlayMode Mode
        {
            get
            {
                return _PlayMode;
            }
            set
            {
                if (_PlayMode == value)
                    return;

                switch (value)
                {
                    case PlayMode.Paused:
                        Pause();
                        break;

                    case PlayMode.Stopped:
                        Stop();
                        break;

                    case PlayMode.Play:
                        Play();
                        break;
                }
            }
        }

        private class TrackModeChanger : IDisposable
        {
            private MusicPlayer _Father;
            //private bool _NeedTosendEvent;
            private PlayMode _PlayMode;

            internal TrackModeChanger(MusicPlayer father, PlayMode entry)
            {
                _Father = father;
                _PlayMode = entry;
                //_NeedTosendEvent = (_Father._PlayMode != entry);
                //if (_NeedTosendEvent)
                //    _Father._PlayMode = entry;
            }

            public void Dispose()
            {
                _Father.PrivateMode = _PlayMode;
                //_Father.PropertyHasChanged(_ModeProperty);
            }

        }

        private IDisposable TrackChanger(PlayMode entry)
        {
            return new TrackModeChanger(this, entry);
        }

        private PlayMode PrivateMode
        {
            set
            {
                Set(ref _PlayMode, value, _ModeProperty);
                //if (_PlayMode == value)
                //    return;

                //_PlayMode = value;
                //PropertyHasChanged(_ModeProperty);
            }
        }

        private IInternalTrack _TrackSource;
        private IReadOnlyPlayList _PlayList;


        private void SelectionChanged(object sender, SelectionChangedargs e)
        {
            IReadOnlyPlayList Rpl = _PlayList;
            IInternalTrack tr = Rpl.CurrentTrack as IInternalTrack;
            IInternalTrack mytrack = Rpl.CurrentTrack as IInternalTrack;

            if (mytrack == null)
            {
                MusicRawTrack = null;
                return;
            }

            if (mytrack.UpdatedState == ObjectState.FileNotAvailable)
            {
                Rpl.Transition();
                return;
            }

            MusicRawTrack = mytrack;       
            Play();
        }


        public IAlbumPlayList AlbumPlayList
        {
            get { return _PlayList as IAlbumPlayList; }
            set { (this as IMusicPlayer).PlayList = value; }
        }

        public IReadOnlyPlayList PlayList
        {
            get { return _PlayList; }
            set
            {
                var old = _PlayList;
                if (!Set(ref _PlayList, value))
                    return;

                if (old != null)
                    old.SelectionChanged -= SelectionChanged;

                if (_PlayList == null)
                {
                    MusicRawTrack = null;
                    return;
                }

                IReadOnlyPlayList Rpl = _PlayList;
                _PlayList.SelectionChanged += SelectionChanged;

                ITrack current = _PlayList.CurrentTrack;

                MusicRawTrack = current as IInternalTrack;
                if (current != null)
                    Play();
            }
        }

        private IInternalTrack MusicRawTrack
        {
            set
            {
                var oldtrack = _TrackSource;
                bool ChangeTrack = !object.ReferenceEquals(_TrackSource, value);

                PlayMode Old = Mode;

                if (ChangeTrack)
                {
                    _Foto = null;

                    if (_TrackSource != null)
                    {
                        if (this.Position.Seconds>=2)
                            SendTrackEvent(TrackPlayingEvent.Skipped);
                    }
                }

                if (_TrackSource != null)
                {
                    _TrackSource.RemovePlayer(this);
 
                    if (_TrackSource.InternalState != ObjectState.UnderEdit)//dem bien reflechir a ca
                        RawStop();//todo pourrait etre enleve, mais gerer imapct gestion porri m4a par mediaplayer
                }

                if (value == null)
                {
                    RawClose();
                }

                _TrackSource = value;

                if (_TrackSource == null)
                {
                    PrivateMode = PlayMode.Stopped;
                    return;
                }

                _TrackSource.AddPlayer(this);

                FileSource = _TrackSource.Path;

                if (Old == PlayMode.Play)
                {
                    Play();
                }

                if (ChangeTrack)
                    PropertyHasChanged(_MusicTrackSourceProperty, oldtrack,value);
            }
        }

        public ITrack MusicTrackSource
        {
            get { return _TrackSource; }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            if (_Player != null)
            {
                _Player.Listener = null;
                _Player.Dispose();
                _Player = null;
            }
        }
    }
}
