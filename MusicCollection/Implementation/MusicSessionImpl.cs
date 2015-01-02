using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

using NHibernate;

using MusicCollection.Itunes;
using MusicCollection.WebServices;
using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Mapping;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.Nhibernate.Utilities;
using MusicCollection.FileConverter;
using MusicCollection.ToolBox;
using MusicCollection.MusicPlayer;
using MusicCollection.PlayList;
using MusicCollection.SettingsManagement;
using MusicCollection.Properties;
using MusicCollection.Infra;
using MusicCollection.Utilies;
using MusicCollection.Implementation.Session;


namespace MusicCollection.Implementation
{
    internal class MusicSessionImpl : NotifySimpleAdapter, IInternalMusicSession, IMusicSession, 
        IInvariant, INotifyPropertyChanged 
    {
        private SemaphoreSlim _sem = new SemaphoreSlim(1);
        private USBDriveListener _DisListener = new USBDriveListener();
        private IMainWindowHwndProvider _IMW;
        private Lazy<IMusicConverter> _MusicConverter;


        static internal MusicSessionImpl GetSession(ISessionBuilder isb, IMainWindowHwndProvider imp=null)
        {
            return new MusicSessionImpl(isb, imp);
        }


        public IntPtr MainWindow
        {
            get { return (_IMW == null) ? (IntPtr)null : _IMW.MainWindow; }
        }
    
        public IInternetFinder GetInternetFinder(IWebQuery iWebQuer)
        {
            return new InternetFinder(_ISFact.WebUserSettings, iWebQuer);
        }

        private Queue<Tuple<string, bool>> _ToBeRemoved;

        private Queue<Tuple<string, bool>> FileToRemove
        {
            get
            {
                if (_ToBeRemoved == null)
                    _ToBeRemoved = new Queue<Tuple<string, bool>>();

                return _ToBeRemoved;
            }
        }

        public void AddFileTobeRemovedLater(string File, bool Reversible)
        {
            FileToRemove.Enqueue(new Tuple<string, bool>(File, Reversible));
        }

        private void CleanFilesToBeRemoved()
        {
            if (_ToBeRemoved == null)
                return;

            int C = _ToBeRemoved.Count;

            for (int i = 0; i < C; i++)
            {
                Tuple<string, bool> res = _ToBeRemoved.Dequeue();
                _MusicFolderHelper.RemoveFileAndEmptyDirectory(res.Item1, res.Item2);

            }

            _ToBeRemoved = null;
        }

        IMusicImporter IMusicSession.GetDBImporter()
        {
            return LazyLoadingMusicImporter.GetFactory(this).GetDBImporter();
        }

        IMusicImporterBuilder IMusicSession.GetImporterBuilder(MusicImportType itype)
        {
            return MusicImporterBuilder.GetFromType(this, itype);
        }

        private MusicExporterFactory _MEF;
        private MusicExporterFactory ExporterFactory
        {
            get
            {
                if (_MEF == null)
                    _MEF = new MusicExporterFactory(this);

                return _MEF;
            }
        }


        IMusicExporterFactory IMusicSession.GetExporterFactory()
        {
            return ExporterFactory;
        }

        USBDriveListener IInternalMusicSession.DriverListener
        {
            get { return _DisListener; }
        }

        public bool IsEnded 
        {
            get { return _IsClosed; }
        }

        private bool _IsClosed = false;
        public void Dispose()
        {
            if (_IsClosed)
            {
                Trace.WriteLine("MusicCollection already closed");
                return;
            }

            Trace.WriteLine("Closing MusicCollection");
            _IsClosed = true; 

            if (_sem != null)
            {
                Trace.WriteLine(string.Format("Under transaction: {0}", _sem.CurrentCount == 0)); 

                // Here we wait for current import(s) if any to finalize.
                // It should be not too long as task should have been canceled.
                // This waiting is needed in order for the import to perform clean if needed.
                _sem.Wait();
                _sem.Release();
                _sem.Dispose();
                _sem = null;
            }

            SplashScreen.GenerateIfNeccessary();

            Setting.SessionEnds();

            if (this._SessionCose != null)
            {
                new SQLExecute(_SessionCose,this).Execute();
                _SessionCose = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (this._MusicConverter.IsValueCreated)
            {
                _MusicConverter.Value.Dispose();
            }

            CleanFilesToBeRemoved();

            if (_DisListener != null)
            {
                _DisListener.Dispose();
                _DisListener = null;
            }

            if (_ISF != null)
            {
                _ISF.Dispose();
                _ISF = null;
            }

            if (_AllTracks != null)
            {
                _AllTracks.Dispose();
                _AllTracks = null;
            }

            if (_AllAlbums != null)
            {
                _AllAlbums.Dispose();
                _AllAlbums = null;
            }

            if (_AllArtists != null)
            {
                _AllArtists.Dispose();
                _AllArtists = null;
            }

            if (_AllGenres != null)
            {
                _AllGenres.Dispose();
                _AllGenres = null;
            }

            if (_PlayListFactory!=null)
            {
                _PlayListFactory.Dispose();
                _PlayListFactory = null;
            }

            Trace.WriteLine("MusicCollection Closed");
        }

        public IInfraDependencies Dependencies { get; private set; }

        IMusicRemover IMusicSession.GetMusicRemover()
        {
            return MusicRemover.GetMusicRemover(this);
        }

        private ISessionFactory _ISF;
        public ISessionFactory GetNHibernateFactory()
        {
            return _ISF;
        }

        private MusicFolderHelper _MusicFolderHelper;
        MusicFolderHelper IInternalMusicSession.Folders
        {
            get { return _MusicFolderHelper; }
        }

        private IMusicSettings _ISFact;
        public IMusicSettings Setting
        {
            get { return _ISFact; }
        }

        public IEmailFactory GetEmailFactory()
        {
            return new EmailFactory( _ISFact.EmailInformationSettings); 
        }

        private Nullable<bool> _CleanOnOpen;
        public Nullable<bool> DBCleanOnOpen
        {
            get { return _CleanOnOpen; }
        }

        private string _SessionCose;

        private MusicSessionImpl(ISessionBuilder isb,IMainWindowHwndProvider mp)
        {
            _IMW = mp;
            _AllAlbums = new AlbumCollection(this);
            _AllArtists = new ArtistCollection(this);
            _AllTracks = new TrackCollection(this);
            _MusicFolderHelper = isb.Folders;
            _ISF = isb.GetNhibernateConfiguration(DBFactoryBuilder.GetConfiguration).BuildSessionFactory();
 
            _ISFact = isb.SettingFactory;
            _CleanOnOpen = isb.DBCleanOnOpen;
            _SessionCose = isb.OnSessionClose;
            this.Dependencies = isb.InfraTools;

            TraceListener = new ApplicationTraceListener();

            _MusicConverter = new Lazy<IMusicConverter>(isb.MusicConverterBuilder);

            Trace.Listeners.Add(TraceListener);
        }

        internal ApplicationTraceListener TraceListener { get; private set; }

        ImportTransaction IInternalMusicSession.GetNewSessionContext(AlbumMaturity iDefaultMaturity)
        {
            return new ImportTransaction(this, iDefaultMaturity);
        }

        private class SessionLocker : IDisposable
        {
            private SemaphoreSlim _sem;
            private MusicSessionImpl _MSI;

            internal SessionLocker(SemaphoreSlim sem, MusicSessionImpl iMSI)
            {
                _sem = sem;
                _sem.Wait();
                _MSI = iMSI;
            }

            public void Dispose()
            {
                if (_MSI._sem != null)
                    _sem.Release();
            }
        }

        public IDisposable GetSessionLock()
        {
            return new SessionLocker(_sem,this);
        }

        bool IMusicSession.IsUnderTransaction
        {
            get { return (_sem.CurrentCount == 0); }
        }

        #region Album

        private AlbumCollection _AllAlbums = null;

        AlbumCollection IInternalMusicSession.Albums
        {
            get { return _AllAlbums; }
        }

        #endregion

        #region Artist

        private ArtistCollection _AllArtists = null;

        ArtistCollection IInternalMusicSession.Artists
        {
            get { return _AllArtists; }
        }

        public IObservableCollection<IArtist> AllArtists
        {
            get { return _AllArtists.ReadOnlyUICollection; }
        }

        public IArtist CreateArtist(string ArtistName)
        {
            string name = ArtistName.TitleLike();
            return _AllArtists.FindOrCreate(name, (s) => new Artist(name));
        }


        internal IEntityFinder<IArtist> ArtistFinder
        {
            get { return _AllArtists.ArtistFinder; }
        }


        #endregion

        #region Genre

        private GenreCollection _AllGenres;

        GenreCollection IInternalMusicSession.Genres
        {
            get { return PrivateGenres; }
        }

        private GenreCollection PrivateGenres
        {
            get
            {
                if (_AllGenres == null)
                    _AllGenres = new GenreCollection(this);

                return _AllGenres;
            }
        }

        public ObservableCollection<IGenre> AllGenres
        {
            get { return PrivateGenres.Genres; }
        }

        public IMusicGenreFactory GetGenreFactory()
        {
            return Genre.GetFactory(this);
        }

        #endregion

        #region Tracks

        private TrackCollection _AllTracks = null;//new TrackCollection();


        TrackCollection IInternalMusicSession.Tracks
        {
            get { return _AllTracks; }
        }

        public IFullObservableCollection<ITrack> AllTracks
        {
            get { return _AllTracks; }
        }

        #endregion

        public IFullObservableCollection<IAlbum> AllAlbums
        {
            get { return _AllAlbums; }
        }

        private PlayListFactory _PlayListFactory = null;

        IPlayListFactory IMusicSession.PlayListFactory
        {
            get
            {
                if (_PlayListFactory == null)
                    _PlayListFactory = new PlayListFactory(this);

                return _PlayListFactory;
            }
        }


        private IMusicPlayer _AM = null;

        public IMusicPlayer MusicPlayer
        {
            get
            {
                if (_AM == null)
                {
                    _AM = new MusicCollection.MusicPlayer.MusicPlayer(Dependencies.MusicFactory);
                    PropertyHasChanged("MusicPlayer");
                }
                
                return _AM;
            }
        }

        public bool Invariant
        {
            get
            {
                if (!this._AllAlbums.Invariant)
                    return false;

                if (!this._AllTracks.Invariant)
                    return false;

                var othertrack = AllAlbums.SelectMany(al => al.Tracks).OrderBy(tr=>tr.Path);
                return AllTracks.OrderBy(tr => tr.Path).SequenceEqual(othertrack);
          }
        }

        private AlbumSorter _Sorter;
        public IAlbumSorter AlbumSorter
        {
            get
            {
                if (_Sorter == null)
                    _Sorter = new AlbumSorter(this);

                return _Sorter;
            }
        }

        public IMusicSplashScreenHelper SplashScreen 
        {
            get { return new SplashScreenGenerator(this); }
        }

        private WebQueryFactory _WQF;
        public IWebQueryFactory WebQueryFactory 
        {
            get { if (_WQF == null) _WQF = new WebQueryFactory(this); return _WQF; }
        }

        private MergeStrategyFactory _SF;
        public IMergeStrategyFactory Strategy
        {
            get
            {
                if (_SF == null)
                    _SF = new MergeStrategyFactory();

                return _SF;
            }
        }

        public IMultiEntityEditor GetTrackEditor(IEnumerable<ITrack> tracks)
        {
            return new AlbumInfoEditor(tracks.Cast<Track>().Distinct(), this);
        }

        public IMultiEntityEditor GetAlbumEditor(IEnumerable<IAlbum> albums)
        {
            return new AlbumInfoEditor(albums.Cast<Album>().Distinct().SelectMany(a => a.RawTracks), this);
        }


        public IList<IArtist> GetArtistFromName(string name)
        {
            IEnumerable<Artist> res = Artist.GetArtistFromName(name, this);

            return (res.Any() ? new List<IArtist>(res) : null);
        }

        public IDiscInformationProvider GetITunesCDIdentificator()
        {
            return new iTunesCDInformationFinder();
        }

        private EntityFinder _EF;
        public ISessionEntityFinder EntityFinder
        {
            get { if (_EF == null) { _EF = new EntityFinder(this); } return _EF; }
        }

        
        IMusicConverter IInternalMusicSession.MusicConverter
        {
            get { return _MusicConverter.Value; }
        }
    }
}
