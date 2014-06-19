using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.ZipTools;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public abstract class ExchangeKeyViewModelBase : ViewModelBase
    {
        //protected IWebUserSettings _IWebServicesSettings;
        protected IDiscogsAuthentificationProvider _IWebServicesSettingsWrapper;
        protected IInfraDependencies _Infra;

        protected ExchangeKeyViewModelBase(IDiscogsAuthentificationProvider iIWebServicesSettings, IInfraDependencies idep)
        {
            //_IWebServicesSettings = iIWebServicesSettings;
            _IWebServicesSettingsWrapper = iIWebServicesSettings;
            _Infra = idep;
            _Password = string.Empty; 
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { Set(ref _Password, value); }
        }

        private string _Directory;
        public string Directory
        {
            get { return _Directory; }
            set { Set(ref _Directory, value); }
        }   
    }
}
