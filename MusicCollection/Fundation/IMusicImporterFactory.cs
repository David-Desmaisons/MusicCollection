using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMusicExporterFactory
    {
        IMusicExporter FromType(MusicImportExportType type);

        IMusicFileExporter GetMover(); 
    }
}
