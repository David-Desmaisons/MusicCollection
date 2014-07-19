using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using System.Collections.ObjectModel;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.ToolBox.Collection.Observable;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class ConfirmationAlbumViewModel : ViewModelBase
    {
        public ConfirmationAlbumViewModel(IList<IMusicObject> iInitialAlbums)
        {
            _InitialAlbums = new WrappedObservableCollection<IMusicObject>(iInitialAlbums);
            _SelectedAlbums = new WrappedObservableCollection<IMusicObject>(iInitialAlbums);

            OK = Register(RelayCommand.Instanciate(Commit, () => (Answer != null) && SelectedAlbums.Count > 0));
        }

        private void Commit()
        {
            IsOK = true;
            Window.Close();
        }

        private bool? _Answer;
        public bool? Answer
        {
            get { return _Answer; }
            set { Set(ref _Answer,value); }
        }

        private bool _IsOK;
        public bool IsOK
        {
            get { return _IsOK; }
            set { Set(ref _IsOK, value); }
        }

        private WrappedObservableCollection<IMusicObject> _InitialAlbums;
        public IList<IMusicObject> InitialAlbums
        {
            get { return _InitialAlbums; }
        }

        private WrappedObservableCollection<IMusicObject> _SelectedAlbums;
        public IList<IMusicObject> SelectedAlbums
        {
            get { return _SelectedAlbums; }
        }

        public string Title
        {
            get;
            set;
        }

        public string Question
        {
            get;
            set;
        }

        public ICommand OK {get;private set;}

    }
}
