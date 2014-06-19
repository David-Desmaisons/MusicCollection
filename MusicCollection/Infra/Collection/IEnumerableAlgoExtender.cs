using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.Collection
{
    public static class IEnumerableAlgoExtender
    {
        public static ICollection<T> SortFirst<T>(this IEnumerable<T> @this, int iFirst, IComparer<T> iComparer = null)
        {
            if (@this == null)
                throw new ArgumentNullException();

            if (iFirst <= 0)
                throw new ArgumentException("iFirst");

            PriorityQueue<T> pq = new PriorityQueue<T>(iComparer, iFirst + 1);

            foreach (T el in @this.Take(iFirst))
            {
                pq.Enqueue(el);
            }

            foreach (T el in @this.Skip(iFirst))
            {
                if (pq.ItemComparer.Compare(el, pq.Peek()) < 0)
                {
                    pq.Enqueue(el);
                    pq.Dequeue();
                }
            }

            LinkedList<T> res = new LinkedList<T>();
            while (pq.Count > 0)
            {
                res.AddFirst(pq.Dequeue());
            }

            return res;
        }

        public static ICollection<T> SortLast<T>(this IEnumerable<T> @this, int iFirst, IComparer<T> iComparer = null)
        {
            IComparer<T> IntComparer = iComparer ?? Comparer<T>.Default;
            return @this.SortFirst(iFirst, IntComparer.Revert());
        }

        private static void Merge<T>(this PriorityQueue<T> first, PriorityQueue<T> second)
        {
            bool needtoinsert = false;
            while (second.Count > 0)
            {
                T el = second.Dequeue();
                if ((needtoinsert) || (first.ItemComparer.Compare(el, first.Peek()) < 0))
                {
                    first.Enqueue(el);
                    first.Dequeue();
                    needtoinsert = true;
                }
            }
        }

        public static ICollection<T> SortFirstParallel<T>(this IEnumerable<T> @this, int iFirst, IComparer<T> iComparer = null)
        {
            if (@this == null)
                throw new ArgumentNullException();

            if (iFirst <= 0)
                throw new ArgumentException("iFirst");

            if (10 * iFirst > @this.Count())
            {
                return @this.SortFirst(iFirst, iComparer);
            }

            LinkedList<T> res = new LinkedList<T>();
            PriorityQueue<T> refpq = null;

            Parallel.ForEach(@this,
                   () => new PriorityQueue<T>(iComparer, iFirst + 1),
                   (el, lc, localqueue) =>
                   {
                       if (localqueue.Count < iFirst)
                       {
                           localqueue.Enqueue(el);
                       }
                       else if (localqueue.ItemComparer.Compare(el, localqueue.Peek()) < 0)
                       {
                           localqueue.Enqueue(el);
                           localqueue.Dequeue();
                       }
                       return localqueue;
                   },
                   (llist) =>
                   {
                       lock (res)
                       {
                           if (refpq == null)
                               refpq = llist;
                           else
                               refpq.Merge(llist);
                       }
                   });


            while (refpq.Count > 0)
            {
                res.AddFirst(refpq.Dequeue());
            }

            return res;
        }

        public static T MinBy<T>(this IEnumerable<T> @this, IComparer<T> icomparer = null) where T : class
        {
            if (@this == null)
                throw new ArgumentNullException();

            if (icomparer == null)
            {
                icomparer = Comparer<T>.Default;
            }

            using (IEnumerator<T> sourceIterator = @this.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    return null;
                }

                T min = sourceIterator.Current;

                while (sourceIterator.MoveNext())
                {
                    T candidate = sourceIterator.Current;
                    if (icomparer.Compare(candidate, min) < 0)
                    {
                        min = candidate;
                    }
                }
                return min;
            }
        }

        public static T MaxBy<T>(this IEnumerable<T> @this, IComparer<T> icomparer = null) where T : class
        {
            if (icomparer == null)
            {
                icomparer = Comparer<T>.Default;
            }

            return @this.MinBy(icomparer.Revert());
        }
    }
}
