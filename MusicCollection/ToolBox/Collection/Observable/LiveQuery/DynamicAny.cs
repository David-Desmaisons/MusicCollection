using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal sealed class DynamicAny<TSource> : DynamicResultFunction<TSource, bool, bool> where TSource : class
    {
        static WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, bool>>>, DynamicAny<TSource>> _Cache;

        static private WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, bool>>>, DynamicAny<TSource>> Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = new WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, bool>>>, DynamicAny<TSource>>();

                return _Cache;
            }
        }


        static internal DynamicAny<TSource> GetDynamicAny(IList<TSource> source, Expression<Func<TSource, bool>> Transformer)
        {
            Tuple<IList<TSource>, Expression<Func<TSource, bool>>> tuple = new Tuple<IList<TSource>, Expression<Func<TSource, bool>>>(source, Transformer);
            return Cache.FindOrCreateEntity(tuple, (t) => new DynamicAny<TSource>(t.Item1, t.Item2));
        }


        private DynamicAny(IList<TSource> source, Expression<Func<TSource, bool>> Transformer)
            : base(source, Transformer)
        {
            Update();
        }

        private bool Expected
        {
            get { return _Source.Any(this._Function.Evaluate); }
        }

        //private bool OldExpected
        //{
        //    get { return _Source.Any(this._Function.CurrentOrOldValueComputer); }
        //}

        private void Update()
        {
            Value = Expected;
        }

        //public override ObjectModifiedArgs this[string iAttributeName]
        //{
        //    get
        //    {
        //        if (iAttributeName != "Value")
        //            return base[iAttributeName];

        //        bool excp = Expected;
        //        bool oexcp = OldExpected;

        //        if (excp == oexcp)
        //            return null;

        //        return new ObjectAttributeChangedArgs<bool>(this, "Value", oexcp, excp);
        //    }
        //}

        protected override void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<bool> changes)
        {
            if (changes.New)
            {
                Value = true;
                return;
            }

            Value = Expected;

        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
 
            if (this._Function.Evaluate(newItem))
                Value = true;
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
           if (this._Function.Evaluate(oldItem))
                Value = Expected;

            return true;
        }

        protected override IDisposable GetFactorizable()
        {
            return null;
        }

        protected override void OnClear()
        {
            Update();
        }

        public override bool Invariant
        {
            get { return Value == Expected; }
        }
    }
}
