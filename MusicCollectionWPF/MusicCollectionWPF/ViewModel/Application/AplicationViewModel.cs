﻿using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel.Interface;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
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
            PlayerViewModel = new PlayerViewModel(_IMusicSession.MusicPlayer);

            ShowSettings = RelayCommand.Instanciate(DoShowSettings);
            Import = RelayCommand.InstanciateAsync(()=>DoImport());
            iPodSync = RelayCommand.InstanciateAsync(() => DoiPodSynchro());
            Move = RelayCommand.Instanciate<IEnumerable<IAlbum>>(DoMove, al => NotBroken(al));
            Export = RelayCommand.InstanciateAsync<IEnumerable<IAlbum>>(DoExport, al => NotBroken(al));
            Edit = RelayCommand.InstanciateAsync<IEnumerable<IMusicObject>>((ims) => DoEdit(ims));
            Delete = RelayCommand.InstanciateAsync<IEnumerable<IMusicObject>>((ims)=>DoDelete(ims));
            Play = RelayCommand.Instanciate<IEnumerable<IMusicObject>>(DoPlay);
           
        }

        #region property

        public IList<IAlbum> Albums { get; private set; }

        public IList<TrackView> Tracks { get; private set; }

        public PlayerViewModel PlayerViewModel { get; private set; }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { Set(ref _Status, value); }
        }

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

        #endregion

        private bool NotBroken(IEnumerable<IAlbum> al)
        {
            if (al == null)
                return false;

           return al.Any(a => (a.State != ObjectState.FileNotAvailable) || (a.UpdatedState != ObjectState.FileNotAvailable));
        }

        private void DoMove(IEnumerable<IAlbum> al)
        {
            if (al == null)
                return;

            IWindow mafw = this.Window.CreateFromViewModel(new MoveAlbumFileWindowViewModel(_IMusicSession, al));
            mafw.ShowDialog();
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


        private async Task DoExport(IEnumerable<IAlbum> alls)
        {
             if (alls == null)
                return;

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


        private async Task DoDelete( IEnumerable<IMusicObject> al )
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

        private void DoPlay(IEnumerable<IMusicObject> tobeplayed)
        {
            var res = tobeplayed as IEnumerable<IAlbum>;

            if (res != null)
            {
                PlayerViewModel.AddAlbumAndPlay(res);
            }
            else
            {
                IEnumerable<ITrack> trcs = tobeplayed as IEnumerable<ITrack>;
                if (trcs == null)
                    return;

                PlayerViewModel.AddAlbumAndPlay(trcs);
            }
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
                PlayerViewModel.StopPlay();
                _ImporterCollection.CancelAll();
            }

            return quit;
        }


        public override void Dispose()
        {
            _IMusicSession.Dispose();
            base.Dispose();
        }

    }
}
