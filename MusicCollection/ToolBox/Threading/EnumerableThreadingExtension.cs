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

        //public static ParallelQuery<T> Parallelize<T>(this IList<T> @this)
        //{
        //    return @this.AsParallel().WithDegreeOfParallelism(@this.Count);
        //}
    }
}
