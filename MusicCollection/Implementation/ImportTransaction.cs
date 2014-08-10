using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using NHibernate;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Nhibernate.Mapping;
using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.Nhibernate.Utilities;
using MusicCollection.ToolBox;

using MusicCollection.SettingsManagement;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.Utilies;

namespace MusicCollection.Implementation
{
    internal enum CRUD 
    { 
        Unkown, 
        Ignore, 
        Created, 
        Update, 
        Delete, 
        Alreadysave 
    };


    internal class ImportTransaction : SimpleImportEventAdapter, IImportContext
    {
        private Dictionary<ISessionPersistentObject, Tuple<CRUD, int>> _Albums = new Dictionary<ISessionPersistentObject, Tuple<CRUD, int>>();

        private DBDisposable _DBTransaction = null;
        private int _AlbumCount = 0;

        private ExternalBlobManagerTransaction _BlobTransaction;
        public ExternalBlobManagerTransaction BlobTransaction
        {
            get
            {
                if (_BlobTransaction == null)
                    _BlobTransaction = new ExternalBlobManagerTransaction(this);

                return _BlobTransaction;
            }
        }

        #region Mananagers

        private IRarManager _RaraManager;
        public IRarManager RarManager
        {
            get { return _RaraManager; }
        }

        private IConvertManager _ConvertManager;
        public IConvertManager ConvertManager
        {
            get { return _ConvertManager; }
        }

        private IDeleteManager _DeleteManager;
        public IDeleteManager DeleteManager
        {
            get { return _DeleteManager; }
        }

        private ImageFormatManager _ImageFormatManager;
        public ImageFormatManager ImageManager
        {
            get { return _ImageFormatManager; }
        }

        private IXMLImportManager _XMLManager;
        public IXMLImportManager XMLManager
        {
            get { return _XMLManager; }
        }


        private AlbumMaturity _DefaultMaturity;

        public AlbumMaturity DefaultMaturity
        {
            get { return _DefaultMaturity; }
            private set { _DefaultMaturity = value; }
        }

        #endregion

        internal ImportTransaction(MusicSessionImpl session, AlbumMaturity iDefaultMaturity)
        {
            Session = session;
            DefaultMaturity = iDefaultMaturity;
            
            var setting = Session.Setting;

            _RaraManager = new RarManagerImpl(this, setting.RarFileManagement);
            _ConvertManager = new ConvertManagerImpl(this, setting.ConverterUserSettings);
            _DeleteManager = new DeleteManagerImpl(setting.CollectionFileSettings);
            _ImageFormatManager = new ImageManagerImpl(setting.ImageFormatManagerUserSettings);
            _XMLManager = new XMLManagerImpl();
        }

 
        public IMaturityUserSettings MaturityUserSettings { get { return Session.Setting.CollectionFileSettings; } }

        #region IImportContext

        MusicFolderHelper IImportContext.Folders
        {
            get { return Session.Folders; }
        }

        public string FindNewUnknownNameAlbumForArtist(string Artist)
        {
            int i = 0;
            Album Al = null;
            string AN = null;

            bool Virgem = false;

            while (Virgem == false)
            {
                AN = string.Format("Unknown Album {0}", ++i);
                Al = Session.Albums.FindByName(AN, Artist);
                Virgem = (Al == null);
            }

            if (AN == null)
                throw new Exception("Algo Error FindNewUnknownNameAlbumForArtist");

            return AN;
        }

        public IInternalMusicSession Session { get; private set; }

        private Tuple<CRUD, int> GetAlbumStatus(ISessionPersistentObject inAlbum)
        {
            Tuple<CRUD, int> alreadycalculated = new Tuple<CRUD, int>(CRUD.Unkown, 0);
            if (_Albums.TryGetValue(inAlbum, out alreadycalculated))
                return alreadycalculated;

            return null;
        }


        public void Publish(IArtist artist)
        {
            Session.Artists.Publish(artist);
        }

        private class DBDisposable : IMusicTransaction
        {
            private ImportTransaction _Owner;
            private HashSet<ISessionPersistentObject> _Albums = new HashSet<ISessionPersistentObject>();

            internal DBDisposable(ImportTransaction Owner)
            {
                if (Owner == null)
                    throw new Exception("session management");
                _Owner = Owner;
            }

            public IImportContext ImportContext
            {
                get { return _Owner; }
            }

            public void AddAlbum(ISessionPersistentObject inAlbum)
            {
                _Albums.Add(inAlbum);
            }

            public void Cancel()
            {
                //foreach (ISessionPersistentObject al in _Albums)
                //    _Owner.Cancel(al);
                _Albums.Apply(al => _Owner.Cancel(al));
                _Albums.Clear();
            }

