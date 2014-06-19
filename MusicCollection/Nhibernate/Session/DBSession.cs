using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Diagnostics;

using NHibernate;
using NHibernate.Context;
using NHibernate.Metadata;
using NHibernate.SqlCommand;
using NHibernate.Type;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;


using MusicCollection.Nhibernate.Blob;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.FileImporter;
using MusicCollection.Nhibernate.Mapping;



namespace MusicCollection.Nhibernate.Session
{
    internal class DBSession : IDBSession
    {


        static internal IDBSession CreateorGetCurrentSession(IImportContext MI)
        {
            return new DBSession(MI);
        }

       

        private NhibernateSession _NHS;

        public ISession NHSession
        {
            get;
            private set;
        }

        internal DBSession(IImportContext msi)
        {
            if (msi == null)
                throw new ArgumentNullException("session can not be null IDBSession CreateorGetCurrentSession");

            _NHS = new NhibernateSession(msi, this);
            NHSession = msi.Session.GetNHibernateFactory().OpenSession(_NHS);
            
        }

        internal void ObjectLoads(ObjectInSessionArgs ois)
        {
            EventHandler<ObjectInSessionArgs> lOnObjectLoads = OnObjectLoads;
            if (lOnObjectLoads == null)
            {
                Trace.WriteLine("Big bug probable");
                return;
            }

            lOnObjectLoads(this, ois);
        }

       

        public void Dispose()
        {
            NHSession.Dispose();
        }

        public event EventHandler<ObjectInSessionArgs> OnObjectLoads;


        private class NhibernateSession : IInterceptor
        { 
            internal NhibernateSession(IImportContext msi, DBSession dbref)
            {
                _IT = msi;
                _DB = dbref;
            }

            private ExternalBlobManagerTransaction _Transaction
            {
                get { return _IT.BlobTransaction; }
            }

            private IImportContext _IT = null;

 
            //
            // Summary:
            //     Called after a transaction is committed or rolled back.
            public void AfterTransactionCompletion(ITransaction tx)
            {
                if (_ListObjects == null)
                    return;

                _DB.ObjectLoads(new ObjectInSessionArgs(ListObjects));
                ListObjects = null;
            }

           

          

            private DBSession _DB;

          


            // Summary:
            //     Called when a NHibernate transaction is begun via the NHibernate NHibernate.ITransaction
            //     API. Will not be called if transactions are being controlled via some other
            //     mechanism.
            public void AfterTransactionBegin(ITransaction tx)
            {
                if (tx == null)
                    return;

            }





            //
            // Summary:
            //     Called before a transaction is committed (but not before rollback).
            public void BeforeTransactionCompletion(ITransaction tx)
            {
                if ((_Transaction == null))
                    throw new Exception("Not Handled");
            }



            //
            // Summary:
            //     Called from Flush(). The return value determines whether the entity is updated
            //
            // Parameters:
            //   entity:
            //     A persistent entity
            //
            //   currentState:
            //
            //   id:
            //
            //   previousState:
            //
            //   propertyNames:
            //
            //   types:
            //
            // Returns:
            //     An array of dirty property indicies or null to choose default behavior
            //
            // Remarks:
            //      an array of property indicies - the entity is dirty an empty array - the
            //     entity is not dirty null - use Hibernate's default dirty-checking algorithm
            public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
            {
                return null;
            }



            //
            // Summary:
            //     Get a fully loaded entity instance that is cached externally
            //
            // Parameters:
            //   entityName:
            //     the name of the entity
            //
            //   id:
            //     the instance identifier
            //
            // Returns:
            //     a fully initialized entity
            public object GetEntity(string entityName, object id)
            {
                return null;
            }


            //
            // Summary:
            //     Get the entity name for a persistent or transient instance
            //
            // Parameters:
            //   entity:
            //     an entity instance
            //
            // Returns:
            //     the name of the entity
            public string GetEntityName(object entity)
            {
                return null;
            }


            //
            // Summary:
            //     Instantiate the entity class. Return null to indicate that Hibernate should
            //     use the default constructor of the class
            //
            // Parameters:
            //   entityName:
            //     the name of the entity
            //
            //   entityMode:
            //     The type of entity instance to be returned.
            //
            //   id:
            //     the identifier of the new instance
            //
            // Returns:
            //     An instance of the class, or null to choose default behaviour
            //
            // Remarks:
            //     The identifier property of the returned instance should be initialized with
            //     the given identifier.
            public object Instantiate(string entityName, EntityMode entityMode, object id)
            {
                return null;
            }


