using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

//using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;
using System.Diagnostics;
using System.Collections;

namespace MusicCollection.ToolBox.Collection.Observable
{
     
    [DebuggerDisplay("Count = {Count}")]
    internal class ModelToUISafeCollectionHandler<Torigin, Target> : NotifyCompleteAdapterNoCache
        where Torigin : class,Target
        where Target : class
    {
        private IList<Torigin> _List;
        private ModelCollectionList<Torigin, Target> _OML;
        private UICollection<Torigin, Target> _ORO;
        private UICollection<Torigin, Target> _OMO;
 
        internal ModelToUISafeCollectionHandler(IList<Torigin> iList)
        {
            _List = iList?? new List<Torigin>();
        }

        internal IImprovedList<Torigin> ModelCollection
        {
            get
            {
                if (_OML == null)
                    _OML = new ModelCollectionList<Torigin, Target>(this);

                return _OML;
            }
        }

        internal ICompleteObservableCollection<Target> ReadOnlyUICollection
        {
            get
            {
                if (_ORO == null)
                    _ORO = new UICollection<Torigin, Target>(this,true);

                return _ORO;
            }
        }

        internal IObservableCollection<Target> ModifiableUICollection
        {
            get
            {
                if (_OMO == null)
                    _OMO = new UICollection<Torigin, Target>(this, false);

                return _OMO;
            }
        }

        #region Model Collection


        [DebuggerDisplay("Count = {Count}")]
        private class ModelCollectionList<T1, T2> : NotifyCompleteAdapterNoCache, IImprovedList<T1> 
            where T1 : class,T2
            where T2 : class
        {
            private ModelToUISafeCollectionHandler<T1, T2> _Father;


            internal ModelCollectionList(ModelToUISafeCollectionHandler<T1, T2> father)
            {
                _Father = father;
                _Father._ModelCollectionChanged += (o, e) => Event(e);
                _Father.ObjectChanged += _Father_ObjectChanged;
            }

            private void _Father_ObjectChanged(object sender, ObjectModifiedArgs e)
            {
                PropertyHasChanged(e.AttributeName, e.OldAttributeValue, e.NewAttributeValue);
            }

            public int IndexOf(T1 item)
            {
                return _Father._List.IndexOf(item);
            }

            public T1 this[int index]
            {
                get
                {
                    return _Father._List[index];
                }
                set
                {
                    _Father[index] = value;
                }
            }

            public bool Contains(T1 item)
            {
                return _Father._List.Contains(item);
            }

            public void CopyTo(T1[] array, int arrayIndex)
            {
                _Father._List.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _Father._List.Count; }
            }

            public bool IsReadOnly
            {
                get { return _Father._List.IsReadOnly; }
            }


            public void Insert(int index, T1 item)
            {
                _Father.PrivateInsert(index, item);
            }

            public void RemoveAt(int index)
            {
                _Father.PrivateRemoveAt(index);
            }

            public void Add(T1 item)
            {
                _Father.PrivateAdd(item);
            }

            public void Clear()
            {
                _Father.PrivateClear();
            }

            public bool Remove(T1 item)
            {
                return _Father.PrivateRemove(item);
            }

            public void Move(int old, int inew)
            {
                _Father.PrivateMove(old, inew);
            }

            public IEnumerator<T1> GetEnumerator()
            {
                return _Father._List.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _Father._List.GetEnumerator();
            }

            public Func<T1, T1, int> Comparator
            {
                set { _Father.Comparator = value; }
                get { return _Father.Comparator; }
            }

            public void Sort(bool Asc = true, Func<T1, T1, int> iComparator = null)
            {
                _Father.Sort(Asc, iComparator);
            }

            private event NotifyCollectionChangedEventHandler _CollectionChanged;

            public event NotifyCollectionChangedEventHandler CollectionChanged
            { add { _CollectionChanged += value; } remove { _CollectionChanged -= value; } }


            private void Event(NotifyCollectionChangedEventArgs e)
            {
                NotifyCollectionChangedEventHandler CollectionChange=_CollectionChanged;

                if (CollectionChange != null)
                    CollectionChange(this, e);
            }
        }
        #endregion

        #region ReadOnly UI Collection


