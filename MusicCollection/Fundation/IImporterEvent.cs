using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IImporterEvent
    {
        event EventHandler<ImportExportErrorEventArgs> Error;
        event EventHandler<ProgessEventArgs>  Progress;
    }
}
