using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MusicCollection.Fundation;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel
{
    public class ImporterItem
    {
        private CancellationTokenSource _CancellationTokenSource;
        public ImporterItem(IMusicImporter iIMusicImporter)
        {
            Importer = iIMusicImporter;
            _CancellationTokenSource = new CancellationTokenSource();
        }

        public Task RunAsync(IImportExportProgress iIImportExportProgress)
        {
            return Importer.LoadAsync(iIImportExportProgress, _CancellationTokenSource.Token);
        }

        public void Cancel()
        {
            _CancellationTokenSource.Cancel();
        }
        IMusicImporter Importer {get;private set;}  
    }


    public class ImporterCollection
    {
        private WrappedObservableCollection<ImporterItem> _Importers = new WrappedObservableCollection<ImporterItem>();
        public async Task Import(IMusicImporter iIMusicImporter,IImportExportProgress iIImportExportProgress)
        {
            var imp = new ImporterItem(iIMusicImporter);
            _Importers.Add(imp);
            await imp.RunAsync(iIImportExportProgress);
            _Importers.Remove(imp);
        }

        public void CancelAll()
        {
            _Importers.Apply(im => im.Cancel());
        }
    }
}
