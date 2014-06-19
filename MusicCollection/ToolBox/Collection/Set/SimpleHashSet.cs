using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection.Set
{
    internal class SimpleHashSet<T> : HashSet<T>, ILetterSimpleSet<T> where T :class
    {
        public SimpleHashSet()
        {
        }

        public SimpleHashSet(IEnumerable<T> collection):base(collection)
        {
        }

        public ILetterSimpleSet<T> Add(T item, out bool success)
        {
            success = this.Add(item);
            return this;
        }

        public ILetterSimpleSet<T> Remove(T item, out bool success)
        {
            success = this.Remove(item);
            return this;
        }
    }
}
