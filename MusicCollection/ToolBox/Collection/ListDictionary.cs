using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection
{
    internal class ListDictionary<TK,TV> : IDictionary<TK,TV>
    {
        private List<KeyValuePair<TK, TV>> _List;

        public ListDictionary()
        {
            _List = new List<KeyValuePair<TK, TV>>();
        }

        public ListDictionary(IDictionary<TK, TV> from)
        {
            _List = new List<KeyValuePair<TK, TV>>();
            from.Apply(tkv => _List.Add(tkv));
        }

        private bool Compare(TK x, TK y)
        {
            return  x.Equals(y);
        }

        public void Add(TK key, TV value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "ArgumentNull_Key");
            }

            if (ContainsKey(key))
                    throw new ArgumentException("Argument_AddingDuplicate");

            _List.Add(new KeyValuePair<TK, TV>(key, value));
        }

        public bool ContainsKey(TK key)
        {
            return _List.Select(tkv => tkv.Key).Any(k => Compare(k,key));
        }

        public ICollection<TK> Keys
        {
            get { return _List.Select(tkv => tkv.Key).ToList(); }
        }

        public bool Remove(TK key)
        {
            var index = _List.Select((kvp, i) => new Tuple<int, TK>(i, kvp.Key)).Where(t => Compare(t.Item2, key)).FirstOrDefault();
            if (index == null)
                return false;

            _List.RemoveAt(index.Item1);
            return true;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            var index = _List.Select((kvp, i) => new Tuple<int, TK>(i, kvp.Key)).Where(t => Compare(t.Item2, key)).FirstOrDefault();
            if (index == null)
            {
                value = default(TV);
                return false;
            }

            value =  _List[index.Item1].Value;
            return true;
        }

        public ICollection<TV> Values
        {
            get { return _List.Select(kv=>kv.Value).ToList(); }
        }

        public TV this[TK key]
        {
            get
            {
                try
                {
                    return _List.Where(t => Compare(t.Key, key)).First().Value;
                }
                catch
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                if (!Remove(key))
                    throw new KeyNotFoundException();

                _List.Add(new KeyValuePair<TK, TV>(key, value));
            }
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            if (ContainsKey(item.Key))
                throw new ArgumentException("Argument_AddingDuplicate");

            _List.Add(item);
        }

        public void Clear()
        {
            _List.Clear();
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            if (! _List.Where(kv => Compare(kv.Key, item.Key)).Any())
                return false;

            var index = _List.Where(kv => Compare(kv.Key, item.Key)).FirstOrDefault();
            return object.Equals( index.Value , item.Value);
        } 
        
        public bool Remove(KeyValuePair<TK, TV> item)
        {
            var index = _List.Select((kvp, i) => new Tuple<int, TK>(i, kvp.Key)).Where(t => Compare(t.Item2, item.Key)).FirstOrDefault();
            if (index == null)
            {
                return false;
            }

            if  (!object.Equals(item.Value,_List[index.Item1].Value))
                return false;

            _List.RemoveAt(index.Item1);
            return true;
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _List.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _List.GetEnumerator();
        }
    }
}
