using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using TagLib;

using MusicCollection.Itunes;
using MusicCollection.Fundation;
using MusicCollection.FileImporter;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Implementation.Modifier;

namespace MusicCollection.Implementation
{
    public sealed class Album : StateObjectAdapter, IAlbum, IInternalAlbum, IObjectStateCycle, ISessionPersistentObject, IComparable
    {

        private string _Name;
        private uint _TracksNumber;
        private DateTime _DateAdded;
        private int _Year;

        private string _CDDB;
        private string _Asin;
        private string _MusicBrainzID;
        private string _MusicBrainzHash;

        private IList<Track> _Tracks;// = new List<Track>();
        private Genre _Genre = null;

        private int _SessionID;

        static private int _SessionIDCount;

        private const string _AuthorProperty = "Author";
        private const string _NameProperty = "Name";
        private const string _GenreProperty = "Genre";
        private const string _MainGenreProperty = "MainGenre";
        private const string _TracksNumberProperty = "TracksNumber";
        private const string _YearProperty = "Year";
        private const string _ImagesProperty = "Images";
        private const string _FrontImageProperty = "CoverImage";
        private const string _TracksNumberPropery = "TracksNumber";
        private const string _AlbumMaturityProperty = "Maturity";
        private const string _HashesProperty = "Hashes";
        private const string _AsinProperty = "Asin";
        private const string _MusicBrainzIDProperty = "MusicBrainzID";
        private const string _NormalizedNameProperty = "NormalizedName";

        private AlbumModifier _Modifier = null;

        private int ID { get; set; }

        internal IAlbum Interface { get { return this; } }

        private IInternalMusicSession _Impl;
        private IImportContext _Context = null;

        internal IInternalMusicSession MusicSession { get { return _Impl; } set { if ((_Impl != value) && (_Impl != null)) throw new Exception("Session Management"); _Impl = value; } }

        internal IImportContext Context { get { return _Context; } set { if ((null != value) && (_Context != null) && !(object.ReferenceEquals(value, _Context))) throw new Exception("Session Management"); _Context = value; } }

        internal Album() { _SessionID = _SessionIDCount++; }

        public IMusicSession Session
        {
            get
            {
                return _Impl;
            }
        }

        int ISessionPersistentObject.ID
        { get { return ID; } }

        private Album(IImportContext iSession)
            : this()
        {
            _Context = iSession;
        }

        private Album(AlbumDescriptorDecorator add, bool InjectImages=false)
            : this(add.ImportContext)
        {
            _Name = add.CorrectName;
            _Genre = add.MainGenre;

            _TracksNumber = add.TracksNumber;
            _DateAdded = DateTime.Now;
            _Year = add.Year;
            _Maturity = add.Maturity;

            CDIDs = add.IDs;

            if (InjectImages)
                this.ImportImageFromDescriptor(add.Wrapped as IFullAlbumDescriptor);

            if (add.Artists != null)
            {
                ArtistHandler.ModelCollection.AddCollection(add.Artists);
            }

            
        }

        #region DiscId

        public IDiscIDs CDIDs
        {
            private set
            {
                CDDB = (value == null) ? null : value.CDDB;
                Asin = (value == null) ? null : value.Asin;
                MusicBrainzID = (value == null) ? null : value.MusicBrainzID;
                MusicBrainzHash = (value == null) ? null : value.MusicBrainzCDId;
            }
            get
            {
                return DiscIDs.FromIDsAndHashes(Asin, MusicBrainzID, CDDB, MusicBrainzHash);
            }
        }

        internal DiscHash Hashes
        {
            get { return DiscHash.InstanciateFromHash(MusicBrainzHash, CDDB); }
        }


        internal string Asin
        {
            get { return _Asin; }
            set
            {
                Set(ref _Asin, value); 
                //if (_Asin == value)
                //    return;

                //string old = _Asin;
                //_Asin = value;
                //PropertyHasChanged(_AsinProperty, old, value);
            }
        }

