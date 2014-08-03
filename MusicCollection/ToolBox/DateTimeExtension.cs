using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox
{
    public static class DateTimeExtension
    {
        public static long ToUnixTime(this DateTime value)
        {
            return (long)(Math.Truncate((value - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds));
        }

        public static long NowUtcUnixTime()
        {
            return DateTime.UtcNow.ToUnixTime();
        }
    }
}
