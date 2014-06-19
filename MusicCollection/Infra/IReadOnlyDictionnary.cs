using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public interface IReadOnlyDictionnary<TKey, TValue> where TValue : class 
    {
        IEnumerable<TValue> Values { get; }

        IEnumerable<TKey> Keys { get; }

        TValue Find(TKey key);

    }
}
