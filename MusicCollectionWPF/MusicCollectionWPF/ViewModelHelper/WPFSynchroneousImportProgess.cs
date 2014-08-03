using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicCollectionWPF.ViewModelHelper
{
    public class WPFSynchroneousImportProgess : IImportExportProgress
    {
        private Action<ImportExportErrorEventArgs> _ActionError;
        private Action<ProgessEventArgs>        _ActionProgress;
        private Dispatcher _UIDispatcher;

        public WPFSynchroneousImportProgess(
            Action<ImportExportErrorEventArgs> error,
            Action<ProgessEventArgs> progress)
        {
            _ActionError = error;
            _ActionProgress = progress;
            if (App.Current!=null)
                _UIDispatcher = App.Current.Dispatcher;
        }
 
        public void Report(ImportExportErrorEventArgs value)
        {
            if (_ActionError == null)
                return;

            Action ac = () => _ActionError(value);
            if (_UIDispatcher != null)
                _UIDispatcher.Invoke(ac);
            else
                ac();
        }

        public void Report(ProgessEventArgs value)
        {
            if (_ActionProgress == null)
                return;

            Action ac = () => _ActionProgress(value);
            if (_UIDispatcher != null)
                _UIDispatcher.Invoke(ac);
            else
                ac();
        }
    }
}
