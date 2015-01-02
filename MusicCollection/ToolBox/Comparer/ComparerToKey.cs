using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra
{
    public class ComparerToKey<T> : IComparable<ComparerToKey<T>>
    {
        private T _Key;
        private IComparer<T> _Comparer;
        public ComparerToKey(IComparer<T> iComparer, T iKey)
        {
            _Comparer = iComparer;
            _Key = iKey;
        }

        public int CompareTo(ComparerToKey<T> other)
        {
            return _Comparer.Compare(_Key, other._Key);
        }

        public override bool Equals(object obj)
        {
            ComparerToKey<T> other = obj as ComparerToKey<T>;
            if (other == null) return false;
            return (CompareTo(other)==0);
        }

        public override int GetHashCode()
        {
            return _Key.GetHashCode();
        }
    }
}
