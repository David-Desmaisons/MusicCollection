using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Utilies;

namespace MusicCollectionWPF.ViewModel
{
    public class TrackView : NotifyCompleteListenerObject, IEditableObject, INotifyPropertyChanged
    {
        private ITrack _Track;

        internal ITrack Track
        {
            get { return _Track; }
        }

        private ISingleTrackEditor _Editor;

        private TrackView(ITrack track)
        {
            _Track = track;
        }

        public IAlbum Album
        {
            get { return _Track.Album; }
        }

        #region Album related

        public DateTime? LastPLayed
        {
            get { return Get<TrackView, DateTime?>(() => (t) => t._Track.LastPlayed); }
        }

        public string AlbumAuthor
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Album.Author); }
            set { _Editor.Author = value; }
        }

        public string AlbumName
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Album.Name); }
            set { _Editor.NameOption.Choosed = value; }
        }

        public string AlbumGenre
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Album.Genre); }
            set { _Editor.GenreOption.Choosed = value; }
        }

        public int AlbumYear
        {
            get { return Get<TrackView, int>(() => (t) => t._Track.Album.Year); }
            set { _Editor.YearOption.Choosed = value; }
        }

        #endregion

        #region Track

        public string Artist
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Artist); }
            set { _Editor.Artist = value; }
        }

        public string Name
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Name); }
            set { _Editor.Name = value; }
        }

        public uint TrackNumber
        {
            get { return Get<TrackView, uint>(() => (t) => t._Track.TrackNumber); }
            set { _Editor.TrackNumber = value; }
        }

        public uint DiscNumber
        {
            get { return Get<TrackView, uint>(() => (t) => t._Track.DiscNumber); }
            set { _Editor.DiscNumber = value; }
        }

        public string Path
        {
            get { return Get<TrackView, string>(() => (t) => t._Track.Path); }
        }

        public int PlayCount
        {
            get { return Get<TrackView, int>(() => (t) => t._Track.PlayCount); }
        }

        public TimeSpan Duration
        {
            get { return Get<TrackView, TimeSpan>(() => (t) => t._Track.Duration); }
        }

        public int SkippedCount
        {
            get { return Get<TrackView, int>(() => (t) => t._Track.SkippedCount); }
        }

        public FileStatus FileExists
        {
            get { return Get<TrackView, FileStatus>(() => (t) => t._Track.FileExists); }
        }

        public uint Rating
        {
            get { return Get<TrackView, uint>(() => (t) => t._Track.Rating); }
            set {  _Track.Rating = value; }
        }

        #endregion

        public DateTime? DateAdded
        {
            get { return _Track.DateAdded; }
        }

        public bool IsAlive
        {
            get { return Get<TrackView, bool>(() => (t) => t._Track.IsAlive); }
        }
     
        public void BeginEdit()
        {
            if (_Editor == null)
            {
                _Editor = _Track.GetEditor();
            }
        }

        public void CancelEdit()
        {
            if (_Editor != null)
            {
                _Editor.Cancel();
                _Editor = null;
            }
        }

        public void EndEdit()
        {
            if (_Editor != null)
            {
                //_Editor.CommitChanges(true);
                _Editor.Commit();
                _Editor.Dispose();
                _Editor = null;
            }
        }

        public override bool Equals(object obj)
        {
            TrackView tv = obj as TrackView;
            if (tv == null)
                return false;

            return object.ReferenceEquals(_Track, tv._Track);
        }

        public override int GetHashCode()
        {
            return _Track.GetHashCode();
        }

        public void PrefixArtistName()
        {
            BeginEdit();
            Name = string.Format("{0}-{1}", Album.Artists.GetDisplayName(), Name);
            EndEdit();
        }

        public void RemoveTrackNumber()
        {
            StringTrackParser stp = new StringTrackParser(Name, false);
            if (stp.FounSomething)
            {
                BeginEdit();
                Name = stp.TrackName;
                if ((TrackNumber == 0) && (stp.TrackNumber != null))
                    TrackNumber = (uint)stp.TrackNumber;
                EndEdit();
            }
        }

        static private SortedDictionary<ITrack, TrackView> _Dic = new SortedDictionary<ITrack, TrackView>();

        static public TrackView GetTrackView(ITrack track)
        {
            return _Dic.FindOrCreateEntity(track, t => new TrackView(t));
        }

        public override void Dispose()
        {
            base.Dispose();
            _Dic.Remove(this._Track);
        }
           

        public override string ToString()
        {
            return _Track.ToString();
        }
    }
}
