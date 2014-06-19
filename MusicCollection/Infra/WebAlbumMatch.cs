using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.WebServices;

namespace MusicCollection.Infra
{
    public class WebMatch<T>:Match<T> where T:class
    {
        public WebProvider WebProvider
        {
            get;
            private set;
        }

        public WebMatch(T item, MatchPrecision mp, WebProvider iWebProvider)
            : base(item, mp)
        {
            WebProvider = iWebProvider;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", FindItem.ToString(), WebProvider);
        }
    }
}