        internal string MusicBrainzID
        {
            get { return _MusicBrainzID; }
            set
            {
                Set(ref _MusicBrainzID, value); 
                //if (_MusicBrainzID == value)
                //    return;

                //string old = _MusicBrainzID;
                //_MusicBrainzID = value;
                //PropertyHasChanged(_MusicBrainzIDProperty, old, value);
            }
        }

        internal string MusicBrainzHash
        {
            get { return _MusicBrainzHash; }
            set
            {
                //SetValue(ref _MusicBrainzHash, value);
                if (_MusicBrainzHash == value)
                    return;

                DiscHash old = Hashes;
                _MusicBrainzHash = value;
                PropertyHasChanged(_HashesProperty, old, Hashes);
            }
        }

        internal string CDDB
        {
            get { return _CDDB; }
            set
            {
                //if (_CDDB == value)
                //    return;

                //DiscHash old = Hashes;
                //_CDDB = value;
                //PropertyHasChanged(_HashesProperty, old, Hashes);
                Set(ref _CDDB, value); 
            }
        }

        #endregion

        static internal AlbumStatus GetAvailableAlbumFromTrackDescriptor(TrackDescriptorDecorator TD, bool InjectImage=false)
        {
            return TD.ImportContext.FindAlbumOrCreate(TD.AlbumInfo, () => new Album(TD.AlbumInfo, InjectImage));
        }

        static internal AlbumStatus GetAvailableAlbumFromTrackDescriptor(AlbumDescriptorDecorator TD)
        {
            return TD.ImportContext.FindAlbumOrCreate(TD, () => new Album(TD));
        }

        static internal Album GetAlbumFromExportAlbum(AlbumDescriptor TD, IImportContext Context, ITrackStatusVisitor TSV, bool ImportAllMetaData)
        {
            Album al = null;

            foreach (TrackDescriptor et in TD.RawTrackDescriptors)
            {
                TrackStatus tr = Track.GetTrackFromExportTrackDescriptor(et, false, Context, ImportAllMetaData);

                TSV.Visit(et.Path, tr);
                if ((tr != null) && (tr.Continue))
                {
                    if (al == null)
                        al = tr.Found.RawAlbum;
                    else
                        if (!object.ReferenceEquals(al, tr.Found.RawAlbum))
                            throw new Exception("Not supported");
                }
            }

            if (al == null)
                return null;

            al.ImportImageFromDescriptor(TD);
            return al;
        }

        private void ImportImageFromDescriptor(IFullAlbumDescriptor iad)
        {
            if ((iad == null) || (iad.Images==null) || (iad.Images.Count==0))
                return;

            PersistentImages = new List<AlbumImage>();

            foreach (IIMageInfo im in iad.Images)
            {
                if (im.ImageBuffer.IsOK)
                {
                    AlbumImage ai = AlbumImage.GetFromBuffer(this, im.ImageBuffer);
                    PersistentImages.Add(ai);
                    ai.Rank = im.ID;
                }
            }
        }

        #region Genre

        public IGenre MainGenre
        {
            get { return MusicalGenre; }
        }

        internal Genre MusicalGenre
        {
            get { return _Genre ?? MusicCollection.Implementation.Genre.CreateDummy(_Impl); }
        }

        public string Genre
        {
            get
            {
                return (_Genre == null) ? null : _Genre.FullName;
            }

            internal set
            {
                string old = Genre;

                if (old == value)
                    return;

                Genre OldGenre = _Genre ?? MusicCollection.Implementation.Genre.CreateDummy(_Impl);

                if (_Genre != null)
                {
                    _Genre.DetachAlbum(this);
                }

                _Genre = _Context.GetGenreFromName(value, false);

                if (_Genre != null)
                {
                    _Genre.AttachAlbum(this);
                }
  
                //MusicCollection.Implementation.Genre.GetGenre(value, Session);
                PropertyHasChanged(_GenreProperty, old, Genre);
                PropertyHasChanged(_MainGenreProperty, OldGenre , _Genre);
            }
        }

