using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public enum CollectionStatus { Find, Create };

    public struct CollectionResult<T>
    {
        public T Item { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
    }

    public static class DictionaryExtension
    {

        //public static CollectionResult<TValue> FindOrCreate_ThreadSafe<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        //{
        //    TValue res = default(TValue);
        //    if (dic.TryGetValue(key, out res))
        //        return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Find };

        //    lock (dic)
        //    {
        //        if (dic.TryGetValue(key, out res))
        //            return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Find };

        //        res = Fac(key);
        //        dic.Add(key, res);
        //        return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Create };
        //    }   
        //}

        public static CollectionResult<TValue> FindOrCreate_ThreadSafe<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            TValue res = default(TValue);
            lock (dic)
            {
                if (dic.TryGetValue(key, out res))
                    return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Find };

                res = Fac(key);
                dic.Add(key, res);
                return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Create };
            }
        }

        public static CollectionResult<TValue> FindOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            TValue res = default(TValue);
            if (dic.TryGetValue(key, out res))
                return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Find };

            res = Fac(key);
            dic.Add(key, res);
            return new CollectionResult<TValue>() { Item = res, CollectionStatus = CollectionStatus.Create };
        }

        public static TValue FindOrCreateEntity<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            TValue res = default(TValue);
            if (dic.TryGetValue(key, out res))
                return res;

            res = Fac(key);
            dic.Add(key, res);
            return res;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue dvalue,
            Func<TKey, TValue, TValue> Updater)
        {
            TValue res = dvalue;
            if (dic.TryGetValue(key, out res))
            {
                TValue nv = Updater(key, res);
                dic[key] = nv;
                return nv;
            }

            dic.Add(key, dvalue);
            return dvalue;
        }

        public static IDictionary<TKey, TValue> Import<TKey, TValue>(this IDictionary<TKey, TValue> @this, IDictionary<TKey, TValue> source)
        {
            source.Apply(el => @this.Add(el.Key, el.Value));
            return @this;
        }
    }
}
