using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MusicCollection.MusicPlayer
{
    public interface IInternalPlayerListener
    {
        void OnBroken();

        void OnTrackEnd();

        void OnTrackLoadedPlay();

        void OnVolumeChange();

        void OnTrackLoadingForPlay();

        void OnTrackPlayingEvent(TimeSpan Position, TimeSpan MaxPoistion);
    }

    public interface IInternalPlayer : IDisposable
    {

        IInternalPlayerListener Listener
        {
            set;
            get;
        }

        string FileSource
        {
            set;
            get;
        }

        void Stop();

        void Play();

        void Pause();

        void Close();

        TimeSpan Position
        {
            get;
            set;
        }

        double Volume
        {
            get;
            set;
        }

        TimeSpan MaxPosition
        {
            get;
        }
    }
}
