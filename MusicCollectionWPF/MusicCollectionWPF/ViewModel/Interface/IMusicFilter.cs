using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel
{
    public interface IMusicFilter: IDisposable
    {
        FilterType Type
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        Expression<Func<IAlbum, bool>> AlbumFilter
        {
            get;
        }

        Expression<Func<ITrack, bool>> TrackFilter
        {
            get;
        }

        event EventHandler<EventArgs> OnFilterReset;
    }

   
}
