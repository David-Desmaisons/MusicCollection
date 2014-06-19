using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMusicImporterExporterUserSettings
    {
        MusicImportExportType LastExportType { get; set; }

        MusicImportExportType LastImportType { get; set; }

        bool OpenCDDoorOnEndImport { get; set; }

        bool SynchronizeBrokeniTunes { get; set; }

        string OutputPath { get; set; }

        string PathMove { get; set; }
    }
}
