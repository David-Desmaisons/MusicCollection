using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Text;

using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection
{
     [DebuggerDisplay("Count = {Count}")]
    internal abstract class AbstractCacheCollection<TKey, TValue> : AbstractKeyListenerCollection<TKey, TValue>, ICacheCollection<TKey, TValue>
        where TValue : class
    {

        protected AbstractCacheCollection(Expression<Func<TValue, TKey>> iAcessor, Func<TKey, TKey> Preprocessing = null)
            :base(iAcessor,Preprocessing)
        {
        }

        protected override IEnumerable<TValue> Elements
        {
            get { return Values; }
        }

        protected override bool RawRegister(TValue iEl)
        {
            return RawRegister(GetKey(iEl), iEl);
        }

        protected override bool RawRemove(TValue iEl, TKey key)
        {
            return RawRemove(key);
        }

        abstract internal protected bool RawRegister(TKey key, TValue value);

        abstract internal protected TValue RawFind(TKey key);

        abstract internal protected bool RawRemove(TKey key);

        //abstract internal protected TValue RawRemoveAndFind(TKey key);

        public abstract IEnumerable<TValue> Values { get; }

        public abstract IEnumerable<TKey> Keys { get; }

        public void Register(TValue value)
        {
            lock (_Locker)
            {
                if ( (IsUnsafeOnRename) && (Values.Contains(value)) )
                { 
                    throw new ArgumentException("item already in cache");
                }

                GoAndRegister(GetKey(value), value);
            }
        }

        //public Tuple<TValue, bool> FindOrRegisterValue(TValue value)
        //{
        //    lock (_Locker)
        //    {
        //        if (_Acessor == null)
        //            throw new ArgumentException("CacheCollection Acessor needed");

        //        TKey key = GetKey(value);

        //        if (key == null)
        //        {
        //            Trace.WriteLine("Something strange here");
        //            return new Tuple<TValue, bool>(null, false);
        //        }

        //        TValue res = RawFind(key);
        //        if (res != null)
        //            return new Tuple<TValue, bool>(res, true);

        //        GoAndRegister(key, value);
        //        return new Tuple<TValue, bool>(value, false);
        //    }
        //}

        //public TValue FindOrRegister(TValue value)
        //{
        //    lock (_Locker)
        //    {
        //        TKey key = GetKey(value);

        //        if (key == null)
        //            return null;

        //        TValue res = RawFind(key);
        //        if (res != null)
        //            return res;

        //        GoAndRegister(key, value);
        //        return value;
        //    }
        //}

        public Tuple<TValue, bool> FindOrCreateValue(TKey key, Func<TKey, TValue> Constructor)
        {
            lock (_Locker)
            {
                if (key == null)
                    return null;

                TKey normalized = ProcessKey(key);

                TValue res = RawFind(normalized);

                if (res != null)
                    return new Tuple<TValue, bool>(res, true);

                res = Constructor.Invoke(normalized);

                GoAndRegister(normalized, res);

                return new Tuple<TValue, bool>(res, false);
            }
        }

        //public TValue FindOrCreate(TKey key, Func<TKey, TValue> Constructor)
        //{
        //    lock (_Locker)
        //    {
        //        if (key == null)
        //            return null;

        //        return FindOrCreateValue(key, Constructor).Item1;
        //    }
        //}

        public TValue Find(TKey key)
        {
            lock (_Locker)
            {
                if (key == null)
                    return null;

                return RawFind(ProcessKey(key));
            }
        }

        //public TValue Find(TValue key)
        //{
        //    lock (_Locker)
        //    {
        //        if (key == null)
        //            return null;

        //        return RawFind(GetKey(key));
        //    }
        //}

        //public bool Remove(TKey key)
        //{
        //    lock (_Locker)
        //    {
        //        if (key != null)
        //            return GoAndRemove(ProcessKey(key));

        //        return false;
        //    }
        //}

        public bool Remove(TValue key)
        {
            lock (_Locker)
            {
                return GoAndRemove(key);
            }
        }

        //public void OnObjectChange(object sender, ClassTimeChangeEvent<TValue> what)
        //{
        //    if (what.Before)
        //    {
        //        RawRemove(GetKey(what.Who));
        //    }
        //    else
        //    {
        //        RawRegister(GetKey(what.Who), what.Who);
        //    }
        //}

        private void GoAndRegister(TKey key, TValue value)
        {
            if (key == null)
                return;

            RawRegister(key, value);
            OnAdd(value);
        }

        //private bool GoAndRemove(TKey key)
        //{
        //    if (key == null)
        //        return false;

        //    TValue res = RawRemoveAndFind(key);
        //    if (res == null)
        //        return false;

        //    OnRemove(res);
        //    return true;
        //}

        private bool GoAndRemove(TValue value)
        {
            TKey key = GetKey(value);
            if (key == null)
                return false;

            if (RawRemove(key))
            {
                OnRemove(value);
                return true;
            }

            return false;
        }  
    }
}