        #endregion

        #region Artist

        private IList<Artist> _Artists;
        private ModelToUISafeCollectionHandler<Artist, IArtist> _ArtistHandler;

        public ICompleteObservableCollection<IArtist> Artists
        {
            get { return ArtistHandler.ReadOnlyUICollection; }
        }


        private ModelToUISafeCollectionHandler<Artist, IArtist> ArtistHandler
        {
            get
            {
                if (_ArtistHandler == null)
                {
                    if (_Artists == null)
                        _Artists = new List<Artist>();

                    _ArtistHandler = new ModelToUISafeCollectionHandler<Artist, IArtist>(_Artists);
                }
                return _ArtistHandler;
            }
        }

        internal IIsolatedMofiableList<IArtist> GetArtistModifier()
        {
            SafeCollectionModifierConverter<IArtist, IArtist> sfc = new SafeCollectionModifierConverter<IArtist, IArtist>(ArtistHandler.ModifiableUICollection, a => a, a => a);
            string oldn = this.Author;

            sfc.OnCommit += (o, e) => sfc_OnCommit(oldn);
            return sfc;
        }

        void sfc_OnCommit(string oldn)
        {
            string newau = this.Author;
            PropertyHasChanged("Author", oldn, newau);
        }

        internal IList<Artist> RawArtists
        {
            get { return _Artists; }
        }

        private void SetAuthours(IList<Artist> NewAuthors, IImportContext iic)
        {
            string old = Author;

            if ((Author != null) && (ArtistHandler.ModelCollection.SequenceEqual(NewAuthors)))
                return;

            ArtistHandler.ModelCollection.Where(ar => ((NewAuthors == null) || (!NewAuthors.Contains(ar)))).Apply(ar => ar.RemoveAlbum(this, iic));

            if (NewAuthors != null)
            {
                NewAuthors.Where(ar => !ArtistHandler.ModelCollection.Contains(ar)).Apply(ar=>ar.AddAlbum(this,iic));
                ArtistHandler.ModelCollection.Clear();
                ArtistHandler.ModelCollection.AddCollection(NewAuthors);
            }
            else
            {
                 ArtistHandler.ModelCollection.Clear();
            }

            PropertyHasChanged(_AuthorProperty, old, Author);

        }


        public string Author
        {
            get
            {
                return Artist.AuthorName(_Artists);
            }

            private set
            {
                SetAuthours(_Context.GetArtistFromName(value), _Context);// new List<Artist>(Artist.GetArtistFromName(value, Session));               
            }

        }

        #endregion

        #region Name

        private string InitName
        {
            set
            {
                _Name = value;
                UpdateNormalizedName();
            }
            get
            {
                return _Name;
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
                string v = value.Trim().NormalizeSpace();
                string old = _Name;

                if (v == old)
                    return;

                string onm = NormalizedName;

                _Name = v;
                _NormalizedName = null;
                PropertyHasChanged(_NameProperty, old, _Name);
                PropertyHasChanged(_NormalizedNameProperty, onm, NormalizedName);
            }
        }


        private void UpdateNormalizedName()
        { 
            if (_NormalizedName == null)
                    _NormalizedName = (Name == null) ? null : Name.ToLowerWithoutAccent();
        }

        private string _NormalizedName;
        public string NormalizedName
        {
            get
            {
                UpdateNormalizedName();
                //.NormalizeSpace().ToLower().WithoutAccent()
                return _NormalizedName;
            }
        }

        #endregion

        uint IAlbum.TracksNumber
        {
            get { return _TracksNumber;}
        }

        public DateTime DateAdded
        {
            get { return _DateAdded; }
            internal set { _DateAdded = value; }
        }

        public int Year
        {
            get
            {
                return _Year;
            }
            internal set
            {
                Set(ref _Year,value);
            }
        }

