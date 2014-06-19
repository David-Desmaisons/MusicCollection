using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardImageFormatManagerSettings : IImageFormatManagerUserSettings
    {
        public double ImageSizeMoLimit
        {
            get { return Settings.Default.ImageSizeMoLimit; }
            set { Settings.Default.ImageSizeMoLimit = value; }
        }

        public bool ImageNumberLimit
        {
            get { return Settings.Default.ImageNumber; }
            set { Settings.Default.ImageNumber = value; }
        }

        public uint ImageNumber
        {
            get { return Settings.Default.ImageNumberLimit; }
            set { Settings.Default.ImageNumberLimit = value; }
        }

    }
}
