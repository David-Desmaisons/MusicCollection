using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    
    public class InfoViewModel : ViewModelBase
    {
        public InfoViewModel()
        {
            IsOK = false;
            OK = RelayCommand.Instanciate(() => { IsOK = true; Window.DialogResult = true; Window.Close(); });
            Cancel = RelayCommand.Instanciate(() => { IsOK = false; Window.Close(); });
        }

        public string Title{get;set;}

        public string Message { get; set; }

        public string MessageAdditional { get; set; }

        private bool _IsOK;
        public bool IsOK
        {
            get { return _IsOK; }
            set { this.Set(ref _IsOK, value); }
        }

        public bool ConfirmationNeeded { get; set; }

        public ICommand Cancel { get; private set; }
        public ICommand OK { get; private set; }
    }

    public class ImportExportErrorEventArgsViewModel : InfoViewModel
    {
        private ConfirmationNeededEventArgs _IEEA;
        public ImportExportErrorEventArgsViewModel(ImportExportErrorEventArgs iArg)
        {
            _IEEA = iArg as ConfirmationNeededEventArgs;
            Title = iArg.WindowName;
            Message = iArg.What;
            MessageAdditional =iArg.Who;

            ConfirmationNeeded = (_IEEA != null);
        }

        protected override void OnChanged(string iPropertyName, object old)
        {
            if ((_IEEA != null) && (iPropertyName == "IsOK"))
            {
                _IEEA.Continue = IsOK;
            }
        }
    }
}
