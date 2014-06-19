using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class WebAlbumFoundSelectorViewModel : ViewModelBase
    {
        private CDCoverInformationArgs _CDCoverInformationArgs;
        private IMergeStrategyFactory _IMergeStrategyFactory;

        public WebAlbumFoundSelectorViewModel(CDCoverInformationArgs cdcia, IMergeStrategyFactory imfs)
        {
            _CDCoverInformationArgs = cdcia;
            _IMergeStrategyFactory = imfs;

            _OriginalAlbumName = _CDCoverInformationArgs.Current.ToString();

            if (_CDCoverInformationArgs.CDInfos.Count == 1)
            {
                SelectedInfos.Add(_CDCoverInformationArgs.CDInfos[0]);
            }

            OK = Register(RelayCommand.Instanciate(() => DoSelect(), () => SelectedInfos.Count > 0));
            Cancel = RelayCommand.Instanciate(() => this.Window.Close());
        }

        private void DoSelect()
        {
            this.Window.Close();

            foreach (var alinf in SelectedInfos)
            {
                _CDCoverInformationArgs.Current.InjectImages(alinf.FindItem,true);
            }    
        }

        #region Commands

        public ICommand OK { get; private set; }

        public ICommand Cancel { get; private set; }

        #endregion

        #region Properties

        private WrappedObservableCollection<WebMatch<IFullAlbumDescriptor>> _SelectedInfos =
            new WrappedObservableCollection<WebMatch<IFullAlbumDescriptor>>();
        public IList<WebMatch<IFullAlbumDescriptor>> SelectedInfos
        {
            get { return _SelectedInfos; }
        }

        private string _OriginalAlbumName;
        public string OriginalAlbumName
        {
            get { return _OriginalAlbumName; }
        }

        public IList<WebMatch<IFullAlbumDescriptor>> CDInfos
        {
            get { return _CDCoverInformationArgs.CDInfos; }
        }

        #endregion

    }
}