            public void Commit()
            {
                using (IDBSession session = DBSession.CreateorGetCurrentSession(_Owner))
                {
                    using (ITransaction trans = session.NHSession.BeginTransaction())
                    {
                        IList<ISessionPersistentObject> alls = null;
  
                        bool OK = true;

                        try
                        {
                            alls = _Albums.OrderBy(a => _Owner.GetAlbumStatus(a).Item2).Where(a => _Owner.RawCommit(session, trans, a)).Distinct().ToList();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine("Nhibernate commit problem");
                            Trace.WriteLine(e);
                            OK = false;
                            trans.Rollback();
                            _Albums.Apply(a => _Owner.RawRollBack(session, trans, a));
                        }

                        if (OK)
                        {
                            trans.Commit();
                            alls.Apply(al => al.Publish());
                        }
                    }
                }


                _Albums.Apply(io => _Owner._Albums.Remove(io));
                _Albums.Clear();

                if (_Owner.BlobTransaction != null)
                    _Owner.BlobTransaction.Commit();
            }

            public void Dispose()
            {
                if (_Owner == null)
                    return;

                if (_Owner._DBTransaction == null)
                    return;

                Cancel();

                _Owner._DBTransaction = null;
                _Owner = null;
            }
        }

        public IMusicTransaction CreateTransaction()
        {
            if (_DBTransaction != null)
                throw new Exception("session management");

            _DBTransaction = new DBDisposable(this);

            return _DBTransaction;
        }

        public void AddFileTobeRemovedLater(string File, bool rev)
        {
            Session.AddFileTobeRemovedLater(File, rev);
        }

        #region Entity finder

        public Album FindByName(string AN, string Artist)
        {
            return Session.Albums.FindByName(AN, Artist);
        }

        public IList<Artist> GetArtistFromName(string name)
        {
            IEnumerable<Artist> res = Artist.GetArtistFromName(name, Session);

            return (res.Any() ? new List<Artist>(res) : null);
        }

        public Genre GetGenreFromName(string name, bool Create)
        {
            return Genre.GetGenre(name, Session, Create);
        }

        public AlbumStatus FindAlbumOrCreate(IAlbumDescriptor InPath, Func<Album> Factory)
        {
            AlbumStatus ast = FindAlbum(InPath);
            if (ast != null)
                return ast;

            Album res = Factory();
            AddForCreated(res);
            return new AlbumStatus(res, AlbumInfo.NewToTransaction);
        }


        //cache de status d'album pour suspicion
        private HashSet<DiscHash> _NoImportDiscHash;
        private void AddRefusedDischash(DiscHash dh)
        {
            if (_NoImportDiscHash == null)
                _NoImportDiscHash = new HashSet<DiscHash>();

            _NoImportDiscHash.Add(dh);
        }

        private bool IsComputedAndRefused(DiscHash dh)
        {
            if (_NoImportDiscHash == null)
                return false;

            return _NoImportDiscHash.Contains(dh);
        }

        //recherche + inpout utilisateur si objet deja existant ou suspitions
        // output: AlbumStatus si status trouve (deux cas trouve et on continue ou trouve et on arrete)
        //         null si besoin de creer un album
        private AlbumStatus FindAlbum(IAlbumDescriptor InPath)
        {

            IEnumerable<MatchAlbum> res = Session.Albums.FindAlbums(InPath);

            if ((res == null) || !(res.Any()))
                return null;

            Match<Album> Winner = res.First();

            //cherchons dans les caches
            if (Winner.Precision == MatchPrecision.Exact)
            {
                Nullable<bool> ress = UpdateStatusForAlbumifNeeded(Winner.FindItem);
                if (ress != null)
                {
                    return new AlbumStatus((ress == true) ? Winner.FindItem : null, AlbumInfo.AlreadyImported_ExactMatch);
                }
            }
            else
            {
                //verifions si ce hash n est pas deja interdit
                if (IsComputedAndRefused(InPath.IDs.RawHash))
                    return new AlbumStatus(null, AlbumInfo.AlreadyImported_ExactMatch);
            }

            //demande end user
            OtherAlbumsConfirmationNeededEventArgs ati = new OtherAlbumsConfirmationNeededEventArgs(res);
            OnError(ati);

            if (!ati.Continue)
            {
                OnFactorisableError<AlbumAlreadyImported>(InPath.DisplayName());
            }


            if (Winner.Precision == MatchPrecision.Exact)
            {
                AddAlbum(Winner.FindItem, ati.Continue ? CRUD.Update : CRUD.Ignore);
                return ati.Continue ? new AlbumStatus(Winner.FindItem, AlbumInfo.ValidatedByEU) : new AlbumStatus(null, AlbumInfo.RefusedByEU);
            }

            if (!ati.Continue)
            {
                //arrive ici on a refus EU d importer un disque car le hash a trouve un autre disque
                //je registre le hash pour sauvegarder ce choix
                AddRefusedDischash(InPath.IDs.RawHash);
                return new AlbumStatus(null, AlbumInfo.RefusedByEU);
            }

            //album similaire trouve mais EU demande continuer: je vais creer un album avec
            //les infos du descripteur
            return null;
        }

