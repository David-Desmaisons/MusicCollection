using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Set
{
    public class SingleSet<T> : ILetterSimpleSet<T> where T : class
    {
        private T _SingleItem=null;
        private int _Count = 0;

        internal SingleSet()
        {
        }

        internal SingleSet(T item)
        {
            Add(item);
        }

        private Nullable<bool> Add(T item)
        {
            if (_SingleItem == null)
            {
                _SingleItem = item;
                _Count = (_SingleItem != null) ? 1 : 0;
                return true;
            }
            else
            {
                if (item == _SingleItem)
                    return false;
            }

            return null;
        }

        private bool Remove(T item)
        {
            if (_SingleItem == item)
            {
                _SingleItem = null;
                _Count = 0;
                return true;
            }

            return false;
        }

        public bool Contains(T item)
        {
            return _SingleItem == item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _SingleItem.SingleItemCollection().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _SingleItem.SingleItemCollection().GetEnumerator();
        }


        public int Count
        {
            get { return _Count; }
        }

        public ILetterSimpleSet<T> Add(T item, out bool success)
        {
            Nullable<bool> res = Add(item);
            if (res != null)
            {
                success = res.Value;
                return this;
            }

            return new ListSet<T>(LetterSimpleSetFactory<T>.MaxList, _SingleItem).Add(item,out success); 
        }

        public ILetterSimpleSet<T> Remove(T item, out bool success)
        {
            success = Remove(item);
            return this;
        }
    }
}
