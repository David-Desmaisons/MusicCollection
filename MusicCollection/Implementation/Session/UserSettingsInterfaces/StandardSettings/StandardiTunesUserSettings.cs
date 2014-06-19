using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardiTunesUserSettings : IiTunesUserSettings
    {
        public BasicBehaviour ImportBrokenTrack
        {
            get { return Settings.Default.ImportBrokenItunesTrack; }
            set { Settings.Default.ImportBrokenItunesTrack = value; }
        }
    }
}
