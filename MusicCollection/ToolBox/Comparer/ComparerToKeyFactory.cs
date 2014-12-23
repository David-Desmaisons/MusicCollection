using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra
{
    public class ComparerToKeyFactory<T>
    {
        private IComparer<T> _Comparer;
        public ComparerToKeyFactory(IComparer<T> iComparer)
        {
            _Comparer = iComparer;
        }

        public ComparerToKey<T> Get(T ikey)
        {
            return new ComparerToKey<T>(_Comparer, ikey);
        }
    }
}
