using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.Collection
{
    public static class IComparerExtender
    {
        private class RevertComparer<T> : IComparer<T>
        {
            private IComparer<T> _Comparer;
            internal RevertComparer(IComparer<T> iComparer)
            {
                _Comparer = iComparer;
            }

            public int Compare(T x, T y)
            {
                return _Comparer.Compare(y, x);
            }
        }


        public static IComparer<T> Revert<T>(this IComparer<T> @this)
        {
            return new RevertComparer<T>(@this);
        }

    }
}
