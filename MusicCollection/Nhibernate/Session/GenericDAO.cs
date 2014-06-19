using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

namespace MusicCollection.Nhibernate.Session
{
    internal class GenericDAO<T,TID>
    {
        private ISession _ISession;

        internal GenericDAO(ISession session)
        {
            _ISession = session;
            if (session==null)
                throw new NullReferenceException("Isession can not be null");
        }


        //internal T Get(TID ID)
        //{
        //    T item = _ISession.Get<T>(ID);
        //    return item;
        //}

        //internal T Load(TID ID)
        //{
        //    return _ISession.Load<T>(ID);
        //}


        internal IList<T> LoadAll()
        {
             return _ISession.CreateCriteria(typeof(T)).List<T>();
        }

        //internal void Save(T Item)
        //{
        //    _ISession.Save(Item);
        //}

        internal void SaveOrUpdate(T Item)
        {
            _ISession.SaveOrUpdate(Item);
        }

        //internal void SaveOrUpdate(IEnumerable<T> Items)
        //{
        //    foreach (T item in Items)
        //        SaveOrUpdate(item);
        //}

        //internal void Save(IEnumerable<T> Items)
        //{
        //    foreach (T item in Items)
        //        Save(item);
        //}

        //internal void Update(IEnumerable<T> Items)
        //{
        //    foreach (T item in Items)
        //        Update(item);
        //}

        //internal void Update(T entity) 
        //{
        //    _ISession.Update(entity);
        //}

        //internal void MakeTransient(IEnumerable<T> Items)
        //{
        //    foreach (T item in Items)
        //        MakeTransient(item);
        //}

        internal void MakeTransient(T entity)
        {
            _ISession.Delete(entity);
        }
    }

    internal class GenericIntDAO<T>:GenericDAO<T,int>
    {
        internal GenericIntDAO(ISession session):base(session)
        {
        }
    }
}
