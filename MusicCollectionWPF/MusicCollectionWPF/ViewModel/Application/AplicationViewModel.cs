using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Utilies;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel.Interface;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AplicationViewModel : ViewModelBase, IMusicFileImporter
    {
        private IMusicSession _IMusicSession;
        private ImporterCollection _ImporterCollection = new ImporterCollection();

        private bool _Importing = false;

        public AplicationViewModel(IMusicSession iIMusicSession)
        {
            _IMusicSession = iIMusicSession;
            Player = new PlayerViewModel(_IMusicSession.MusicPlayer, _IMusicSession.PlayListFactory);

            ShowSettings = RelayCommand.Instanciate(DoShowSettings);
            Import = RelayCommand.InstanciateAsync(() => DoImport(), false);
            iPodSync = RelayCommand.InstanciateAsync(() => DoiPodSynchro());
            Move = RelayCommand.Instanciate<IAlbum>(DoMove, al => NotBroken(al));
            Export = RelayCommand.InstanciateAsync<IAlbum>(DoExport, al => NotBroken(al), false);
            Edit = RelayCommand.InstanciateAsync<object>((ims) => DoEdit(ims));
            Delete = RelayCommand.InstanciateAsync<object>((ims) => DoDelete(ims));
            Play = RelayCommand.InstanciateAsync<object>((o) => DoPlay(o), false);
            GoToArtist = RelayCommand.Instanciate<IArtist>(DoGoToArtist);
            GoToGenre = RelayCommand.Instanciate<IGenre>(DoGoToGenre);

            Settings = _IMusicSession.Setting;
            AlbumSorter = _IMusicSession.AlbumSorter;
            FilterView = new FilterView(_IMusicSession);
            AlbumSorter.OnChanged += AlbumSorter_OnChanged;

            RemoveTrackNumber = RelayCommand.Instanciate<TrackView>(DoRemoveTrackNumber);
            PrefixArtistName = RelayCommand.Instanciate<TrackView>(DoPrefixArtistName);


            Albums = _IMusicSession.AllAlbums.LiveWhere(FilterView.FilterAlbum);
            Tracks = _IMusicSession.AllTracks.LiveWhere(FilterView.FilterTrack).SelectLive(t => TrackView.GetTrackView(t));

            SetOrderColection();

            Grouped = new GroupedAlbumViewModel(Albums);
            Centered = new CenteredAlbumViewModel(Albums, _IMusicSession);

            GoToPlay = Register(RelayCommand.Instanciate(() => Show(MainDisplay.Play), () => Player.ShoulBePlayed && (MainDisplay == MainDisplay.Browse)));
            GoToBrowse = Register(RelayCommand.Instanciate(() => Show(MainDisplay.Browse), () => Player.ShoulBePlayed && (MainDisplay == MainDisplay.Play)));
            FocusOnPlay = RelayCommand.Instanciate(DoFocusOnPlay);

            _PresenterMode = _IMusicSession.Setting.AparencyUserSettings.PresenterMode;

            Finder = new FindItemsControlViewModel(_IMusicSession.EntityFinder, FilterView);

            _SelectedTracks.CollectionChanged += SelectedTracks_CollectionChanged;

        }

        private void SelectedTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems!=null)
                e.OldItems.Cast<TrackView>().Apply(tr => tr.ShowAlbum = false);
            if (e.NewItems != null)
                e.NewItems.Cast<TrackView>().Apply(tr => tr.ShowAlbum = (_SelectedTracks.Where(st=>st.Album==tr.Album).Count()==1));
        }

        private MainDisplay _MainDisplay = MainDisplay.Browse;
        public MainDisplay MainDisplay
        {
            get { return _MainDisplay; }
            set { Set(ref _MainDisplay, value); }
        }

        private AlbumPresenter _PresenterMode;
        public AlbumPresenter PresenterMode
        {
            get { return _PresenterMode; }
            set
            {
                if (Set(ref _PresenterMode, value))
                    _IMusicSession.Setting.AparencyUserSettings.PresenterMode = value;
            }
        }


        public FindItemsControlViewModel Finder { get; private set; }


        private void Show(MainDisplay iMainDisplay)
        {
            MainDisplay = iMainDisplay;
        }

        private void SetOrderColection()
        {
            var ComparerToKeyFactory = new ComparerToKeyFactory<IAlbum>(AlbumSorter.Sorter);
            OrderedAlbums = Albums.LiveOrderBy(al => ComparerToKeyFactory.Get(al));
        }

        private void AlbumSorter_OnChanged(object sender, EventArgs e)
        {
            SetOrderColection();
        }

        private IList<IAlbum> _SelectedAlbums = new ObservableCollection<IAlbum>();
        public IList<IAlbum> SelectedAlbums
        {
            get { return _SelectedAlbums; }
        }

        private ObservableCollection<TrackView> _SelectedTracks = new ObservableCollection<TrackView>();
        public IList<TrackView> SelectedTracks
        {
            get { return _SelectedTracks; }
        }



        #region property

        public IAlbumSorter AlbumSorter { get; private set; }

        public IList<IAlbum> Albums { get; private set; }

        private IExtendedObservableCollection<IAlbum> _OrderedAlbums;
        public IExtendedObservableCollection<IAlbum> OrderedAlbums
        {
            get { return _OrderedAlbums; }
            private set
            {
                var old = _OrderedAlbums;
                if ((Set(ref _OrderedAlbums, value)) && (old != null))
                    old.Dispose();
            }
        }

        public IExtendedObservableCollection<TrackView> Tracks { get; private set; }

        public IMusicSettings Settings { get; private set; }

        public PlayerViewModel Player { get; private set; }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { Set(ref _Status, value); }
        }

        public FilterView FilterView { get; private set; }

        public GroupedAlbumViewModel Grouped { get; private set; }

        public CenteredAlbumViewModel Centered { get; private set; }

        #endregion

        #region Command

        public ICommand ShowSettings { get; private set; }

        public ICommand Import { get; private set; }

        public ICommand iPodSync { get; private set; }

        public ICommand Move { get; private set; }

        public ICommand Export { get; private set; }

        public ICommand Edit { get; private set; }

        public ICommand Delete { get; private set; }

        public ICommand Play { get; private set; }

        public ICommand RemoveTrackNumber { get; private set; }

        public ICommand PrefixArtistName { get; private set; }

        public ICommand GoToPlay { get; private set; }

        public ICommand GoToBrowse { get; private set; }

        public ICommand GoToArtist { get; private set; }

        public ICommand GoToGenre { get; private set; }

        public ICommand FocusOnPlay { get; private set; }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            Tracks.Dispose();
        }

        private bool NotBroken(IAlbum a)
        {
            if (a == null)
                return false;

            return (a.State != ObjectState.FileNotAvailable) || (a.UpdatedState != ObjectState.FileNotAvailable);
        }

        private void DoShowSettings()
        {
            var settingsviewmodel = new SettingsViewModel(_IMusicSession.Setting, _IMusicSession.Dependencies);
            var window = this.Window.CreateFromViewModel(settingsviewmodel);
            window.ShowDialog();
        }

        private async Task DoImport()
        {
            ImporterViewModel im = new ImporterViewModel(_IMusicSession);
            IWindow iw = this.Window.CreateFromViewModel(im);
            iw.ShowDialog();

            if (im.Continue)
            {
                await DoImportAsync(im.Importer);
            }
        }

        private async Task DoiPodSynchro()
        {
            IItunesExporter itte = _IMusicSession.GetExporterFactory().FromType(MusicExportType.iTunes) as IItunesExporter;

            InfoQuestionViewModel question = new InfoQuestionViewModel()
            {
                Title = "Confirm to synchronize iTunes with MusicCollection",
                Question = "Delete broken iTunes file?",
                Answer = null
            };

            IWindow w = this.Window.CreateFromViewModel(question);
            if (w.ShowDialog() == false)
                return;

            WPFSynchroneousImportProgess ImportProgess = new WPFSynchroneousImportProgess(OnImportError, Progress);

            await itte.SynchronizeAsync(question.Answer.Value, ImportProgess);
        }

        private void Progress(ImportExportProgress pea)
        {
            if (pea.ImportEnded)
                MessageBoxProgress(pea);
        }

        private void MessageBoxProgress(ImportExportProgress pea)
        {
            this.Window.ShowMessage(pea.Operation, pea.Operation, pea.Entity, false);
        }

        async Task IMusicFileImporter.ImportCompactedFileAsync(string iPath)
        {
            ICustoFilesImporterBuilder imib = _IMusicSession.GetImporterBuilder(MusicImportType.Custo) as ICustoFilesImporterBuilder;
            imib.Files = new string[] { iPath };
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;
            await DoImportAsync(imib.BuildImporter());
        }

        private async Task DoImportAsync(IMusicImporter IMu)
        {
            if (IMu == null)
                return;

            WPFSynchroneousImportProgess ImportProgess = new WPFSynchroneousImportProgess(OnImportError, OnImportProgress);
            await _ImporterCollection.Import(IMu, ImportProgess);
        }

        private void OnImportError(ImportExportError error)
        {
            IWindow res = this.Window.CreateFromViewModel(ViewModelFactory.GetViewModelBaseFromImporterror(error, _IMusicSession));
            res.ShowDialog();
        }

        private void OnImportProgress(ImportExportProgress progress)
        {
            if (!progress.ImportEnded)
            {
                _Importing = true;
                Status = progress.ToString();
            }
            else
            {
                _Importing = false;
                Status = null;
            }
        }

        private void DoFocusOnPlay()
        {
            if (!this.Player.IsPlaying)
                return;

            this.MainDisplay = MainDisplay.Play;
        }

        private IEnumerable<IAlbum> GetContextual(IAlbum context)
        {
            if (SelectedAlbums.Contains(context))
                return SelectedAlbums;

            return context.SingleItemCollection();
        }

        private IEnumerable<TrackView> GetContextualView(TrackView context)
        {
            if (SelectedTracks.Contains(context))
                return SelectedTracks;

            return context.SingleItemCollection();
        }

        private IEnumerable<ITrack> GetContextual(TrackView context)
        {
            return GetContextualView(context).Select(t => t.Track);
        }

        private IEnumerable<IMusicObject> GetContextual(object context)
        {
            var al = context as IAlbum;
            if (al != null)
                return GetContextual(al);

            var tv = context as TrackView;
            if (tv != null)
                return GetContextual(tv);

            var art = context as IArtist;
            if (art != null)
                return art.SingleItemCollection();

            var tr = context as ITrack;
            if (tr != null)
                return tr.SingleItemCollection();

            return Enumerable.Empty<IMusicObject>();
        }

        private void DoMove(IAlbum ial)
        {
            if (ial == null)
                return;

            var al = GetContextual(ial);

            IWindow mafw = this.Window.CreateFromViewModel(new MoveAlbumFileWindowViewModel(_IMusicSession, al));
            mafw.ShowDialog();
        }

        private async Task DoExport(IAlbum ialls)
        {
            if (ialls == null)
                return;

            var alls = GetContextual(ialls);

            Exporter exp = new Exporter(_IMusicSession, alls);

            this.Window.CreateFromViewModel(exp).ShowDialog();

            IMusicExporter res = exp.MusicExporter;
            if (res != null)
            {
                WPFSynchroneousImportProgess ImportProgess = new WPFSynchroneousImportProgess(OnImportError, ProgressExport);
                await res.ExportAsync(ImportProgess);
            }
        }

        private void ProgressExport(ImportExportProgress pea)
        {
            if (!pea.ImportEnded)
            {
                Status = pea.ToString();
            }
            else
            {
                MessageBoxProgress(pea);
                Status = string.Empty;
            }
        }

        private async Task DoEdit(object al)
        {
            IsUnderEdit = true;
            await DoEdit(GetContextual(al));
            IsUnderEdit = false;
        }

        private async Task DoEdit(IEnumerable<IMusicObject> res)
        {
            if (res == null)
                return;

            var mvb = EditorViewModelFactory.FromEntities(res, _IMusicSession);
            if (mvb == null)
                return;

            IWindow window = this.Window.CreateFromViewModel(mvb);
            if (window == null)
                return;

            window.ShowDialog();

            var importer = mvb as IInformationEditor;
            if (importer == null)
                return;

            var imp = importer.GetCommiter();
            if (imp == null)
                return;

            WPFSynchroneProgress<ImportExportError> progressor =
                new WPFSynchroneProgress<ImportExportError>(OnImportError);

            await imp.CommitAsync(progressor);
        }

        private async Task DoDelete(object al)
        {
            await DoDelete(GetContextual(al));
        }

        private async Task DoDelete(IEnumerable<IMusicObject> al)
        {
            if (al == null)
                return;

            using (IMusicRemover imu = _IMusicSession.GetMusicRemover())
            {
                ConfirmationAlbumViewModel tma = new ConfirmationAlbumViewModel(al.ToList())
                {
                    Answer = imu.IncludePhysicalRemove,
                    Title = "Confirm the deletion",
                    Question = "Delete associated files"
                };

                this.Window.CreateFromViewModel(tma).ShowDialog();

                if (!tma.IsOK)
                    return;

                var res = tma.SelectedAlbums;
                IEnumerable<IAlbum> als = res.ConvertMusicObject<IAlbum>();

                if (als.Any())
                    imu.AlbumtoRemove.AddCollection(als);
                else
                {
                    IEnumerable<ITrack> tcs = res.ConvertMusicObject<ITrack>();
                    if (!tcs.Any())
                        return;
                    imu.TrackRemove.AddCollection(tcs);
                }

                imu.IncludePhysicalRemove = tma.Answer.Value;

                IMusicSettings ims = _IMusicSession.Setting;
                ims.CollectionFileSettings.DeleteRemovedFile = (imu.IncludePhysicalRemove == true) ? BasicBehaviour.Yes : BasicBehaviour.No;

                await imu.ComitAsync();
            }
        }

        private async Task DoPlay(object tobeplayed)
        {
            await this.GetDispatcher().ExecuteAsync(() => DoPlayAll(this.GetContextual(tobeplayed)));
        }

        private void DoPlayAll(IEnumerable<IMusicObject> tobeplayed)
        {
            var res = tobeplayed as IEnumerable<IAlbum>;

            if (res != null)
            {
                Player.AddAlbumAndPlay(res);
            }
            else
            {
                IEnumerable<ITrack> trcs = tobeplayed as IEnumerable<ITrack>;
                if (trcs != null)
                    Player.AddAlbumAndPlay(trcs);
                else
                {
                    IEnumerable<IArtist> arts = tobeplayed as IEnumerable<IArtist>;
                    if (arts == null)
                        return;

                    Player.AddAlbumAndPlay(arts.SelectMany(ar=>ar.Albums));
                }
            }

            Show(MainDisplay.Play);
        }

        private bool _IsUnderEdit = false;
        public bool IsUnderEdit
        {
            get { return _IsUnderEdit; }
            set { Set(ref _IsUnderEdit, value); }
        }

        public IPersistGrid GridPersistence
        {
            get { return _IMusicSession.Setting.GetIUIGridManagement().Default; }
        }

        public IAsyncLoad TrackStatusLoader
        {
            get { return new TrackFileStatusLoader(_IMusicSession); }
        }

        private void DoRemoveTrackNumber(TrackView context)
        {
            GetContextualView(context).Apply(tv => tv.RemoveTrackNumber());
        }

        private void DoPrefixArtistName(TrackView context)
        {
            GetContextualView(context).Apply(tv => tv.PrefixArtistName());
        }

        private void DoGoToArtist(IArtist iartist)
        {
            this.GetDispatcher().ExecuteAsync(() =>
                {
                    this.PresenterMode = AlbumPresenter.Library;
                    this.MainDisplay = MainDisplay.Browse;
                    this.Grouped.GoToArtist(iartist);
                }).DoNotWaitSafe();
        }

        private void DoGoToGenre(IGenre igenre)
        {
            this.GetDispatcher().ExecuteAsync(() =>
            {
                this.PresenterMode = AlbumPresenter.Library;
                this.MainDisplay = MainDisplay.Browse;
                this.Grouped.GoToGenre(igenre);
            }).DoNotWaitSafe();
        }

        internal override bool CanClose()
        {
            bool quit = true;

            if (_Importing || _IMusicSession.IsUnderTransaction)
            {
                string Message = string.Format("Music Collection is {0}", _Importing ? "importing Music" : "busy");
                quit = this.Window.ShowConfirmationMessage(Message, "Are you sure to quit Music Collection?");
            }

            if (quit)
            {
                Player.StopPlay();
                _ImporterCollection.CancelAll();
            }

            return quit;
        }
    }
}
