using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MusicCollection.Fundation
{
    public enum PlayMode 
    { 
        Play, 
        
        Paused, 
        
        Stopped 
    };

    public interface IMusicPlayer : INotifyPropertyChanged, IDisposable
    {
        PlayMode Mode { get; set; }

        IReadOnlyPlayList PlayList { get; set; }

        IAlbumPlayList AlbumPlayList { get; set; }

        ITrack MusicTrackSource { get; }

        TimeSpan Position { get; set; }

        TimeSpan MaxPosition { get; }

        double Volume { get; set; }

        event EventHandler<MusicTrackEventArgs> TrackEvent;
        event EventHandler<MusicTrackPlayingEventArgs> TrackPlaying;
   
    }
}
