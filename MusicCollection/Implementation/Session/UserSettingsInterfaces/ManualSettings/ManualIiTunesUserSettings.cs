using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualIiTunesUserSettings : IiTunesUserSettings
    {
        public BasicBehaviour ImportBrokenTrack{get;set;}
    }
}
