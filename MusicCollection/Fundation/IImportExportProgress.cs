using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IImportExportProgress : IProgress<ImportExportErrorEventArgs>, IProgress<ProgessEventArgs>
    {
    }

}
