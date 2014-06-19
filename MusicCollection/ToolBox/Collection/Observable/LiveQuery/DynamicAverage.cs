using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal sealed class DynamicAverage<TSource> : DynamicResultFunction<TSource,Nullable<double>, int> where TSource:class
    {
        static WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicAverage<TSource>> _Cache;

        static private WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicAverage<TSource>> Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = new WeakDictionary<Tuple<IList<TSource>, Expression<Func<TSource, int>>>, DynamicAverage<TSource>>();

                return _Cache;
            }
        }


        static internal DynamicAverage<TSource> GetDynamicAverage(IList<TSource> source, Expression<Func<TSource, int>> Transformer)
        {
            Tuple<IList<TSource>, Expression<Func<TSource, int>>> tuple = new Tuple<IList<TSource>, Expression<Func<TSource, int>>>(source, Transformer);
            return Cache.FindOrCreateEntity(tuple, (t) => new DynamicAverage<TSource>(t.Item1, t.Item2));
        }

        private DynamicAverage(IList<TSource> source, Expression<Func<TSource, int>> Transformer)
            : base(source, Transformer)
        {
            Update();
        }

        private double _Sum;
        private Nullable<double> Expected
        {
            get { return (_Source.Count == 0) ? (Nullable<double>)null : _Source.Average(_Function.Evaluate); }
        }

        public override bool Invariant
        {
            get { return Expected == Value; }
        }


        private void SetSum(double v)
        {
            _Sum = v;
            int count = _Source.Count;
            if (count == 0)
            {
                Value = null;
            }
            else
            {
                Value = _Sum / _Source.Count;
            }
        }

        private void SetSumInc(double inc)
        {
            SetSum(_Sum + inc);
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            if (_Trav != null)
                return;

            int nv = _Function.Evaluate(newItem); 
            SetSumInc(nv);
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            if (_Trav != null)
                return true;

            int ov = - _Function.CurrentOrOldValueComputer(oldItem);
            
            SetSumInc(ov);
            return true;
        }

        #region DifferedEvent

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
            private DynamicAverage<TSource> _Father;
            internal NoUpadte(DynamicAverage<TSource> Father)
            {
                _Father = Father;
            }


            public void Dispose()
            {
                _Father._Trav = null;
                _Father.Update();
            }
        }

        private void Update()
        {
            if (_Trav != null)
                return;

            SetSum(_Source.Sum(_Function.Evaluate));
        }

        #endregion

        protected override void OnClear()
        {
            Update();
        }

        protected override void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<int> changes)
        {
            if (_Trav != null)
                return;

            int C = _Source.Count;
            if (C==0)
                return;

            SetSumInc(changes.New - changes.Old);

            //double Sum = ((double)Value * C) + changes.New - changes.Old;
            //Value = Sum /C;
        }
    }
}
