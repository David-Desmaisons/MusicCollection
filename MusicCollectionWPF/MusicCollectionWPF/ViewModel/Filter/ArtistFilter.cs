using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel.Filter
{
    public class FilterAdaptor<T> : NotifyCompleteListenerObject where T : IObjectState
    {
        protected T _Filter;
        protected FilterAdaptor(T value)
        {
            _Filter = value;
            _Filter.ObjectStateChanges += FilterObjectChanged;
        }

        private void FilterObjectChanged(object sender, ObjectStateChangeArgs iObjectStateChanges)
        {
            if ((iObjectStateChanges.NewState == ObjectState.Removed) || (iObjectStateChanges.NewState == ObjectState.UnderRemove))
            {
                EventHandler<EventArgs> ofr = OnFilterReset;
                if (ofr != null)
                    ofr(this, null);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _Filter.ObjectStateChanges -= FilterObjectChanged;
        }

        public event EventHandler<EventArgs> OnFilterReset;
    }


    public class ArtistFilter : FilterAdaptor<IArtist>, IMusicFilter
    {
        public ArtistFilter(IArtist ar):base(ar)
        {
        }

        public FilterType Type
        {
            get { return FilterType.Artist; }
        }

        public Expression<Func<IAlbum, bool>> AlbumFilter
        {
            get { return (a) => a.Artists.Contains(_Filter); }
        }

        public Expression<Func<ITrack, bool>> TrackFilter
        {
            get { return (t) => t.Album.Artists.Contains(_Filter); }
        }

        public string DisplayName
        {
            get { return _Filter.Name; }
        }
    }

    public class AlbumFiltering : FilterAdaptor<IAlbum>, IMusicFilter
    {
        public AlbumFiltering(IAlbum al):base(al)
        {
        }

        public Expression<Func<IAlbum, bool>> AlbumFilter
        {
            get { return a => a == _Filter; }
        }

        public Expression<Func<ITrack, bool>> TrackFilter
        {
            get { return t => t.Album == _Filter; }
        }

        public FilterType Type
        {
            get { return FilterType.Name; }
        }

        //static readonly IResultListenerFactory<AlbumFiltering, string> _AlbumNamerFactory = ListenerFunctionBuilder.Register<AlbumFiltering, string>((t) => t._Filter.Name, "DisplayName");

        public string DisplayName
        {
            //get { return this.GetValue(_AlbumNamerFactory); }
            get { return Get<AlbumFiltering, string>(() => (t) => t._Filter.Name); }
        }
    }

    public class TrackFiltering : FilterAdaptor<ITrack>, IMusicFilter
    {
        public TrackFiltering(ITrack al):base(al)
        {
        }

        public Expression<Func<IAlbum, bool>> AlbumFilter
        {
            get { return a => a == _Filter.Album; }
        }

        public Expression<Func<ITrack, bool>> TrackFilter
        {
            get { return t => t == _Filter; }
        }

        public FilterType Type
        {
            get { return FilterType.Track; }
        }

        //static readonly IResultListenerFactory<TrackFiltering, string> _TrackNamerFactory = ListenerFunctionBuilder.Register<TrackFiltering, string>((t) => t._Filter.Name, "DisplayName");

        public string DisplayName
        {
            get { return Get<TrackFiltering, string>(() => (t) => t._Filter.Name); }
            //get { return this.GetValue(_TrackNamerFactory); }
        }
    }


    //public class YearFilter : GenericFilter<int>
    //{
    //    public override Expression<Func<IAlbum, bool>> AlbumFilter
    //    {
    //        get { return a => a.Year == _Filter; }
    //    }

    //    public override Expression<Func<ITrack, bool>> TrackFilter
    //    {
    //        get { return t => t.Album.Year == _Filter; }
    //    }

    //    public override FilterType Type
    //    {
    //        get { return FilterType.Year; }
    //    }

    //    public override string GetDisplayName(int o)
    //    {
    //        return o.ToString();
    //    }
    //}

    //public class GenreFilter : GenericFilter<IGenre>
    //{
    //    public override Expression<Func<IAlbum, bool>> AlbumFilter
    //    {
    //        get { return a => a.MainGenre == _Filter; }
    //    }

    //    public override Expression<Func<ITrack, bool>> TrackFilter
    //    {
    //        get { return t => t.Album.MainGenre == _Filter; }
    //    }

    //    public override FilterType Type
    //    {
    //        get { return FilterType.Genre; }
    //    }

    //    public override string GetDisplayName(IGenre o)
    //    {
    //        return o.FullName;
    //    }
    //}


    //public class MaturityFilter : GenericFilter<AlbumMaturity>
    //{
    //    public override Expression<Func<IAlbum, bool>> AlbumFilter
    //    {
    //        get { return a => a.Maturity == _Filter; }
    //    }

    //    public override Expression<Func<ITrack, bool>> TrackFilter
    //    {
    //        get { return t => t.Album.Maturity == _Filter; }
    //    }

    //    public override FilterType Type
    //    {
    //        get { return FilterType.Maturity; }
    //    }

    //    public override string GetDisplayName(AlbumMaturity o)
    //    {
    //        return o.ToString();
    //    }
    //}


}
