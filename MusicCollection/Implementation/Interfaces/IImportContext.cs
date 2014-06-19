using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.DataExchange;
using MusicCollection.Nhibernate.Session;

namespace MusicCollection.Implementation
{
    interface IMusicTransaction : IDisposable
    {
        void Cancel();
        void Commit();
        IImportContext ImportContext
        {
            get;
        }
    }

   

    internal interface IImportContext : IImporterEvent,IDisposable
    {
        IInternalMusicSession Session { get; }

        void Publish(IArtist artist);

        ExternalBlobManagerTransaction BlobTransaction
        {
            get;
        }

        #region Settings

        AlbumMaturity DefaultMaturity
        {
            get;
        }

        MusicFolderHelper Folders
        {
            get;
        }

        IConvertManager ConvertManager
        {
            get;
        }

        IRarManager RarManager
        {
            get;
        }

        //IMaturityManager MaturityManager
        //{ get; }

        IMaturityUserSettings MaturityUserSettings { get; }

        IDeleteManager DeleteManager
        {
            get;
        }

        ImageFormatManager ImageManager
        {
            get;
        }

        //IWebServicesManager WebServicesManager
        //{
        //    get;
        //}

        IXMLImportManager XMLManager
        {
            get;
        }

        #endregion  

        #region Facto Event

        void AddFileTobeRemovedLater(string File, bool reversible);

        void OnFactorisableError<T>(string message) where T : ImportExportErrorEventListItemsArgs;

        void OnFactorisableError<T>(IEnumerable<string> message) where T : ImportExportErrorEventListItemsArgs;

        void OnFactorisableError<T>(ImportErrorItem message) where T : ImportExportErrorEventListItemsArgs;

        void OnFactorisableError<T>(IEnumerable<ImportErrorItem> message) where T : ImportExportErrorEventListItemsArgs;

        void FireFactorizedEvents();

        #endregion

        #region CRUD

        void AddForUpdate(ISessionPersistentObject Al);

        void AddForCreated(ISessionPersistentObject Al);  
        
        void AddForRemove(ISessionPersistentObject Al);

        #endregion
   
        #region transaction

        //bool ExecuteInANewDBTransaction(Func<IDBSession, bool> Action);

        IMusicTransaction CreateTransaction();       
        
        void Commit();

        //void Cancel();

        IDisposable SessionLock();

        #endregion

        Track Find(ITrackDescriptor InPath);

        //Nullable<CRUD> AlbumStatus(Album Al);

        IEnumerable<MatchAlbum> FindFromHashes(DiscHash hash);

        IList<Artist> GetArtistFromName(string name);

        Genre GetGenreFromName(string name, bool Create); 

        AlbumStatus FindAlbumOrCreate(IAlbumDescriptor ad, Func<Album> Factory);

        Album FindByName(string AN, string Artist);

        string FindNewUnknownNameAlbumForArtist(string Artist);

        void LoadAllFromDB(Nullable<bool> CleanOnOpen);

    }
}
