using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Utilies;

namespace MusicCollectionTest.ToolBox.FunctionListener
{

    public class SimpleNewTrackViewer : ProprietyListener, IEditableObject, INotifyPropertyChanged
    {
        private ITrack _Track;

        internal ITrack Track
        {
            get { return _Track; }
        }

        private ISingleTrackEditor _Editor;

        private SimpleNewTrackViewer(ITrack track)
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
            get { return RegisterDinamic(() => _Track.LastPlayed); }
        }

        public string AlbumAuthor
        {
            get { return RegisterDinamic(() => _Track.Album.Author); }
            set { _Editor.AutorOption.Choosed = value; }
        }

        public string AlbumName
        {
            get { return RegisterDinamic(() => _Track.Album.Name); }
            set { _Editor.NameOption.Choosed = value; }
        }

        public string AlbumGenre
        {
            get { return RegisterDinamic(() => _Track.Album.Genre); }
            set { _Editor.GenreOption.Choosed = value; }
        }

        public int AlbumYear
        {
            get { return RegisterDinamic(() => _Track.Album.Year); }
            set { _Editor.YearOption.Choosed = value; }
        }

        #endregion

        #region Track

        public string Artist
        {
            get { return RegisterDinamic(() => _Track.Artist); }
            set { _Editor.Artist = value; }
        }

        public string Name
        {
            get { return RegisterDinamic(() => _Track.Name); }
            set { _Editor.Name = value; }
        }

        public uint TrackNumber
        {
            get { return RegisterDinamic(() => _Track.TrackNumber); }
            set { _Editor.TrackNumber = value; }
        }

        public uint DiscNumber
        {
            get { return RegisterDinamic(() => _Track.DiscNumber); }
            set { _Editor.DiscNumber = value; }
        }

        public string Path
        {
            get { return RegisterDinamic(() => _Track.Path); }
        }

        public int PlayCount
        {
            get { return RegisterDinamic(() => _Track.PlayCount); }
        }
        
        public TimeSpan Duration
        {
            get { return RegisterDinamic(() => _Track.Duration); }
        }

        public int SkippedCount
        {
            get { return RegisterDinamic(() => _Track.SkippedCount); }
        }

        public FileStatus FileExists
        {
            get { return RegisterDinamic(() => _Track.FileExists); }
        }

        public uint Rating
        {
            get
            {
                return RegisterDinamic(() => _Track.Rating); 
            }
            set
            {
                _Track.Rating = value;
            }
        }

        #endregion
    
        public DateTime? DateAdded
        {
            get { return _Track.DateAdded; }
        }

        public bool IsAlive
        {
            get { return RegisterDinamic(() => _Track.IsAlive); }
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
                _Editor.Commit();
                _Editor.Dispose();
                _Editor = null;
            }
        }

        public override bool Equals(object obj)
        {
            SimpleNewTrackViewer tv = obj as SimpleNewTrackViewer;
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

        static private Lazy<SortedDictionary<ITrack, SimpleNewTrackViewer>> _Dic = new Lazy<SortedDictionary<ITrack, SimpleNewTrackViewer>>();

        static public SimpleNewTrackViewer GetTrackView(ITrack track)
        {
            return _Dic.Value.FindOrCreateEntity(track, t => new SimpleNewTrackViewer(t));
        }

        public override void Dispose()
        {
            base.Dispose();
            _Dic.Value.Remove(this._Track);
        }

        public override string ToString()
        {
            return _Track.ToString();
        }
    }

}
