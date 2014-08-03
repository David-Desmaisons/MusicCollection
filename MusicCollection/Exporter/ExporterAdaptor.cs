using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.Exporter
{
    public abstract class ExporterAdaptor
    {
        protected abstract void PrivateExport(IImportExportProgress iIImportProgress = null, CancellationToken? iCancelationToken = null);

        public void Export(IImportExportProgress iIImportExportProgress=null)
        {
            PrivateExport(iIImportExportProgress, CancellationToken.None);
        }

        public Task ExportAsync(IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken=null)
        {
            return Task.Factory.StartNew(
              () =>
              {
                  PrivateExport(iIImportExportProgress, iCancellationToken.HasValue ? iCancellationToken.Value : CancellationToken.None);
              },
               CancellationToken.None,
               TaskCreationOptions.LongRunning,
               TaskScheduler.Default);
        }

    }
}
