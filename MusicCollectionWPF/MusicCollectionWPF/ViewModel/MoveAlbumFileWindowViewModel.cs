using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class MoveAlbumFileWindowViewModel : ViewModelBase
    {
        private IMusicSession _IMusicSession;
        public MoveAlbumFileWindowViewModel(IMusicSession ims, IEnumerable<IAlbum> al)
        {
            _IMusicSession = ims;
            AllAlbums = al;
            Directory = _IMusicSession.Setting.MusicImporterExporter.PathMove;
            _SelectedAlbums = new WrappedObservableCollection<IAlbum>();
            _SelectedAlbums.CollectionChanged += inc_CollectionChanged;

            Move = this.Register( RelayCommand.Instanciate(() => DoMove(), ()=>IsValid));
        }

        private async void DoMove()
        {
            if (!IsValid)
                return;

            _IMusicSession.Setting.MusicImporterExporter.PathMove = Directory;

            IMusicFileExporter imf = _IMusicSession.GetExporterFactory().GetMover();
            if (imf == null)
                return;

            imf.AlbumToExport = SelectedAlbums;
            imf.FileDirectory = Directory;

            await imf.ExportAsync();
            Window.Close();
        }

        public ICommand Move
        {
            get;
            private set;
        }

        private List<IAlbum> _Albums;
        public IEnumerable<IAlbum> AllAlbums
        {
            get { return _Albums; }
            set { this.Set(ref _Albums, (value == null) ? null : value.ToList()); }
        }

        private WrappedObservableCollection<IAlbum> _SelectedAlbums;
        public IList<IAlbum> SelectedAlbums
        {
            get { return _SelectedAlbums; }
        }

        private void inc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SizeChecker.Albums = _SelectedAlbums;
        }

        private string _DirectoryTargets;
        public string Directory
        {
            get { return _DirectoryTargets; }
            set
            {
                if (this.Set(ref _DirectoryTargets, value))
                {
                    SizeChecker.DirectoryName = value;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return Get<MoveAlbumFileWindowViewModel, bool>(() => t => ((t.SizeChecker.SpaceCheck != null) && (t.SizeChecker.SpaceCheck.OK) && (t.SelectedAlbums.Count>0)));
            }
        }

        private AlbumListSizeChecker _ALSC;
        public AlbumListSizeChecker SizeChecker
        {
            get
            {
                if (_ALSC == null)
                {
                    _ALSC = new AlbumListSizeChecker(Directory);
                }
                return _ALSC;
            }
        }

        public override void Dispose()
        {
            _SelectedAlbums.CollectionChanged -= inc_CollectionChanged;
            base.Dispose();
        }
    }
}
