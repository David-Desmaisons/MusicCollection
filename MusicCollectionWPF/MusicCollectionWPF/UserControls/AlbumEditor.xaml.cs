using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Utilies;

using MusicCollectionWPF.Infra;
using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;


namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumEditor.xaml
    /// </summary>
    public partial class AlbumEditor : UserControl, IDisposable, IDropTargetAdvisor, IDragSourceAdvisor
    {
        private IModifiableAlbum _IMA;
        private string _CurrentDirectory = null;
        //private bool _SearchingOnTheNet = false;
        //private IInternetFinder _IIF = null;
        private IMusicSession _Session;

        internal IMusicSession Session
        {
            get { return _Session; }
            set { _Session = value; GenreCombo.Factory = FactoryBuilder.Instanciate((n) => _Session.GetGenreFactory().Create(n)); }
        }
        //GenreCombo.ItemsSource = _Session.AllGenres;

        public event Action<Boolean> ShouldClose;

        public AlbumEditor()
        {
            //  _Genres = ((System.Windows.Data.CollectionViewSource)(this.FindResource("iAlbumViewSource")));
            this.DataContextChanged += ((o, e) => InitAlbum(e.NewValue as IModifiableAlbum));
            _IMA = DataContext as IModifiableAlbum;
            InitializeComponent();

        }

        private ArtistSearchableFactory _AF;

        public void InitAlbum(IModifiableAlbum Al)
        {
            if (_IMA != null)
                throw new Exception("");
            _IMA = Al;

            _AF = new ArtistSearchableFactory(_IMA.Session);
            this.ArtistsControl1.SF = _AF;

            // GenreCombo.ItemsSource = _IMA.Session.AllGenres.LiveOrderBy((g) => g.FullName);

            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            if (view.IsEmpty == false)
            {
                view.MoveCurrentToFirst();
            }

            _IMA.Images.CollectionChanged += ImageListener;
        }

        private void ImageListener(object sender, NotifyCollectionChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            if (view.CurrentItem == null)
                view.MoveCurrentToFirst();
        }

        #region Paste

        private void PreviewCanPasteEditWindow(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = false;

            if (Clipboard.ContainsImage())
            {
                e.CanExecute = true;
                e.Handled = true;
                return;
            }

            if (Clipboard.ContainsFileDropList())
            {
                try
                {
                    StringCollection res = Clipboard.GetFileDropList();

                    if (res != null)
                    {
                        foreach (string File in res)
                        {
                            if (FileServices.GetFileType(File) == FileType.Image)
                            {
                                e.CanExecute = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Problem using clipboard:" + ex.ToString());
                }
            }

            e.Handled = true;
        }

        private void CanPasteEditWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void PreviewPasteEditWindow(object sender, ExecutedRoutedEventArgs e)
        {
            int c = (CurrentIndex == -1) ? 0 : CurrentIndex;

            if (Clipboard.ContainsImage())
            {
                try
                {

                    SwitchView(_IMA.AddAlbumPicture(Clipboard.GetImage(), c));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Problem using clipboard: Memory issue?:" + ex.ToString());
                }

                return;
            }

            if (Clipboard.ContainsFileDropList())
            {
                IAlbumPicture first = null;
                foreach (string File in Clipboard.GetFileDropList())
                {
                    if (FileServices.GetFileType(File) == FileType.Image)
                    {

                        IAlbumPicture res = _IMA.AddAlbumPicture(File, c++);
                        if (first == null)
                            first = res;
                    }

                }

                if (first != null)
                    SwitchView(first);
            }
        }

        #endregion

        private void SwitchView(IAlbumPicture iap)
        {
            CollectionViewSource.GetDefaultView(_IMA.Images).MoveCurrentTo(iap);
        }

        #region delete

        private void PreviewCanDeleteEditWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_IMA != null)
                e.CanExecute = (_IMA.Images.Count != 0);
            e.Handled = true;

        }

        private void CanDeleteEditWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void PreviewDeleteEditWindow(object sender, ExecutedRoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);

            _IMA.Images.RemoveAt(view.CurrentPosition);
        }

        #endregion

        #region moveimage

        private void CommandBinding_CanExecute_Last(object sender, CanExecuteRoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            e.CanExecute = (view.CurrentPosition != _IMA.Images.Count() - 1);
            e.Handled = true;
        }

        private void CommandBinding_CanExecute_First(object sender, CanExecuteRoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            e.CanExecute = (view.CurrentPosition != 0);
            e.Handled = true;
        }

        private void Set_As_Last(object sender, ExecutedRoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            int Index = view.CurrentPosition;
            IAlbumPicture IAL = view.CurrentItem as IAlbumPicture;
            _IMA.Images.RemoveAt(Index);
            _IMA.Images.Add(IAL);
            view.MoveCurrentToLast();
        }

        private void Set_As_First(object sender, ExecutedRoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            int Index = view.CurrentPosition;
            IAlbumPicture IAL = view.CurrentItem as IAlbumPicture;
            _IMA.Images.RemoveAt(Index);
            _IMA.Images.Insert(0, IAL);
            view.MoveCurrentToFirst();
        }


        #endregion

        //internal bool IsWorking { get { return _SearchingOnTheNet; } }

        #region Webservices

        private void FindDiscog(object sender, RoutedEventArgs e)
        {
            WebAlbumSelectorViewModel wasvm = new WebAlbumSelectorViewModel(_Session, _IMA);
            IWindow window = new WebInfoChooserWindow() { ModelView = wasvm };
            window.CenterScreenLocation = true;
            window.ShowInTaskbar = false;
            window.LogicOwner = Window.GetWindow(this) as IWindow;
            window.ShowDialog();
        }

        //private void FindDiscog2(object sender, RoutedEventArgs e)
        //{
        //    if (_SearchingOnTheNet == true)
        //        return;

        //    this.ArtistsControl1.Commit();

        //    _SearchingOnTheNet = true;


        //    IWebQuery wb = _Session.WebQueryFactory.FromAlbumDescriptor(_IMA.GetAlbumDescriptor());
        //    wb.NeedCoverArt = true;
        //    wb.MaxResult = 5;

        //    //_IIF = _Session.InternetFinder;

        //    _IIF = _Session.GetInternetFinder(wb);
        //    //_IMA

        //    _IIF.OnInternetError += OnInternetDown;
        //    _IIF.OnResult += OnInternetResult;
        //    //_IIF.OnSearchProgress += OnInternetProgress;

        //    BeginEdit();

        //    //_IIF.Query = wb;


        //    _IIF.Compute(false);
        //}

        //private void OnInternetDown(object sender, InternetFailedArgs ev)
        //{
        //    OnInternetError(string.Format("Intenet connection to {0} is down. Check site availability and your internet connection.", ev.WebService), "Connection Failed!");
        //}

        //private void BeginEdit()
        //{
        //    progressBar1.Visibility = Visibility.Visible;
        //    progressBar1.IsIndeterminate = true;
        //}

        //private void OnInternetProgress(object sender, OnInternetFinderProgressEventArgs ev)
        //{
        //    InternetStatus.Text = ev.ToString();
        //}


        //private void FinalizeInternetSearch()
        //{
        //    _SearchingOnTheNet = false;
        //    progressBar1.IsIndeterminate = false;
        //    progressBar1.Visibility = Visibility.Hidden;

        //    if (_IIF != null)
        //    {
        //        _IIF.Cancel();

        //        _IIF.OnInternetError -= OnInternetDown;
        //        _IIF.OnResult -= OnInternetResult;
        //        //_IIF.OnSearchProgress -= OnInternetProgress;

        //        _IIF = null;
        //    }
        //}

        //private async void OnInternetResult(object sender, InternetFinderResultEventArgs ev)
        //{
        //    //IInternetFinder iif = sender as IInternetFinder;
        //    InternetStatus.Text = null;
        //    FinalizeInternetSearch();

        //    if (ev == null)
        //        return;

        //    if ((ev.Found.Found == null) || (ev.Found.Found.Count == 0))
        //    {
        //        OnInternetError("No information found for this Album.", "No Information found");
        //        return;
        //    }

        //    CDCoverInformationArgs cdif = new CDCoverInformationArgs(ev.Found.Found, this._IMA.GetAlbumDescriptor());

        //    InternetResultWindow wt = new InternetResultWindow(cdif);
        //    wt.Owner = App.Current.MainWindow;
        //    wt.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        //    if (wt.ShowDialog() == true)
        //    {
        //        if (cdif.SelectedInfo == null)
        //            return;

        //        BeginEdit();
        //        //    Action act = FinalizeInternetSearch;
        //        //    _IMA.BeginMergeFromMetaData(cdif.SelectedInfo.FindItem, Session.Strategy.Standard, () => this.Dispatcher.Invoke(act));

        //        await _IMA.MergeFromMetaDataAsync(cdif.SelectedInfo.FindItem, Session.Strategy.Standard);
        //        FinalizeInternetSearch();
        //    }

        //}

        //private void OnInternetError(string Message, string Name)
        //{
        //    new CustoMessageBox(Message, Name, false).ShowDialog();
        //}


        //private void FinalizeInternetImport()
        //{
        //    ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);

        //    if (!view.IsEmpty)
        //        view.MoveCurrentToFirst();

        //    FinalizeInternetSearch();

        //}


        private void FindOnInternet(object sender, RoutedEventArgs e)
        {
            //InternetCoverFinder ICF = new InternetCoverFinder(_IMA);
            //ICF.Show();

            InternetCoverFinder ICF = new InternetCoverFinder() { ModelView = new InternetFinderViewModel(_IMA) };
            ICF.Show();
        }

        #endregion

        private IEnumerable<IModifiableTrack> TrackFromContext(IModifiableTrack track)
        {
            //deux possibilite: si album a deleter fait l'objet d'une multi-selection
            // je delete toute la selection. sinon je delete que l'object concerne
            if (track == null)
                return null;

            IList l = listView1.SelectedItems;

            if (l.Contains(track) && (l.Count > 1))
            {
                return (from object imt in l select imt as IModifiableTrack);
            }

            return track.SingleItemCollection<IModifiableTrack>();
        }

        private IEnumerable<IModifiableTrack> TracktoConsider(object sender)
        {

            MenuItem mi = sender as MenuItem;
            if (mi == null)
                return null;

            IModifiableTrack track = ((mi.CommandParameter as ContextMenu).PlacementTarget as FrameworkElement).DataContext as IModifiableTrack;

            return TrackFromContext(track);
        }

        private void OnWindowOpenTrack(object sender, RoutedEventArgs e)
        {
            IEnumerable<IModifiableTrack> tc = TracktoConsider(sender);

            if (tc == null)
                return;

            FileServices.OpenExplorerWithSelectedFiles(((from imt in tc where imt.State != ObjectState.Removed select imt.Path).ToList()));
        }

        private void OnDeleteTrack(object sender, RoutedEventArgs e)
        {
            IEnumerable<IModifiableTrack> tc = TracktoConsider(sender);

            if (tc == null)
                return;

            foreach (IModifiableTrack imt in tc.ToList())
            {
                imt.Delete();
            }
        }




        private void FindImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All Image Files | " + FileServices.GetImagesFilesSelectString();

            if (_CurrentDirectory == null)
                _CurrentDirectory = _IMA.MainDirectory;

            openFileDialog.InitialDirectory = _CurrentDirectory;
            openFileDialog.Title = "Select Image";

            IAlbumPicture first = null;


            if (openFileDialog.ShowDialog() == true)
            {
                _CurrentDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                int Index = CurrentIndex + 1;

                foreach (string File in openFileDialog.FileNames)
                {
                    IAlbumPicture res = _IMA.AddAlbumPicture(File, Index++);
                    if (first == null)
                        first = res;
                }

            }

            if (first != null)
                SwitchView(first);
        }


        private void SplitImage(object sender, RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            IAlbumPicture IAP = view.CurrentItem as IAlbumPicture;

            if (IAP == null)
                return;

            SwitchView(_IMA.SplitImage(CurrentIndex));

        }

        private void RotateImage(object sender, RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);
            IAlbumPicture IAP = view.CurrentItem as IAlbumPicture;

            if (IAP == null)
                return;

            SwitchView(_IMA.RotateImage(CurrentIndex, true));

        }

        private int CurrentIndex
        {
            get
            {
                return CollectionViewSource.GetDefaultView(_IMA.Images).CurrentPosition;
            }
        }

        internal void RootGrid_KeyDown(object sender, KeyEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_IMA.Images);

            switch (e.Key)
            {
                case Key.Right:

                    if (view.IsEmpty == false)
                    {
                        view.MoveCurrentToNext();
                        if (view.IsCurrentAfterLast)
                            view.MoveCurrentToPrevious();

                        e.Handled = true;// true;
                    }
                    break;

                case Key.Left:

                    if (view.IsEmpty == false)
                    {
                        view.MoveCurrentToPrevious();
                        if (view.IsCurrentBeforeFirst)
                            view.MoveCurrentToNext();

                        e.Handled = true;//true;
                    }
                    break;


                case Key.Escape:
                case Key.Enter:
                    this.image1.Focus();
                    e.Handled = true;
                    break;


                case Key.F12:

                    if (view.IsEmpty == false)
                    {
                        view.MoveCurrentToFirst();
                        e.Handled = true;// true;
                    }
                    break;

            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //if (!IsWorking)
            //{
            _IMA.Dispose();
            _IMA = null;

            if (ShouldClose != null)
                ShouldClose(false);

            //IDisposable id = GenreCombo.ItemsSource as IDisposable;
            //if (id != null)
            //    id.Dispose();
            //}
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ////if (IsWorking)
            ////    return;

            //dem changes temp
            //_IMA.Error += ((o, ev) => ImportError(ev));

            //_IMA.Commit(false);
            //_IMA.Dispose();
            //_IMA = null;

            //if (ShouldClose != null)
            //    ShouldClose(true);
        }

        //internal void OnClosing()
        //{
        //    if (_IIF != null)
        //    {
        //        _IIF.Cancel();
        //        _IIF = null;
        //    }
        //}

        private void ImportError(ImportExportErrorEventArgs m)
        {
            WindowFactory.GetWindowFromImporterror(m, _Session).ShowDialog();
        }


        public void Dispose()
        {
            //FinalizeInternetSearch();

            if (_IMA != null)
            {
                _IMA.Images.CollectionChanged -= ImageListener;
                _IMA.Dispose();
                _IMA = null;
            }

            if (_AF != null)
            {
                _AF.Dispose();
                _AF = null;
            }

            GenreCombo.Dispose();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            IEnumerable<IModifiableTrack> tc = TracktoConsider(sender);

            if (tc == null)
                return;

            foreach (IModifiableTrack mt in tc)
            {
                mt.Name = System.IO.Path.GetFileNameWithoutExtension(mt.Path);
            }

        }

        private void listView1_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ICollectionView icv = CollectionViewSource.GetDefaultView(listView1.ItemsSource); ;
            if (icv == null)
                return;

            if (icv.SortDescriptions.Count == 3)
                return;

            using (var b = icv.DeferRefresh())
            {
                icv.SortDescriptions.Clear();
                icv.SortDescriptions.Add(new SortDescription("DiscNumber", ListSortDirection.Ascending));
                icv.SortDescriptions.Add(new SortDescription("TrackNumber", ListSortDirection.Ascending));
                icv.SortDescriptions.Add(new SortDescription("Path", ListSortDirection.Ascending));
            }
        }

        private void Remove_track_Number_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IModifiableTrack> tc = TracktoConsider(sender);

            if (tc == null)
                return;

            foreach (IModifiableTrack mt in tc)
            {
                StringTrackParser stp = new StringTrackParser(mt.Name, false);
                if (stp.FounSomething)
                {
                    mt.Name = stp.TrackName;
                    if ((mt.TrackNumber == 0) && (stp.TrackNumber != null))
                        mt.TrackNumber = (uint)stp.TrackNumber;
                }
            }
        }

        #region IDropTargetAdvisor

        private UIElement _DropTraget;
        UIElement IDropTargetAdvisor.TargetUI
        {
            get
            {
                return _DropTraget;
            }
            set
            {
                _DropTraget = value;
            }
        }

        bool IDropTargetAdvisor.ApplyMouseOffset
        {
            get { return true; }
        }

        bool IDropTargetAdvisor.IsValidDataObject(IDataObject obj)
        {
            return obj.GetDataPresent("ImageContent");
        }

        bool IDropTargetAdvisor.OnDropCompleted(IDataObject obj, System.Windows.Point dropPoint, object Originalsource)
        {
            FrameworkElement fe = Originalsource as FrameworkElement;
            ListBoxItem lbitarget = fe.FindAncestor<ListBoxItem>();
            if (lbitarget == null)
                return false;

            int newindex = this.DiscImages.ItemContainerGenerator.IndexFromContainer(lbitarget);

            ListBoxItem lbi = obj.GetData("ImageContent") as ListBoxItem;

            int oldindex = this.DiscImages.ItemContainerGenerator.IndexFromContainer(lbi);

            IAlbumPicture item = lbi.Content as IAlbumPicture;

            if (newindex != oldindex)
            {
                _IMA.Images.RemoveAt(oldindex);
                _IMA.Images.Insert(newindex, item);
                return true;
            }

            return false;
        }

        UIElement IDropTargetAdvisor.GetVisualFeedback(IDataObject obj)
        {

            UIElement elt = obj.GetData("RawUI") as UIElement;
            System.Windows.Shapes.Rectangle rect = elt.CreateSnapshot();
            rect.Opacity = 0.5;
            rect.IsHitTestVisible = false;
            return rect;
        }

        UIElement IDropTargetAdvisor.GetTopContainer()
        {
            return this.DiscImages;
        }

        #endregion

        #region IDragSourceAdvisor

        private UIElement _SourceDrag;
        UIElement IDragSourceAdvisor.SourceUI
        {
            get
            {
                return _SourceDrag;
            }
            set
            {
                _SourceDrag = value;
            }
        }

        DragDropEffects IDragSourceAdvisor.SupportedEffects
        {
            get { return DragDropEffects.Move; }
        }

        DataObject IDragSourceAdvisor.GetDataObject(UIElement draggedElt)
        {
            DataObject Do = new DataObject();
            Do.SetData("RawUI", draggedElt);

            ListBoxItem listViewItem = draggedElt.FindAncestor<ListBoxItem>();
            if (listViewItem != null)
            {
                Do.SetData("ImageContent", listViewItem);
            }


            return Do;
        }

        void IDragSourceAdvisor.FinishDrag(UIElement draggedElt, DragDropEffects finalEffects, bool DropOk)
        {
        }

        bool IDragSourceAdvisor.IsDraggable(UIElement dragElt)
        {
            if (dragElt == this.DiscImages)
                return false;

            ListBoxItem listViewItem = dragElt.FindAncestor<ListBoxItem>();
            if (listViewItem == null)
                return false;

            return true;
        }

        UIElement IDragSourceAdvisor.GetTopContainer()
        {
            return this.DiscImages;
        }

        #endregion

        //private SmartPanel2 _SP;
        //private void SmartPanel2_Loaded(object sender, RoutedEventArgs e)
        //{
        //    _SP = sender as SmartPanel2;
        //}

        private void MenuItem_SetNumber(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi == null)
                return;

            var res = mi.Header as TextBox;
            if (res == null)
                return;

            uint DN = 0;

            if (!uint.TryParse(res.Text, out DN))
                return;

            var trcs = TrackFromContext(mi.DataContext as IModifiableTrack);

            if (trcs == null)
                return;

            trcs.Apply(tr => tr.DiscNumber = DN);
        }


        private void PrefixeTrack(IModifiableTrack imt)
        {
            imt.Name = string.Format("{0}-{1}", imt.Father.Artists.GetDisplayName(), imt.Name);
        }

        private void PreFixByArtistName(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            var trcs = TrackFromContext(mi.DataContext as IModifiableTrack);

            if (trcs == null)
                return;

            trcs.Apply(PrefixeTrack);
        }

        private void MenuItem_MouseEnter(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi == null)
                return;

            //var intmi = mi.Items[0] as MenuItem;

            var res = mi.Header as UIElement;
            bool i = res.Focus();
            //Console.WriteLine(i);
        }
    }
}
