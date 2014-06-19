using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualPathUserSettings : IPathUserSettings
    {
        public string PathRar { get; set; }

        public string PathCusto { get; set; }

        public string PathExport { get; set; }

        public string PathFolder { get; set; }
    }
}