        [DebuggerDisplay("Count = {Count}")]
        private class UICollection<T1, T2> : NotifyCompleteAdapterNoCache,ICompleteObservableCollection<T2>
            where T1 : class,T2
            where T2 : class
        {
            private ModelToUISafeCollectionHandler<T1, T2> _Father;
            private CollectionUISafeEvent _CollectionChanged;
            private bool _ReadOnly;

            internal UICollection(ModelToUISafeCollectionHandler<T1, T2> father, bool iReadOnly)
            {
                _Father = father;
                _Father._UICollectionChanged += (o, e) => Event(e);
                _Father.ObjectChanged += _Father_ObjectChanged;
                _CollectionChanged = new CollectionUISafeEvent(this);
                _ReadOnly = iReadOnly;
            }

            private void _Father_ObjectChanged(object sender, ObjectModifiedArgs e)
            {
                PropertyHasChanged(e.AttributeName, e.OldAttributeValue, e.NewAttributeValue);
            }

            public void CopyTo(T2[] array, int arrayIndex)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[arrayIndex + i] = _Father._List[i];
                }
            }

            public int IndexOf(T2 item)
            {
                return _Father._List.IndexOf(item as T1);
            }

            public bool Contains(T2 item)
            {
                return _Father._List.Contains(item);
            }

            public int Count
            {
                get { return _Father._List.Count; }
            }

            public bool IsReadOnly
            {
                get { return _ReadOnly; }
            }

            public IEnumerator<T2> GetEnumerator()
            {
                return _Father._List.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _Father._List.GetEnumerator();
            }


            public void Insert(int index, T2 item)
            {
                if (_ReadOnly)
                {
                    throw new ArgumentException("Collection read-only");
                }
                
                _Father.PrivateInsert(index, item as T1);
                   
            }

            public T2 this[int index]
            {
                get
                {
                    return _Father._List[index];
                }
                set
                {
                    if (_ReadOnly)
                    {
                        throw new ArgumentException("Collection read-only");
                    } 
                    _Father[index] = value as T1;
                }
            }

            public void Add(T2 item)
            {
                if (_ReadOnly)
                {
                    throw new ArgumentException("Collection read-only");
                } 
                _Father.PrivateAdd(item as T1);
            }

            public void Clear()
            {
                if (_ReadOnly)
                {
                    throw new ArgumentException("Collection read-only");
                } 
                
                _Father.PrivateClear();
            }

            public void RemoveAt(int index)
            {
                if (_ReadOnly)
                {
                    throw new ArgumentException("Collection read-only");
                } 
                
                _Father.PrivateRemoveAt(index);

            }

            public bool Remove(T2 item)
            {
                if (_ReadOnly)
                {
                    throw new ArgumentException("Collection read-only");
                } 

                return _Father.PrivateRemove(item as T1);
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            { add { _CollectionChanged.Event+=value; } remove { _CollectionChanged.Event-=value; } }


            private void Event(NotifyCollectionChangedEventArgs e)
            {
                _CollectionChanged.Fire(e, true);
                //FireEmpyEvent();
               
            }


            public int Add(object value)
            {
                T2 it = value as T2;
                Add(it);
                return this.Count - 1;
            }

            public bool Contains(object value)
            {
                T2 it = value as T2;
                return Contains(it);
            }

            public int IndexOf(object value)
            {
                T2 it = value as T2;
                return IndexOf(it);
            }

            public void Insert(int index, object value)
            {
                T2 it = value as T2;
                Insert(index,it);
            }

            public bool IsFixedSize
            {
                get { return this._ReadOnly; }
            }

            public void Remove(object value)
            {
                T2 it = value as T2;
                Remove(it);
            }

            object System.Collections.IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    T2 it = value as T2;
                    this[index] = it;
                }
            }

            public void CopyTo(Array array, int index)
            {
                T2[] myarr = new T2[array.Length];
                this.CopyTo(myarr, index);
                myarr.CopyTo(array, 0);
            }

            public bool IsSynchronized
            {
                get { return true; }
            }

            public object SyncRoot
            {
                get { return null; }
            }
        }
        #endregion

        #region events

        private event NotifyCollectionChangedEventHandler _ModelCollectionChanged;
        private event NotifyCollectionChangedEventHandler _UICollectionChanged;

        private void Event(NotifyCollectionChangedEventArgs e,int oldCount)
        {
            NotifyCollectionChangedEventHandler ModelCollectionChanged = _ModelCollectionChanged;
            if (ModelCollectionChanged != null)
                ModelCollectionChanged(this, e);

           switch(e.Action)
           {
               case NotifyCollectionChangedAction.Add:
               case NotifyCollectionChangedAction.Remove:
               case NotifyCollectionChangedAction.Reset:
                   this.PropertyHasChanged("Count",oldCount,this._List.Count);
                   break;
           }

            NotifyCollectionChangedEventHandler UICollectionChanged = _UICollectionChanged;
            if (UICollectionChanged != null)
                UICollectionChanged(this, e);
        }

        #endregion

        #region Modification

        private Torigin this[int index]
        {
            get
            {
                return _List[index];
            }
            set
            {
                Torigin old = _List[index];


                if (old != value)
                {
                    _List[index] = value;

                    Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old, index),_List.Count);
                }
            }
        }

        private void PrivateAdd(Torigin item)
        {
            int oldcount = _List.Count;
            _List.Add(item);

            Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _List.Count - 1), oldcount);
        }

        private void PrivateClear()
        {
            int oldcount = _List.Count;
            _List.Clear();

            Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), oldcount);
        }

        private void PrivateInsert(int index, Torigin item)
        {
            int oldcount = _List.Count;
            _List.Insert(index, item);

            Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index), oldcount);
        }

        private void PrivateRemoveAt(int index)
        {
            int oldcount = _List.Count;
            Torigin item = _List[index];
            _List.RemoveAt(index);

            Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index), oldcount);
        }

        private void PrivateMove(int oldindex, int newindex)
        {
            int oldcount = _List.Count;
            Torigin item = _List[oldindex];
            _List.RemoveAt(oldindex);
            _List.Insert(newindex, item);

            Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newindex, oldindex), oldcount);
        }

        private bool PrivateRemove(Torigin item)
        {
            int oldcount = _List.Count;
            int index = _List.IndexOf(item);
            bool res = _List.Remove(item);

            if (res)
                Event(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index), oldcount);

            return res;
        }

        #endregion

        #region sorting

        private Func<Torigin, Torigin, int> Comparator
        {
            set;
            get;
        }

        private void Sort(bool Asc = true, Func<Torigin, Torigin, int> iComparator = null)
        {
            Func<Torigin, Torigin, int> theComparator = iComparator ?? Comparator;

            if (theComparator == null)
                throw new ArgumentNullException("Bad iComparator");


            for (int i = _List.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    Torigin o1 = _List[j - 1];
                    Torigin o2 = _List[j];

                    int res = theComparator(o1, o2);
                    res = Asc ? res : -res;

                    if (res > 0)
                    {
                        PrivateMove(j - 1, j);
                    }
                }
            }
        }

        #endregion

    }
}
