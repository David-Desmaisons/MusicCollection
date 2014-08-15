using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicCollectionWPF.ViewModelHelper
{
    public class WPFSynchroneousImportProgess : WPFSynchroneProgress<ImportExportError,ImportExportProgress>,  IImportExportProgress
    {        
        public WPFSynchroneousImportProgess( Action<ImportExportError> error, Action<ImportExportProgress> progress)
            : base(error, progress)
        {
        }

    }
}
