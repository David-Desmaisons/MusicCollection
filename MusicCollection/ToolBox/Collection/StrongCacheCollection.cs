using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MusicCollection.ToolBox.Collection
{
    internal class StrongCacheCollection<TKey, TValue> : AbstractCacheCollection<TKey, TValue>, ICacheCollection<TKey, TValue>
        where TValue : class
        where TKey : class
    {
        private IDictionary<TKey, TValue> _Dic;


        internal StrongCacheCollection(Expression<Func<TValue, TKey>> iAcessor, Func<TKey, TKey> Preprocessing = null, Func<IDictionary<TKey, TValue>> Factory=null)
            : base(iAcessor, Preprocessing)
        {
            _Dic = (Factory==null) ? new Dictionary<TKey, TValue>() : Factory();
        }

        public override IEnumerable<TValue> Values 
        {
           get
           {
               return _Dic.Values;
           }
       }

        public override IEnumerable<TKey> Keys
        {
            get
            {
                return _Dic.Keys;
            }
        }

        
        protected internal override bool RawRegister(TKey key, TValue value)
        {
            _Dic.Add(key, value);
            return true;
        }

        protected internal override bool RawRemove(TKey key)
        {
            return _Dic.Remove(key);
        }

        //protected internal override TValue RawRemoveAndFind(TKey key)
        //{
        //    TValue find = null;
        //    if (!_Dic.TryGetValue(key, out find))
        //    {
        //        return null;
        //    }

        //    _Dic.Remove(key);
        //    return find;
        //}

        internal int Count
        {
            get
            {
                lock (_Locker)
                {
                    return _Dic.Count;
                }
            }
        }

        protected internal override TValue RawFind(TKey key)
        {
            TValue res = null;
            if (_Dic.TryGetValue(key, out res))
            {
                    return res;    
            }

            return null;
        }
    }

     
}
