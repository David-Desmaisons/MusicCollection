using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{

    internal class ComparableKeyValuePair<TKey, TValue> : IComparable<ComparableKeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
    {

        internal TKey Key
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("<Key:{0} Value:{1} >", Key, Value);
        }

        internal TValue Value
        {
            get;
            private set;
        }

        internal ComparableKeyValuePair(TKey key, TValue value)
        {
            Value = value;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            ComparableKeyValuePair<TKey, TValue> o = (ComparableKeyValuePair<TKey, TValue>)obj;
            return object.Equals(Key, o.Key) && object.Equals(Value, o.Value);
        }

        public override int GetHashCode()
        {
            return ((Key == null) ? 0 : Key.GetHashCode()) ^ ((Value == null) ? 0 : Value.GetHashCode());
        }

        public int CompareTo(ComparableKeyValuePair<TKey, TValue> other)
        {
            return ((Key == null) ? 1 : Key.CompareTo(other.Key));
        }
    }

    abstract class CollectionOrderedBase<TKey, TValue> : ReadOnlyCollection<TValue>, IList<TValue>, IList where TKey : IComparable<TKey>
    {

        protected List<ComparableKeyValuePair<TKey, TValue>> _List = new List<ComparableKeyValuePair<TKey, TValue>>();

        internal CollectionOrderedBase()
        {
        }

        #region Read Only Collection

        public override int IndexOf(TValue item)
        {
            return _List.Index((p) => object.Equals(p.Value, item));
        }

        public override bool Contains(TValue item)
        {
            return _List.Where((p) => object.Equals(item, p.Value)).Any();
        }

        public override TValue this[int index]
        {
            get { return _List[index].Value; }
            set { throw new AccessViolationException(); }
        }

        public int Count
        {
            get { return _List.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override IEnumerator<TValue> GetEnumerator()
        {
            return _List.Select((p) => p.Value).GetEnumerator();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new AccessViolationException();
                //this[index] = (TValue)value;
            }
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this[arrayIndex + i];
            }
        }

        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(this[index + i], i);
            }
        }

        #endregion

        #region sorter helper

        private int BinarySearch(ComparableKeyValuePair<TKey, TValue> cvk, bool Up)
        {
            int res = _List.BinarySearch(cvk);

            if (res < 0)
                return ~res;

            if (Up)
            {
                return res;
            }

            while ((res++ < Count) && (object.Equals(_List[res].Key, cvk.Key))) { }
            return res;
        }

        private static bool Compare(TKey One, TKey Two)
        {
            int Greater = 0;
            if (One != null)
            {
                Greater = One.CompareTo(Two);
            }
            else if (Two != null)
            {
                Greater = -Two.CompareTo(One);
            }
            return (Greater >= 0);
        }

        #endregion

        #region Changes listeners

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion

        #region Edition

        protected void Add(TValue value, TKey key, bool Silent = false)
        {
            ComparableKeyValuePair<TKey, TValue> cvk = new ComparableKeyValuePair<TKey, TValue>(key, value);
            int Index = BinarySearch(cvk, true);
            _List.Insert(Index, cvk);

            if (!Silent)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, Index));

        }

        protected void RealAddRange(IEnumerable<Tuple<TValue, TKey>> ranges, bool Silent = false)
        {
            ranges.Apply((t) => Add(t.Item1, t.Item2, Silent));
        }

        protected bool RemoveValue(TValue value, TKey key)
        {
            ComparableKeyValuePair<TKey, TValue> nd = new ComparableKeyValuePair<TKey, TValue>(key, value);
            int index = _List.IndexOf(nd);
            if (index == -1)
                return false;

            _List.RemoveAt(index);

            if (_List.Count == 0)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            return true;
        }

        //protected bool RemoveValueAt(int index)
        //{
        //    TValue value = this[index];
        //    _List.RemoveAt(index);
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        //    return true;
        //}

        protected void ChangeKey(TValue value, TKey oldKey, TKey newKey)
        {
            bool pg = Compare(newKey, oldKey);
            ComparableKeyValuePair<TKey, TValue> oldvk = new ComparableKeyValuePair<TKey, TValue>(oldKey, value);
            ComparableKeyValuePair<TKey, TValue> newvk = new ComparableKeyValuePair<TKey, TValue>(newKey, value);
            int index = !pg ? _List.IndexOf(oldvk) : _List.LastIndexOf(oldvk);


            if (index == -1)
                return;

            _List.RemoveAt(index);
            int newindex = this.BinarySearch(newvk, pg);
            _List.Insert(newindex, newvk);

            if (index != newindex)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newindex, index));


        }

        protected void RealClear(bool Silent = false)
        {
            _List.Clear();
            if (!Silent)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void SendResetEvent()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion


        public abstract IDisposable GetFactorizableEvent();
    }

    abstract class CollectionOrderedObservableBase<TKey, TValue> : CollectionOrderedBase<TKey, TValue>, IExtendedObservableCollection<TValue> where TKey : IComparable<TKey>
    {

        private CollectionUISafeEvent _CollectionChanged;

        internal CollectionOrderedObservableBase()
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => PropertyHasChanged(string.Empty));
        }

        public abstract void Dispose();

        #region event

        public event NotifyCollectionChangedEventHandler CollectionChanged
        { add { _CollectionChanged.Event += value; } remove { _CollectionChanged.Event += value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChanged(string pn)
        {
            PropertyChangedEventHandler c = PropertyChanged;
            if (c != null)
                c(this, new PropertyChangedEventArgs(pn));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _CollectionChanged.CollectionChanged(e);
        }

        protected IDisposable GetEventFactorizable()
        {
            return _CollectionChanged.GetEventFactorizable();
        }

        #endregion
    }

    abstract class CollectionOrderedBaseWithFunction<TKey, TValue> : CollectionOrderedObservableBase<TKey, TValue>, IExtendedObservableCollection<TValue>, IInvariant
        where TValue : class
        where TKey : IComparable<TKey>
    {

        protected IList<TValue> _Source;
        private RegistorCollectionChanged<TValue> _RCF;
        protected IFunction<TValue, TKey> _Function;


        internal CollectionOrderedBaseWithFunction(IList<TValue> source, Expression<Func<TValue, TKey>> Transformer)
            : this(source, Transformer.CompileToObservable())
        {
        }

        internal CollectionOrderedBaseWithFunction(IList<TValue> source, IFunction<TValue, TKey> Transformer)
        {
            _Source = source;
            _Function = Transformer;
            _RCF = RegistrorCollectionIFunctionChanges<TValue, TKey>.GetListener(source, _Function, new ListenerAdapter(this));
        }

        public override void Dispose()
        {
            _RCF.Dispose();
        }

        public override IDisposable GetFactorizableEvent()
        {
            return this.GetEventFactorizable();
        }

        public virtual bool Invariant
        {
            get
            {
                return Expected.SequenceEqual(this);
            }
        }

        #region listeneradapter

        private class ListenerAdapter : ICollectionFunctionListener<TValue, TKey>
        {
            private CollectionOrderedBaseWithFunction<TKey, TValue> _Father;
            internal ListenerAdapter(CollectionOrderedBaseWithFunction<TKey, TValue> iFather)
            {
                _Father = iFather;
            }

            void ICollectionFunctionListener<TValue, TKey>.OnCollectionItemPropertyChanged(TValue item, ObjectAttributeChangedArgs<TKey> changes)
            {
                _Father.OnCollectionItemPropertyChanged(item, changes);
            }

            void ICollectionListener<TValue>.AddItem(TValue newItem, int index, Nullable<bool> First)
            {
                _Father.AddItem(newItem, index, First);
            }

            bool ICollectionListener<TValue>.RemoveItem(TValue oldItem, int index, Nullable<bool> Last)
            {
                return _Father.RemoveItem(oldItem, index, Last);
            }

            IDisposable ICollectionListener<TValue>.GetFactorizable()
            {
                return _Father.GetFactorizableEvent();
            }

            void ICollectionListener<TValue>.OnClear()
            {
                _Father.OnClear();
            }

            public void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TValue, TKey> Changes)
            {
                _Father.OnCollectionItemsPropertyChanged(Changes);
            }

            public bool FactorizeEvents
            {
                get { return _Father.FactorizeEvents; }
            }
        }

        #endregion

        protected abstract void OnCollectionItemPropertyChanged(TValue item, ObjectAttributeChangedArgs<TKey> changes);

        protected abstract void AddItem(TValue newItem, int index, Nullable<bool> First);

        protected abstract bool RemoveItem(TValue oldItem, int index, Nullable<bool> Last);

        protected virtual void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TValue, TKey> Changes)
        {
            throw new NotImplementedException();
        }

        protected virtual bool FactorizeEvents
        {
            get { return false; }
        }

        protected abstract IEnumerable<TValue> Expected
        {
            get;
        }


        abstract protected void OnClear();
    }
}
