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
      //  NotifyCompleteAdapterWithCache, IDisposable, INotifyPropertyChanged, IObjectBuildAttributeListener
    //IComparable
    {
        private ITrack _Track;

        internal ITrack Track
        {
            get { return _Track; }
        }

        private ISingleTrackEditor _Editor;

        //private IResultListener<string> _AlbumNamer;
        //private IResultListener<string> _AlbumGenre;
        //private IResultListener<int> _AlbumYear;
        //private IResultListener<string> _AlbumArtist;
        //private IResultListener<string> _TrackNamer;
        //private IResultListener<uint> _TrackNumber;
        //private IResultListener<string> _TrackPather;
        //private IResultListener<string> _TrackArtist;
        //private IResultListener<int> _TrackPlayCount;
        //private IResultListener<uint> _TrackRatinger;
        //private IResultListener<FileStatus> _TrackFileExists;
        //private IResultListener<int> _TrackSkippedCount;
        //private IResultListener<TimeSpan> _TrackDuration;
        //private IResultListener<uint>  _TrackRating;
        //private IResultListener<Nullable<DateTime>> _TrackLastPLayed;
        //private IResultListener<uint>  _TrackDiscNumber;


        //     //      = _TrackFileExistsFactory.CreateListener(this);
        //     //= _TrackSkippedCountFactory.CreateListener(this);
        //     //= _TrackDurationFactory.CreateListener(this);
  
        private TrackView(ITrack track)
        {
            _Track = track;
        }

            //_AlbumNamer = _AlbumNamerFactory.CreateListener(this);
            //_AlbumGenre = _AlbumGenreFactory.CreateListener(this);
            //_AlbumYear = _AlbumYearFactory.CreateListener(this);
            //_AlbumArtist = _AlbumArtistFactory.CreateListener(this);
            //_TrackNamer = _TrackNamerFactory.CreateListener(this);
            //_TrackNumber = _TrackNumberFactory.CreateListener(this);
            //_TrackPather = _TrackPatherFactory.CreateListener(this);
            //_TrackArtist = _TrackArtistFactory.CreateListener(this);
            //_TrackPlayCount = _TrackPlayCountFactory.CreateListener(this);
            //_TrackRatinger = _TrackRatingerFactory.CreateListener(this);
            //_TrackFileExists = _TrackFileExistsFactory.CreateListener(this);
            //_TrackSkippedCount = _TrackSkippedCountFactory.CreateListener(this);
            //_TrackDuration = _TrackDurationFactory.CreateListener(this);
            //_TrackRating = _TrackRatingFactory.CreateListener(this);
            //_TrackLastPLayed = _TrackLastPLayedFactory.CreateListener(this);
            //_TrackDiscNumber = _TrackDiscNumberFactory.CreateListener(this);
        //}

        public IAlbum Album
        {
            get { return _Track.Album; }
        }

        #region Album related

        public DateTime? LastPLayed
        {
            get { return Get<TrackView, DateTime?>(() => (t) => t._Track.LastPlayed); }
            //get { return FullDynamic<NewTrackViewer, DateTime?>(() => ((t) => t._Track.LastPlayed)); }
        }

        public string AlbumAuthor
        {
            //get { return FullDynamic<NewTrackViewer, DateTime?>(() => ((t) => t._Track.LastPlayed)); }
            get { return Get<TrackView, string>(() => (t) => t._Track.Album.Author); }
            set { _Editor.AutorOption.Choosed = value; }
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
            get
            {
                return Get<TrackView, uint>(() => (t) => t._Track.Rating);
            }
            set
            {
                _Track.Rating = value;
            }
        }

        #endregion

        #region old Album related

        //public DateTime? LastPLayed
        //{
        //    get { return this.GetValue(_TrackLastPLayedFactory); }
        //}

        //public string AlbumAuthor
        //{
        //    get { return this.GetValue(_AlbumArtistFactory); }
        //    set { _Editor.AutorOption.Choosed = value; }
        //}

        //public string AlbumName
        //{
        //    get { return this.GetValue(_AlbumNamerFactory); }
        //    set { _Editor.NameOption.Choosed = value; }
        //}

        //public string AlbumGenre
        //{
        //    get { return this.GetValue(_AlbumGenreFactory); }
        //    set { _Editor.GenreOption.Choosed = value; }
        //}

        //public int AlbumYear
        //{
        //    get { return this.GetValue(_AlbumYearFactory); }
        //    set { _Editor.YearOption.Choosed = value; }
        //}

        #endregion

        #region old Track

        //public string Artist
        //{
        //    get { return this.GetValue(_TrackArtistFactory); }
        //    set { _Editor.Artist = value; }
        //}

        //public string Name
        //{
        //    get { return this.GetValue(_TrackNamerFactory); }
        //    set { _Editor.Name = value; }
        //}

        //public uint TrackNumber
        //{
        //    get { return this.GetValue(_TrackNumberFactory); }
        //    set { _Editor.TrackNumber = value; }
        //}

        //public uint DiscNumber
        //{
        //    get { return this.GetValue(_TrackDiscNumberFactory); }
        //    set { _Editor.DiscNumber = value; }
        //}

        //public string Path
        //{
        //    get { return this.GetValue(_TrackPatherFactory); }
        //}

        //public int PlayCount
        //{
        //    get { return this.GetValue(_TrackPlayCountFactory); }
        //}
        
        //public TimeSpan Duration
        //{
        //    get { return this.GetValue(_TrackDurationFactory); }
        //}

        //public int SkippedCount
        //{
        //    get { return this.GetValue(_TrackSkippedCountFactory); }
        //}

        //public FileStatus FileExists
        //{
        //    get { return this.GetValue(_TrackFileExistsFactory); }
        //}

        //public uint Rating
        //{
        //    get
        //    {
        //        return this.GetValue(_TrackRatingFactory);
        //    }
        //    set
        //    {
        //        _Track.Rating = value;
        //    }
        //}

        #endregion

        //static readonly IResultListenerFactory<TrackView, string> _AlbumNamerFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Album.Name, "AlbumName");
        //static readonly IResultListenerFactory<TrackView, string> _AlbumGenreFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Album.Genre, "AlbumGenre");
        //static readonly IResultListenerFactory<TrackView, int> _AlbumYearFactory = ListenerFunctionBuilder.Register<TrackView, int>((t) => t._Track.Album.Year, "AlbumYear");
        //static readonly IResultListenerFactory<TrackView, string> _AlbumArtistFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Album.Author, "AlbumAuthor");
        //static readonly IResultListenerFactory<TrackView, string> _TrackNamerFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Name, "Name");
        //static readonly IResultListenerFactory<TrackView, uint> _TrackNumberFactory = ListenerFunctionBuilder.Register<TrackView, uint>((t) => t._Track.TrackNumber, "TrackNumber");
        //static readonly IResultListenerFactory<TrackView, string> _TrackPatherFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Path, "Path");
        //static readonly IResultListenerFactory<TrackView, string> _TrackArtistFactory = ListenerFunctionBuilder.Register<TrackView, string>((t) => t._Track.Artist, "Artist");
        //static readonly IResultListenerFactory<TrackView, int> _TrackPlayCountFactory = ListenerFunctionBuilder.Register<TrackView, int>((t) => t._Track.PlayCount, "PlayCount");
        //static readonly IResultListenerFactory<TrackView, uint> _TrackRatingFactory = ListenerFunctionBuilder.Register<TrackView, uint>((t) => t._Track.Rating, "Rating");
        //static readonly IResultListenerFactory<TrackView, TimeSpan> _TrackDurationFactory = ListenerFunctionBuilder.Register<TrackView, TimeSpan>((t) => t._Track.Duration, "Duration");
        //static readonly IResultListenerFactory<TrackView, int> _TrackSkippedCountFactory = ListenerFunctionBuilder.Register<TrackView, int>((t) => t._Track.SkippedCount, "SkippedCount");
        //static readonly IResultListenerFactory<TrackView, FileStatus> _TrackFileExistsFactory = ListenerFunctionBuilder.Register<TrackView, FileStatus>((t) => t._Track.FileExists, "FileStatus");
        //static readonly IResultListenerFactory<TrackView, Nullable<DateTime>> _TrackLastPLayedFactory = ListenerFunctionBuilder.Register<TrackView, Nullable<DateTime>>((t) => t._Track.LastPlayed, "LastPLayed");
        //static readonly IResultListenerFactory<TrackView, uint> _TrackDiscNumberFactory = ListenerFunctionBuilder.Register<TrackView, uint>((t) => t._Track.DiscNumber, "DiscNumber");
        //static readonly IResultListenerFactory<TrackView, bool> _TrackIsAliveNumberFactory = ListenerFunctionBuilder.Register<TrackView, bool>((t) => t._Track.IsAlive, "IsAlive");

        //static readonly IResultListenerFactory<TrackView, uint> _TrackRatingFactory;


        //static TrackView()
        //{
        //    //_AlbumNamerFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Album.Name, "AlbumName");
        //    //_AlbumGenreFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Album.Genre, "AlbumGenre");
        //    //_AlbumYearFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, int>((t) => t._Track.Album.Year, "AlbumYear");
        //    //_AlbumArtistFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Album.Author, "AlbumAuthor");
        //    //_TrackNamerFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Name, "Name");
        //    //_TrackNumberFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, uint>((t) => t._Track.TrackNumber, "TrackNumber");
        //    //_TrackPatherFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Path, "Path");
        //    //_TrackArtistFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, string>((t) => t._Track.Artist, "Artist");
        //    //_TrackPlayCountFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, int>((t) => t._Track.PlayCount, "PlayCount");
        //    ////_TrackRatingerFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, uint>((t) => t._Track.Rating, "Rating");
        //    //_TrackDurationFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, TimeSpan>((t) => t._Track.Duration, "Duration");
        //    //_TrackSkippedCountFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, int>((t) => t._Track.SkippedCount, "SkippedCount");
        //    //_TrackFileExistsFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, FileStatus>((t) => t._Track.FileExists, "FileStatus");
        //    //_TrackRatingFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, uint>((t) => t._Track.Rating, "Rating");
        //    //_TrackLastPLayedFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, Nullable<DateTime>>((t) => t._Track.LastPlayed, "LastPLayed");
        //    //_TrackDiscNumberFactory = ListenerFunctionBuilder.BuildFunctionListenerFactory<TrackView, uint>((t) => t._Track.DiscNumber, "DiscNumber");
        //}

        public DateTime? DateAdded
        {
            get { return _Track.DateAdded; }
        }

        //public bool IsAlive
        //{
        //    get { return this.GetValue(_TrackIsAliveNumberFactory); }
        //}

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
                _Editor.Dispose();
                _Editor = null;
            }
        }

        public void EndEdit()
        {
            if (_Editor != null)
            {
                _Editor.CommitChanges(true);
                _Editor.Dispose();
                _Editor = null;
            }
        }

        //internal bool IsTrackFiltered(Predicate<Object> RawFilter)
        //{
        //    return RawFilter(_Track);
        //}

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

        //public void AttributeChanged(string AttributeName, object oldv, object newv)
        //{
        //    OnElementChanged(this, new PropertyChangedEventArgs(AttributeName));
        //}

        static private SortedDictionary<ITrack, TrackView> _Dic = new SortedDictionary<ITrack, TrackView>();

        static public TrackView GetTrackView(ITrack track)
        {
            return _Dic.FindOrCreateEntity(track, t => new TrackView(t));
        }

        //private bool _IsDisposed = false;
        public override void Dispose()
        {
            base.Dispose();
            _Dic.Remove(this._Track);
        }
            //if (!_IsDisposed)
            //{
            //    _IsDisposed = true;
            //    _AlbumNamer.Dispose();
            //    _AlbumGenre.Dispose();
            //    _AlbumYear.Dispose();
            //    _AlbumArtist.Dispose();
            //    _TrackNamer.Dispose();
            //    _TrackNumber.Dispose();
            //    _TrackPather.Dispose();
            //    _TrackArtist.Dispose();
            //    _TrackPlayCount.Dispose();
            //    _TrackRatinger.Dispose();
            //    _TrackFileExists.Dispose(); 
            //    _TrackSkippedCount.Dispose();
            //    _TrackDuration.Dispose();
            //    _TrackRating.Dispose();
            //    _TrackLastPLayed.Dispose();  
            //}
        //}

        public override string ToString()
        {
            return _Track.ToString();
        }

        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        //{
        //    add { PropertyChanged+=value; }
        //    remove { PropertyChanged-=value; }
        //}
    }
}
