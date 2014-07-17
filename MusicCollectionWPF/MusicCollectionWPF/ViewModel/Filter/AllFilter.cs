//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//using MusicCollection.Fundation;
//using MusicCollection.Infra;

//namespace MusicCollectionWPF.ViewModel.Filter
//{
//    public class AllFilter : NotifyCompleteAdapterNoCache, IMusicFilter
//    {

//        public AllFilter()
//        {
//            _FV = string.Empty;
//            _FFV = string.Empty;
//        }

//        public AllFilter(string filter)
//        {
//            _FV = filter;
//            _FFV = filter.ToLower().WithoutAccent();
//        }

//        public FilterType Type
//        {
//            get { return FilterType.All; }
//        }


//        public string GetDisplayName(object option)
//        {
//            return null;
//        }


//        private string _FV;
//        private string _FFV;

//        public string DisplayName
//        {
//            get { return _FV; }
//        }

//        public string Filter
//        {
//            get { return _FV; }
//            set
//            {
//                if (value == null)
//                    value = string.Empty;

//                if (value == _FV)
//                    return;

//                var old = _FV;
//                _FV = value;
//                this.PropertyHasChanged("Filter", old, _FV);

//                RealFilterValue = value.ToLower().WithoutAccent();
//            }
//        }

//        private string RealFilterValue
//        {
//            get { return _FFV; }
//            set
//            {
//                if (value == _FFV)
//                    return;

//                var old = _FFV;
//                _FFV = value;
//                this.PropertyHasChanged("RealFilterValue", old, _FFV);

//            }
//        }

//        public Expression<Func<IAlbum, bool>> AlbumFilter
//        {
//            get { return (a) => a.NormalizedName.FastContains(RealFilterValue) || a.Author.ToLower().WithoutAccent().FastContains(RealFilterValue); }
//        }

//        public Expression<Func<ITrack, bool>> TrackFilter
//        {
//            get { return (t) => t.Album.NormalizedName.FastContains(RealFilterValue) || (t.Album.Author.WithoutAccent().IndexOf(RealFilterValue, StringComparison.InvariantCultureIgnoreCase) >= 0) || (t.Name.WithoutAccent().IndexOf(RealFilterValue, StringComparison.InvariantCultureIgnoreCase) >= 0); }
//        }

//        public void Dispose()
//        {
//        }

//        public event EventHandler<EventArgs> OnFilterReset
//        {
//            add { }
//            remove { }
//        }
//    }
//}
