using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    class StandardMaturitySetting : IMaturityUserSettings
    {
        public bool ExportCollectionFiles
        {
            get { return Settings.Default.ExportCollectionFiles; }
            set { Settings.Default.ExportCollectionFiles = value; }
        }

        public string DirForPermanentCollection
        {
            get { return Settings.Default.DirForPermanentCollection; }
            set { Settings.Default.DirForPermanentCollection = value; }
        }

        public BasicBehaviour DeleteRemovedFile
        {
            get { return Settings.Default.DeleteRemovedFile; }
            set { Settings.Default.DeleteRemovedFile = value; }
        }
    }
}
