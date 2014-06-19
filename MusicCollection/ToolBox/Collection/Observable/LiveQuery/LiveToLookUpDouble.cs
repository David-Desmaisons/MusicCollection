using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    [DebuggerDisplay("Count = {Count}")]
    sealed class LiveToLookUpDouble<TKey, T, TElement> : LiveToLookUpBase<TKey, T, TElement, Tuple<TKey, TElement>> where T : class
    {
        private Func<T, TElement> _ElementSelector;
        private Func<T, TKey> _KeySelector;


        private List<Tuple<TKey, TElement>> _Treated;

        internal LiveToLookUpDouble(IList<T> Source, Expression<Func<T, TKey>> KeySelector, Expression<Func<T, TElement>> ElementSelector, Expression<Func<T, Tuple<TKey, TElement>>> Composed)
            : base(Source, Composed)
        {
            _ElementSelector = ElementSelector.Compile();
            _KeySelector = KeySelector.Compile();
            InitFromSource(true);
        }

        protected override void InitFromCollection()
        {
            InitFromSource(true);
        }

        static internal LiveToLookUpDouble<TKey, T, TElement> BuildFromKeyElementSelectors(IList<T> Source, Expression<Func<T, TKey>> KeySelector, Expression<Func<T, TElement>> ElementSelector)
        {
            return new LiveToLookUpDouble<TKey, T, TElement>(Source, KeySelector, ElementSelector, KeySelector.Merge(ElementSelector));
        }


        //protected override ILookup<TKey, Tuple<int, TElement>> ExpectedGroupingWithRank
        //{
        //    get { return _Source.AsIndexed().ToLookup((t)=>_KeySelector(t.Item2), (t)=>new Tuple<int,TElement>(t.Item1,_ElementSelector(t.Item2))); }
        //}

        protected override ILookup<TKey, TElement> ExpectedGrouping
        {
            get { return _Source.ToLookup(_KeySelector, _ElementSelector); }
        }

        protected override TElement GetElementFromSource(T original, bool Old)
        {
            if (Old)
                return _Function.CurrentOrOldValueComputer(original).Item2;

            return _Function.Evaluate(original).Item2;
        }

        protected override TKey GetKeyFromSource(T original, bool Old)
        {
            if (Old)
                return _Function.CurrentOrOldValueComputer(original).Item1;

            return _Function.Evaluate(original).Item1;
        }

        private void InitFromSource(bool silent)
        {
            _Treated = new List<Tuple<TKey, TElement>>(_Source.Count);

            using (TimeTracer.TimeTrack(string.Format("LookUp Double Base Update + _Treated")))
            {
                 this.RealClear(silent);

                foreach (T source in _Source)
                { 
                    TKey key = _KeySelector(source);
                    TElement el = _ElementSelector(source);
                    CreateonInit(key, el, silent);
                    _Treated.Add(new Tuple<TKey, TElement>(key, el));
                }

            }

            //_Treated.Clear();

            //using (TimeTracer.TimeTrack(string.Format("_Treated creation")))
            //{
            //    Func<T, Tuple<TKey, TElement>> f = _Function.Evaluate;
            //    _Treated.AddRange(_Source.Select(f));
            //}
        }


        protected override void OnClear()
        {
            _Treated.Clear();
            base.OnClear();
        }

        protected override bool GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<TElement> Deployed, int index, out int outindex)
        {
            return _Treated.GetIndex(Deployed.Select((el) => new Tuple<TKey, TElement>(key, el)).ToList(), index, out outindex);
        }

        protected override int GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<TElement> Deployed, int index)
        {
            return _Treated.GetIndex(Deployed.Select((el) => new Tuple<TKey, TElement>(key, el)).ToList(), index);
        }

        protected override void AddItem(T newItem, int index, Nullable<bool> First)
        {
            _Treated.Insert(index, new Tuple<TKey, TElement>(_KeySelector(newItem), _ElementSelector(newItem)));
            base.AddItem(newItem, index, First);
        }

        protected override bool RemoveItem(T oldItem, int index, Nullable<bool> Last)
        {
            _Treated.RemoveAt(index);
            return base.RemoveItem(oldItem, index, Last);
        }

        protected override void OnCollectionItemPropertyChanged(T item, ObjectAttributeChangedArgs<Tuple<TKey, TElement>> changes)
        {
            bool ChangesKey = !object.Equals(changes.Old.Item1, changes.New.Item1);
            bool ChangesElement = !object.Equals(changes.Old.Item2, changes.New.Item2);

            InternalGrouped ig = this.GetFromKey(changes.Old.Item1);
            if (ig == null)
            {
                Trace.WriteLine("Problem ");
                return;
            }

            var indexes = _Source.Indexes(item);

            //indexes.Apply((i) => _Treated[i] = changes.New);
            //think about this!!!!!!!!


            if (!ChangesKey)
            {
                ig.ChangeItems(changes, indexes);
                //indexes.Apply((i) => ig.ChangeItem(changes.New.Item2, changes.Old.Item2, i));
            }
            else
            {
                bool resOK = RemoveItems(ig, changes.Old.Item2, indexes); ;
                //indexes.Apply((i) => resOK = resOK && RemoveItem(ig,changes.Old.Item2, i));

                if (!resOK)
                    return;

                RegisterItems(changes.New.Item1, changes.New.Item2, indexes);
            }
        }
    }
}
