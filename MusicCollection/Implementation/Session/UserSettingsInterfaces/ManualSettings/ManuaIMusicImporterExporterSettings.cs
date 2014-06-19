using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualMusicImporterExporterSettings : IMusicImporterExporterUserSettings
    {  
        public MusicImportExportType LastExportType { get; set; }

        public MusicImportExportType LastImportType { get; set; }

        public bool OpenCDDoorOnEndImport { get; set; }

        public bool SynchronizeBrokeniTunes { get; set; }

        public string OutputPath { get; set; }

        public string PathMove { get; set; }
    }
}