        public IEnumerable<MatchAlbum> FindFromHashes(DiscHash hash)
        {
            return Session.Albums.FindByCDHashes(hash);
        }

        public Track Find(ITrackDescriptor InPath)
        {
            Track result = null;

            IEnumerable<Match<Track>> res = Session.Tracks.Find(InPath);

            if ((res == null) || !(res.Any()))
                return null;

            Match<Track> tm = res.First();

            if (tm.Precision == MatchPrecision.Exact)
            {
                result = tm.FindItem;
            }
            else
            {
                AmbigueousTrackImportArgs ati = new AmbigueousTrackImportArgs(InPath.Name, from r in res select r.FindItem);
                OnError(ati);
                if (!ati.Continue)
                    result = tm.FindItem;
            }

            if (result != null)
                OnFactorisableError<FileAlreadyImported>(result.Path);

            return result;
        }

        #endregion


        private Nullable<bool> UpdateStatusForAlbumifNeeded(Album Al)
        {
            Tuple<CRUD, int> alreadycalculated = GetAlbumStatus(Al);
            if (alreadycalculated == null)
                return null;

            switch (alreadycalculated.Item1)
            {
                case CRUD.Created:
                case CRUD.Update:
                    Al.Context = this;
                    return true;

                case CRUD.Alreadysave:
                    _Albums.Remove(Al);
                    AddAlbum(Al, CRUD.Update);
                    return true;

                case CRUD.Ignore:
                    return false;

                default:
                    throw new Exception("ImportTransaction error");
            }

        }


        private void Commit(IDBSession session)
        {
            int C = _Albums.Count;
            if (C == 0)
                return;

            bool NeedToRollback = false;

            using (ITransaction transaction = session.NHSession.BeginTransaction())
            {
                HashSet<ISessionPersistentObject> alls = new HashSet<ISessionPersistentObject>();

                OnProgress(new Finalizing(1, 2));

                foreach (var Al in from atc in _Albums orderby atc.Value.Item2 select atc)
                {
                    try
                    {
                        bool NeedToRegisterInSession = Al.Key.ChangeCommitted(Al.Value.Item1, this, session.NHSession);
                        if (NeedToRegisterInSession)
                            alls.Add(Al.Key);

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Problem during session Nhibernate Commit:");
                        Trace.WriteLine(e);
                        NeedToRollback = true;
                        break;
                    }                        
                }

                if (NeedToRollback == false)
                {
                    transaction.Commit();

                    OnProgress(new DisplayingProgress());
                    alls.Apply(al => al.Publish());
                }
                else
                {
                    OnError(new UnknowError());
                    transaction.Rollback();
                    _Albums.Apply(al => al.Key.RollBackChanges(al.Value.Item1,this));
                }

            }

            _Albums.Clear();

            if ((BlobTransaction != null) && (NeedToRollback==false))
                BlobTransaction.Commit();
        }


        private bool RawCommit(IDBSession session, ITransaction transaction, ISessionPersistentObject iAlbum)
        {
            Tuple<CRUD, int> operation = GetAlbumStatus(iAlbum);
            if (operation == null)
                throw new Exception("Commit session management");

            return iAlbum.ChangeCommitted(operation.Item1, this, session.NHSession);
        }

        private bool RawRollBack(IDBSession session, ITransaction transaction, ISessionPersistentObject iAlbum)
        {
            Tuple<CRUD, int> operation = GetAlbumStatus(iAlbum);
            if (operation == null)
                throw new Exception("Commit session management");

            return iAlbum.RollBackChanges(operation.Item1, this);
        }
    
        private void OnLoadCompleted(object sender, ObjectInSessionArgs e)
        {
            e.NewObjectInSession.Apply(ob => ob.OnLoad(this));
        }

