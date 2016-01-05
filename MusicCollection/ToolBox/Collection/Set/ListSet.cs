using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection.Set
{
    internal class ListSet<T> : ILetterSimpleSet<T> where T : class
    {  
        private T[] _Items;
        private int _Count = 0;

        internal ListSet(int Capacity)
        {
            _Items = new T[Capacity];
        }

        internal ListSet(int Capacity, T item)
        {
            _Items = new T[Capacity];
            _Items[0] = item;
            _Count = 1;
        }

        internal ListSet(int Capacity, IEnumerable<T> items)
        {
            int count = items.Count();
            if (count > Capacity)
            {
                throw new Exception("ListSet constructor");
            }

            _Items = new T[Capacity];

            int index = 0;
            foreach (T item in items)
            {
                _Items[index++] = item;
            }

            _Count = count;
        }

        private bool Add(T item)
        {
            if (Contains(item))
                return false;

            _Count++;
            _Items[_Count-1] = item;
            return true;
        }

        private bool Remove(T item)
        {
            if (item==null)
                return false;

            for(int i=0; i<_Count;i++)
            {
                T iitem = _Items[i];
                if (item == iitem)
                {
                    if (i == _Count - 1)
                    {
                        _Items[i] = null;
                    }
                    else
                    {
                        _Items[i] = _Items[_Count-1];
                        _Items[_Count-1] = null;
                    }

                    _Count--;
                    return true;
                }
            }
            
            return false;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _Count; i++)
            {
                if (item == _Items[i])
                    return true;
            }
            return false;
        }

        public int Count
        {
            get { return _Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Items.Where(t => t != null).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Items.Where(t => t != null).GetEnumerator();
        }

        public ILetterSimpleSet<T> Add(T item, out bool success)
        {
            if (_Count == _Items.Length)
            {
                return new SimpleHashSet<T>(_Items).Add(item,out success);
            }

            success = Add(item);
            return this;
        }

        public ILetterSimpleSet<T> Remove(T item, out bool success)
        {
            success = Remove(item);
            return this;
        }
    }
}
