using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{

    internal abstract class LiveToLookUpBase<TKey, TSource, TElement, Tint> : LiveCollectionFunction<TSource, IObservableGrouping<TKey, TElement>, Tint>, IObservableLookup<TKey, TElement> where TSource : class
    {
        #region collection
        protected class MyCollection : ROCollection<TElement>
        {
            internal MyCollection(IEnumerable<TElement> coll)
                : base(coll)
            {
            }

            internal MyCollection()
                : base()
            {
            }

            internal new void RealAdd(TElement item,bool Silent=false)
            {
                base.RealAdd(item, Silent);
            }

            //internal new bool RealRemove(TElement item)
            //{
            //    return base.RealRemove(item);
            //}

            internal new void RealInsert(int oldi, TElement el)
            {
                base.RealInsert(oldi, el);
            }

            //internal new void RealMove(int oldi, int newi)
            //{
            //    base.RealMove(oldi, newi);
            //}

            internal new void RealOverWrite(int index, TElement value)
            {
                base.RealOverWrite(index, value);
            }

            internal new void RealRemoveAt(int index)
            {
                base.RealRemoveAt(index);
            }


        }
        #endregion

        #region innergrouping
        protected class InternalGrouped : IObservableGrouping<TKey, TElement>
        {
            private LiveToLookUpBase<TKey, TSource, TElement, Tint> _Father;
            internal InternalGrouped(IGrouping<TKey, TElement> entry, LiveToLookUpBase<TKey, TSource, TElement, Tint> iFather)
            {
                Key = entry.Key;
                CollectionImpl = new MyCollection(entry);
                _Father = iFather;
            }

            internal InternalGrouped(TKey ikey, TElement element, int Index, LiveToLookUpBase<TKey, TSource, TElement, Tint> iFather)
            {
                Key = ikey;
                CollectionImpl = new MyCollection();
                CollectionImpl.RealAdd(element);
                _Father = iFather;
            }

            internal InternalGrouped(TKey ikey, TElement element, LiveToLookUpBase<TKey, TSource, TElement, Tint> iFather, bool Silent)
            {
                Key = ikey;
                CollectionImpl = new MyCollection();
                CollectionImpl.RealAdd(element, Silent);
                _Father = iFather;
            }

            public TKey Key
            {
                get;
                private set;
            }

            private MyCollection CollectionImpl
            {
                get;
                set;
            }

            public IExtendedObservableCollection<TElement> Collection
            {
                get { return CollectionImpl; }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                return CollectionImpl.GetEnumerator();
            }

            internal int CollectionCount
            {
                get { return CollectionImpl.Count; }
            }

            internal bool RemoveFromCollection(TElement elem, int Position)
            {
                CollectionImpl.RealRemoveAt(_Father.GetInjectedCollectionIndexFromSourceIndex(Key, CollectionImpl, Position));
                return true;
            }

            internal void AddInCollection(TElement elem, bool Silent=false)
            {
                CollectionImpl.RealAdd(elem,Silent);
            }

            internal void InsertInCollection(TElement elem, int Pos)
            {
                int outindex = -1;
                if (_Father.GetInjectedCollectionIndexFromSourceIndex(Key, CollectionImpl, Pos, out outindex))
                {
                    CollectionImpl.RealAdd(elem);
                }
                else
                {
                    CollectionImpl.RealInsert(outindex, elem);
                }
            }

            internal void ChangeItem(ObjectAttributeChangedArgs<Tuple<TKey, TElement>> changes, int Pos)
            {
                   CollectionImpl.RealOverWrite(_Father.GetInjectedCollectionIndexFromSourceIndex(Key, CollectionImpl, Pos), changes.New.Item2);
            }

            internal void ChangeItems(ObjectAttributeChangedArgs<Tuple<TKey, TElement>> changes, IEnumerable<int> indexes)
            {
                indexes.Apply((i) => ChangeItem(changes, i));
            }


            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public override bool Equals(object obj)
            {
                InternalGrouped ig = obj as InternalGrouped;
                if (ig == null)
                    return false;

                if (!object.Equals(Key, ig.Key))
                    return false;

                return CollectionImpl.SequenceEqual(ig.CollectionImpl);
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("<Key: {0}; Values:{1}>", Key, string.Join(",", CollectionImpl));
            }
        }
        #endregion

        protected LiveToLookUpBase(IList<TSource> Source, Expression<Func<TSource, Tint>> KeySelector)
            : base(Source, KeySelector)
        {
        }

        protected void CreateonInit(TKey Key, TElement el,bool silent)
        {
            InternalGrouped ig = null;
            if (_Dic.TryGetValue(Key, out ig))
            {
                ig.AddInCollection(el, silent);
            }
            else
            {
                Register(new InternalGrouped(Key, el, this, silent), silent);
            }
        }

        protected void UpdateFromSource(bool silent = false)
        {
            this.RealClear(silent);
            this.ExpectedGrouping.Apply(g => this.Register(new InternalGrouped(g, this), silent));
        }

        private Dictionary<TKey, InternalGrouped> _Dic = new Dictionary<TKey, InternalGrouped>();
        private void Register(InternalGrouped ig, bool silent=false)
        {
            this.RealAdd(ig, silent);
            _Dic.Add(ig.Key, ig);
        }

        private bool UnRegister(InternalGrouped ig)
        {
            bool res = this.RealRemove(ig);
            _Dic.Remove(ig.Key);

            return res;
        }

        protected InternalGrouped GetFromKey(TKey key)
        {
            InternalGrouped res = null;
            _Dic.TryGetValue(key, out res);
            return res;
        }

        protected override IEnumerable<IObservableGrouping<TKey, TElement>> Expected
        {
            get { return this.ExpectedGrouping.Select(t => new InternalGrouped(t,this)); }
        }

        protected abstract ILookup<TKey, TElement> ExpectedGrouping
        {
            get;
        }

        public override bool Invariant
        {
            get
            {
                if (! Expected.SequenceCompareWithoutOrder(this))
                    return false;

                var expected = ExpectedGrouping;

                if (!expected.All(g => g.SequenceEqual(this[g.Key])))
                    return false;

                return this.All<IGrouping<TKey, TElement>>(g => g.SequenceEqual(expected[g.Key]));
            }
        }

        protected abstract TElement GetElementFromSource(TSource original, bool Old);

        protected abstract TKey GetKeyFromSource(TSource original, bool Old);

        protected abstract bool GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<TElement> Deployed, int index, out int outindex);

        protected abstract int GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<TElement> Deployed, int index);

        protected abstract void InitFromCollection();

        protected InternalGrouped RegisterItem(TKey mykey, TElement newItem, int Index)
        {
            InternalGrouped ig = null;
            if (_Dic.TryGetValue(mykey, out ig))
            {
                ig.InsertInCollection(newItem,Index);
                return ig ;
            }

            ig = new InternalGrouped(mykey, newItem, Index,this);
            Register(ig);
            return ig;
        }


        protected bool RegisterItems(TKey mykey, TElement newItem, IEnumerable<int> indexes, bool RemoveIfEmpty = false)
        {
            if (!indexes.Any())
                return false;

            InternalGrouped res = RegisterItem(mykey, newItem, indexes.First());
            indexes.Skip(1).Apply((i) => res.InsertInCollection(newItem, i));
            return true;
        }

        protected bool RemoveItem(InternalGrouped ig, TElement item, int Position)
        {
            if (ig.CollectionCount == 1)
            {
                return UnRegister(ig);
            }

            return ig.RemoveFromCollection(item, Position);
        }

        protected bool RemoveItems(InternalGrouped ig, TElement item, IEnumerable<int> indexes)
        {
            bool resOK = true;
            indexes.Apply((i) => resOK = resOK && RemoveItem(ig, item, i));
            return resOK;
        }


        protected override void AddItem(TSource newItem, int index, Nullable<bool> First)
        {
            RegisterItem(GetKeyFromSource(newItem, false), GetElementFromSource(newItem, false), index);
        }

        private InternalGrouped GetOld(TSource oldItem)
        {
            TKey mykey = GetKeyFromSource(oldItem, true);
            InternalGrouped ig = null;
            _Dic.TryGetValue(mykey, out ig);

            return ig;
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> Last)
        {
            InternalGrouped ig = GetOld(oldItem);
            if (ig==null)
                return false;

            return RemoveItem(ig, GetElementFromSource(oldItem, true),index);
        }

        protected override void OnClear()
        {
            _Dic.Clear();
            this.RealClear(true);
            InitFromCollection();
            SendResetEvent();
        }

        #region LookUp

        public bool Contains(TKey key)
        {
            return _Dic.ContainsKey(key);
        }

        public IObservableGrouping<TKey, TElement> GetObservableFromKey(TKey key)
        {
            return _Dic[key];
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                return _Dic[key];
            }
        }

        public new System.Collections.IEnumerator GetEnumerator()
        {
            return MyList.GetEnumerator();
        }

        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
        {
            return this.MyList.GetEnumerator();
        }

        #endregion

    }
}
