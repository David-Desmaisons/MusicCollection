using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
   // public enum ExportMode { Compressed, Custom, File };

    public interface IMusicExporter 
        //: IImporterEvent
    {
        //void Export(bool Sync);

        void Export(IImportExportProgress iIImportProgress = null);

        Task ExportAsync(IImportExportProgress iIImportProgress = null, CancellationToken? iCancelationToken = null);

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
        MusicExportType CompactFiles { get;}
    }


}
