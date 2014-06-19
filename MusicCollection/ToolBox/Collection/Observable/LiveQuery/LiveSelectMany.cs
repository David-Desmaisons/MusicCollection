using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.Diagnostics;

using MusicCollection.ToolBox.Event;
using MusicCollection.Infra;


namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class LiveSelectMany<TSource, TDest> : LiveSelectManyAbstract<TSource, TDest, TDest>
        where TSource : class
    {

        internal LiveSelectMany(IList<TSource> list, Expression<Func<TSource, IList<TDest>>> Acessor)
            : base(list, Acessor)
        {
        }

        protected override IEnumerable<TDest> FromCache
        {
            get { return _Source.SelectMany(item => _Cache[item].Deployed); }
        }

        protected override TDest Transform(TDest icool,TSource source)
        {
            return icool;
        }

        protected override IEnumerable<TDest> Expected
        {
            get { return _Source.SelectMany(_Function.Evaluate); }
        }
    }

    internal sealed class LiveSelectManyTuple<TSource, TDest> : LiveSelectManyAbstract<TSource, TDest, Tuple<TSource, TDest>>
     where TSource : class
    {

        internal LiveSelectManyTuple(IList<TSource> list, Expression<Func<TSource, IList<TDest>>> Acessor)
            : base(list, Acessor)
        {
        }

        protected override IEnumerable<Tuple<TSource,TDest>> FromCache
        {
            get { return _Source.SelectMany(item => _Cache[item].Deployed, (s, c) => new Tuple<TSource,TDest>(s, c)); }
        }

        protected override Tuple<TSource, TDest> Transform(TDest icool, TSource source)
        {
            return new Tuple<TSource,TDest>(source, icool);
        }

        protected override IEnumerable<Tuple<TSource,TDest>> Expected
        {
            get { return _Source.SelectMany(_Function.Evaluate, (s, c) => new Tuple<TSource,TDest>(s, c)); }
        }
    }





    //internal sealed class LiveSelectMany<TSource, TDest> : LiveCollectionFunction<TSource, TDest, IList<TDest>>
    //    where TSource : class
    internal abstract class LiveSelectManyAbstract<TSource, TCollection, TDest> : LiveCollectionFunction<TSource, TDest, IList<TCollection>>
    where TSource : class
    {

        protected Dictionary<TSource, ListenedList> _Cache;
        //= new Dictionary<TSource, ListenedList>();

        internal LiveSelectManyAbstract(IList<TSource> list, Expression<Func<TSource, IList<TCollection>>> Acessor)
            : base(list, Acessor)
        {
            _Cache = new Dictionary<TSource, ListenedList>(_Source.Count);
            //this.RealAddRange(Expected);

            foreach (TSource s in _Source)
            {
                ListenedList ll = _Cache.FindOrCreateEntity(s, so => new ListenedList(so, this));
                ll.Deployed.Apply(c=>this.RealAdd(Transform(c,ll.Source),true));
            }

            //_Source.Distinct().Apply(o => this.Register(o));
            //this.RealAddRange(FromCache,true);
            
        }

        private class CollectionChangedArgs
        {
            internal CollectionChangedArgs(IList<TCollection> old, NotifyCollectionChangedEventArgs nce, Func<List<TCollection>> Factory)
            {
                Old = new List<TCollection>(old);
                New = old;
                if (New.ApplyChanges(nce) == null)
                {
                    New = Factory();
                }
                Changes = nce;
            }

            internal  NotifyCollectionChangedEventArgs Changes
            {
                get;private set;
            }

            internal CollectionChangedArgs(IList<TCollection> iold, IList<TCollection> inew)
            {
                Old = iold.ToList();
                New = inew.ToList();
            }

            internal IList<TCollection> Old
            {
                get;
                private set;
            }

            internal IList<TCollection> New
            {
                get;
                private set;
            }
        }

        #region ListenedList

        protected class ListenedList
        {
            private LiveSelectManyAbstract<TSource, TCollection,TDest> _Father;
            private INotifyCollectionChanged _Listened;

            internal ListenedList(TSource iSource, LiveSelectManyAbstract<TSource, TCollection, TDest> Father)
            {
                Source = iSource;
                _Father = Father;
                var target = Target;
                
                _Listened = target as INotifyCollectionChanged;

                if (_Listened != null)
                {
                    _Listened.CollectionChanged += TreeCollectionChanged;
                    Deployed = target.ToList();
                }
                else
                {
                    Deployed = target;
                }
            }

            private IList<TCollection> Target
            {
                get { return _Father._Function.GetCached(Source); }
            }

            internal void UpdateCollection(IList<TCollection> iDeployed)
            {
                INotifyCollectionChanged nic = iDeployed as INotifyCollectionChanged;
                bool nreg = (object.ReferenceEquals(nic, _Listened)); 
                Deployed = iDeployed.ToList();

                if (nreg)
                    return;

                if (_Listened != null)
                {
                    _Listened.CollectionChanged -= TreeCollectionChanged;
                }
                
                _Listened = nic;

                if (_Listened != null)
                {
                    _Listened.CollectionChanged += TreeCollectionChanged;
                }
               
            }

            internal TSource Source
            {
                get;
                private set;
            }

            internal IList<TCollection> Deployed
            {
                get;
                private set;
            }

            internal void Deregister()
            {
                if (_Listened != null)
                {
                    _Listened.CollectionChanged -= TreeCollectionChanged;
                }
            }

            private void TreeCollectionChanged(object sender, NotifyCollectionChangedEventArgs nce)
            {
                if (nce == null)
                    return;

                CollectionChangedArgs cc = new CollectionChangedArgs(Deployed, nce, () => _Father._Function.Evaluate(Source).ToList());
                Deployed = cc.New;
                _Father.TreeCollectionChanged(this, nce, cc);
            }
        }

        #endregion


        //protected override IEnumerable<TDest> Expected
        //{
        //    get { return _Source.SelectMany(_Function.Evaluate); }
        //}

        protected abstract IEnumerable<TDest> FromCache
        {
            get;
        }
        //private IEnumerable<TDest> FromCache
        //{
        //    get { return _Source.SelectMany(item => _Cache[item].Deployed); }
        //}

        protected abstract TDest Transform(TCollection icool,TSource source);

        private Func<TCollection, TDest> FromSource(TSource isource)
        {
            return (coll) => Transform(coll, isource);
        }
   

        private int CountUntilIndex(int index)
        {
            int count = 0;
            for (int i = 0; i < index; i++)
            {
                count += _Cache[_Source[i]].Deployed.Count;
            }
            return count;
        }

        private void InsertForceBrute(TSource correspondingsource, CollectionChangedArgs cc, int outIndex)
        {
            IList<TCollection> newList = cc.New;

            int newCount = newList.Count;
            int oldCount = cc.Old.Count;

            if (newCount < oldCount)
            {
                int diff = oldCount - newCount;
                this.RealRemoveRange(outIndex + newCount, diff);
            }
            else if (newCount > oldCount)
            {
                List<TCollection> nl = newList.ToList();
                int diff = newCount - oldCount;
                this.RealInsertRange(outIndex + oldCount, nl.GetRange(oldCount, diff).Select(FromSource(correspondingsource)));
                nl.RemoveRange(oldCount, diff);
                newList = nl;
            }

            this.RealReplaceRange(outIndex, newList.Select(FromSource(correspondingsource)));
        }

        private void TreeCollectionChanged(ListenedList EC, NotifyCollectionChangedEventArgs nce, CollectionChangedArgs cc)
        {
            foreach (int ind in _Source.Indexes(EC.Source))
            {
                int count =  CountUntilIndex(ind);
                Nullable<int> delta = this.ApplyChanges(nce,o=>Transform((TCollection)o, EC.Source),count);
                if (delta == null)
                {
                    InsertForceBrute(EC.Source,cc, count);
                }       
            }

        }

        public override bool Invariant
        {
            get
            {
                if (!FromCache.SequenceEqual(Expected))
                    return false;

                return base.Invariant;
            }
        }

        public override IDisposable GetFactorizableEvent()
        {
            return null;
        }


        protected override void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<IList<TCollection>> changes)
        {
            CollectionChangedArgs cc = new CollectionChangedArgs(changes.Old, changes.New);

            _Cache[item].UpdateCollection(changes.New);

            _Source.Indexes(item).Apply(i => InsertForceBrute(item,cc, this.CountUntilIndex(i)));

        }

        private ListenedList Register(TSource newItem)
        {
            ListenedList ll = new ListenedList(newItem, this);
            _Cache.Add(newItem, ll);
            return ll;
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> First)
        {
            ListenedList ll = null;
            if (First==true)
            {
                ll =Register(newItem);       
            }
            else if (First == false)
            {
                ll = _Cache[newItem];
            }
            else
            {
                if (!_Cache.TryGetValue(newItem,out ll))
                    ll = Register(newItem);  
            }

            this.RealInsertRange(CountUntilIndex(index), ll.Deployed.Select(FromSource(newItem)));
        }

        protected override void OnClear()
        {
            _Cache.Values.Apply(il => il.Deregister());
            _Cache.Clear();
            this.RealClear(true);
            foreach (TSource s in _Source)
            {
                ListenedList ll = _Cache.FindOrCreateEntity(s, so => new ListenedList(so, this));
                ll.Deployed.Apply(c => this.RealAdd(Transform(c, ll.Source), true));
            }
            SendResetEvent();
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            ListenedList mylistener = _Cache[oldItem];

            bool RealLast = false;
            if (last != null)
            {
                RealLast = last.Value;
            }
            else
            {
                RealLast = !_Source.Contains(oldItem);
            }

          
            if (RealLast==true)
            {
                mylistener.Deregister();
                _Cache.Remove(oldItem);
            }

            this.RealRemoveRange(CountUntilIndex(index), mylistener.Deployed.Count);
            return true;
        }


    }
}