        public void LoadAllFromDB(Nullable<bool> cleanonOpen)
        {
            OnProgress(new OpeningAlbums());

            using (IDBSession session = DBSession.CreateorGetCurrentSession(this))
            {
                session.OnObjectLoads += OnLoadCompleted;

                using (ITransaction trans = session.NHSession.BeginTransaction())
                {
                    new SQLExecute("PRAGMA cache_size = 200000; PRAGMA temp_store=2;", session.NHSession).Execute();

                    if (cleanonOpen == true)
                    {
                        using (var tt = TimeTracer.TimeTrack("Load Time 1 (Clean Artist)"))
                        {
                            new OrphanArtistDBCleaner(session.NHSession).Clean();
                        }
                    }


                    using (var tt = TimeTracer.TimeTrack("Load Time 2 (Load Album)"))
                    {
                        GenericIntDAO<Album> AD = new GenericIntDAO<Album>(session.NHSession);
                        AD.LoadAll();
                    }

                    using (var tt = TimeTracer.TimeTrack("Load Time 3 (Load Genre)"))
                    {
                        GenericIntDAO<Genre> GD = new GenericIntDAO<Genre>(session.NHSession);
                        GD.LoadAll();
                    }

                    if (cleanonOpen == false)
                    {
                        GenericIntDAO<Artist> ARD = new GenericIntDAO<Artist>(session.NHSession);
                        ARD.LoadAll();
                    }

                    using (var tt = TimeTracer.TimeTrack("Load Time 4 (Commit DB changes)"))
                    {
                        trans.Commit();
                    }

                }
                session.OnObjectLoads -= OnLoadCompleted;
            }

        }



        public void Commit()
        {
            if (_Albums.Count == 0)
                return;

            using (IDBSession session = DBSession.CreateorGetCurrentSession(this))
            {
                Commit(session);
            }
        }

        public void AddForUpdate(ISessionPersistentObject Al)
        {
            Tuple<CRUD, int> alreadycalculated = GetAlbumStatus(Al);
            if (alreadycalculated != null)
            {
                if ((alreadycalculated.Item1 != CRUD.Created) && (alreadycalculated.Item1 != CRUD.Update))
                    throw new Exception("ImportTransaction error");
            }
            else
            {
                AddAlbum(Al, CRUD.Update);
            }
        }

        public void AddForCreated(ISessionPersistentObject Al)
        {
            AddAlbum(Al, CRUD.Created);
        }

        public void AddForRemove(ISessionPersistentObject Al)
        {
            AddAlbum(Al, CRUD.Delete);
        }

        public IDisposable SessionLock()
        {
            return Session.GetSessionLock();
        }

        public bool IsEnded
        {
            get { return Session.IsEnded; }
        }

        private void AddAlbum(ISessionPersistentObject Al, CRUD operation)
        {
            _Albums.Add(Al, new Tuple<CRUD, int>(operation, _AlbumCount++));

            if (!Al.ChangeRequested(operation, this))
                return;

            if (_DBTransaction != null)
                _DBTransaction.AddAlbum(Al);

        }

        private void Cancel(ISessionPersistentObject Al)
        {
            _Albums.Remove(Al);
            Al.Context = null;
        }

        #endregion


        private Dictionary<Type, List<ImportErrorItem>> _Errors = new Dictionary<Type, List<ImportErrorItem>>();


        public void OnFactorisableError<T>(IEnumerable<ImportErrorItem> message) where T : ImportExportErrorEventListItemsArgs
        {
            foreach (ImportErrorItem M in message)
            {
                OnFactorisableError<T>(M);
            }
        }

        public void Dispose()
        {
        }

        public void OnFactorisableError<T>(string message) where T : ImportExportErrorEventListItemsArgs
        {
            OnFactorisableError<T>(new ImportErrorItem(message));
        }

        public void OnFactorisableError<T>(IEnumerable<string> message) where T : ImportExportErrorEventListItemsArgs
        {
            OnFactorisableError<T>(message.Select(s => new ImportErrorItem(s)));
        }

        public void OnFactorisableError<T>(ImportErrorItem path) where T : ImportExportErrorEventListItemsArgs
        {
            List<ImportErrorItem> LS = null;
            if (!_Errors.TryGetValue(typeof(T), out LS))
            {
                LS = new List<ImportErrorItem>();
                _Errors.Add(typeof(T), LS);
            }
            LS.Add(path);
        }

        public void FireFactorizedEvents()
        {
            if (!ListeningError)
                return;

            foreach (var Couple in _Errors)
            {
                ImportExportErrorEventListItemsArgs Error = (ImportExportErrorEventListItemsArgs)Activator.CreateInstance(Couple.Key, Couple.Value);
                OnError(Error);
            }
        }
    }
}


