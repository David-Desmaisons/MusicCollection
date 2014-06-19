using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollectionTest.TestObjects
{
    public static class EnumerableFactory
    {
        public static IEnumerable<T> Create<T>(int iNumber,Func<int,T> Factory)
        {
            return Enumerable.Range(0, iNumber).Select(Factory);
        }

        public static IList<T> CreateList<T>(int iNumber, Func<int, T> Factory)
        {
            return Enumerable.Range(0, iNumber).Select(Factory).ToList();
        }

        public static IEnumerable<T> CreateWhile<T>(Func<Tuple<bool, T>> Factory)
        {
            bool ok=true;
            while (ok == true)
            {
                var res = Factory();
                ok = res.Item1;
                if (ok == false)
                    yield break;
                else
                    yield return res.Item2;
            }
        }

        public static IList<T> CreateListWhile<T>(Func<Tuple<bool, T>> Factory)
        {
            return CreateWhile(Factory).ToList();
        }
    }
}