            //
            // Summary:
            //     Called when a transient entity is passed to SaveOrUpdate.
            //
            // Parameters:
            //   entity:
            //     A transient entity
            //
            // Returns:
            //     Boolean or null to choose default behaviour
            //
            // Remarks:
            //     The return value determines if the object is saved true - the entity is passed
            //     to Save(), resulting in an INSERT false - the entity is passed to Update(),
            //     resulting in an UPDATE null - Hibernate uses the unsaved-value mapping to
            //     determine if the object is unsaved
            public bool? IsTransient(object entity)
            {
                return null;
            }


            //
            // Summary:
            //     Called before a collection is (re)created.
            public void OnCollectionRecreate(object collection, object key)
            {
            }

            //
            // Summary:
            //     Called before a collection is deleted.
            public void OnCollectionRemove(object collection, object key)
            {
            }

            //
            // Summary:
            //     Called before a collection is updated.
            public void OnCollectionUpdate(object collection, object key)
            {
            }

            //
            // Summary:
            //     Called before an object is deleted
            //
            // Parameters:
            //   entity:
            //
            //   id:
            //
            //   propertyNames:
            //
            //   state:
            //
            //   types:
            //
            // Remarks:
            //     It is not recommended that the interceptor modify the state.
            public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
            {
                bool res = _Transaction.UpdateBlobToDiscIfNeeded(entity, id, null, state, propertyNames, types);
            }

            private List<ISessionPersistentObject> _ListObjects;
            private List<ISessionPersistentObject> ListObjects
            {
                get { if (_ListObjects == null) _ListObjects = new List<ISessionPersistentObject>(); return _ListObjects; }
                set { _ListObjects = value; }

            }

            //
            // Summary:
            //     Called just before an object is initialized
            //
            // Parameters:
            //   entity:
            //
            //   id:
            //
            //   propertyNames:
            //
            //   state:
            //
            //   types:
            //
            // Returns:
            //     true if the user modified the state in any way
            //
            // Remarks:
            //     The interceptor may change the state, which will be propagated to the persistent
            //     object. Note that when this method is called, entity will be an empty uninitialized
            //     instance of the class.
            public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
            {
                ISessionPersistentObject ispo = entity as ISessionPersistentObject;

                if (ispo != null)
                {
                    ListObjects.Add(ispo);
                }


                return false;
            }

            //
            // Summary:
            //     Called when sql string is being prepared.
            //
            // Parameters:
            //   sql:
            //     sql to be prepared
            //
            // Returns:
            //     original or modified sql
            public SqlString OnPrepareStatement(SqlString sql)
            {
                return sql;
            }

            //
            // Summary:
            //     Called when an object is detected to be dirty, during a flush.
            //
            // Parameters:
            //   currentState:
            //
            //   entity:
            //
            //   id:
            //
            //   previousState:
            //
            //   propertyNames:
            //
            //   types:
            //
            // Returns:
            //     true if the user modified the currentState in any way
            //
            // Remarks:
            //     The interceptor may modify the detected currentState, which will be propagated
            //     to both the database and the persistent object. Note that all flushes end
            //     in an actual synchronization with the database, in which as the new currentState
            //     will be propagated to the object, but not necessarily (immediately) to the
            //     database. It is strongly recommended that the interceptor not modify the
            //     previousState.
            public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
            {
                return _Transaction.UpdateBlobToDiscIfNeeded(entity, id, currentState, previousState, propertyNames, types);
            }

            //
            // Summary:
            //     Called before an object is saved
            //
            // Parameters:
            //   entity:
            //
            //   id:
            //
            //   propertyNames:
            //
            //   state:
            //
            //   types:
            //
            // Returns:
            //     true if the user modified the state in any way
            //
            // Remarks:
            //     The interceptor may modify the state, which will be used for the SQL INSERT
            //     and propagated to the persistent object
            public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
            {
                return _Transaction.UpdateBlobToDiscIfNeeded(entity, id, state, null, propertyNames, types);
            }


            //
            // Summary:
            //     Called after a flush that actually ends in execution of the SQL statements
            //     required to synchronize in-memory state with the database.
            //
            // Parameters:
            //   entities:
            //     The entitites
            public void PostFlush(ICollection entities)
            {
            }

            //
            // Summary:
            //     Called before a flush
            //
            // Parameters:
            //   entities:
            //     The entities
            public void PreFlush(ICollection entities)
            {
            }
            //
            // Summary:
            //     Called when a session-scoped (and only session scoped) interceptor is attached
            //     to a session
            //
            // Remarks:
            //     session-scoped-interceptor is an instance of the interceptor used only for
            //     one session.  The use of singleton-interceptor may cause problems in multi-thread
            //     scenario.
            public void SetSession(ISession session)
            {
            }

        }
    }



   



}
