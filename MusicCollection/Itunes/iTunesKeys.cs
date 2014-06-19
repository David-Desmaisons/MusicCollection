using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using TagLib;
using System.Diagnostics;
using System.ComponentModel;

using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollection.Itunes
{
    static internal class iTunesKeys
    {
        internal const string List = "plist";

        internal const string dict = "dict";

        internal const string Key = "key";
    }
}
