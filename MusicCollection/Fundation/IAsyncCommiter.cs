using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IAsyncCommiter
    {
        bool Commit(IProgress<ImportExportError> progress = null);

        Task<bool> CommitAsync(IProgress<ImportExportError> progress);
    }
}
