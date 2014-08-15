using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;

namespace MusicCollection.FileImporter
{
    internal class IEventListenerAdaptor : IEventListener
    {
        private IImportExportProgress _IImportExportProgress;
        private IImportContext _IImportContext;
        public IEventListenerAdaptor(IImportExportProgress iIImportExportProgress, IImportContext iIImportContext)
        {
            _IImportExportProgress = iIImportExportProgress;
            _IImportContext = iIImportContext;
        }

        public void Report(Fundation.ImportExportError Error)
        {
            _IImportExportProgress.SafeReport(Error);
        }

        public void Report(Fundation.ImportExportProgress Where)
        {
            _IImportExportProgress.SafeReport(Where);
        }

        public void OnFactorisableError<T>(string message) where T : Fundation.ImportExportErrorEventListItemsArgs
        {
            _IImportContext.OnFactorisableError<T>(message);
        }
    }
}
