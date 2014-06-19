//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;

//using MusicCollection.Fundation;
//using MusicCollection.Infra;
//using MusicCollection.Utilies;

//namespace MusicCollectionTest.ToolBox.FunctionListener
//{
   
//    public class NewTrackViewer : NotifyCompleteListenerObject, IEditableObject, INotifyPropertyChanged
//    {
//        private ITrack _Track;

//        internal ITrack Track
//        {
//            get { return _Track; }
//        }

//        private ISingleTrackEditor _Editor;

//        private NewTrackViewer(ITrack track)
//        {
//            _Track = track;
//        }

//        public IAlbum Album
//        {
//            get { return _Track.Album; }
//        }

//        #region Album related

//        public DateTime? LastPLayed
//        {
//            get { return Dynamic<NewTrackViewer, DateTime?>(()=>(t) => t._Track.LastPlayed); }
//            //get { return FullDynamic<NewTrackViewer, DateTime?>(() => ((t) => t._Track.LastPlayed)); }
//        }

//        public string AlbumAuthor
//        {
//            //get { return FullDynamic<NewTrackViewer, DateTime?>(() => ((t) => t._Track.LastPlayed)); }
//            get { return Dynamic<NewTrackViewer, string>(()=>(t) => t._Track.Album.Author); }
//            set { _Editor.AutorOption.Choosed = value; }
//        }

//        public string AlbumName
//        {
//            get { return Dynamic<NewTrackViewer, string>(()=>(t) => t._Track.Album.Name); }
//            set { _Editor.NameOption.Choosed = value; }
//        }

//        public string AlbumGenre
//        {
//            get { return Dynamic<NewTrackViewer,string>(()=>(t) => t._Track.Album.Genre); }
//            set { _Editor.GenreOption.Choosed = value; }
//        }

//        public int AlbumYear
//        {
//            get { return Dynamic<NewTrackViewer, int>(()=>(t) => t._Track.Album.Year); }
//            set { _Editor.YearOption.Choosed = value; }
//        }

//        #endregion

//        #region Track

//        public string Artist
//        {
//            get { return Dynamic<NewTrackViewer, string>(()=>(t) => t._Track.Artist); }
//            set { _Editor.Artist = value; }
//        }

//        public string Name
//        {
//            get { return Dynamic<NewTrackViewer,string>(()=>(t) => t._Track.Name); }
//            set { _Editor.Name = value; }
//        }

//        public uint TrackNumber
//        {
//            get { return Dynamic<NewTrackViewer, uint>(()=>(t) => t._Track.TrackNumber); }
//            set { _Editor.TrackNumber = value; }
//        }

//        public uint DiscNumber
//        {
//            get { return Dynamic<NewTrackViewer, uint>(()=>(t) => t._Track.DiscNumber); }
//            set { _Editor.DiscNumber = value; }
//        }

//        public string Path
//        {
//            get { return Dynamic<NewTrackViewer, string>(() => (t) => t._Track.Path); }
//        }

//        public int PlayCount
//        {
//            get { return Dynamic<NewTrackViewer, int>(() => (t) => t._Track.PlayCount); }
//        }
        
//        public TimeSpan Duration
//        {
//            get { return Dynamic<NewTrackViewer, TimeSpan>(() => (t) => t._Track.Duration); }
//        }

//        public int SkippedCount
//        {
//            get { return Dynamic<NewTrackViewer, int>(() => (t) => t._Track.SkippedCount); }
//        }

//        public FileStatus FileExists
//        {
//            get { return Dynamic<NewTrackViewer, FileStatus>(() => (t) => t._Track.FileExists); }
//        }

//        public uint Rating
//        {
//            get
//            {
//                return Dynamic<NewTrackViewer, uint>(() => (t) => t._Track.Rating); 
//            }
//            set
//            {
//                _Track.Rating = value;
//            }
//        }

//        #endregion
    
//        public DateTime? DateAdded
//        {
//            get { return _Track.DateAdded; }
//        }

//        public bool IsAlive
//        {
//            get { return Dynamic<NewTrackViewer, bool>(()=>(t) => t._Track.IsAlive); }
//        }
  
//        public void BeginEdit()
//        {
//            if (_Editor == null)
//            {
//                _Editor = _Track.GetEditor();
//            }
//        }

//        public void CancelEdit()
//        {
//            if (_Editor != null)
//            {
//                _Editor.Cancel();
//                _Editor.Dispose();
//                _Editor = null;
//            }
//        }

//        public void EndEdit()
//        {
//            if (_Editor != null)
//            {
//                _Editor.CommitChanges(true);
//                _Editor.Dispose();
//                _Editor = null;
//            }
//        }

//        internal bool IsTrackFiltered(Predicate<Object> RawFilter)
//        {
//            return RawFilter(_Track);
//        }

//        public override bool Equals(object obj)
//        {
//            NewTrackViewer tv = obj as NewTrackViewer;
//            if (tv == null)
//                return false;

//            return object.ReferenceEquals(_Track, tv._Track);
//        }

//        public override int GetHashCode()
//        {
//            return _Track.GetHashCode();
//        }

//        public void PrefixArtistName()
//        {
//            BeginEdit();
//            Name = string.Format("{0}-{1}", Album.Artists.GetDisplayName(), Name);
//            EndEdit();
//        }

//        public void RemoveTrackNumber()
//        {
//            StringTrackParser stp = new StringTrackParser(Name, false);
//            if (stp.FounSomething)
//            {
//                BeginEdit();
//                Name = stp.TrackName;
//                if ((TrackNumber == 0) && (stp.TrackNumber != null))
//                    TrackNumber = (uint)stp.TrackNumber;
//                EndEdit();
//            }
//        }

//        //static private Lazy<SortedDictionary<ITrack, NewTrackViewer>> _Dic = new Lazy<SortedDictionary<ITrack, NewTrackViewer>>();
//        static private IDictionary<ITrack, NewTrackViewer> _Dic = new Dictionary<ITrack, NewTrackViewer>();


//        static public NewTrackViewer GetTrackView(ITrack track)
//        {
//            return _Dic.FindOrCreateEntity(track, t => new NewTrackViewer(t));
//        }

//        public override void Dispose()
//        {
//            base.Dispose();
//            _Dic.Remove(this._Track);
//        }

//        public override string ToString()
//        {
//            return _Track.ToString();
//        }
//    }

//}
