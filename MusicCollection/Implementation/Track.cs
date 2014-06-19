using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TagLib;
using System.Diagnostics;
using System.ComponentModel;


using MusicCollection.Itunes;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Implementation.Modifier;
using MusicCollection.Utilies;


namespace MusicCollection.Implementation
{
    internal class Track : StateObjectAdapter, IInternalTrack, ITrack, IObjectStateCycle, ISessionPersistentObject, IComparable, IComparable<ITrack>, IComparable<Track>
    {
        private string _Author;
        private string _Name;
        private Album _Album;
        private TimeSpan _Duration;
        private uint _TrackNumber;
        private DateTime? _DateAdded;
        private uint _SessionID;

        static private uint _SessionIDCount;

        const string _TrackNumberProperty = "TrackNumber";
        const string _ArtistProperty = "Artist";
        const string _NameProperty = "Name";
        const string _RatingProperty = "Rating";
        //const string _StateProperty = "State";
        const string _PathProperty = "Path";
        const string _PathHelperProperty = "PathHelper";
        const string _LastPlayedProperty = "LastPlayed";
        const string _PlayCountProperty = "PlayCount";
        const string _DurationProperty = "Duration";
        const string _AlbumProperty = "Album";
        const string _SkippedProperty = "SkippedCount";
        const string _DiscNumberProperty = "DiscNumber";

        private int ID { get; set; }

        internal ITrack Interface { get { return this; } }

        internal Track()
        {
            _SessionID = _SessionIDCount++;
        }

        private Track(Album iAlbum, ITrackDescriptor TD)
            : this()
        {
            _Album = iAlbum;
            _Name = TD.Name.ToMaxLength(200);
            _Author = TD.Artist;
            _Path = TD.Path.ToLower();
            _Duration = TD.Duration;
            _TrackNumber = TD.TrackNumber;
            _DateAdded = DateTime.Now;
            _MD5 = TD.MD5;
            //_ISRC = TD.ISRC == null ? null : TD.ISRC.Name;// ISRCFormat.Format(TD.ISRC);
            _DiscNumber = TD.DiscNumber;

            if (_Album != null)
                _Album.AddTrack(this);
        }

        internal void SetAlbum(Album ial)
        {
            //if ((_Album != null) && (_Album != ial) && (ial != null))
            //    throw new Exception("not supported");

            //if (ial == null)
            //    throw new Exception("not supported");

            if (ial == _Album)
                return;

            var old = _Album;
            _Album = ial;

            PropertyHasChanged(_AlbumProperty, old, _Album);
        }

        //internal Track Clone(IImportContext IIC)
        //{
        //    _Album.RemoveTrack(this);
        //    SetAlbum(null);
        //    return this;
        //}

        internal Track CloneTo(IImportContext IIC, Album Destination)
        {
            _Album.RemoveTrack(this);
            SetAlbum(Destination);
            return this;
        }

        int ISessionPersistentObject.ID
        { get { return ID; } }

        internal Album Owner { get { return _Album; } }

        static public TrackStatus GetTrackFromPath(string iPath, IImportHelper NameHelper, IImportContext Context)
        {
            try
            {
                using (PathTrackDescriptor Des = new PathTrackDescriptor(iPath, NameHelper, Context))
                {
                    return GetTrackFromTrackDescriptor(Des, false, Context);
                }
            }
            catch (Exception e)
            {
                Context.OnFactorisableError<FileCorrupted>(iPath);
                Trace.WriteLine(e);
                return null;
            }
        }


        static internal TrackStatus GetTrackFromExportTrackDescriptor(TrackDescriptor TD, bool SaveTodisc, IImportContext Context, bool ImportAllMetaData)
        {
            if (!System.IO.File.Exists(TD.Path))
            {
                Context.OnFactorisableError<FileBrokenCannotBeImported>(TD.Path);
                return null;
            }

            TrackStatus res = GetTrackFromTrackDescriptor(TD, SaveTodisc, Context);

            if (res == null)
                return null;

            if (res.Continue == false)
                return res;

            if (ImportAllMetaData)
            {
                Track tr = res.Found;

                tr.DateAdded = TD.DateAdded;

                tr.LastPlayed = TD.LastPLayed;

                tr.RawRating = TD.Rating;

                tr.PlayCount = TD.PlayCount;
            }

            return res;
        }

