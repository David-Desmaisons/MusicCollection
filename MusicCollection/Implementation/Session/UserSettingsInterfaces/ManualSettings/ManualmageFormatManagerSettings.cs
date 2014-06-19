using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualmageFormatManagerSettings : IImageFormatManagerUserSettings
    {
        public uint ImageNumber { get; set; }

        public bool ImageNumberLimit { get; set; }

        public double ImageSizeMoLimit { get; set; }
    }
}
