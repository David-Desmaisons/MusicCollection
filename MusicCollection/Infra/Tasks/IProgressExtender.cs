using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra
{
    public static class IProgressExtender
    {
        public static void SafeReport<T>(this IProgress<T> iprogress, T Event)
        {
            if (iprogress != null) iprogress.Report(Event);
        }
    }
}