        static internal TrackStatus GetTrackFromTrackDescriptor(ITrackDescriptor TD, bool SaveTodisc, IImportContext Context, bool InjectImages = false)
        {
            TrackDescriptorDecorator tdd = new TrackDescriptorDecorator(TD, Context);

            Track res = Context.Find(tdd);
            if (res != null)
            {
                //track already exists I will not return anything
                return new TrackStatus(res);
            }

            AlbumStatus Al = MusicCollection.Implementation.Album.GetAvailableAlbumFromTrackDescriptor(tdd, InjectImages);

            if (Al.Continue == false)
            {
                return new TrackStatus(Al);
            }

            res = new Track(Al.Found, tdd);
            Context.AddForCreated(res);

            if (SaveTodisc)
                res.SavetoDiskwithimages(Context);

            return new TrackStatus(res, Al);
        }

        private DateTime? _LastPlayed;
        private int _PlayCount = 0;
        private int _Skipped = 0;

        public DateTime? LastPlayed
        {
            get { return _LastPlayed; }
            private set { _LastPlayed = value; }
        }

        public int SkippedCount
        {
            get { return _Skipped; }
            private set { _Skipped = value; }
        }

        public int PlayCount
        {
            get { return _PlayCount; }
            private set { _PlayCount = value; }
        }

        private uint _DiscNumber;
        public uint DiscNumber
        {
            get { return _DiscNumber; }
            private set
            {
                //var old = _DiscNumber;

                //if (old == value)
                //    return;

                //_DiscNumber = value;

                //PropertyHasChanged(_DiscNumberProperty, old, _DiscNumber);

                Set(ref _DiscNumber, value);

                //_Album.ResetOrder();

            }
        }


        private Nullable<FileStatus> _IsBroken = null;

        public bool IsBroken
        {
            get
            {
                return (FileServices.IsBroken(FileExists));
            }
        }

        private IInternalMusicSession PrivateSession
        {
            get
            {
                return Session as IInternalMusicSession;
            }
        }

        private uint _Rating = 0;

        public uint Rating
        {
            get
            {
                return _Rating;
            }
            set
            {
                //uint Rating = Math.Max(0, Math.Min(5, value));

                //if (_Rating == Rating)
                //    return;

                //uint old = _Rating;

                //_Rating = Rating;

                //DBUpadte();

                //PropertyHasChanged(_RatingProperty, old, _Rating);

                Set(ref _Rating, Math.Max(0, Math.Min(5, value)));
                DBUpadte();
            }
        }

        private uint RawRating
        {
            set
            {
                uint Rating = Math.Max(0, Math.Min(5, value));
                _Rating = Rating;
            }
        }


        private FileStatus GetFileExists(bool ForceUpdate, bool UpdateState)
        {
            bool virgem = _IsBroken == null;
            if (virgem || (ForceUpdate))
            {
                _IsBroken = FileServices.GetFileStatus(_Path);

                if ((virgem) && (UpdateState))
                    CacheState(false);

                if (((FileStatus)_IsBroken).PotentialRemovable())
                {
                    PrivateSession.DriverListener._DriversChanged += FileStatusChanged;
                }

            }

            return (FileStatus)_IsBroken;
        }

        private void FileStatusChanged(object sender, USBDriverEventArgs uea)
        {
            CacheState(true);
        }

        public FileStatus FileExists
        {
            get
            {
                return GetFileExists(false, true);
            }
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal TagLib.File TagFile
        {
            get
            {
                if (IsBroken)
                    return null;

                try
                {
                    return TagLib.File.Create(_Path);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);

                    if (e is FileNotFoundException)
                        CacheState(true);

                    return null;
                }
            }
        }



        public uint TrackNumber
        {
            get
            {
                return _TrackNumber;
            }


            private set
            {
                //var old = _TrackNumber;

                //if (old == value)
                //    return;

                //_TrackNumber = value;
                //PropertyHasChanged(_TrackNumberProperty, old, _TrackNumber);

                Set(ref _TrackNumber, value);

                //_Album.ResetOrder();
            }
        }


