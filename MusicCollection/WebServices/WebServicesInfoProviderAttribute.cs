using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.WebServices
{
    public enum WebProvider
    {
        Unknown,
        MusicBrainz,
        Freedb,
        Discogs,
        Amazon,
        iTunes
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class WebServicesInfoProviderAttribute : Attribute
    {
        internal WebProvider WebProvider
        {
            get;
            private set;
        }

        internal WebServicesInfoProviderAttribute(WebProvider iWebProvider)
        {
            WebProvider = iWebProvider;
        }
    }
}
