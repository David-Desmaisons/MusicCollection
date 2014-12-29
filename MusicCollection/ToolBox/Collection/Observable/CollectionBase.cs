using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;

namespace MusicCollection.ToolBox.Collection.Observable
{

    abstract class ReadOnlyCollection<T> : NotifyCompleteAdapterNoCache
    {

        #region public generic methods

        public abstract int IndexOf(T item);

        public abstract T this[int index]
        {
            get;
            set;
        }

        public abstract bool Contains(T item);

        public abstract IEnumerator<T> GetEnumerator();

        #endregion

        #region none-generic based on generic

        public bool Contains(object value)
        {
            if (value is T)
                return Contains((T)value);

            return false;
        }

        public int IndexOf(object value)
        {
            if (value is T)
                return IndexOf((T)value);

            return -1;
        }

        #endregion

        #region public Write-only


        public void Add(T item)
        {
            throw new AccessViolationException();
        }

        public void Clear()
        {
            throw new AccessViolationException();
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Insert(int index, object value)
        {
            throw new AccessViolationException();
        }

        public int Add(object value)
        {
            throw new AccessViolationException();
        }

        public void Remove(object value)
        {
            throw new AccessViolationException();
        }

        public bool Remove(T item)
        {
            throw new AccessViolationException();
        }

        public void Insert(int index, T item)
        {
            throw new AccessViolationException();
        }

        public void RemoveAt(int index)
        {
            throw new AccessViolationException();
        }

        #endregion

        #region Trivial

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion
    }


    [DebuggerDisplay("Count = {Count}")]
    class CollectionBase<T> : ReadOnlyCollection<T>, IList<T>, IList
    {
        private const int _MaxIndividualDispatch = 15;

        protected IList<T> MyList
        {
            get;
            private set;
        }


        protected CollectionBase()
        {
            MyList = new List<T>();
        }

        protected CollectionBase(int sized)
        {
            MyList = new List<T>(sized);
        }

        protected CollectionBase(IEnumerable<T> copyfrom)
        {
            MyList = new List<T>(copyfrom);
        }

        #region Changes listeners

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion

        #region ReadOnly Collection

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
            }
        }

        public override int IndexOf(T item)
        {
            return MyList.IndexOf(item);
        }

        public override T this[int index]
        {
            get
            {
                return MyList[index];
            }
            set
            {
                throw new AccessViolationException();
            }
        }

