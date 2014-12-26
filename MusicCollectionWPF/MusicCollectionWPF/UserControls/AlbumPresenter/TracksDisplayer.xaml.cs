using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.Utilies;

using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.UserControls.AlbumPresenter;
using MusicCollectionWPF.Infra;


namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for TracksDisplayer.xaml
    /// </summary>
    public partial class TracksDisplayer : UserControl, IDisposable
    {
        //public static readonly DependencyProperty TracksProperty = DependencyProperty.Register("Tracks",
        //typeof(IList<ITrack>), typeof(TracksDisplayer), new PropertyMetadata(null));

        //public IList<ITrack> Tracks
        //{
        //    get { return (IList<ITrack>)GetValue(TracksProperty); }
        //    set { SetValue(TracksProperty, value); }
        //}


        public TracksDisplayer()
        {
            InitializeComponent();
        }


        private IMusicSession _Session;

        private bool _IsCollectionInit = false;
        private void CollectionInit()
        {
            if (_IsCollectionInit)
                return;

            if (LCV == null)
                return;

            if (_Session == null)
                return;

            LCV.Filter = FilterTrackView;

            _IsCollectionInit = true;
        }

        private IMusicSession Session
        {
            get
            {
                return _Session;
            }
            set
            {
                if (value == null)
                    return;

                _Session = value;
                _Session.Setting.GetIUIGridManagement().Default.FromPersistance(dataGrid1.Columns);

                CollectionInit();
            }
        }

        private bool FilterTrackView(object tv)
        {
            TrackView tvv = tv as TrackView;
            if (tvv == null)
                return false;

            //return tvv.IsTrackFiltered(_Session.AlbumFilter.TrackFilter); OldFilter
            return true;
        }

        private ListCollectionView LCV
        {
            get
            {
                return CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource) as ListCollectionView;
            }
        }


        private void dataGrid1_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ?
                                     ListSortDirection.Ascending : ListSortDirection.Descending;

            e.Column.SortDirection = direction;

            IComparer mySort = null;

            object CN = e.Column;

            if (object.ReferenceEquals(CN, NameC))
            {
                mySort = new NameSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Rating))
            {
                mySort = new RatingSorter(direction);
            }
            else if (object.ReferenceEquals(CN, TrackNumber))
            {
                mySort = new TrackSorter(direction);
            }
            else if (object.ReferenceEquals(CN, PlayCount))
            {
                mySort = new PlayCountSorter(direction);
            }
            else if (object.ReferenceEquals(CN, LastPLayed))
            {
                mySort = new LastPLayedSorter(direction);
            }
            else if (object.ReferenceEquals(CN, DateAdded))
            {
                mySort = new DateAddedSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Genre))
            {
                mySort = new GenreSorter(direction);
            }
            else if (object.ReferenceEquals(CN, DiscNumber))
            {
                mySort = new DiscNumberSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Name))
            {
                mySort = new AlbumNameSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Album))
            {
                mySort = new AlbumNameSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Artist))
            {
                mySort = new ArtistNameSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Year))
            {
                mySort = new YearSorter(direction);
            }
            else if (object.ReferenceEquals(CN, SkippedCount))
            {
                mySort = new SkippedCountSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Duration))
            {
                mySort = new DurationSorter(direction);
            }
            else if (object.ReferenceEquals(CN, Broken))
            {
                if (_TFL != null)
                    return;

                _TFL = new TrackFileStatusLoader(_Session);
                _TFL.StatusLoaded += OnBrokenSorterReady;
                _TFL.Load(false);
                e.Handled = true;

                return;
            }
            else if (object.ReferenceEquals(CN, Path))
            {
                mySort = new PathSorter(direction);
            }

            if (mySort != null)
            {
                LCV.CustomSort = mySort;
                e.Handled = true;
                if (_TFL != null)
                {
                    CleanSorterPrepare();
                }
                return;
            }

        }

        private TrackFileStatusLoader _TFL;

        private void CleanSorterPrepare()
        {
            _TFL.StatusLoaded -= OnBrokenSorterReady;
            _TFL = null;
        }

        private void OnBrokenSorterReady(object sender, EventArgs eh)
        {
            Action ac = OnBrokenSorterReadyST;
            this.Dispatcher.BeginInvoke(ac, null);
        }

        private void OnBrokenSorterReadyST()
        {
            ListSortDirection direction = (Broken.SortDirection != ListSortDirection.Ascending) ?
                             ListSortDirection.Ascending : ListSortDirection.Descending;

            LCV.CustomSort = new BrokenSorter(direction);

            CleanSorterPrepare();
        }

        private abstract class Sorter : IComparer
        {
            protected ListSortDirection _Direc;

            protected Sorter(ListSortDirection s)
            {
                _Direc = s;
            }

            protected abstract int Comparer(TrackView at, TrackView ta);

            private int CompareAbs(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                TrackView xx = x as TrackView;
                TrackView yy = y as TrackView;

                if ((xx == null) || (yy == null))
                    return 0;

                return Comparer(xx, yy);
            }

            public int Compare(object x, object y)
            {
                return (_Direc == ListSortDirection.Ascending) ? CompareAbs(x, y) : -CompareAbs(x, y);
            }


        }

        private class NameSorter : Sorter
        {
            public NameSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                if (at.Name == null)
                    return string.Empty.CompareTo(ta.Name);

                return at.Name.CompareTo(ta.Name);
            }
        }

        private class RatingSorter : Sorter
        {
            public RatingSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.Rating.CompareTo(ta.Rating);
            }
        }

        private class GenreSorter : Sorter
        {
            public GenreSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                string cp = at.AlbumGenre ?? string.Empty;
                return cp.CompareTo(ta.AlbumGenre);
            }
        }

        private class AlbumNameSorter : Sorter
        {
            public AlbumNameSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumName.CompareTo(ta.AlbumName);
            }
        }




        private class TrackSorter : Sorter
        {
            public TrackSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.TrackNumber.CompareTo(ta.TrackNumber);
            }
        }

        private class PlayCountSorter : Sorter
        {
            public PlayCountSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.PlayCount.CompareTo(ta.PlayCount);
            }
        }

        private class ArtistNameSorter : Sorter
        {
            public ArtistNameSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumAuthor.CompareTo(ta.Album.Author);
            }
        }

        private class YearSorter : Sorter
        {
            public YearSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumYear.CompareTo(ta.AlbumYear);
            }
        }



        private class DiscNumberSorter : Sorter
        {
            public DiscNumberSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.DiscNumber.CompareTo(ta.DiscNumber);
            }
        }

        private class SkippedCountSorter : Sorter
        {
            public SkippedCountSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.SkippedCount.CompareTo(ta.SkippedCount);
            }
        }



        private class BrokenSorter : Sorter
        {
            public BrokenSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {

                return at.FileExists.CompareTo(ta.FileExists);
            }
        }

        private class PathSorter : Sorter
        {
            public PathSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                string p = at.Path ?? string.Empty;
                return p.CompareTo(ta.Path);
            }
        }

        private class DurationSorter : Sorter
        {
            public DurationSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.Duration.CompareTo(ta.Duration);
            }
        }



        private abstract class DateTimeSorter : Sorter
        {
            public DateTimeSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected abstract DateTime? DTAcessor(TrackView tr);

            private int SignedDiff
            {
                get
                {
                    return (_Direc == ListSortDirection.Ascending) ? 1 : -1;
                }
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                if (DTAcessor(at) == null)
                {
                    return (DTAcessor(ta) == null) ? 0 : SignedDiff;
                }

                if (DTAcessor(ta) == null)
                    return -SignedDiff;


                DateTime dt = (DateTime)DTAcessor(at);
                return dt.CompareTo((DateTime)DTAcessor(ta));

            }

        }




        private class LastPLayedSorter : DateTimeSorter
        {
            public LastPLayedSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override DateTime? DTAcessor(TrackView tr)
            {
                return tr.LastPLayed;
            }
        }

        private class DateAddedSorter : DateTimeSorter
        {
            public DateAddedSorter(ListSortDirection s)
                : base(s)
            {
            }

            protected override DateTime? DTAcessor(TrackView tr)
            {
                if (tr.Album == null)
                    return null;

                return tr.Album.DateAdded;
            }
        }

        private bool _NeedRefreshFilter = false;
        private void RefreshFilter()
        {
            if (_UnderEdit)
            {
                _NeedRefreshFilter = true;
                return;
            }

            LCV.Refresh();
            _NeedRefreshFilter = false;
        }


        private void dataGrid1_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ListCollectionView res = CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource) as ListCollectionView;
            if (res == null)
                return;

            CollectionInit();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem me = sender as MenuItem;
            me.IsChecked = !me.IsChecked;

        }


        public void Dispose()
        {
            _Session.Setting.GetIUIGridManagement().Default.PersistChange(dataGrid1.Columns);
            //_Session.Setting.GridManagement.Default.PersistChange(dataGrid1.Columns);

            //IDisposable id = dataGrid1.ItemsSource as IDisposable;

            //if (id != null)
            //{
            //    id.Dispose();
            //}
        }

        #region IAlbumPresenter

        public IEnumerable<IAlbum> SelectedAlbums
        {
            get
            {
                return dataGrid1.SelectedItems.Cast<TrackView>().Select(tv => tv.Album).Distinct();
            }
            set
            {
                //dataGrid1.SelectedItems.Clear();

                dataGrid1.SelectedItems.Clear();
                value.Apply(a => a.Tracks.Apply(tr => dataGrid1.SelectedItems.Add(TrackView.GetTrackView(tr))));
            }
        }

        private bool _UnderEdit = false;

        public void EditEntity(IEnumerable<IMusicObject> al)
        {
            _UnderEdit = true;

        //    IEditableCollectionView iec = LCV as IEditableCollectionView;

        //    al.ConvertToTracks().ToList().Apply(tr => iec.EditItem(TrackView.GetTrackView(tr)));
        }

        public void Remove(IEnumerable<IMusicObject> al)
        {
            ////to be implemented
            ////IEnumerable<ITrack> tracks = al.ConvertToTracks();

            //ListCollectionView lcv = LCV;

            ////IEditableCollectionView iec = LCV as IEditableCollectionView;

            //al.ConvertToTracks().ToList().Apply(tr => lcv.Remove(TrackView.GetTrackView(tr)));
        }


        public void CancelEdit()
        {
            FinalizeEdit();
        }

        public void EndEdit()
        {
            FinalizeEdit();
        }

        private void FinalizeEdit()
        {
            //(LCV as IEditableCollectionView).CommitEdit();
            _UnderEdit = false;
            if (_NeedRefreshFilter)
            {
                this.RefreshFilter();
            }

        }

        public IEnumerable<TrackView> GetSelectedTRackViewEntities(IObjectAttribute Context)
        {
            TrackView tr = Context as TrackView;
            if (tr == null)
                return null;

            if (!dataGrid1.SelectedItems.Contains(tr))
                return tr.SingleItemCollection().ToList();

            return dataGrid1.SelectedItems.Cast<TrackView>().ToList();
        }

        public IEnumerable<IObjectAttribute> GetSelectedEntities(IObjectAttribute Context)
        {
            var trackviewre = GetSelectedTRackViewEntities(Context);
            if (trackviewre == null)
                return null;

            return trackviewre.Select(trv => trv.Track);
        }

        public bool IsCommandAllowed(ICommand command)
        {
            if (command == null)
                return false;

            if (command == ApplicationCommands.Save)
                return false;

            if (command == ApplicationCommands.SaveAs)
                return false;


            return true;
        }

        #endregion

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Session = e.NewValue as IMusicSession;
        }

        private void HeaderMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            DataGridColumnHeader fg = sender as DataGridColumnHeader;

            fg.ContextMenu.ItemsSource = this.dataGrid1.Columns;
        }

        private IEnumerable<TrackView> GetTrackViews(ExecutedRoutedEventArgs e)
        {
            var trcs = e.Parameter as IEnumerable<IObjectAttribute>;
            if (trcs == null)
                return null;

            return trcs.Cast<ITrack>().Select(t => TrackView.GetTrackView(t));
        }

        private void RemoveTrackNumber(object sender, ExecutedRoutedEventArgs e)
        {
            GetTrackViews(e).Apply(tv=>tv.RemoveTrackNumber());
        }

        private void PrefixArtistName(object sender, ExecutedRoutedEventArgs e)
        {
            GetTrackViews(e).Apply(tv => tv.PrefixArtistName());
        }

    }




}