        internal AlbumModifier GetModifiableAlbum(bool resetCorruptedImage, IImportContext IT)
        {
            if (_Modifier != null)
                return null;

            AlbumModifier Modifier = null;

            if ((IT == null) || (!object.ReferenceEquals(IT, _Context) && (_Context != null)))
                throw new Exception("Session Management");

            if (_Modifier == null)
            {
                Modifier = new AlbumModifier(this, resetCorruptedImage, IT);
            }

            lock (_Lockobject)
            {
                if (_Modifier == null)
                {
                    _Modifier = Modifier;
                    return _Modifier;
                }
                else
                    return null;
            }

        }


        IModifiableAlbum IAlbum.GetModifiableAlbum(bool resetCorruptedImage)
        {
            if ((_Context != null) && (_Modifier == null))
            {
                return null;
            }

            return GetModifiableAlbum(resetCorruptedImage, MusicSession.GetNewSessionContext(AlbumMaturity.Discover));
        }

        #region Images

        private IList<AlbumImage> _Images = null;

        internal IEnumerable<AlbumImage> ImagesFromFile
        {
            get
            {
                int Rank = 0;

                foreach (Track Tr in CrudeTracks)//.ToList())//threadsafeness
                {
                    using (TagLib.File Ff = Tr.TagFile)
                    {

                        bool OK = false;

                        if (Ff != null)
                        {
                            foreach (TagLib.IPicture IP in Ff.Tag.Pictures)
                            {
                                AlbumImage res = AlbumImage.GetFromIPicture(this, IP);
                                res.Rank = Rank++;
                                yield return res;
                                OK = true;
                            }
                        }

                        if (OK)
                            yield break;
                    }
                }

                yield break;
            }
        }


        private void ImagesChanged(object sender, EventArgs evea)
        {
            if (_Images == null)
                throw new Exception("image modification error");

            int aic = _Images.Count;

            for (int k = 0; k < aic; k++)
            {
                _Images[k].Rank = k;
            }

            CachedImage.Update();

            PropertyHasChangedUIOnly(_ImagesProperty);
            PropertyHasChangedUIOnly(_FrontImageProperty);
        }

        internal void RegenerateCoverArt()
        {
            CachedImage.Update(true);
            PropertyHasChangedUIOnly(_FrontImageProperty);
        }


        private IList<AlbumImage> PersistentImages
        {
            get
            {
                if (_Images == null)
                    return ImagesFromFile.ToList();

                return _Images;
            }

            set { _Images = value; }
        }

        private void InitListImages()
        {
            if (_Images == null)
                _Images = ImagesFromFile.ToList<AlbumImage>();
        }

        private IList<IAlbumPicture> privateImages
        {
            get
            {
                InitListImages();
                return new List<IAlbumPicture>(_Images);
            }
        }

        private ModelToUISafeCollectionHandler<AlbumImage, IAlbumPicture> _ImagesHandler;

        private ModelToUISafeCollectionHandler<AlbumImage, IAlbumPicture> AlbumHandler
        {
            get
            {
                if (_ImagesHandler == null)
                {
                    InitListImages();
                    _ImagesHandler = new ModelToUISafeCollectionHandler<AlbumImage, IAlbumPicture>(_Images);
                }

                return _ImagesHandler;
            }
        }

        public ICompleteObservableCollection<IAlbumPicture> Images
        {
            get
            {
                return AlbumHandler.ReadOnlyUICollection;
            }
        }

        internal IObservableCollection<AlbumImage> ModifiableImages
        {
            get
            {
                return AlbumHandler.ModelCollection;
            }
        }

        protected override void OnFirstCompute()
        {
            CrudeTracks.Apply(tr => tr.ObjectStateChanges += StateChanged);
        }

        private void StateChanged(object sender, ObjectStateChangeArgs osca)
        {
            if (osca.NewState == ObjectState.Removed)
                return;

            if (osca.NewState == ObjectState.UnderEdit)
                return;

            CacheState(true); 
        }

