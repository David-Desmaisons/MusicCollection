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
    
    public class InfoQuestionViewModel : ViewModelBase
    {
        public InfoQuestionViewModel()
        {
            IsOK = false;
          
            Cancel = RelayCommand.Instanciate(() => { IsOK = false; Window.Close(); });
            OK = Register(RelayCommand.Instanciate(
                () => { IsOK = true; Window.DialogResult = true; Window.Close(); },
                () => Answer != null));
        }

        public string Title { get; set;}

        public string Question { get; set; }

        private bool _IsOK;
        public bool IsOK
        {
            get { return _IsOK; }
            set { this.Set(ref _IsOK, value); }
        }

        private Nullable<bool> _Answer;
        public Nullable<bool> Answer
        {
            get { return _Answer; }
            set { this.Set(ref _Answer, value); }
        }

        public ICommand Cancel { get; private set; }
        public ICommand OK { get; private set; }
    }

}
