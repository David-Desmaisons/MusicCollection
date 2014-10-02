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
    public class RarPasswordViewModel : ViewModelBase
    {
        private CorruptedRarOrMissingPasswordArgs _CorruptedRarOrMissingPasswordArgs;
        public RarPasswordViewModel(CorruptedRarOrMissingPasswordArgs iCorruptedRarOrMissingPasswordArgs)
        {
            _CorruptedRarOrMissingPasswordArgs = iCorruptedRarOrMissingPasswordArgs;
            _SavePassword = _CorruptedRarOrMissingPasswordArgs.SavePassword;
            _Who = _CorruptedRarOrMissingPasswordArgs.Who;
            _What = _CorruptedRarOrMissingPasswordArgs.What;

            OK = RelayCommand.Instanciate(() => { 
                _CorruptedRarOrMissingPasswordArgs.accept = !string.IsNullOrEmpty(Password); 
                _CorruptedRarOrMissingPasswordArgs.Password = Password;
                _CorruptedRarOrMissingPasswordArgs.SavePassword = SavePassword;
                Window.Close(); });

            Cancel = RelayCommand.Instanciate(() =>{ _CorruptedRarOrMissingPasswordArgs.accept = false; Window.Close();});
        }

        private bool _SavePassword;
        public bool SavePassword
        {
            get { return _SavePassword; }
            set { Set(ref _SavePassword, value); }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { Set(ref _Password, value); }
        }

        private string _Who;
        public string Who
        {
            get { return _Who; }
            set { Set(ref _Who, value); }
        }

        private string _What;
        public string What
        {
            get { return _What; }
            set { Set(ref _What, value); }
        }

        public ICommand Cancel { get; private set; }
        public ICommand OK { get; private set; }
    }
}
