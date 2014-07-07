using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IAsyncCommiter
    {
        bool Commit(IProgress<ImportExportErrorEventArgs> progress = null);

        Task<bool> CommitAsync(IProgress<ImportExportErrorEventArgs> progress);
    }
}