        public string Artist
        {
            get
            {
                return _Author;
            }

            private set
            {
                //var old = _Author;
                //if (value == old)
                //    return;
                //_Author = value;
                //PropertyHasChanged(_ArtistProperty, old, _Author);
                Set(ref _Author, value);
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            internal set
            {
                //var old = _Name;
                //if (old == value)
                //    return;

                //_Name = value;
                //PropertyHasChanged(_NameProperty, old, _Name);
                Set(ref _Name, value);
            }
        }

        internal Album RawAlbum
        {
            get
            {
                return _Album;
            }
            set
            {
                if (_Album != null)
                    throw new Exception();

                _Album = value;
            }
        }

        public IAlbum Album
        {
            get
            {
                return _Album;
            }
        }

        private PathDecomposeur _PD;
        private string _Path
        {
            get { return (_PD == null) ? null : _PD.FullName; }
            set
            {
                PathHelper = PathDecomposeur.FromName(value);
            }
        }

        internal PathDecomposeur PathHelper
        {
            get { return _PD; }
            private set
            {
                if (_PD == value)
                    return;

                var old = _PD;
                var oldname = _Path;
                _PD = value;

                PropertyHasChanged(_PathHelperProperty, old, _PD);
                PropertyHasChanged(_PathProperty, oldname, _Path);

            }
        }

        public string Path
        {
            get
            {
                return _Path;
            }
            internal set
            {
                _Path = value;
            }
        }

        private DateTime? DateAdded
        {
            set
            {
                _DateAdded = value;
            }
        }

        DateTime? ITrack.DateAdded
        {
            get
            {
                return _DateAdded;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return _Duration;
            }
            private set
            {
                if (value.TotalMilliseconds == 0)
                    return;

                if (value == _Duration)
                    return;

                var old = _Duration;
                _Duration = value;

                DBUpadte();

                PropertyHasChanged(_DurationProperty, old, value);
            }
        }

        private void DBUpadte()
        {
            if ((this as ISessionPersistentObject).Context != null)
                return;

            using (IImportContext Context = PrivateSession.GetNewSessionContext())
            {
                using (IMusicTransaction imut = Context.CreateTransaction())
                {
                    try
                    {
                        Context.AddForUpdate(this);
                        imut.Commit();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(string.Format("Track DB updtade failed: {0}", e));
                    }
                }
            }
        }

        public IMusicSession Session
        {
            get { return this._Album.MusicSession; }
        }

        internal void LogicallyRemove()
        {
            _Album.RemoveTrack(this);
        }

        #region MD5-FileComparaison


        private Stream MusicStream()
        {
            using (TagLib.File TLF = this.TagFile)
            {
                return (TLF == null) ? null : TLF.RawMusicStream();
            }
        }

        internal bool? CompareMusic(Stream Path, bool disposestream = false)
        {
            if (Path == null)
                return null;

            bool? res = null;

            using (Stream my = MusicStream())
            {
                if (IsBroken)
                {
                    if (disposestream)
                        Path.Dispose();

                    return null;
                }

                res = my.Compare(Path);
            }

            if (disposestream)
                Path.Dispose();

            return res;
        }

        internal bool? CompareMusic(Track ct)
        {
            if (ct == null)
                return false;

            if (MD5HashKey != ct.MD5HashKey)
                return false;

            if (ct.IsBroken)
                return null;

            using (Stream toaie = ct.MusicStream())
            {
                return CompareMusic(toaie);
            }
        }

        private string _MD5;
        public string MD5HashKey
        {
            get
            {
                return _MD5;
            }
        }

        #endregion

        #region ISRC

        //private string _ISRC;
        //public ISRC ISRC
        //{
        //    get
        //    {
        //        return ISRC.Fromstring(_ISRC);
        //    }
        //}

        //internal string ISRCName
        //{
        //    get
        //    {
        //        return _ISRC;
        //    }
        //}

        //internal void UpdateISRC()
        //{
        //    using (TagLib.File TLF = this.TagFile)
        //    {
        //        ISRC res = TLF.ISRC();
        //        _ISRC = res == null ? null : res.Name;// ISRCFormat.Format(TLF.ISRC());
        //    }
        //}

        #endregion


        public override string ToString()
        {
            return String.Format(@"{0}:{1} - {2} :{3}", _TrackNumber, _Name, _Album.ToString(), DiscNumber);
        }

        internal void SavetoDiskwithimages(IImportContext iContext)
        {
            IPicture[] PictureTobeStored = null;
            if (RawAlbum.HasImage)
            {
                int Count = Album.Images.Count;

                PictureTobeStored = (from imama in (from im in Album.Images where (iContext.ImageManager.IsImageRankOKToEmbed(im.Rank, Count)) select im.ConvertToIPicture()) where imama != null select imama).ToArray();

            }
            SavetoDisk(iContext, PictureTobeStored);
        }

        internal void SavetoDisk(IImportContext Context, IPicture[] pictures = null)
        {
            IDisposable locker = Locker.GetLocker(this, Context);
            if (locker == null)
                return;

            using (locker)
            {
                using (TagLib.File TLF = this.TagFile)
                {
                    if (TLF == null)
                    {
                        Trace.WriteLine("unable to update tag for track" + ToString());
                        return;
                    }


                    TLF.Tag.Title = _Name;
                    TLF.Tag.Performers = new string[1] { _Author };
                    TLF.Tag.Track = _TrackNumber;

                    IAlbum Al = _Album;

                    TLF.Tag.Album = Al.Name;
                    if (Al.Genre != null)
                        TLF.Tag.Genres = new string[1] { Al.Genre };
                    TLF.Tag.Year = (uint)(Al.Year);
                    TLF.Tag.AlbumArtists = new string[1] { Al.Author };
                    TLF.Tag.Disc = DiscNumber;

                    if (pictures != null)
                    {
                        TLF.Tag.Pictures = pictures;
                    }

                    try
                    {
                        TLF.Save();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Problem saving files " + e.ToString());
                    }

                }
            }
        }

        internal string NormalizedName
        {
            get
            {
                return string.Format("{0}-{1}{2}", TrackNumber, Name.FormatFileName(150), System.IO.Path.GetExtension(PathHelper.LocalPath));
            }
        }

        internal string NormalizedNameNoExtension
        {
            get
            {
                return string.Format("{0}-{1}", TrackNumber, Name.FormatFileName(150));
            }
        }

        internal bool ReRoot(string iDirectory, IImportContext Context)
        {
            if (IsBroken)
            {
                Context.OnFactorisableError<FileBrokenCannotBeExported>(ToString());
                return false;
            }

            string mypath = _Path;

            if (string.Equals(System.IO.Path.GetDirectoryName(mypath), iDirectory, StringComparison.OrdinalIgnoreCase))
                return false;

            bool res = false;

            string NPath = FileInternalToolBox.CreateNewAvailableName(iDirectory, NormalizedNameNoExtension, System.IO.Path.GetExtension(PathHelper.LocalPath));

            if (NPath == null)
                return false;

            try
            {

                IDisposable locker = Locker.GetLocker(this, Context);
                if (locker == null)
                    return false;

                using (locker)
                {
                    System.IO.File.Move(_Path, NPath);
                    _Path = NPath;
                }
                Context.AddForUpdate(this);

                res = true;

            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem moving files " + e.ToString());
            }

            return res;

        }



        internal bool Modify(TrackModifier TN, IImportContext iic)
        {
            if (TN.NewRating != null)
            {
                Rating = TN.Rating;
            }

            if (!TN.NeedToUpdateFile)
                return true;

            IDisposable locker = Locker.GetLocker(this, iic);
            if (locker == null)
                return false;

            bool res = true;

            using (locker)
            {
                try
                {

                    using (TagLib.File TLF = this.TagFile)
                    {
                        if (TLF == null)
                        {
                            Trace.WriteLine("unable to write tag for track " + ToString());
                            return false;
                        }


                        if (TN.IsModified)
                        {
                            if (TN.NewName != null)
                            {
                                if (TLF != null)
                                    TLF.Tag.Title = TN.NewName;
                                Name = TN.NewName;
                            }

                            if (TN.NewArtist != null)
                            {
                                if (TLF != null)
                                    TLF.Tag.Performers = new string[1] { TN.NewArtist };
                                Artist = TN.NewArtist;
                            }

                            if (TN.NewTrackNumber != null)
                            {
                                if (TLF != null)
                                    TLF.Tag.Track = (uint)TN.NewTrackNumber;

                                TrackNumber = (uint)TN.NewTrackNumber;
                            }

                            if (TN.NewDiscNumber != null)
                            {
                                if (TLF != null)
                                    TLF.Tag.Disc = TN.DiscNumber;

                                DiscNumber = TN.DiscNumber;
                            }

                        }

                        IInternalAlbumModifier AM = TN.Album;

                        if (AM.IsAlbumOnlyModified)
                        {
                            if (AM.NewName != null)
                                TLF.Tag.Album = AM.NewName;

                            if (AM.NewGenre != null)
                                TLF.Tag.Genres = new string[1] { AM.NewGenre };

                            if (AM.NewYear != null)
                                TLF.Tag.Year = (uint)AM.NewYear;

                            if (AM.AuthorDirty)
                                TLF.Tag.AlbumArtists = new string[1] { AM.ArtistName };

                            if (AM.IsImageDirty)
                                TLF.Tag.Pictures = AM.PictureTobeStored;

                        }

                        TLF.Save();

                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving files " + e.ToString());
                    res = false;
                }
            }

            return res;

        }

        #region Listener To Player

        private void PlayerEvent(object sender, MusicTrackEventArgs tpe)
        {
            if (!(object.ReferenceEquals(this, tpe.Track)))
                return;

            switch (tpe.What)
            {
                case TrackPlayingEvent.BeginPlay:
                    if (tpe.TotalDuration != null)
                        Duration = (TimeSpan)tpe.TotalDuration;
                    break;


                case TrackPlayingEvent.Broken:
                    CacheState(true);
                    break;


                case TrackPlayingEvent.Skipped:

                    _Skipped++;

                    DBUpadte();

                    PropertyHasChanged(_SkippedProperty, _Skipped - 1, _Skipped);

                    break;

                case TrackPlayingEvent.EndPlay:


                    var old = _LastPlayed;

                    _LastPlayed = DateTime.Now;
                    _PlayCount++;

                    DBUpadte();

                    PropertyHasChanged(_LastPlayedProperty, old, _LastPlayed);
                    PropertyHasChanged(_PlayCountProperty, _PlayCount - 1, _PlayCount);

                    break;

            }
        }

        #endregion


        #region ILifeCycle


        internal override void OnRemove(IImportContext iic)
        {
            _Album.RemoveTrack(this);

            if ((_IsBroken != null) && (((FileStatus)_IsBroken)).PotentialRemovable())
                PrivateSession.DriverListener._DriversChanged -= FileStatusChanged;
        }

        protected override bool IsFileBroken(bool UpdateStatusFile)
        {
            FileStatus res = GetFileExists(UpdateStatusFile, false);
            return (FileServices.IsBroken(res));
        }

        #endregion


        #region IDBPersistentObject

        IImportContext ISessionPersistentObject.Context
        {
            get { return _Album.Context; }
            set
            {
                if (_Album == null)
                {
                    //if (value == null)
                    //    return;

                    throw new Exception("Algo error");
                }
                _Album.Context = value;
            }
        }

        void ISessionPersistentObject.UnRegisterFromSession(IImportContext iSession)
        {
            iSession.Session.Tracks.Remove(this);
        }

        void ISessionPersistentObject.Register(IImportContext iic)
        {
            _Album.MusicSession.Tracks.Register(this);
        }

        void ISessionPersistentObject.Publish()
        {
            _Album.MusicSession.Tracks.Publish(this);
        }

        void ISessionPersistentObject.OnLoad(IImportContext iic)
        {
            iic.Session.Tracks.RegisterPublish(this);
        }

        int ITrack.ID
        {
            get { return ID; }
        }

        #endregion

        #region Player management

        void IInternalTrack.AddPlayer(IInternalMusicPlayer pl)
        {
            ObjectStateChanges += pl.OnLockEvent;
            pl.TrackEvent += PlayerEvent;
        }

        void IInternalTrack.RemovePlayer(IInternalMusicPlayer pl)
        {
            ObjectStateChanges -= pl.OnLockEvent;
            pl.TrackEvent -= PlayerEvent;
        }


        #endregion

        internal void UpdateTrackOnly(ITrackMetaDataDescriptor tm)
        {
            if (tm == null)
                return;

            Name = tm.Name;
            Artist = tm.Artist;
            TrackNumber = tm.TrackNumber;
            DiscNumber = tm.DiscNumber;

            _Album.SortList();
        }

        public ISingleTrackEditor GetEditor()
        {
            return new SingleTrackUpdater(this, Session);
        }

        #region Compare

        public int CompareTo(object obj)
        {
            Track ot = obj as Track;
            if (ot == null)
                return 1;

            return CompareTo(ot);
        }


        public int CompareTo(ITrack other)
        {
            return CompareTo(other as Track);
        }

        public int CompareTo(Track other)
        {
            if (other == null)
                return 1;

            if (object.ReferenceEquals(other, this))
                return 0;

            return this._SessionID.CompareTo(other._SessionID);
        }

        #endregion
    }
}
