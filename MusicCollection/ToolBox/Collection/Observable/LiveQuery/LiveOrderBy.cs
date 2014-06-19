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
    internal sealed class LiveOrderBy<TSource, TKey> :
            CollectionOrderedBaseWithFunction<TKey,TSource>,
            IExtendedOrderedObservableCollection<TSource>
        where TSource : class
        where TKey : IComparable<TKey>

    {
        private Expression<Func<TSource, TKey>> _KeySelector;

        internal LiveOrderBy(IList<TSource> Orginal, Expression<Func<TSource, TKey>> KeySelector)
            : base(Orginal, KeySelector)
        {
            _KeySelector = KeySelector;
            this.RealAddRange(ExpectedTuple);
        }

        internal LiveOrderBy(IList<TSource> Orginal, IFunction<TSource, TKey> KeySelector)
            : base(Orginal, KeySelector)
        {
            _KeySelector = null;
            this.RealAddRange(ExpectedTuple,true);
        }

        private IEnumerable<Tuple<TSource, TKey>> ExpectedTuple
        {
            get { return this._Source.Select((t)=>new Tuple<TSource, TKey>(t, _Function.Evaluate(t))).OrderBy((t)=>t.Item2); }
        }

        protected override IEnumerable<TSource> Expected
        {
            get { return this._Source.OrderBy(_Function.Evaluate); }
        }

        private IEnumerable<TKey> AllComputedKeys
        {
            get
            {
                return Expected.Select(_Function.Evaluate);
            }
        }

        protected override void OnCollectionItemPropertyChanged( TSource item, ObjectAttributeChangedArgs<TKey> changes)
        {
            _Source.Indexes(item).Apply((i)=>ChangeKey(item, changes.Old, changes.New));                                                                                                                            
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            this.Add(newItem,_Function.Evaluate(newItem));
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            return this.RemoveValue(oldItem, _Function.CurrentOrOldValueComputer(oldItem));
        }

        public override bool Invariant
        {
            get
            {
                if (!this.AllComputedKeys.SequenceEqual(_List.Select((p)=>p.Key)))
                    return false;

                return base.Invariant;
            }
        }

        #region MixedKey

        private class MixedKey<TKey1, TKey2> : IComparable<MixedKey<TKey1, TKey2>>
            where TKey1 : IComparable<TKey1>
            where TKey2 : IComparable<TKey2>
        {
            internal MixedKey(TKey1 ik1, TKey2 ik2)
            {
                Key1 = ik1;
                Key2 = ik2;
            }

            internal TKey1 Key1
            {
                get;
                private set;
            }

            internal TKey2 Key2
            {
                get;
                private set;
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;

                var mk = obj as MixedKey<TKey1, TKey2>;
                if (mk == null)
                    return false;

                if (!object.Equals(mk.Key1, Key1))
                    return false;

                return object.Equals(mk.Key2, Key2);
            }

            public override int GetHashCode()
            {
                return Key1.GetHashCode() ^ Key2.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("({0},{1})", Key1, Key2);
            }

            private static int Comparer<T>(IComparable<T> comp, T comto)
            {
                if (comp == null)
                    return (comto == null) ? 0 : -1;

                return comp.CompareTo(comto);
            }

            int IComparable<MixedKey<TKey1, TKey2>>.CompareTo(MixedKey<TKey1, TKey2> other)
            {
                if (other == null)
                    return 1;

                int res = Comparer(Key1, other.Key1);
                if (res != 0)
                    return res;

                return Comparer(Key2, other.Key2);
            }
        }

        #endregion

        public IExtendedOrderedObservableCollection<TSource> CreateOrderedEnumerable<TKey2>(Expression<Func<TSource, TKey2>> keySelector) where TKey2 : IComparable<TKey2>
        {
            if (_KeySelector == null)
            {
                Trace.WriteLine("Not supported double ordered (this key selector is constant)");
                return this;
            }

            return new LiveOrderBy<TSource, MixedKey<TKey, TKey2>>(_Source, _KeySelector.Merge<TSource, TKey, TKey2, MixedKey<TKey, TKey2>>(keySelector, (k1, k2) => new MixedKey<TKey, TKey2>(k1, k2)));
        }


        protected override void OnClear()
        {
            this.RealClear(true);
            this.RealAddRange(ExpectedTuple,true);
            SendResetEvent();
        }
    }
}
