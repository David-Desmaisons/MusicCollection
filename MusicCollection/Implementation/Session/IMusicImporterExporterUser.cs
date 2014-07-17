using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMusicImporterExporterUserSettings
    {
        MusicExportType LastExportType { get; set; }

        MusicImportType LastImportType { get; set; }

        bool OpenCDDoorOnEndImport { get; set; }

        bool SynchronizeBrokeniTunes { get; set; }

        string OutputPath { get; set; }

        string PathMove { get; set; }
    }
}
