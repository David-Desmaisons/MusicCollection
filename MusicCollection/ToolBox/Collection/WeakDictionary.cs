using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection
{
    internal class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        internal WeakDictionary()
        {
           _Dic = new Dictionary<TKey, WeakReference<TValue>>();
        }

        internal WeakDictionary(IDictionary<TKey, WeakReference<TValue>> iDictionaryImpl)
        {
           _Dic = iDictionaryImpl;
        }

        private IDictionary<TKey, WeakReference<TValue>> _Dic;

        private Tuple<TValue,bool> GetValueFromKey(TKey key)
        {
            WeakReference<TValue> Wr = null;
            if (_Dic.TryGetValue(key, out Wr))
            {
                TValue res = null;

                if (Wr.TryGetTarget(out res))
                {
                    return new Tuple<TValue, bool>(res, true);
                }

                _Dic.Remove(key);
            }

            return new Tuple<TValue, bool>(default(TValue), false);
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerable()
        {
            //return _Dic.Where(wk => wk.Value.IsAlive).Select(wk => new KeyValuePair<TKey, TValue>(wk.Key, (TValue)wk.Value.Target));
            TValue res = null;
            foreach(var wk in _Dic)
            {       
                if (wk.Value.TryGetTarget(out res)) yield return new KeyValuePair<TKey, TValue>(wk.Key,res); 
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException();

            _Dic.Add(key, new WeakReference<TValue>(value));
        }

        public bool ContainsKey(TKey key)
        {
            return GetValueFromKey(key).Item2;
        }

        public ICollection<TKey> Keys
        {
            get { return GetEnumerable().Select(t=>t.Key).ToList(); }
        }

        public bool Remove(TKey key)
        {
            return _Dic.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Tuple<TValue,bool> res = GetValueFromKey(key);
            if (res.Item2)
            {
                value = res.Item1;
                return true;
            }
    
            value = default(TValue);
            return false;
            
        }

        public ICollection<TValue> Values
        {
            get { return GetEnumerable().Select(t => t.Value).ToList(); }
        }

        public TValue this[TKey key]
        {
            get
            {
                Tuple<TValue,bool>  res =GetValueFromKey(key);
                if (!res.Item2)
                    throw new KeyNotFoundException();

                return  res.Item1;
            }
            set
            {
                if (!Remove(key))
                    throw new KeyNotFoundException();

                Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _Dic.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            Tuple<TValue,bool> res = GetValueFromKey(item.Key);

            if (!res.Item2)
                return false;

            return object.Equals(res.Item1,item.Value);
        }

        public int Count
        {
            get { return GetEnumerable().Count(); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                Remove(item.Key);
                return true;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.GetEnumerable().ToList().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
