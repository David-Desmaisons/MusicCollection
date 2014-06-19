using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public enum MatchPrecision { Exact, Suspition };

    public class Match<T>
    {
        public T FindItem
        {
            get;
            private set;
        }

        public MatchPrecision Precision
        {
            get;
            private set;
        }

        internal Match(T itrack, MatchPrecision iPrecision)
        {
            Precision = iPrecision;
            FindItem = itrack;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", FindItem,Precision);
        }
    }

}
