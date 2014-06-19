using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MusicCollection.ToolBox.LambdaExpressions;

namespace MusicCollection.ToolBox.Collection
{
    internal class LookUpImpl<TKey, TElement> : AbstractKeyListenerCollection<TKey,TElement>,
        ILookup<TKey, TElement> where TElement : class
    {
        private Dictionary<TKey, List<TElement>> _Elements = null;
        private int _Count = 0;

       

        internal LookUpImpl(Expression<Func<TElement, TKey>> iAcessor, int Size, Func<TKey, TKey> iPrepro = null):base(iAcessor,iPrepro)
        {
            lock (_Locker)
            {
                _Elements = new Dictionary<TKey, List<TElement>>(Size);
            }
        }

        public IEnumerable<TElement> AllElements
        {
            get { return Elements; }
        }
        

        #region AbstractKeyListenerCollectionn

        protected override IEnumerable<TElement> Elements
        {
            get { return (from o in _Elements.Values from el in o select el); }
        }

        protected override bool RawRegister(TElement elem)
        {
            if (elem == null)
                throw new Exception("elem can not be null");

            TKey key = ComputeKey(elem);

            if (key == null)
                return false;

            List<TElement> res = GetForKey(key);


            if (res == null)
            {
                res = new List<TElement>();
                _Elements.Add(key, res);
            }

            res.Add(elem);
            _Count++;
            return true;
        }

        protected override bool RawRemove(TElement elem, TKey key)
        {
            List<TElement> res = null;
            if (!_Elements.TryGetValue(key, out res))
            {
                return false;
            }

            bool ok = res.Remove(elem);
            if (!ok)
                return false;

            if (res.Count == 0)
            {
                ok = _Elements.Remove(key);
                if (!ok)
                    throw new Exception("elem not be found");
            }

            _Count--;
            return true;
        }

        #endregion

 
        private bool RawRemove(TElement elem)
        {
            if (elem == null)
                throw new ArgumentNullException("elem can not be null");

            TKey key = ComputeKey(elem);
            if (key == null)
                return false;

            return RawRemove(elem, key);
        }

        internal bool Remove(TElement elem)
        {
            lock (_Locker)
            {
                bool res = RawRemove(elem);
                if (res)
                    OnRemove(elem);
                return res;
            }
        }

        private List<TElement> GetForKey(TKey kiki)
        {
            List<TElement> res = null;
            if (_Elements.TryGetValue(kiki, out res))
                return res;

            return null;
        }

        internal bool Add(TElement elem)
        {
            lock (_Locker)
            {
                bool res = RawRegister(elem);
                if (res)
                    OnAdd(elem);
                return res;
            }
        }

        private TKey ComputeKey(TElement elem)
        {
            return _Acessor(elem);
        }

        public bool Contains(TKey key)
        {
            lock (_Locker)
            {
                return (_Elements.ContainsKey(key));
            }
        }

        public int Count
        {
            get
            {
                lock (_Locker)
                { 
                    return _Count; 
                }
            }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get { return GetForKey(key) ?? Enumerable.Empty<TElement>(); }
        }

        private class Grouping<TKey2,TElement2>:IGrouping<TKey2,TElement2>
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
            foreach (TKey key in _Elements.Keys)
            {
                yield return new Grouping<TKey, TElement>(key, _Elements[key]);
            }

            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
     
       
    }
}
