using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardPathUserSettings : IPathUserSettings
    {
        public string PathFolder
        {
            get { string res = Settings.Default.LastPathImportFolder; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
            set { Settings.Default.LastPathImportFolder = value; }//Settings.Default.Save(); }
        }

        public string PathCusto
        {
            get { string res = Settings.Default.LastPathImportCusto; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
            set { Settings.Default.LastPathImportCusto = value; }// Settings.Default.Save(); }
        }

        public string PathRar
        {
            get { string res = Settings.Default.LastPathImportRar; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
            set { Settings.Default.LastPathImportRar = value; }// Settings.Default.Save(); }
        }

        public string PathExport
        {
            get { string res = Settings.Default.LastPathExportFile; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
            set { Settings.Default.LastPathExportFile = value; }// Settings.Default.Save(); }
        }
    }
}
