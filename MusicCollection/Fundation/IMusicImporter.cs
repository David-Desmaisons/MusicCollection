using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;

using MusicCollection.Infra;
using System.Threading.Tasks;
using System.Threading;


namespace MusicCollection.Fundation
{

    public interface IMusicImporter
    {
        void Load(IImportExportProgress iIImportProgress=null);

        Task LoadAsync(ThreadProperties tp);

        Task LoadAsync( IImportExportProgress iIImportProgress=null, CancellationToken? iCancelationToken=null);
    }

}
