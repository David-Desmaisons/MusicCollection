using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMusicExporter
    {
        IList<IAlbum> AlbumToExport { get; }

        bool ExportImages { get; set; }

        bool CompactFiles { get; set; }

        void ExportToDirectory(string FileDirectory);

        event EventHandler<ImportExportErrorEventArgs> Error;
    }
}
