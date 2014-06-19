using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
   // public enum ExportMode { Compressed, Custom, File };

    public interface IMusicExporter : IImporterEvent
    {
        void Export(bool Sync);


        IEnumerable<IAlbum> AlbumToExport
        {
            set;
            get;
        }
    }

    public interface IMusicFileExporter : IMusicExporter
    {
        string FileDirectory { set; get; }
    }

    public interface IMusicCompleteFileExporter : IMusicFileExporter
    {
        MusicImportExportType CompactFiles { get;}
    }


}
