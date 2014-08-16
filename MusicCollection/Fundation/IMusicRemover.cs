using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMusicRemover:IDisposable
    {
        IList<IAlbum> AlbumtoRemove { get; }

        IList<ITrack> TrackRemove { get; }

        bool? IncludePhysicalRemove { get; set; }

        bool IsValid { get; }

        void Comit();

        Task ComitAsync();

        void Cancel();

        //event EventHandler<EventArgs> Completed;
    }
}
