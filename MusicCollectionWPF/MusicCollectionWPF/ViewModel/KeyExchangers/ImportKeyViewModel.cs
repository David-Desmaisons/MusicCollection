using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.ToolBox.ZipTools;
using System.Diagnostics;
using MusicCollection.Infra.File;

namespace MusicCollectionWPF.ViewModel
{
    class ImportKeyViewModel : ExchangeKeyViewModelBase
    {
        public ImportKeyViewModel(IMusicSession imusicsession) :
            this(imusicsession.Setting.WebUserSettings.GetDiscogsAutentificator(), imusicsession.Dependencies)
        {     
        }

        public bool Force{ get; set; }
        private IFileTools _FileTools;

        public ImportKeyViewModel(IDiscogsAuthentificationProvider iIWebServicesSettings, IInfraDependencies idp) :
            base(iIWebServicesSettings, idp)
        {
            Directory = idp.File.DocumentFolder;
            _FileTools =  _Infra.File;

            Commit = this.Register(RelayCommand.Instanciate(() => DoCommit(), () => _FileTools.FileExists(FilePath)));
    
            FileExtension = _FileTools.GetFileFilter(_FileTools.KeysFileExtesion,"key files"); 
        }

        public string FileExtension {get; private set;}
       
        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set { Set(ref _FilePath, value); }
        }

        private bool Import()
        {
            try
            {
                IDictionary<string, string> res = _Infra.Zip.UnSerializeSafe(FilePath, Password);
                if (res == null)
                    return false;

                //return _IWebServicesSettings.ImportPrivateKeys(res, Force);
                return _IWebServicesSettingsWrapper.ImportPrivateKeys(res, Force); 
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Problem importing keys: {0}", ex));
                return false;
            }    
        }

        private void DoCommit()
        {
            if (Import())
            {
                this.Window.ShowMessage("Keys Imported successfully!", "Importing Discogs Key", false);
                this.Window.Close();
            }
            else
            {
                this.Window.ShowMessage("Unable to import keys!", "Importing Discogs Key", false);             
            }
        }

        public ICommand Commit { get; private set; } 
    }
}
