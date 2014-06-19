using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.PlayList
{
    public class PlayListBase : NotifyCompleteAdapterNoCache
    {
        private string _Name;
        private ITrack _Track = null;
        private int _Index = -1;

        protected PlayListBase(string Name)
        {
            _Name = Name;
        }

        public virtual void Dispose()
        {
            _ReadOnlyTracks.CollectionChanged -= ReadOnlyTracks_CollectionChanged;
        }


        #region Event

        public event EventHandler<SelectionChangedargs> SelectionChanged;

        protected void FireSelectionChanged()
        {
            EventHandler<SelectionChangedargs> sc = SelectionChanged;
            if (sc != null)
                sc(this, null);
        }

        #endregion

        #region Basic properties

        private bool _AutoReplay=false;
        public bool AutoReplay
        {
            get { return _AutoReplay; }
            set 
            {
                //if (_AutoReplay == value)
                //    return;

                //bool old = _AutoReplay;
                //_AutoReplay = value;
                //PropertyHasChanged("AutoReplay", old, _AutoReplay);
                Set(ref _AutoReplay, value);
            }           
        }
 

        public string PlayListname
        {
            get
            {
                return _Name;
            }
            set
            {
                //var old = _Name;
                //if (old == value)
                //    return;

                //_Name = value;
                //PropertyHasChanged("PlayListname", old, _Name);
                Set(ref _Name, value);
            }
        }

        #endregion

        private IObservableCollection<ITrack> _ReadOnlyTracks;
        public IObservableCollection<ITrack> ReadOnlyTracks
        {
            get { return _ReadOnlyTracks; }
            protected set
            {
                _ReadOnlyTracks = value;
                _ReadOnlyTracks.CollectionChanged += ReadOnlyTracks_CollectionChanged;
            }
        }

        void ReadOnlyTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_Track == null)
                return;

            int index = _ReadOnlyTracks.IndexOf(_Track);
            if (index < 0)
            {
                PrivateCurrentTrack = null;
                return;
            }

            _Index = index;
        }


        private ITrack PrivateCurrentTrack
        {
            set
            {
                if (_Track == value)
                    return;

                if (value != null)
                {
                    int index = ReadOnlyTracks.IndexOf(value);
                    if (index < 0)
                        return;

                    _Index = index;
                }
                else
                {
                    _Index = -1;
                }

                var old = _Track;
                var oldAlbum = CurrentAlbumItem;

                _Track = value;

                FireSelectionChanged();

                this.PropertyHasChanged("CurrentTrack", old, _Track);
                this.PropertyHasChanged("CurrentAlbumItem", oldAlbum, CurrentAlbumItem);
            }
        }

        public IAlbum CurrentAlbumItem
        {
            get
            {
                return (CurrentTrack == null) ? null : CurrentTrack.Album;
            }
        }

        public ITrack CurrentTrack
        {
            get
            {
                return _Track;
            }
            set
            {
                if (value == null)
                    return;

                PrivateCurrentTrack = value;
            }
        }

        public void Init()
        {
            if (_ReadOnlyTracks.Count == 0)
            {
                _Index = -1;
                CurrentTrack = null;
                return;
            }

            _Index = 0;
            CurrentTrack = _ReadOnlyTracks[0];
        }

        public bool Transition()
        {
            if (_ReadOnlyTracks.Count == 0)
                return false;

            if (_Index == _ReadOnlyTracks.Count - 1)
            {
                if (this.AutoReplay)
                    Init();
                else
                    PrivateCurrentTrack = null;

                return AutoReplay;
            }

            _Index++;
            CurrentTrack = _ReadOnlyTracks[_Index];
            return true;
        }

    }
}
