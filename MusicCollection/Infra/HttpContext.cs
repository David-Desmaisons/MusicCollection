using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public class HttpContext
    {
        internal string UserAgent { get; private set; }
        internal string Referer { get;private set; }

        internal HttpContext(string UA, string R)
        {
            UserAgent = UA;
            Referer = R;
        }
    }
}