        public override bool Contains(T item)
        {
            return MyList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            MyList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return MyList.Count; }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return MyList.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            (MyList as IList).CopyTo(array, index);
        }

        #endregion

        #region protected Write Methods

        protected void SendResetEvent()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void RealClear(bool silent = false)
        {
            MyList.Clear();

            if (!silent)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void RealInsert(int index, T value)
        {
            if (index == MyList.Count)
                MyList.Add(value);
            else
                MyList.Insert(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        protected void RealAdd(T value, bool Silent = false)
        {
            MyList.Add(value);

            if (!Silent)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, MyList.Count - 1));
        }

        protected bool RealRemove(T value)
        {
            int index = MyList.IndexOf(value);
            if (index == -1)
                return false;

            MyList.RemoveAt(index);

            //WPF Limitation if collection has no element should send a reset event.
            if (MyList.Count == 0)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            return true;

        }

        protected void RealRemoveAt(int Index)
        {
            T value = MyList[Index];
            MyList.RemoveAt(Index);
            if (MyList.Count == 0)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, Index));
        }

        protected void RealOverWrite(int index, T value)
        {
            T oldvalue = MyList[index];

            if (object.Equals(oldvalue, value))
                return;

            MyList[index] = value;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldvalue, index));
        }

        protected void RealMove(int oldindex, int newindex)
        {
            if (oldindex == newindex)
                return;

            T value = MyList[oldindex];
            MyList.RemoveAt(oldindex);
            MyList.Insert(newindex, value);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newindex, oldindex));
        }

        protected void RealAddRange(IEnumerable<T> collection, bool silent = false)
        {
            collection.Apply(item => RealAdd(item, silent));
        }

        protected void RealInsertRange(int index, IEnumerable<T> collection)
        {
            collection.Apply(item => RealInsert(index++, item));
        }

        protected void RealRemoveRange(int index, int count)
        {
            for (; count > 0; count--)
            {
                this.RealRemoveAt(index);
            }
        }

        protected void RealReplaceRange(int index, IEnumerable<T> collection)
        {
            collection.Apply(item => RealOverWrite(index++, item));
        }

        //private static T Convertion(object o)
        //{
        //    return (T)o;
        //}

        //protected Nullable<int> ApplyChanges(NotifyCollectionChangedEventArgs nce, int Delta = 0)
        //{
        //    return ApplyChanges(nce, Convertion, Delta);
        //}

        protected Nullable<int> ApplyChanges(NotifyCollectionChangedEventArgs nce, Func<object, T> Trans, int Delta = 0)
        {
            if (nce == null)
                return null;

            int IndexNew = nce.NewStartingIndex + Delta;
            int IndexOld = nce.OldStartingIndex + Delta;

            switch (nce.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    return null;

                case NotifyCollectionChangedAction.Add:
                    foreach (object elem in nce.NewItems)
                    {
                        RealInsert(IndexNew++, Trans(elem));
                    }
                    return nce.NewItems.Count;

                case NotifyCollectionChangedAction.Move:
                    foreach (object elem in nce.NewItems)
                    {
                        RealRemoveAt(IndexOld++);
                        RealInsert(IndexNew++, Trans(elem));
                    }
                    return 0;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object elem in nce.OldItems)
                    {
                        RealRemoveAt(IndexOld++);
                    }
                    return nce.OldItems.Count;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object elem in nce.NewItems)
                    {
                        this.RealOverWrite(IndexNew++, Trans(elem));
                    }
                    return 0;
            }

            return null;
        }

        #endregion

        #region GroupedChanged

        protected class GroupedChangedRegistror
        {

            private enum Operation { Insert, Remove };
            private List<Tuple<Operation, int, T>> _Operations = new List<Tuple<Operation, int, T>>();
            private CollectionBase<T> _Father;

            internal GroupedChangedRegistror(CollectionBase<T> iFather)
            {
                _Father = iFather;
            }

            internal void RegisterInsert(int index, T objecttoinsert)
            {
                _Operations.Add(new Tuple<Operation, int, T>(Operation.Insert, index, objecttoinsert));
            }

            internal void RegisterRemoveAt(int index)
            {
                _Operations.Add(new Tuple<Operation, int, T>(Operation.Remove, index, default(T)));
            }

            internal bool GetHeuristictToReplayOrRecreate()
            {
                return (_Operations.Count < _MaxIndividualDispatch);
            }

            private void Replay(Action<int> Remove, Action<int, T> Insert)
            {
                foreach (Tuple<Operation, int, T> Op in this._Operations)
                {
                    switch (Op.Item1)
                    {
                        case Operation.Insert:
                            Insert(Op.Item2, Op.Item3);
                            break;

                        case Operation.Remove:
                            Remove(Op.Item2);
                            break;
                    }
                }
            }

            internal void Replay()
            {

                if (this.GetHeuristictToReplayOrRecreate())
                    Replay(_Father.RealRemoveAt, _Father.RealInsert);
                else
                    _Father.OnCompleteChanges(RebuilFromChanges());
            }


            private List<T> RebuilFromChanges()
            {
                int CCount = _Father.Count;
                int Count = _Operations.Count;

                Tuple<Operation, int, T> nextOperation = Count > 0 ? _Operations[0] : null;
                int operationnumber = 0;
                int currentcountvalue = 0;

                List<T> lt = new List<T>((CCount + Count) * 2);

                while ((currentcountvalue < CCount) || (nextOperation != null))
                {
                    if (nextOperation != null)
                    {
                        while ((nextOperation != null) && (nextOperation.Item2 == lt.Count))
                        {
                            if (nextOperation.Item1 == Operation.Insert)
                            {
                                lt.Add(nextOperation.Item3);
                                //currentcountvalue++;
                                //lt[deployedcountvalue] = nextOperation.Item3;
                                //deployedcountvalue++;

                            }
                            else
                            {
                                currentcountvalue++;
                            }

                            operationnumber++;
                            nextOperation = Count > operationnumber ? _Operations[operationnumber] : null;
                            //continue;                      
                        }

                    }

                    if (currentcountvalue < CCount)
                    {
                        lt.Add(_Father[currentcountvalue]);
                        //lt[deployedcountvalue] = _Father[currentcountvalue];
                        currentcountvalue++;
                    }
                }

                return lt;
            }



        }

        protected GroupedChangedRegistror GetReplayer()
        {
            return new GroupedChangedRegistror(this);
        }

        private void OnCompleteChanges(List<T> newCollection)
        {
            MyList = newCollection;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        #endregion


    }
}