        private void OnBeforeTrackModifiedCommitted(object sender, EventColllectionChangedArgs<IModifiableTrack> e)
        {
           
            Track tr = (e.Who as TrackModifier).Track;

            switch (e.What)
            {
                case  NotifyCollectionChangedAction.Add:             
                tr.SetAlbum(this);
                if (this.IsStatusInitialized)
                {
                    tr.ObjectStateChanges += StateChanged;
                }
                break;

            
                case NotifyCollectionChangedAction.Remove:         
                if (this.IsStatusInitialized)
                {
                    tr.ObjectStateChanges -= StateChanged;
                }
                break;

            }

            
        }

        private void OnTrackModifiedCommitted(object sender, EventArgs ea)
        {
            if (this.IsStatusInitialized)
            {
                CacheState(true);
            }

            SafeCollectionModifierConverter<Track, IModifiableTrack> res = sender as SafeCollectionModifierConverter<Track, IModifiableTrack>;
            res.OnCommit -= OnTrackModifiedCommitted;
            res.OnBeforeChangedCommited -= OnBeforeTrackModifiedCommitted;
        }

        internal IIsolatedMofiableList<IModifiableTrack> GetTrackModifier(AlbumModifier am)
        {
            //UpdateTracks();

            SafeCollectionModifierConverter<Track, IModifiableTrack> res =
                new SafeCollectionModifierConverter<Track, IModifiableTrack>(ModifiableSortedTracks, ((t) => new TrackModifier(t, am)), ((t) => (t as TrackModifier).Track));


            res.OnBeforeChangedCommited += OnBeforeTrackModifiedCommitted;
            //res.OnCommit += OnTrackModifiedCommitted;

            return res;
        }

        internal IIsolatedMofiableList<IAlbumPicture> RawImages
        {
            get
            {
                InitListImages();
                //SafeCollectionModifierConverterDerived<AlbumImage, IAlbumPicture> res = new SafeCollectionModifierConverterDerived<AlbumImage, IAlbumPicture>
                //    (ModifiableImages);

                SafeCollectionModifierConverter<AlbumImage, IAlbumPicture> res =
                    SafeCollectionModifierConverter<AlbumImage, IAlbumPicture>.GetSafeCollectionModifierConverterDerived<AlbumImage, IAlbumPicture>
                    (ModifiableImages);//, AM=>AM, In => (In is AlbumImage) ? In as AlbumImage: (In as fakepicture).GetAlbumPicture() );


                res.OnCommit += ImagesChanged;
                return res;
            }
        }

        internal AlbumImage RawFrontImage
        {
            get
            {
                if (_Images != null)
                    return ((_Images.Count > 0) ? _Images[0] : null);

                return ImagesFromFile.FirstOrDefault();
          }
        }


        internal bool HasImage
        {
            get
            {
                return (RawFrontImage != null);
            }
        }

        #region Image Cache

        private ImageCache _ImageCache;

        private ImageCache CachedImage
        {
            get
            {
                if (_ImageCache == null)
                    _ImageCache = new ImageCache(this);

                return _ImageCache;
            }
        }

        internal string ImageCachePath
        {
            get
            {
                return (CachedImage.Buffer == null) ? null : CachedImage.Buffer.PersistedPath;
            }
        }

        public BitmapImage CoverImage
        {
            //get {AlbumImage ai = RawFrontImage; return ai==null? null : ai.MediumImage;}
            get { return CachedImage.Image; }
        }

        //internal void AfterLoad()
        //{
        //    CachedImage.AfterLoad(false);
        //}

        private ImageCache InitFromBuffer(IBufferProvider ibp)
        {
            if (_ImageCache != null)
                return _ImageCache; //check buffer equality

            _ImageCache = new ImageCache(this, ibp);
            return _ImageCache;
        }

        private IBufferProvider ImageCacheBuffer
        {
            get
            { return CachedImage.Buffer; }
            set
            {
                InitFromBuffer(value);
            }
        }

