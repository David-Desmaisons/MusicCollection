using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.ViewModel.Filter
{
    class NoFilter : IMusicFilter
    {
        public FilterType Type
        {
            get { return FilterType.All; }
        }

        public string DisplayName
        {
            get { return string.Empty; }
        }

        public string GetDisplayName(object option)
        {
            return string.Empty;
        }

        public Expression<Func<MusicCollection.Fundation.IAlbum, bool>> AlbumFilter
        {
            get { return a => true; }
        }

        public Expression<Func<MusicCollection.Fundation.ITrack, bool>> TrackFilter
        {
            get { return t => true; }
        }

        public void Dispose()
        {
        }

        public event EventHandler<EventArgs> OnFilterReset
        {
            add { }
            remove { }
        }
    }
}
