using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable
{
    internal class DummySingleObservableCollection<T> : IObservableCollection<T>
    {
        private T _Reference;
        internal DummySingleObservableCollection(T iRef)
        {
            _Reference = iRef;
        }

        public int IndexOf(T item)
        {
            return (object.Equals(item, _Reference)) ? 0 : -1;
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public T this[int index]
        {
            get
            {
                if (index == 0) return _Reference;
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(T item)
        {
            return (object.Equals(item, _Reference));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            array[0] = this[arrayIndex];
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        private class SingleEnumerator<Tin> : IEnumerator<Tin>
        {
            private Tin _SingleElement;
            private int _Index = -1;
            public SingleEnumerator(Tin element)
            {
                _SingleElement = element;
            }

            public Tin Current
            {
                get 
                { 
                    if (_Index==0) return _SingleElement;
                    throw new InvalidOperationException();
                }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return (++_Index < 1);
            }

            public void Reset()
            {
                _Index = -1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SingleEnumerator<T>(_Reference);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        { add { } remove { } }

        public event PropertyChangedEventHandler PropertyChanged
        { add { } remove { } }
    }
}
