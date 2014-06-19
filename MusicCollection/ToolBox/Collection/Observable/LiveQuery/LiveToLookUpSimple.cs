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
    internal sealed class LiveToLookUpSimple<TKey, T> : LiveToLookUpBase<TKey, T, T, TKey> where T : class
    {
        internal LiveToLookUpSimple(IList<T> Source, Expression<Func<T, TKey>> KeySelector)
            : base(Source, KeySelector)
        {
            UpdateFromSource(true);
        }

        protected override void InitFromCollection()
        {
            UpdateFromSource(true);
        }


        protected override void OnCollectionItemPropertyChanged(T item, ObjectAttributeChangedArgs<TKey> changes)
        {
            InternalGrouped ig = this.GetFromKey(changes.Old);
            if (ig == null)
            {
                Trace.WriteLine("Problem ");
                return;
            }

            var indexes = _Source.Indexes(item);
            bool resOK = RemoveItems(ig, GetElementFromSource(item, true), indexes);
            //indexes.Apply((i) => resOK = resOK && RemoveItem(ig, GetElementFromSource(item, true), i));

            if (!resOK)
                return;

            RegisterItems(changes.New, GetElementFromSource(item, false), indexes);
        }


        protected override bool GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<T> Deployed, int index, out int outindex)
        {
            return _Source.GetIndex(Deployed, index, out outindex);
        }

        protected override int GetInjectedCollectionIndexFromSourceIndex(TKey key, IList<T> Deployed, int index)
        {
            return _Source.GetIndex(Deployed, index);
        }


        protected override ILookup<TKey, T> ExpectedGrouping
        {
            get { return _Source.ToLookup(_Function.Evaluate); }
        }

        //protected override ILookup<TKey, Tuple<int, T>> ExpectedGroupingWithRank
        //{
        //    get { return _Source.AsIndexed().ToLookup((t) => _Function.Evaluate(t.Item2), (t) => new Tuple<int, T>(t.Item1,t.Item2)); }
        //}

        protected override T GetElementFromSource(T original, bool Old)
        {
            return original;
        }

        protected override TKey GetKeyFromSource(T original, bool Old)
        {
            if (Old)
                return _Function.CurrentOrOldValueComputer(original);

            return _Function.Evaluate(original);
        }
    }
}
