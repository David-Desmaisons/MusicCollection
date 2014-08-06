using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.ToolBox.Collection.Observable;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class Exporter : ViewModelBase
    {
        private IMusicImporterExporterUserSettings _session;
        private IMusicExporter _IMusicExporter;

    
        public Exporter(IMusicSession iSession, IEnumerable<IAlbum> iAlbums)
        {
            Session = iSession;
            _session = Session.Setting.MusicImporterExporter;
            Option = _session.LastExportType;
            Directory = _session.OutputPath;
            iPodExport = _session.SynchronizeBrokeniTunes;
            _Albums = iAlbums.ToList();
            _SelectedAlbums = new WrappedObservableCollection<IAlbum>();
            _SelectedAlbums.CollectionChanged += SelectedAlbums_CollectionChanged;
            OK = this.Register(RelayCommand.Instanciate(() => DoOK(), () => IsValid));
            Cancel = RelayCommand.Instanciate(() => Window.Close());
        }

        private void DoOK()
        {
            Window.Close();
            _IMusicExporter = this.GetExporter();
        }

        private void SelectedAlbums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SizeChecker.Albums = _SelectedAlbums;
        }

        private IMusicSession _Session;
        public IMusicSession Session
        {
            get { return _Session; }
            set { _Session = value;  }
        }

        public IMusicExporter MusicExporter
        {
            get { return _IMusicExporter; }
        }

        private bool? _iPodExport;
        public bool? iPodExport
        {
            get { return _iPodExport; }
            set { this.Set(ref _iPodExport, value); }
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

        private MusicExportType _ExportType;
        public MusicExportType Option
        {
            get { return _ExportType; }
            set {this.Set(ref _ExportType,value);}
        }

        private List<IAlbum> _Albums;
        public IEnumerable<IAlbum> AllAlbums
        {
            get { return _Albums; }
        }

        private WrappedObservableCollection<IAlbum> _SelectedAlbums;
        public WrappedObservableCollection<IAlbum> SelectedAlbums
        {
            get { return _SelectedAlbums; }
        }

        private string _DirectoryTargets;
        public string Directory
        {
            get { return _DirectoryTargets; }
            set
            {
                if (this.Set(ref _DirectoryTargets, value))
                    SizeChecker.DirectoryName = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return Get<Exporter, bool>(() => t => (t._SelectedAlbums.Count > 0) && ((t.Option == MusicExportType.WindowsPhone) || ((t.Option != MusicExportType.iTunes) && (t.SizeChecker.SpaceCheck != null) && (t.SizeChecker.SpaceCheck.OK)) || ((t.Option == MusicExportType.iTunes) && (t.iPodExport != null))));
            }
        }

        private IMusicExporter GetExporter()
        {
            if (!IsValid)
                return null;
                   
            _session.LastExportType = Option;

            IMusicExporter Exporter = Session.GetExporterFactory().FromType(Option);
            Exporter.AlbumToExport = SelectedAlbums;

            if (Option == MusicExportType.iTunes)
            {
                _session.SynchronizeBrokeniTunes = (iPodExport == true);
                IItunesExporter itunesExporter = Exporter as IItunesExporter;
                itunesExporter.Exporttoipod = iPodExport.Value;
            }
            else
            {
                IMusicCompleteFileExporter imc = Exporter as IMusicCompleteFileExporter;
                if (imc != null)
                {
                    _session.OutputPath = Directory;
                    imc.FileDirectory = Directory;
                }
            }

            return Exporter;
        }

        public override void Dispose()
        {
            _SelectedAlbums.CollectionChanged -= SelectedAlbums_CollectionChanged;
            base.Dispose();
        }

        #region Commands

        public ICommand OK { get; private set; }
        public ICommand Cancel { get; private set; }

        #endregion
    }
}
