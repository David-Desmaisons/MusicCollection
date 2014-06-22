using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Effects;
using System.Threading.Tasks;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.UserControls;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel.Interface;
using System.ServiceModel;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class MainWindow : CustomWindow, IEditListener, IMusicFileImporter
    {
        private IMusicSession _IS = null;
        private DispatcherTimer _Timer;

        private AlbumBrowserI albumBrowser1;
        private AlbumPlayer albumPlayer1;


        //private BlurEffect _BlurEffect;
        //public MainWindow()
        //    : this(null)
        //{
        //    _BlurEffect = new BlurEffect() { Radius = 0 };
        //    this.Effect = _BlurEffect;
        //}

        public IMusicSession Session
        {
            get { return _IS; }
        }

        public MainWindow(IMusicSession session)
        {
            _IS = session;
            InitializeComponent();
            transitionContainer1.Transition = new FadeTransition();


            albumBrowser1 = new AlbumBrowserI(session);
            albumPlayer1 = new AlbumPlayer(session);

            albumPlayer1.NeedToClose += new EventHandler<EventArgs>(albumPlayer1_NeedToClose);


            transitionContainer1.Children.Add(albumBrowser1);
            transitionContainer1.Children.Add(albumPlayer1);

            transitionContainer1.Associate(GotoPlay, albumPlayer1, (() => ((albumPlayer1.Albums.Count > 0))));
            transitionContainer1.Associate(Browse, albumBrowser1, (() => true));

            _Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(15000) };
            _Timer.Tick += ChangetoPlay;
            _Timer.Start();

            //_BlurEffect = new BlurEffect() { Radius = 0 };
            //this.Effect = _BlurEffect;
        }

        void albumPlayer1_NeedToClose(object sender, EventArgs e)
        {
            transitionContainer1.ChangeNoTransition(albumBrowser1);
        }

        private void DisplayError(object sender, ImportExportErrorEventArgs Ev)
        {
            IWindow res = WindowFactory.GetWindowFromImporterror(Ev, _IS);
            res.LogicOwner = this;
            res.ShowDialog();
        }

        private void Progress(object sender, ProgessEventArgs pea)
        {
            if (pea.ImportEnded)
            {
                CustoMessageBox cmb = new CustoMessageBox(pea);
                cmb.Show();
            }
        }

        private void IPodSynchro_Click(object sender, RoutedEventArgs e)
        {
            IItunesExporter itte = _IS.GetExporterFactory().FromType(MusicImportExportType.iTunes) as IItunesExporter;

            ToogleAdaptor3 ta3 = new ToogleAdaptor3();
            ToogleContineCancelWindow tccgw = new ToogleContineCancelWindow("Confirm to synchronize iTunes with MusicCollection:", "Delete broken iTunes file", ta3);

            if (ShowDialog(tccgw) == false)
                return;

            itte.Error += DisplayError;
            itte.Progress += Progress;

            itte.Synchronize(ta3.ResultValue);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //SettingsWindow se = new SettingsWindow(new SettingsViewModel(_IS.Setting, _IS.Dependencies));
            //ShowDialog(se);

             ShowDialog(CreateFromViewModel(new SettingsViewModel(_IS.Setting, _IS.Dependencies)));
        }

        private bool _Focused = true;
        private void ChangetoPlay(object s, EventArgs ea)
        {
            if (_Focused == true)
            {
                _Focused = this.IsActive;
            }
            else
            {
                if (this.IsActive == false)
                {
                    if ((albumPlayer1.IsPlaying) && (albumBrowser1.InEdit))
                        transitionContainer1.ApplyTransition(albumPlayer1);
                }
                else
                {
                    _Focused = true;
                }
            }
        }

        private void Window_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _Timer.Stop();
            _Timer.Start();
        }

        protected override void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Browse.Visibility = Visibility.Collapsed;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CanClose(e);

            if (e.Cancel == false)
                base.OnClosing(e);
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            ImportWindow iw = new ImportWindow(_IS);
            if (ShowDialog(iw) == true)
            {
                await DoImportAsync(iw.Importer);
            }
        }

        private async Task DoImportAsync(IMusicImporter IMu)
        {
            if (IMu == null)
                return;

            IMu.Progress += ProgressImport;
            IMu.Error += ImportError;
            await IMu.LoadAsync();
        }

        private CancellationTokenSource _CTS;

        private Nullable<bool> ShowDialog(IWindow iwindow)
        {
            //iwindow.LogicOwner = this;
            //iwindow.ShowInTaskbar = false;
            iwindow.CenterScreenLocation = true;

            //iwindow.Loaded += iwindow_Loaded;
            //iwindow.Closing += iwindow_Closing;

            return iwindow.ShowDialog();
        }

        private CancellationTokenSource ResetCancellationTokenSource()
        {
            if (_CTS != null)
            {
                _CTS.Cancel();
            }
            return _CTS = new CancellationTokenSource();
        }


        //private async void iwindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Window iwindow = sender as Window;
        //    iwindow.Loaded -= iwindow_Loaded;
        //    await _BlurEffect.SafeSmoothSet(BlurEffect.RadiusProperty, this, 5, TimeSpan.FromSeconds(0.3), ResetCancellationTokenSource());
        //}

        //private async void iwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    Window iwindow = sender as Window;
        //    iwindow.Closing -= iwindow_Closing;
        //    await _BlurEffect.SafeSmoothSet(BlurEffect.RadiusProperty, this, 0, TimeSpan.FromSeconds(0.3), ResetCancellationTokenSource());
        //}

        //private Nullable<bool> ShowDialog(IWindowEditor iwindow)
        //{
        //    //iwindow.Loaded += iwindow_Loaded;
        //    //iwindow.Closing += iwindow_Closing;

        //    return iwindow.ShowDialog();
        //}

        private void ImportError(object sender, ImportExportErrorEventArgs Ev)
        {
            ShowDialog(WindowFactory.GetWindowFromImporterror(Ev, _IS));
        }

        private bool _Importing = false;

        private void ProgressImport(object sender, ProgessEventArgs ieea)
        {
            if (!ieea.ImportEnded)
            {
                _Importing = true;
                albumBrowser1.Status = ieea.ToString();
            }
            else
            {
                _Importing = false;
                albumBrowser1.Status = null;

                IMusicImporter IMu = sender as IMusicImporter;

                IMu.Progress -= ProgressImport;
                IMu.Error -= ImportError;
            }
        }

        internal void CanClose(System.ComponentModel.CancelEventArgs e)
        {
            if (_Importing || _IS.IsUnderTransaction)
            {
                string Message = string.Format("Music Collection is {0}", _Importing ? "importing Music" : "busy");
                CustoMessageBox cmb = new CustoMessageBox(Message, "Are you sure to quit Music Collection?", true, null);
                e.Cancel = (ShowDialog(cmb) != true);
            }

            if (e.Cancel == false)
                this.albumPlayer1.OnEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.albumBrowser1.Dispose();

            base.OnClosed(e);
        }


        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnPlay(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<IAlbum> res = e.Parameter as IEnumerable<IAlbum>;

            if (res != null)
            {
                albumPlayer1.AddAlbumAndPlay(res);
            }
            else
            {
                IEnumerable<ITrack> trcs = e.Parameter as IEnumerable<ITrack>;
                if (trcs == null)
                    return;

                albumPlayer1.AddAlbumAndPlay(trcs);
            }

            transitionContainer1.ApplyTransition(albumPlayer1);
        }


        private void Edit(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<IMusicObject> res = e.Parameter as IEnumerable<IMusicObject>;

            if (res == null)
                return;

            IWindowEditor window = EditorWindow.FromEntities(res, _IS, this);
            if (window == null)
                return;

            if (!window.IsEditing)
            {
                window.ShowDialog();
                return;
            }

            EditEntity(res);

            window.EndEdit += EndEdit;
            window.Error += ImportError;

            if (window.ShowDialog() != true)
            {
                window.EndEdit -= EndEdit;
                window.Error -= ImportError;
                CancelEdit();
            }

        }

        private void EndEdit(object sender, EventArgs ea)
        {
            IWindowEditor ima = sender as IWindowEditor;
            ima.EndEdit -= EndEdit;
            ima.Error -= ImportError;
            EndEdit();
        }

        private void Delete(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<IMusicObject> al = e.Parameter as IEnumerable<IMusicObject>;
            if (al == null)
                return;

            using (IMusicRemover imu = _IS.GetMusicRemover())
            {
                string Confirm = string.Format("Confirm the deletion");

                ToogleModelAlbum tma = new ToogleModelAlbum(al.ToList()) { Continue = imu.IncludePhysicalRemove, Title = Confirm, ToogleMessage = "Delete associated files" };

                AlbumContinueCancelWindow tccgw = new AlbumContinueCancelWindow(tma);

                if (ShowDialog(tccgw) == false)
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

                imu.IncludePhysicalRemove = tma.Continue.Value;

                IMusicSettings ims = _IS.Setting;
                ims.CollectionFileSettings.DeleteRemovedFile = (imu.IncludePhysicalRemove == true) ? BasicBehaviour.Yes : BasicBehaviour.No;

                imu.Completed += EndRemove;
                imu.Comit(false);
            }
        }

        private void EndRemove(object sender, EventArgs ea)
        {
            IMusicRemover imu = sender as IMusicRemover;
            Remove(null);
            imu.Completed -= EndRemove;
        }

        private void Export(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<IAlbum> alls = e.Parameter as IEnumerable<IAlbum>;

            if (alls == null)
                return;

            Exporter exp = new Exporter(_IS, alls);

            ShowDialog(CreateFromViewModel(exp));

            IMusicExporter res = exp.MusicExporter;
            if (res != null)
            {
                res.Error += DisplayError;
                res.Progress += ProgressExport;

                res.Export(false);
            }
        }

        async Task IMusicFileImporter.ImportCompactedFileAsync(string iPath)
        {
            ICustoFilesImporterBuilder imib = _IS.GetImporterBuilder(MusicImportExportType.Custo) as ICustoFilesImporterBuilder;
            imib.Files = new string[] { iPath };
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;
            await DoImportAsync(imib.BuildImporter());
        }

        private void ProgressExport(object sender, ProgessEventArgs pea)
        {
            if (!pea.ImportEnded)
            {
                albumBrowser1.Status = pea.ToString();
            }
            else
            {
                CustoMessageBox cmb = new CustoMessageBox(pea);
                cmb.Show();
                albumBrowser1.Status = string.Empty;
                IMusicExporter res = sender as IMusicExporter;
                res.Progress -= ProgressExport;
                res.Error -= DisplayError;
            }
        }

        private void CommandBinding_CanExecute_IfnotBroken(object sender, CanExecuteRoutedEventArgs e)
        {
            IEnumerable<IAlbum> al = e.Parameter as IEnumerable<IAlbum>;
            if (al == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = al.Any(a => (a.State != ObjectState.FileNotAvailable) || (a.UpdatedState != ObjectState.FileNotAvailable));
        }

        private void Move(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<IAlbum> al = e.Parameter as IEnumerable<IAlbum>;
            if (al == null)
                return;

            MoveAlbumFileWindow mafw = new MoveAlbumFileWindow();
            mafw.ModelView = new MoveAlbumFileWindowViewModel(_IS, al);

            ShowDialog(mafw);
        }

        #region IEditListener

        private IEnumerable<IEditListener> Editors
        {
            get
            {
                yield return albumBrowser1;
                yield return albumPlayer1;
            }
        }


        public void EditEntity(IEnumerable<IMusicObject> al)
        {
            Editors.Apply(ed => ed.EditEntity(al));
        }

        public void Remove(IEnumerable<IMusicObject> al)
        {
            Editors.Apply(ed => ed.Remove(al));
        }

        public void CancelEdit()
        {
            Editors.Apply(ed => ed.CancelEdit());
        }

        public void EndEdit()
        {
            Editors.Apply(ed => ed.EndEdit());
        }

        #endregion

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _IS;
            this.albumBrowser1.transitionContainer.Current.Focus();
            var res = Keyboard.Focus(this.albumBrowser1.transitionContainer.Current);
        }

        #region TaskbarItemInfo

        private void ThumbButtonInfo_Click_Play(object sender, EventArgs e)
        {
            _IS.MusicPlayer.Mode = PlayMode.Play;
        }

        private void ThumbButtonInfo_Click_Pause(object sender, EventArgs e)
        {
            _IS.MusicPlayer.Mode = PlayMode.Paused;
        }

        private void ThumbButtonInfo_Click_Down(object sender, EventArgs e)
        {
            _IS.MusicPlayer.Volume -= 0.1;
        }

        private void ThumbButtonInfo_Click_Up(object sender, EventArgs e)
        {
            _IS.MusicPlayer.Volume += 0.1;
        }

        private void ThumbButtonInfo_Click_Like(object sender, EventArgs e)
        {
            if (_IS.MusicPlayer.Mode == PlayMode.Stopped)
                return;

            ITrack track = _IS.MusicPlayer.PlayList.CurrentTrack;
            if (track == null)
                return;

            track.Rating = 5;
        }

        #endregion

        #region Animation override

        protected override void OnLoaded()
        {
        }

        #endregion

    }
}
