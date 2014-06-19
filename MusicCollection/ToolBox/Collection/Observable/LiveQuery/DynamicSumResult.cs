using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal sealed class DynamicSumResult<TSource> : DynamicResultFunction< TSource,int,int> where TSource : class
    {
        static WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicSumResult<TSource>> _Cache;

        static private WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicSumResult<TSource>> Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = new WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicSumResult<TSource>>();

                return _Cache;
            }
        }


        static internal DynamicSumResult<TSource> GetDynamicAverage(IList<TSource> source, Expression<Func<TSource, int>> Transformer)
        {
            Tuple<IList<TSource>, Expression<Func<TSource, int>>> tuple = new Tuple<IList<TSource>, Expression<Func<TSource, int>>>(source, Transformer);
            return Cache.FindOrCreateEntity(tuple, (t) => new DynamicSumResult<TSource>(t.Item1, t.Item2));
        }

        private DynamicSumResult(IList<TSource> source, Expression<Func<TSource, int>> Transformer)
            : base(source, Transformer)
        {
            Value = source.Sum(_Function.Evaluate);
        }


        private NoUpadte _Trav;
        protected override IDisposable GetFactorizable()
        {
            if (_Trav == null)
            {
                _Trav = new NoUpadte(this);
            }

            return _Trav;
        }

        private class NoUpadte : IDisposable
        {
            private DynamicSumResult<TSource> _Father;
            internal NoUpadte(DynamicSumResult<TSource> Father)
            {
                _Father = Father;
                Delta = 0;
            }

            internal int Delta
            {
                get;
                set;
            }

            public void Dispose()
            {
                _Father._Trav = null;
                _Father.Update(Delta);
            }
        }

        private void Update(int Delta)
        {
            if (_Trav != null)
            {
                _Trav.Delta += Delta;
                return;
            }

            Value += Delta;
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            Update(_Function.Evaluate(newItem));
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            Update(-_Function.Evaluate(oldItem));
            return true;
        }

        protected override void OnClear()
        {
            if (_Trav != null)
                throw new Exception();

            Value = _Source.Sum(_Function.Evaluate);
        }

        public override bool Invariant
        {
            get
            {
                return (Value == _Source.Sum(_Function.Evaluate));
            }
        }

        protected override void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<int> changes)
        {
            Update(changes.New-changes.Old);
        }
    }

}
