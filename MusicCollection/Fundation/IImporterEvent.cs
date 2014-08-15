using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IImporterEvent
    {
        event EventHandler<ImportExportError> Error;
        event EventHandler<ImportExportProgress>  Progress;
    }
}
