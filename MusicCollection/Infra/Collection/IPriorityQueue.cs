using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.Collection
{
    public interface IPriorityQueue<T> : IEnumerable<T> 
    {
        T Dequeue();

        T Peek();

        void Enqueue(T item);

        int Count { get; }

        void CopyTo(Array array, int index);

        IComparer<T> ItemComparer { get; }
    }
}
