using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.Implementation.Modifier
{
    internal class TrackModifier : IModifiableTrack
    {
        private Track _Track;
        private IInternalAlbumModifier _IMA = null;
        private string _Name = null;
        private string _Artist = null;
        private uint? _TrackNumber = null;
        private uint? _DiscNumber = null;
        private bool _Dirty = false;
        private bool _NeedToUpdateFile = false;
        //private bool _MarkedAsRemoved=false;
        private IImportContext _Context;

        const string _TrackNumberProperty = "TrackNumber";
        const string _ArtistProperty = "Artist";
        const string _NameProperty = "Name";
        const string _RatingProperty = "Rating";
        const string _DiscNumberProperty = "DiscNumber";
        
        public event PropertyChangedEventHandler PropertyChanged;


        internal bool NeedToUpdateFile
        {
            get
            {
                if (_NeedToUpdateFile)
                    return true;

                return _IMA.NeedToUpdateFile;
            }
        }

        internal Track Track { get {return _Track;}}

        internal string NewName { get {return _Name;}}

        internal string NewArtist { get {return _Artist;}}
       
        internal uint? NewDiscNumber { get {return _DiscNumber;}}

        internal uint? NewTrackNumber { get {return _TrackNumber;}}

        internal uint? NewRating { get {return _Rating;}}
      
        internal TrackModifier(Track Track, IInternalAlbumModifier IMA)
        {
            _Track = Track;
            _IMA = IMA;
            _Context = IMA.Context;
        }

        internal IInternalAlbumModifier Album { get {return _IMA;}}
      
        public bool IsModified { get {return _Dirty;}}

        private void PropertyHasChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));

            _Dirty = true;
            if (PropertyName!=_RatingProperty)
                _NeedToUpdateFile = true;
        }

        public uint DiscNumber
        {
            set
            {
                if (_DiscNumber == value)
                    return;

                _DiscNumber = value;
                PropertyHasChanged(_DiscNumberProperty);
            }
            get
            {
                if (_DiscNumber != null)
                    return (uint)_DiscNumber;

                return _Track.DiscNumber;
            }
        }

        public string Artist
        {
            set
            {
                if (_Artist == value)
                    return;

                _Artist = value;
                PropertyHasChanged(_ArtistProperty);
            }
            get
            {
                if (_Artist != null)
                    return _Artist;

                return _Track.Artist;
            }
        }

        public string Name
        {
            set
            {
                _Name = value;
                PropertyHasChanged(_NameProperty);
            }
            get
            {
                if (_Name != null)
                    return _Name;

                return _Track.Name;
            }
        }

        public uint TrackNumber
        {
            set
            {
                //if (value <= _Track.RawAlbum.EffectiveTrackNumber)
                _TrackNumber = value;

                PropertyHasChanged(_TrackNumberProperty);
            }
            get
            {
                if (_TrackNumber != null)
                    return (uint)_TrackNumber;

                return _Track.TrackNumber;
            }
        }

        private uint? _Rating;

        public uint Rating
        {
            set
            {
                if (value == Rating)
                    return;

                if ((value <=5) && (value>=0))
                    _Rating = value;

                PropertyHasChanged(_RatingProperty);
            }
            get
            {
                if (_Rating != null)
                    return (uint)_Rating;

                return _Track.Rating;
            }
        }

        public TimeSpan Duration
        {
            get { return _Track.Duration; }
        }

        public string Path
        {
            get { return _Track.Path; }
        }

        public void Delete()
        {
            _Dirty = true;
            //_MarkedAsRemoved = true;
            _IMA.Remove(this);
        }

        internal bool CommitChanges()
        {
            if ((IsModified) || (_IMA.IsAlbumOnlyModified))
            {
                return _Track.Modify(this, _Context);
            }
            return true;
        }

        public ObjectState State
        {
            get { return _Track.InternalState; }
        }

        public IModifiableAlbum Father
        {
            get { return _IMA; }
        }
    }
}
