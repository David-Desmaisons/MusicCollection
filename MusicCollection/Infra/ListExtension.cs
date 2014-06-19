using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Infra
{
    static public class ListExtension
    {
        static public IList<T> AddCollection<T>(this IList<T> iList, IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return iList;

            if (iList == null)
                throw new Exception("Programm error");

            foreach (T t in enumerable)
                iList.Add(t);

            return iList;
        }

        static public List<T> Randomize<T>(this IList<T> iList, int Capacity)
        {
            if (iList.Count < Capacity)
                return null;

            Random rd = new Random(DateTime.Now.Millisecond);

            List<T> res = new List<T>(Capacity);

            HashSet<int> als = new HashSet<int>();
            for (int i = 0; i < Capacity; i++)
            {
                int cand = rd.Next(iList.Count);
                while (als.Contains(cand))
                {
                    cand = rd.Next(iList.Count);
                }
                als.Add(cand);
                res.Add(iList[cand]);
            }

            return res;
        }

        [DebuggerStepThrough]
        static public IEnumerable<T> Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                return null;

            foreach (T o in enumerable)
            {
                action(o);
            }

            return enumerable;
        }

        static internal bool SequenceEqual<T>(this IList<T> List1, IList<T> List2, Func<T, T, bool> Comparator)
        {
            if (List1.Count != List2.Count)
                return false;

            int i = 0;
            foreach (T t in List1)
            {
                if (!Comparator(t, List2[i++]))
                    return false;
            }

            return true;
        }

        static public IEnumerable<T> OrberWithIndexBy<T>(this IEnumerable<T> enumerable, Func<int, T, int> Orderer)
        {
            return enumerable.Select((t, i) => new Tuple<int, T>(i, t)).OrderBy(t => Orderer(t.Item1, t.Item2)).Select(t => t.Item2);
        }

        static public IEnumerable<Tuple<int, T>> AsIndexed<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((t, i) => new Tuple<int, T>(i, t));
        }

        static public IEnumerable<int> Indexes<T>(this IEnumerable<T> enumerable, T value)
        {
            return enumerable.AsIndexed().Where(t => object.ReferenceEquals(t.Item2, value)).Select(t => t.Item1);
        }

        static public int Index<T>(this IEnumerable<T> enumerable, T value)
        {
            return enumerable.Index((t) => object.Equals(t, value));
        }

        static public int Index<T>(this IEnumerable<T> enumerable, Func<T, bool> Selector)
        {
            int res = -1;

            var l = enumerable.AsIndexed().Where((o) => Selector(o.Item2));

            if (!l.Any())
                return res;

            return l.First().Item1;
        }

        static public bool SequenceCompareWithoutOrder<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
        {
            if (enumerable.Count() != other.Count())
                return false;

            var t1 = enumerable.ToLookup((t) => (t)).Select(g => new Tuple<T, int>(g.Key, g.Count()));
            var t2 = other.ToLookup((t) => (t)).Select(g => new Tuple<T, int>(g.Key, g.Count()));

            if (t1.Count() != t2.Count())
                return false;

            var d = t1.Except(t2);
            return !d.Any();
        }

        static public bool SequenceComparNoOrder<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
        {
            if (enumerable.Count() != other.Count())
                return false;

            var t1 = enumerable.Union(other);
            if (!t1.SequenceEqual(enumerable))
                return false;

            var t2 = enumerable.Except(other);

            return !t2.Any();
        }

        public static int GetIndex<T>(this IList<T> @this, IList<T> CopyCollection, int IndexNotFilter)
        {
            int Max = Math.Min(@this.Count, CopyCollection.Count);
            int FilteredCount = 0;

            for (int i = 0; i < IndexNotFilter; i++)
            {
                if (FilteredCount < Max && object.Equals(@this[i], CopyCollection[FilteredCount]))
                    FilteredCount++;
            }

            return FilteredCount;
        }

        public static bool GetIndex<T>(this IList<T> @this, IList<T> Deployed, int index, out int outindex)
        {
            outindex = -1;

            if (index == @this.Count - 1)
            {
                return true;
            }

            outindex = @this.GetIndex(Deployed, index);
            return false;
        }

        public static IEnumerable<T> SingleItemCollection<T>(this T o)
        {
            if (o != null)
                yield return o;
        }

        public static IObservableCollection<T> SingleObservableCollection<T>(this T o)
        {
            return new DummySingleObservableCollection<T>(o);
        }

       
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        private static readonly Random _Randomizer = new Random();

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            return source.OrderBy<T, int>((item) => _Randomizer.Next());
        }

        public static T RandomizedItem<T>(this IList<T> @this)
        {
            if (@this == null)
                throw new ArgumentNullException();

            if (@this.Count == 0)
                return default(T);

            return @this[_Randomizer.Next(@this.Count)];
        }

        public static T RandomizedItem<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
                throw new ArgumentNullException();

            return @this.ToList().RandomizedItem();
        }

        public static int BinarySearch<T>(this IList<T> array, T value, IComparer<T> comparer=null)
        {
            if (array == null)
                throw new ArgumentNullException(); 

            return InternalBinarySearch(array, 0, array.Count, value,comparer ?? Comparer<T>.Default);
        }

        private static int InternalBinarySearch<T>(IList<T> array, int index, int length, T value, IComparer<T> comparer)
        {
            int num = index;
            int num2 = (index + length) - 1;
            while (num <= num2)
            {
                int num3 = num + ((num2 - num) >> 1);
                int num4 = comparer.Compare(array[num3], value);
                if (num4 == 0)
                {
                    return num3;
                }
                if (num4 < 0)
                {
                    num = num3 + 1;
                }
                else
                {
                    num2 = num3 - 1;
                }
            }
            return ~num;
        }
    }
}
