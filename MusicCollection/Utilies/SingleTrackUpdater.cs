using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.Utilies
{
    internal class SingleTrackUpdater : AlbumInfoEditor, ITrackMetaDataDescriptor, ISingleTrackEditor
    {

        private Track _Tr;
        private bool _SomethingChanged = false;

        internal SingleTrackUpdater(Track track, IMusicSession iContext):base(track.SingleItemCollection(), iContext)
        {
            _Tr = track;
            if (_Tr == null)
                throw new Exception("Track can not be null");

            this._Artist = _Tr.Artist;
            this._Name= _Tr.Name;
            this._TN= _Tr.TrackNumber;
            this._DiscNumber = _Tr.DiscNumber;
        }

        protected override void UpdateTrack(Track tbd)
        {
            if (tbd != _Tr)
                return;

            tbd.UpdateTrackOnly(this);
        }

        protected override bool SingleTrackUpdateNeeded
        {
            get { return _SomethingChanged; }
        }

        #region ITrackMetaDataDescriptor

        public IAlbumDescriptor AlbumDescriptor
        {
            get { return AlbumTargets[0]; }
        }

        private void TrackPropertyChanged(string PN)
        {
            PropertyHasChanged(PN);
            _SomethingChanged = true;
        }

        private string _Artist;
        public string Artist
        {
            get { return _Artist; }
            set
            {
                if (_Artist == value)
                    return;

                _Artist = value;
                TrackPropertyChanged("Artist");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                TrackPropertyChanged("Name");
            }
        }

        private uint _TN;
        public uint TrackNumber
        {
            get { return _TN; }
            set
            {
                if (_TN == value)
                    return;

                _TN = value;
                TrackPropertyChanged("TrackNumber");
            }
        }

        private uint _DiscNumber;
        public uint DiscNumber
        {
            get { return _DiscNumber; }
            set
            {
                if (_DiscNumber == value)
                    return;

                _DiscNumber = value;
                TrackPropertyChanged("DiscNumber");
            }
        }


        public TimeSpan Duration
        {
            get { return _Tr.Duration; }
        }

        //public ISRC ISRC
        //{
        //    get { return _Tr.ISRC; }
        //}

        #endregion
    }
}
