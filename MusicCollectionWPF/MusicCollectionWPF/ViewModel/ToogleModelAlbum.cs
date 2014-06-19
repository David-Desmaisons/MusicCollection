using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using System.Collections.ObjectModel;

namespace MusicCollectionWPF.ViewModel
{
    public class ToogleModelAlbum : NotifySimpleAdapter
    {
        private bool? _Cont;
        public bool? Continue
        {
            get { return _Cont; }
            set
            {
                _Cont = value;
                PropertyHasChanged("Continue");
            }
        }

        private IList<IMusicObject> _InitialAlbums;
        public IList<IMusicObject> InitialAlbums
        {
            get { return _InitialAlbums; }
        }

        private IList<IMusicObject> _SelectedAlbums;
        public IList<IMusicObject> SelectedAlbums
        {
            get { return _SelectedAlbums; }
        }

        public string Title
        {
            get;
            set;
        }

        public string ToogleMessage
        {
            get;
            set;
        }

        public ToogleModelAlbum(IList<IMusicObject> iInitialAlbums)
        {
            _InitialAlbums = iInitialAlbums;
            _SelectedAlbums = new ObservableCollection<IMusicObject>();
            _SelectedAlbums.AddCollection(_InitialAlbums);
        }
    }
}
