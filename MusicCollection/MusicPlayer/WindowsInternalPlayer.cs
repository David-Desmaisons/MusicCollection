using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;

using MusicCollection.Fundation;
using MusicCollection.Implementation;

namespace MusicCollection.MusicPlayer
{

    internal class WindowsInternalPlayer : InternalPlayerAdapter, IInternalPlayer
    {
        private MediaElement _MediaElement;
        private IInternalPlayerListener _Father;
        private bool _IsLoaded = false;

        internal WindowsInternalPlayer()
            : base(100)
        {
            _Father = null;
            _MediaElement = new MediaElement();
            _MediaElement.BeginInit();
            _MediaElement.EndInit();
            _MediaElement.LoadedBehavior = MediaState.Manual;
            _MediaElement.UnloadedBehavior = MediaState.Manual;
            _MediaElement.Clock = null;
            _MediaElement.MediaEnded += ((O, E) => OnTrackEnd());
            _MediaElement.MediaOpened += ((o, e) => OnTrackLoadedPlay());
            _MediaElement.MediaFailed += ((o, e) => OnBroken());

        }

        public IInternalPlayerListener Listener
        {
            set
            {
                if (_Father != null)
                    throw new Exception("Algo error");

                _Father = value;
            }
            get { return _Father; }
        }

        private void OnTrackLoadedPlay()
        {
            _IsLoaded = true;
            //_Max = MaxPosition;
            _Father.OnTrackLoadedPlay();
        }

        private void OnTrackEnd()
        {
            TimerStop();
            _Father.OnTrackEnd();
        }

        private void OnBroken()
        {
            TimerStop();
            _Father.OnBroken();
        }

        public string FileSource
        {
            set
            {
                Action A = () => privateFileSource = value;
                _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, A);
                _Father.OnTrackLoadingForPlay();
                //_Father.OnTrackPlayingEvent(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
            }
            get
            {
                Func<string> get = () => privateFileSource;
                return (string)_MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, get);
            }
        }

        private string privateFileSource
        {
            set
            {
                if (object.ReferenceEquals(value, _MediaElement.Source))
                    return;

                if ((_MediaElement.Source != null) && (_MediaElement.Source.Equals(value)))
                    return;

                _IsLoaded = false;

                _MediaElement.Source = value != null ? new Uri(value) : null;
            }
            get
            {
                return _MediaElement.Source.AbsolutePath;
            }
        }


        protected override void DoStop()
        {
            Action S = () => _MediaElement.Stop();
            _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, S);

            //_Father.OnTrackEvent(TrackPlayingEvent.Stopped);
        }

        protected override void DoPlay()
        {
            if (_IsLoaded)
                OnTrackLoadedPlay();

            Action P = () => _MediaElement.Play();
            _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, P);
        }

        protected override void DoPause()
        {
            Action P = () => _MediaElement.Pause();

            _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, P);
            //_Father.OnTrackEvent(TrackPlayingEvent.Paused);
        }

        protected override void DoClose()
        {

            Action C = () => _MediaElement.Close();
            _MediaElement.Dispatcher.Invoke(DispatcherPriority.Send, C);

            Thread.Sleep(400);

            _IsLoaded = false;
        }

        public TimeSpan Position
        {
            get
            {
                Func<TimeSpan> F = (() => _MediaElement.Position);
                return (TimeSpan)_MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, F); ;
            }
            set
            {
                Action F = (() => PrivatePosition = value);
                _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, F);
            }
        }

        private TimeSpan PrivatePosition
        {
            set
            {
                if (!_MediaElement.NaturalDuration.HasTimeSpan)
                    return;

                if (value.TotalMilliseconds > MaxPosition.TotalMilliseconds)
                    value = TimeSpan.FromMilliseconds(MaxPosition.TotalMilliseconds - 1);

                _MediaElement.Position = value;
            }
        }

        public double Volume
        {
            get
            {
                Func<double> F = (() => _MediaElement.Volume);
                return (double)_MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, F); ;
            }
            set
            {
                Action F = (() => { _MediaElement.Volume = value;});
                //_Father.OnVolumeChange(); });
                _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, F);
            }
        }


        public TimeSpan MaxPosition
        {
            get
            {
                Func<TimeSpan> Get = (() => _MediaElement.NaturalDuration.HasTimeSpan ? _MediaElement.NaturalDuration.TimeSpan : new TimeSpan());
                return (TimeSpan)_MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, Get);
            }
        }

        private void PrivateSafeOnTimer()
        {
            if (!_MediaElement.NaturalDuration.HasTimeSpan)
                return;

            var p = Position;
            var m = MaxPosition;

            if ((m == p) && (m.TotalMilliseconds > 0) && _IsLoaded)
            {
                _MediaElement.Stop();
                Trace.WriteLine("reach end of track, loaded");
                _Father.OnTrackEnd();
                return;
            }

            _Father.OnTrackPlayingEvent(p, m);
        }

        protected override void OnTimer()
        {
            Action time = PrivateSafeOnTimer;
            _MediaElement.Dispatcher.Invoke(DispatcherPriority.DataBind, time);
        }


    }
}
