using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MusicCollection.ToolBox
{
    public static class EnumerableThreadingExtension
    {
        public static List<T> CancelableToList<T>(this IEnumerable<T> @this, CancellationToken iToken)
        {
            return @this.TakeWhile(_ => iToken.IsCancellationRequested == false).ToList();
        }

        public static IEnumerable<T> TakeWhileNotCancelled<T>(this IEnumerable<T> @this, CancellationToken iToken)
        {
            return @this.TakeWhile(_ => iToken.IsCancellationRequested == false);
        }
    }
}
