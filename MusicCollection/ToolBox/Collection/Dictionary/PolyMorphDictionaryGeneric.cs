using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MusicCollection.ToolBox.Collection
{
    internal abstract class PolyMorphDictionaryGeneric<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected IDictionary<TKey, TValue> _Implementation;

        //internal const int TransitionToDictionary=25;
        internal const int TransitionToDictionary = 25;

        internal PolyMorphDictionaryGeneric()
        {
            _Implementation = null;
        }

        static public IDictionary<TKey, TValue> ScalableDictionary()
        {
            bool comparable = typeof(TKey).GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IComparable<>) && (i.GetGenericArguments()[0])==typeof(TKey) ).Any();
            if (comparable)
            {
                return new SortedDictionary<TKey, TValue>();
            }

            return new PolyMorphSimpleDictionary<TKey, TValue>();
        }

        #region SingleDictionary

        protected class SingleDictionary<Tkey, Tvalue> : IDictionary<Tkey, Tvalue>
        {
            private Tkey _Key;
            private Tvalue _Value;
            internal SingleDictionary(Tkey key, Tvalue value)
            {
                _Key = key;
                _Value = value;
            }

            #region Not supported 
                    
            public void Add(KeyValuePair<Tkey, Tvalue> item)
            {
                throw new NotImplementedException();
            }  
                      
            public void Add(Tkey key, Tvalue value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(Tkey key)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<Tkey, Tvalue> item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region readonly 
            
            public bool ContainsKey(Tkey key)
            {
                return Object.Equals(key, _Key);
            }

            public bool TryGetValue(Tkey key, out Tvalue value)
            {
                bool ok = Object.Equals(key, _Key);
                value = ok ? _Value : default(Tvalue);
                return ok;
            }

            public ICollection<Tvalue> Values
            {
                get { var res = new List<Tvalue>(); res.Add(_Value); return res; }
            }

            public Tvalue this[Tkey key]
            {
                get
                {
                    if (Object.Equals(key, _Key))
                        return _Value;

                    throw new KeyNotFoundException();
                }
                set
                {
                    if (Object.Equals(key, _Key))
                    {
                        _Value = value;
                    }
                    else throw new KeyNotFoundException();
                }
            } 
            
            public ICollection<Tkey> Keys
            {
                get { var res = new List<Tkey>(); res.Add(_Key); return res; }
            }

            public IEnumerator<KeyValuePair<Tkey, Tvalue>> GetEnumerator()
            {
                yield return new KeyValuePair<Tkey, Tvalue>(_Key, _Value);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
               yield return new KeyValuePair<Tkey, Tvalue>(_Key, _Value);
            } 
            
            public bool Contains(KeyValuePair<Tkey, Tvalue> item)
            {
                return (Object.Equals(item.Key, _Key)) && (Object.Equals(item.Value, _Value));
            }

            public void CopyTo(KeyValuePair<Tkey, Tvalue>[] array, int arrayIndex)
            {
                if (array==null)
                    throw new ArgumentNullException();

                if(arrayIndex<0)
                 throw new ArgumentOutOfRangeException();

                if (arrayIndex>1)
                    throw new ArgumentException();

                array[0] = new KeyValuePair<Tkey, Tvalue>(_Key, _Value);

            }

            public int Count
            {
                get { return 1; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion
        }

        #endregion

        #region implementation management

        public abstract void Add(TKey key, TValue value);

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _Implementation = null;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_Implementation == null)
                return false;

            TValue tv = default(TValue);

            if (!_Implementation.TryGetValue(item.Key, out tv))
                return false;

            if (!Object.Equals(tv, item.Value))
                return false;

            return Remove(item.Key);

        }

        public abstract bool Remove(TKey key);


        #endregion

        #region delegation to implementation

        public bool ContainsKey(TKey key)
        {
            return (_Implementation==null) ? false : _Implementation.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return (_Implementation == null) ? new List<TKey>() :  _Implementation.Keys; }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_Implementation == null)
            {
                value = default(TValue);
                return false;
            }

            return _Implementation.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return (_Implementation == null) ? new List<TValue>() : _Implementation.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_Implementation == null)
                    throw new KeyNotFoundException();

               return _Implementation[key];
            }
            set
            {
                if (_Implementation == null)
                    throw new KeyNotFoundException();

                _Implementation[key] = value;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (_Implementation == null) ? false : _Implementation.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_Implementation == null)
                return;

            _Implementation.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return (_Implementation == null) ? 0 : _Implementation.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (_Implementation == null) ? Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator() : _Implementation.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Implementation == null) ? Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator() : _Implementation.GetEnumerator();
        }

        #endregion
    }
}