        internal void ClearImages()
        {
            if (_Tracks != null)
                throw new Exception();

            _Images = new List<AlbumImage>();
        }

        internal void UpdateImagesFrom(Album other)
        {

            int k = this.ModifiableImages.Count;
            bool ud = false;

            foreach (AlbumImage ai in other.ModifiableImages.Where(im => !im.IsBroken))
            {
                if (!this.ModifiableImages.Contains(ai, ai.Comparer))
                {
                    AlbumImage nai = ai.Clone(this);
                    nai.Rank = k++;
                    this.ModifiableImages.Add(nai);
                    ud = true;
                }
            }

            if (ud)
            {
                CachedImage.Update();

                PropertyHasChangedUIOnly(_ImagesProperty);
                PropertyHasChangedUIOnly(_FrontImageProperty);
            }
        }

        #endregion

        #endregion

        public string Label
        {
            get;
            private set;
        }

        #region Tracks

        private ModelToUISafeCollectionHandler<Track, ITrack> _ObservableTracks;

        private IList<Track> ModifiableTracks
        {
            get
            {
                if (_Tracks == null)
                {
                    _Tracks = new List<Track>();
                    if (_ObservableTracks != null)
                        throw new Exception("algo error");
                    return _Tracks;
                }

                return (_ObservableTracks != null ? _ObservableTracks.ModelCollection : _Tracks);
            }
        }

        private ModelToUISafeCollectionHandler<Track, ITrack> TrackCollectioHandler
        {
            get
            {
                if (_ObservableTracks == null)
                {
                    _ObservableTracks = new ModelToUISafeCollectionHandler<Track, ITrack>(_Tracks);
                    _ObservableTracks.ModelCollection.Comparator =
                        ((x, y) => { int d = (int)(x.DiscNumber - y.DiscNumber); if (d != 0) return d; d = (int)(x.TrackNumber - y.TrackNumber); return ((d != 0) ? d : String.Compare(x.Path, y.Path)); });
                    _ObservableTracks.ModelCollection.Sort();
                }

                return _ObservableTracks;
            }
        }

        internal IList<Track> ModifiableSortedTracks
        {
            get
            {
                if (_Tracks == null)
                {
                    _Tracks = new List<Track>();
                    if (_ObservableTracks != null)
                        throw new Exception("algo error");
                }

                return TrackCollectioHandler.ModelCollection;
            }
        }

        public ICompleteObservableCollection<ITrack> Tracks
        {
            get
            {
                return TrackCollectioHandler.ReadOnlyUICollection;
            }
        }

        internal IEnumerable<Track> RawTracks
        {
            get
            {
                return CrudeTracks;
            }
        }

        private IList<Track> CrudeTracks
        {
            get
            {
                if (_Tracks == null)
                {
                    throw new Exception("algo error");
                }

                return _Tracks;
            }
        }

        public int EffectiveTrackNumber
        {
            get
            {
                return CrudeTracks.Count;
            }
        }

        private void BasicAddTrack(Track tr)
        {
            ModifiableTracks.Add(tr);
            if (this.IsStatusInitialized)
            {
                tr.ObjectStateChanges += StateChanged;
                CacheState(true);
            }
        }
        
        internal void CloneTrack(Track oldTrack, IImportContext IIC)
        {
            BasicAddTrack(oldTrack.CloneTo(IIC,this));            
            //SortList();
        }

        internal void AddTrack(Track newTrack)
        {
            ITrack it = newTrack;
            if (it.Album != this)
                throw new InvalidProgramException("Collection inconsistency!!");


            BasicAddTrack(newTrack);

            if (_Context == null)
                throw new Exception("Session Management");

            //SortList();

            //ISessionPersistentObject nt = newTrack;
            //nt.Register(_Context);
        }

        internal void RemoveTrack(Track tr)
        {
            ModifiableTracks.Remove(tr);
            if (this.IsStatusInitialized)
            {
                tr.ObjectStateChanges -= StateChanged;
                CacheState(true);
            }
        }

