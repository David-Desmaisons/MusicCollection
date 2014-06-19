using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox
{
    internal static class ObjectEnumerable
    {
        internal static IEnumerable<T> SingleItemCollection<T>(this T o)
        {
            yield return o;
            yield break;
        }
    }
}
