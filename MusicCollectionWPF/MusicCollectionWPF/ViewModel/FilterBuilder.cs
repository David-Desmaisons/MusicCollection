using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.ViewModel.Filter;

namespace MusicCollectionWPF.ViewModel
{
    public class FilterBuilder : NotifyCompleteListenerObject
    {
        public FilterBuilder()
        {
        }

        public FilterView Filter {get;set;}
     
        private object _Filter;
        public object FilterObject
        {
            get { return _Filter; }
            set
            {
                _Filter = value;
                this.PropertyHasChangedUIOnly("AlbumFilter");
                this.PropertyHasChangedUIOnly("TrackFilter");
                this.PropertyHasChangedUIOnly("ArtistFilter");

                if (ArtistFilter != null)
                {
                    FilterEntity = new ArtistFilter(ArtistFilter);
                    return;
                }

                if (TrackFilter != null)
                {
                    FilterEntity = new TrackFiltering(TrackFilter);
                    return;
                }

                if (AlbumFilter != null)
                {
                    FilterEntity = new AlbumFiltering(AlbumFilter);
                    return;
                }

                FilterEntity = new NoFilter();
            }
        }

        public IArtist ArtistFilter
        {
            get { return _Filter as IArtist; }
            set
            {
                FilterObject = value;
            }
        }

        public IAlbum AlbumFilter
        {
            get { return _Filter as IAlbum; }
            set
            {
                FilterObject = value;
            }
        }

        public ITrack TrackFilter
        {
            get { return _Filter as ITrack; }
            set
            {
                FilterObject = value;
            }
        }

        private IMusicFilter _FilterEntity = new NoFilter();
        public IMusicFilter FilterEntity
        {
            get { return _FilterEntity; }
            set
            {
                if (_FilterEntity==value)
                    return;  
                
                if (_FilterEntity != null)
                {
                    _FilterEntity.OnFilterReset -= _FO_OnFilterReset;
                }

                _FilterEntity = value; 
                _FilterEntity.OnFilterReset+=_FO_OnFilterReset;

                if (Filter!=null)
                    Filter.FilteringObject = _FilterEntity;

                this.PropertyHasChangedUIOnly("FilterEntity");
            }
        }            

        private void _FO_OnFilterReset(object sender, EventArgs ea)
        {
            FilterObject = null;
        }
    }
}
