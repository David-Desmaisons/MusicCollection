using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IItunesExporter : IMusicExporter
    {
        Task SynchronizeAsync(bool DeleteBrokenItunes, IImportExportProgress iIImportProgress = null, CancellationToken? iCancellationToken=null);

       // void ExportToItunes(IEnumerable<IAlbum> Albums,bool ExporttoiPod);

        bool Exporttoipod
        {
            set;
            get;
        }

    }
}
