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
using System.Threading;


namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for TracksDisplayer.xaml
    /// </summary>
    public partial class TracksDisplayer : UserControl, IDisposable
    {
        #region GridPersistence

        public static readonly DependencyProperty GridPersistenceProperty = DependencyProperty.Register("GridPersistence",
             typeof(IPersistGrid), typeof(TracksDisplayer), new PropertyMetadata(null, GridPersistenceChanged));

        public IPersistGrid GridPersistence
        {
             get { return (IPersistGrid)GetValue(GridPersistenceProperty); }
             set { SetValue(GridPersistenceProperty, value); }
        }

        private static void GridPersistenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var td = d as TracksDisplayer;
            td.UpdateGrid();
        }

        private void UpdateGrid()
        {
            GridPersistence.FromPersistance(dataGrid1.Columns);
        }

        public void Dispose()
        {
            GridPersistence.PersistChange(dataGrid1.Columns);
        }

        #endregion

        public static readonly DependencyProperty TrackStatusLoaderProperty = DependencyProperty.Register("TrackStatusLoader",
           typeof(IAsyncLoad), typeof(TracksDisplayer), new PropertyMetadata(null));

        public IAsyncLoad TrackStatusLoader
        {
            get { return (IAsyncLoad)GetValue(TrackStatusLoaderProperty); }
            set { SetValue(TrackStatusLoaderProperty, value); }
        }

        public TracksDisplayer()
        {
            InitializeComponent();
        }

        #region sorting

        private ListCollectionView LCV
        {
            get
            {
                return CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource) as ListCollectionView;
            }
        }

        private CancellationTokenSource _CTS = null; 


        private async void dataGrid1_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ?
                                     ListSortDirection.Ascending : ListSortDirection.Descending;

            e.Column.SortDirection = direction;

            IComparer mySort = null;

            if (_CTS!=null)
            {
                _CTS.Cancel();
                _CTS=null;
            }

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
                mySort = new BrokenSorter(direction);

                _CTS = new CancellationTokenSource();
                e.Handled = true;
                bool sucess = await TrackStatusLoader.LoadAsync(_CTS.Token);
                if (!sucess)
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
                return;
            }
        }

        #region Sorters

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
            public NameSorter(ListSortDirection s) : base(s)
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
            public RatingSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.Rating.CompareTo(ta.Rating);
            }
        }

        private class GenreSorter : Sorter
        {
            public GenreSorter(ListSortDirection s) : base(s)
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
            public AlbumNameSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumName.CompareTo(ta.AlbumName);
            }
        }

        private class TrackSorter : Sorter
        {
            public TrackSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.TrackNumber.CompareTo(ta.TrackNumber);
            }
        }

        private class PlayCountSorter : Sorter
        {
            public PlayCountSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.PlayCount.CompareTo(ta.PlayCount);
            }
        }

        private class ArtistNameSorter : Sorter
        {
            public ArtistNameSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumAuthor.CompareTo(ta.Album.Author);
            }
        }

        private class YearSorter : Sorter
        {
            public YearSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.AlbumYear.CompareTo(ta.AlbumYear);
            }
        }


        private class DiscNumberSorter : Sorter
        {
            public DiscNumberSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.DiscNumber.CompareTo(ta.DiscNumber);
            }
        }

        private class SkippedCountSorter : Sorter
        {
            public SkippedCountSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.SkippedCount.CompareTo(ta.SkippedCount);
            }
        }

        private class BrokenSorter : Sorter
        {
            public BrokenSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.FileExists.CompareTo(ta.FileExists);
            }
        }

        private class PathSorter : Sorter
        {
            public PathSorter(ListSortDirection s) : base(s)
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
            public DurationSorter(ListSortDirection s) : base(s)
            {
            }

            protected override int Comparer(TrackView at, TrackView ta)
            {
                return at.Duration.CompareTo(ta.Duration);
            }
        }

        private abstract class DateTimeSorter : Sorter
        {
            public DateTimeSorter(ListSortDirection s) : base(s)
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
            public LastPLayedSorter(ListSortDirection s) : base(s)
            {
            }

            protected override DateTime? DTAcessor(TrackView tr)
            {
                return tr.LastPLayed;
            }
        }

        private class DateAddedSorter : DateTimeSorter
        {
            public DateAddedSorter(ListSortDirection s) : base(s)
            {
            }

            protected override DateTime? DTAcessor(TrackView tr)
            {
                if (tr.Album == null)
                    return null;

                return tr.Album.DateAdded;
            }
        }

        #endregion

        private void RefreshFilter()
        {
            LCV.Refresh();
        }

        #endregion

       
    }
}