        #endregion

        private readonly object _Lockobject = new object();

        internal void ResetChanges(AlbumModifier AM)
        {
            lock (_Lockobject)
            {
                if (_Modifier == null)
                    return;

                if (_Modifier != AM)
                    throw new InvalidOperationException();

                _Modifier = null;
            }
        }

        public bool IsModifiable
        {
            get { return (_Modifier == null); }
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Author, _Name);
        }

        internal void SortList()
        {
            if (_ObservableTracks == null)
                return;

            _ObservableTracks.ModelCollection.Sort();
        }

        internal bool Modify(AlbumModifier AM)
        {
            lock (_Lockobject)//todo readerslim
            {

                try
                {

                    if (AM.IsAlbumOnlyModified)
                    {

                        if (AM.NewGenre != null)
                        {
                            Genre = AM.NewGenre;
                        }

                        if (AM.NewYear != null)
                        {
                            Year = (int)AM.NewYear;
                        }
                    }

                    foreach (IModifiableTrack TN in AM.Tracks)
                    {
                        (TN as TrackModifier).CommitChanges();
                    }

                    SortList();

                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    return false;
                }
            }//+		[System.IO.IOException]	{"The process cannot access the file 'C:\\Documents and Settings\\DEM\\My Documents\\My Music\\Music Collection\\ddd\\0\\Copy of Copy of Copy of Unknown Album 1\\1-Track 1.mp3' because it is being used by another process."}	System.IO.IOException

        }

        private AlbumMaturity _Maturity;
        public AlbumMaturity Maturity
        {
            get { return _Maturity; }
            set
            {
                if ((_Maturity == value) || (_Maturity == AlbumMaturity.Collection))
                    return;

                bool OK = true;
                AlbumMaturity old = _Maturity;

                using (IImportContext Context = _Impl.GetNewSessionContext())
                {
                    using (IMusicTransaction imut = Context.CreateTransaction())
                    {
                        Context.AddForUpdate(this);
                        IMaturityUserSettings imm = Context.MaturityUserSettings;

                        if (imm.ExportCollectionFiles)
                        {
                            OK = Reroot(imm.DirForPermanentCollection, Context, true);
                        }

                        if (OK)
                        {
                            _Maturity = value;
                            imut.Commit();
                        }
                        else
                            imut.Cancel();
                    }
                }

                if (OK)
                {
                    PropertyHasChanged(_AlbumMaturityProperty, old, value);
                }
            }
        }

        internal bool Reroot(string iDirectory, IImportContext Context, bool AddCustomDir)
        {
            if (!Directory.Exists(iDirectory))
            {
                Context.OnFactorisableError<OutputDirectoryNotFound>(this.ToString());
                return false;
            }

            bool del1 = false;
            bool del2 = false;
            DirectoryInfo di = null;
            DirectoryInfo di2 = null;

            string path = iDirectory;


            if (AddCustomDir)
            {
                string OutDirector = Path.Combine(iDirectory, Author.FormatFileName());

                di = new DirectoryInfo(OutDirector);
                if (!di.Exists)
                {
                    di.Create();
                    del1 = true;
                }

                string FinalDirectory = Path.Combine(OutDirector, Name.FormatFileName());

                di2 = new DirectoryInfo(FinalDirectory);
                if (!di2.Exists)
                {
                    di2.Create();
                    del2 = true;
                }
                path = FinalDirectory;
            }

            bool sm = false;

            var init = (from t in CrudeTracks where t.IsBroken == false select t.Path).ToList();
            var TobeMoved = CrudeTracks.ToList();

            foreach (Track tr in TobeMoved)
            {
                if (tr.ReRoot(path, Context))
                {
                    sm = true;
                }
                else
                {
                    Context.OnFactorisableError<UnableToCopyFile>(tr.ToString());
                }
            }

            if (sm)
            {
                Context.Folders.GetFileCleanerFromFiles(init, Context.Folders.IsFileRemovable, true).Remove();
            }
            else
            {
                if (del1)
                    di.Delete(true);
                else if (del2)
                    di2.Delete(true);
            }

            return sm;
        }

