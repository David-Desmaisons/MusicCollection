using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox
{
    internal class SimpleLookImpl<TKey, TElement> : ILookup<TKey, TElement> where TElement : class
    {
        private IDictionary<TKey, List<TElement>> _LookUpDictionary;
        private Func<TElement, TKey> _KeyAcessor;

      

        public SimpleLookImpl(Func<TElement, TKey> iKeyAcessor, Func<IDictionary<TKey, List<TElement>>> Factory =null)
        {
            _KeyAcessor = iKeyAcessor; 
            _LookUpDictionary = (Factory != null) ? Factory() : new Dictionary<TKey, List<TElement>>();
        }

        //public SimpleLookImpl(Func<TElement, TKey> iKeyAcessor, int Size):base()
        //{
        //    _KeyAcessor = iKeyAcessor;
        //    _LookUpDictionary = new Dictionary<TKey, List<TElement>>(Size);
        //}



        public void Add(TElement ele)
        {
            _LookUpDictionary.FindOrCreateEntity(_KeyAcessor(ele), (k)=> new List<TElement>(1)).Add(ele);      
        }

        public void Remove(TElement ele)
        {
            TKey key = _KeyAcessor(ele);
            List<TElement> el = _LookUpDictionary[key];
            if (el.Count == 1)
            {
                _LookUpDictionary.Remove(key);
            }
            else
            {
                el.Remove(ele);
            }
        }

        public bool Contains(TKey key)
        {
            return _LookUpDictionary.ContainsKey(key);
        }

        public int Count
        {
            get { return _LookUpDictionary.Count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                List<TElement> res = null;
                if (_LookUpDictionary.TryGetValue(key, out res))
                    return res;

                return Enumerable.Empty<TElement>();
            }
        }

        #region grouping

        private class Grouping<TKey2, TElement2> : IGrouping<TKey2, TElement2>
        {
            private TKey2 _Key;
            private IEnumerable<TElement2> _Elemes;

            internal Grouping(TKey2 ikey, IEnumerable<TElement2> ielems)
            {
                _Key = ikey;
                _Elemes = ielems;
            }

            public TKey2 Key
            {
                get { return _Key; }
            }

            public System.Collections.Generic.IEnumerator<TElement2> GetEnumerator()
            {
                return _Elemes.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _Elemes.GetEnumerator();
            }
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (TKey key in _LookUpDictionary.Keys)
            {
                yield return new Grouping<TKey, TElement>(key, _LookUpDictionary[key]);
            }
        }

        #endregion

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
