using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.ToolBox.Collection.Set;

namespace MusicCollection.ToolBox.Collection
{
    internal class PolyMorphSet<T> : ISimpleSet<T> where T: class
    {
        private ILetterSimpleSet<T> _Letter;

        internal PolyMorphSet()
        {
            _Letter = LetterSimpleSetFactory<T>.GetDefault();
        }

        internal PolyMorphSet(T firstitem)
        {
            _Letter = LetterSimpleSetFactory<T>.GetDefault(firstitem);
        }

        internal PolyMorphSet(IEnumerable<T> firstitem)
        {
            _Letter = LetterSimpleSetFactory<T>.GetDefault(firstitem);
        }

        public bool Add(T item)
        {
            bool res;
            _Letter = _Letter.Add(item, out res);
            return res;
        }

        public bool Remove(T item)
        {
            bool res;
            _Letter = _Letter.Remove(item, out res);
            return res;
        }

        public bool Contains(T item)
        {
            return _Letter.Contains(item);
        }

        public int Count
        {
            get { return _Letter.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Letter.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Letter.GetEnumerator();
        }
    }
}
