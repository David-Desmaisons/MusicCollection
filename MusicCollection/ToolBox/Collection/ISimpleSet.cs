using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection
{
    public interface ISimpleSet<T> : IEnumerable<T> where T : class
    {
        bool Add(T item);

        bool Remove(T item);

        bool Contains(T item);

        int Count { get; }
    }
}