        void IInternalAlbum.Visit(IAlbumVisitor Visitor)
        {
            if (Visitor == null)
                return;

            Visitor.Album = this;

            foreach (Track tr in CrudeTracks)
            {
                Visitor.VisitTrack(tr);
            }

            foreach (AlbumImage AI in Images)
            {
                Visitor.VisitImage(AI);
            }

            Visitor.EndAlbum();

        }

        #region IDBPersistentObject

        IImportContext ISessionPersistentObject.Context
        {
            get { return Context; }
            set { Context = value; }
        }

        void ISessionPersistentObject.UnRegisterFromSession(IImportContext iSession)
        {
            iSession.Session.Albums.CoreUnRegister(this);           
            
            ArtistHandler.ModelCollection.Apply(ar => ar.RemoveAlbum(this, iSession));
            ArtistHandler.ModelCollection.Clear();

            if (_Genre != null) _Genre.DetachAlbum(this);

            foreach (ISessionPersistentObject tr in CrudeTracks)
            {
                tr.UnRegisterFromSession(iSession);
            }
        }

        void ISessionPersistentObject.Publish(IImportContext Context)
        {
           _Impl.Albums.Publish(this);

           ArtistHandler.ModelCollection.Apply(ar => ar.AddAlbum(this, Context));
  
           if (_Genre != null) 
               _Genre.AttachAlbum(this);
        }

        void ISessionPersistentObject.OnLoad(IImportContext iic)
        {
            MusicSession = iic.Session;
            MusicSession.Albums.RegisterPublish(this);
            CachedImage.AfterLoad(false);
            if (_Genre != null) _Genre.AttachAlbum(this);
        }

        void ISessionPersistentObject.Register(IImportContext iic)
        {
            MusicSession = iic.Session;
            MusicSession.Albums.CoreRegister(this);
        }

        #endregion

        #region Life Cycle

        internal override void OnRemove(IImportContext iic)
        {
             if (_ImageCache != null)
            {
                _ImageCache.Dispose();
                _ImageCache = null;
            }

            List<Track> tracks = CrudeTracks.ToList();

            foreach (Track tr in tracks)
            {
                (tr as IObjectStateCycle).SetInternalState(ObjectState.Removed,iic);
            }
        }

        private Nullable<bool> _FileBroken = null;
        protected override bool IsFileBroken(bool UpdateStatusFile)
        {
            bool virgem = _FileBroken == null;
            if ( UpdateStatusFile)
            {
                _FileBroken = CrudeTracks.All(tr => tr.UpdatedState == ObjectState.FileNotAvailable);
            }
            else if (virgem)
            {
                _FileBroken = CrudeTracks.All(tr => tr.InternalState == ObjectState.FileNotAvailable);
            }
           
            return (bool)_FileBroken;
        }

        internal void SaveTracks(IImportContext context)
        {
            if (context == null)
            {
                Trace.WriteLine("SaveTracks Context can not be null aborting operation.");
                return;
            }

            Context = context;
            IPicture[] PictureTobeStored = null;
            if (HasImage)
            {
                int Count = Images.Count;

                PictureTobeStored = (from imama in (from im in Images where (context.ImageManager.IsImageRankOKToEmbed(im.Rank, Count)) select im.ConvertToIPicture()) where imama != null select imama).ToArray();
            }

            RawTracks.Apply(tr => tr.SavetoDisk(context, PictureTobeStored));

            Context = null;
        }

        public override void HasBeenUpdated()
        {
            SortList();
        }

        #endregion

        int IComparable.CompareTo(object other)
        {
            Album o =  other as Album;
            return this._SessionID - o._SessionID;
        }
    }


}
