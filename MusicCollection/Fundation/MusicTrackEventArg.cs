using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public enum TrackPlayingEvent { Loading, BeginPlay, Playing, EndPlay, Broken, Skipped };

//Paused, Stopped

    public class MusicTrackEventArgs : EventArgs
    {
        public TrackPlayingEvent What
        {
            get;
            private set;
        }
 
        public ITrack Track
        {
            get;
            private set;
        }

        public TimeSpan? TotalDuration
        {
            get;
            private set;
        }

        public MusicTrackEventArgs(ITrack track, TrackPlayingEvent what)
        {
            Track = track;
            What = what;
            TotalDuration = null;
        }

        public MusicTrackEventArgs(ITrack track, TrackPlayingEvent what,TimeSpan duration)
        {
            Track = track;
            What = what;
            TotalDuration = duration;
        }
    }

    public class MusicTrackPlayingEventArgs : MusicTrackEventArgs
    {
        public TimeSpan Position
        {
            get;
            private set;
        }

        public TimeSpan MaxPosition
        {
            get;
            private set;
        }

        public MusicTrackPlayingEventArgs(ITrack track, TimeSpan position, TimeSpan iMaxPosition)
            : base(track, TrackPlayingEvent.Playing)
        {
            Position = position;
            MaxPosition = iMaxPosition;
        }
    }
}
