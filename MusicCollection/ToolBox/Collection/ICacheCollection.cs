using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection
{
    internal interface ICacheCollection<TKey, TValue> : IReadOnlyDictionnary<TKey, TValue> where TValue : class 
    {
        void Register(TValue value);

        bool IsUnsafeOnRename { get; }
        
        Tuple<TValue, bool> FindOrCreateValue(TKey key, Func<TKey, TValue> Constructor);

        bool Remove(TValue key);
    }
}
