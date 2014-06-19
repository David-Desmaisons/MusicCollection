using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Infra.File;
using MusicCollection.ToolBox.ZipTools;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class ExportKeyViewModel : ExchangeKeyViewModelBase
    {
        private IFileTools _FileTools;

        public ExportKeyViewModel(IMusicSession imusicsession) :
            this(imusicsession.Setting.WebUserSettings.GetDiscogsAutentificator(), imusicsession.Dependencies)
        {
        }

        public ExportKeyViewModel(IDiscogsAuthentificationProvider iIWebServicesSettings, IInfraDependencies idp) :
            base(iIWebServicesSettings, idp)
        {
            _FileTools = idp.File;
            this.Directory = _FileTools.DocumentFolder;
            Commit = this.Register(RelayCommand.InstanciateAsync(() => DoCommit(), () => _FileTools.DirectoryExists(Directory)));
        }

        private async Task DoCommit()
        {
            var window = this.Window;

            window.Close();


            string filepath = _FileTools.CreateNewAvailableName(Directory, "MusicCollectionkeys", _FileTools.KeysFileExtesion);
            bool res = await _Infra.Zip.SerializeSafeAsync(_IWebServicesSettingsWrapper.GetPrivateKeys(), filepath, Password);

            string Message = res ? string.Format("Keys exported successfully to {0}!",filepath) : "Unable to export keys!";
            window.ShowMessage(Message, "Exporting Discogs Key", false);
        }

        public ICommand Commit { get; private set; } 
    }
}
