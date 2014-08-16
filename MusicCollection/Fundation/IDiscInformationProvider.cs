using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IDiscInformationProvider
    {
        //event EventHandler<EventArgs> OnCompleted;
        //event EventHandler<ImportExportError> OnError;

        WebMatch<IFullAlbumDescriptor> FoundCDInfo { get; }

        //void Compute(bool Sync);

        void Compute(IProgress<ImportExportError> iIImportExportProgress);

        Task ComputeAsync(IProgress<ImportExportError> iIImportExportProgress);
    }
}
