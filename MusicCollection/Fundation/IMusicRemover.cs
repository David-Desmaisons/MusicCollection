using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMusicRemover:IDisposable
    {
        IList<IAlbum> AlbumtoRemove { get; }

        IList<ITrack> TrackRemove { get; }

        bool? IncludePhysicalRemove { get; set; }

        bool IsValid { get; }

        void Comit(bool Sync);

        void Cancel();

        event EventHandler<EventArgs> Completed;
    }
}
