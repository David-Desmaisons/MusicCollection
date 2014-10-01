using System;
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
using System.Collections.ObjectModel;
//using System.Timers;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumPlayer.xaml
    /// </summary>
    public partial class AlbumPlayer : UserControl, IEditListener
    {
        private CollectionViewSource _ViewAl = null;
        private DispatcherTimer _Timer2;
        private IMusicPlayer _MusicPlayer;
        private IAlbumPlayList _PlayList;
        private object _EventLocker = new object();

        public event EventHandler<EventArgs> NeedToClose;

        private IMusicSession _Session;

        public IMusicSession Session { get { return _Session; } set { _Session = value; InitMusicPlayer(); } }

        public ICollectionView PlayingAlbums
        {
            get { return _ViewAl.View; }
        }

        public AlbumPlayer(IMusicSession ims)
        {
            Session = ims;

            InitializeComponent();        
            
            Albums.CollectionChanged += new NotifyCollectionChangedEventHandler(Albums_CollectionChanged);

            _ViewAl = new CollectionViewSource() { Source = Albums };

            DataContext = this;

   
            _ViewAl.View.CurrentChanging += (o, ev) =>
            {

                ICollectionView view = ImageCollectionView();
                if (view == null)
                    return;

                view.CollectionChanged -= ImageChanged;
            };

            _ViewAl.View.CurrentChanged += (o, ev) =>
            {

                ICollectionView view = ImageCollectionView();
                if (view == null)
                    return;

                view.CollectionChanged += ImageChanged;
            };

            _Timer2 = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(15000)
            };

            _Timer2.Tick += ChangeAlbumViewIfNeeded;
            _Timer2.Start();
        }

        void Albums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Albums.Count == 0)
            {
                EventHandler<EventArgs> ntc = NeedToClose;
                if (ntc != null)
                    ntc(this, null);
            } 
        }


        private void InitMusicPlayer()
        {
            _MusicPlayer = _Session.MusicPlayer;
            _MusicPlayer.TrackEvent += OnTrackEvent;
            _MusicPlayer.TrackPlaying += TrackPlaying;

            _PlayList = _Session.PlayListFactory.CreateAlbumPlayList("Memory PlayList");

            _MusicPlayer.PlayList = _PlayList;

            _PlayList.ObjectChanged += new EventHandler<ObjectModifiedArgs>(_PlayList_ObjectChanged);
        }

        void _PlayList_ObjectChanged(object sender, ObjectModifiedArgs e)
        {
            if (e.AttributeName != "CurrentAlbumItem")
                return;

            if (e.NewAttributeValue == null)
                return;

            if (_ViewAl.View.CurrentItem != e.OldAttributeValue)
                return;

            MoveAlbumToPosition(e.NewAttributeValue as IAlbum);
        }


        private void ImageChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ICollectionView view = ImageCollectionView();
            if (view == null)
                return;

            if ((view.Cast<IAlbumPicture>().ToList().Count > 0) && (view.CurrentPosition == -1))
                view.MoveCurrentToFirst();
        }

        private void OnTrackEvent(object sender, MusicTrackEventArgs TrackEvent)
        {
            switch (TrackEvent.What)
            {
                case TrackPlayingEvent.Loading:
                    double ExpectedMax = TrackEvent.Track.Duration.TotalMilliseconds;
                    slider1.Maximum = ExpectedMax;
                    slider1.Value = 0;
                    UpdateTimeDisplay(0, ExpectedMax);
                    break;

                case TrackPlayingEvent.BeginPlay:
                    slider1.Maximum = _MusicPlayer.MaxPosition.TotalMilliseconds;
                    slider1.Value = _MusicPlayer.Position.TotalMilliseconds;
                    UpdateTimeDisplay(slider1.Value, _MusicPlayer.MaxPosition.TotalMilliseconds);
                    break;

                case TrackPlayingEvent.EndPlay:
                    slider1.Value = 0;
                    UpdateTimeDisplay(0, _MusicPlayer.MaxPosition.TotalMilliseconds);
                    break;

                case TrackPlayingEvent.Broken:
                    Trace.WriteLine("Media failed");
                    slider1.Value = 0;
                    UpdateTimeDisplay(0,null);
                    break;
            }
        }


        private void UpdateTimeDisplay(double value, double? Max)
        {
            if (Max.HasValue && TimeSpan.FromMilliseconds(Max.Value).TotalSeconds==1)
            {
                Trace.WriteLine("Potencial problem");
            }
            string mvalue = ((Max.HasValue) && (Max.Value!=0)) ? TimeFormater.Convert(Max.Value) : "--";
            Timer.Text = string.Format("{0}/{1}", TimeFormater.Convert(value), mvalue);
        }


        private void TrackPlaying(object sender, MusicTrackPlayingEventArgs TrackEvent)
        {
            UpdateTimeDisplay(TrackEvent.Position.TotalMilliseconds, TrackEvent.MaxPosition.TotalMilliseconds);

            if (slider1.InDrag)
            {
                return;
            }

            //if (_Indrag)
            //{
            //    return;
            //}
           
            slider1.Value = TrackEvent.Position.TotalMilliseconds;       
        }

        internal IObservableCollection<IAlbum> Albums
        {
            get
            {
                return _PlayList.Albums;
            }
        }


        private void MoveAlbumToFirst()
        {
            _ViewAl.View.MoveCurrentToFirst();
        }

        private void MoveAlbumToLast()
        {
            _ViewAl.View.MoveCurrentToLast();
        }

        private void MoveAlbumToPrevious()
        {
            if ((_ViewAl.View.CurrentPosition == 0) || (_ViewAl.View.CurrentPosition == -1))
                return;

            using (this.TrackTransition.GetTransitionner().Merge(this.TransitionGrid.GetTransitionner()))
            {
                _ViewAl.View.MoveCurrentToPrevious();
            }
        }

        private void MoveAlbumToNext()
        {
            ListCollectionView lcv = _ViewAl.View as ListCollectionView;

            int MaxPos = lcv.Count;

            if (MaxPos == 0)
                return;

            if (_ViewAl.View.CurrentPosition == MaxPos - 1)
                return;

            using (this.TrackTransition.GetTransitionner().Merge(this.TransitionGrid.GetTransitionner()))
            {
                _ViewAl.View.MoveCurrentToNext();
            }

        }

        private void MoveAlbumToPosition(IAlbum Position, bool needtranstion=true)
        {
            if (object.ReferenceEquals(_ViewAl.View.CurrentItem,Position))
                return;

            if (!needtranstion)
            {
                _ViewAl.View.MoveCurrentTo(Position);
                return;
            }

            using (this.TrackTransition.GetTransitionner().Merge(this.TransitionGrid.GetTransitionner()))
            {
                _ViewAl.View.MoveCurrentTo(Position);
            }
        }

        private void AddAlbum(IEnumerable<IAlbum> Al)
        {
            Al.Apply(al=>_PlayList.AddAlbum(al));
        }


        internal void AddAlbumAndPlay(IEnumerable<IAlbum> Al)
        {
            bool emptybefore = _PlayList.Albums.Count == 0;
             AddAlbum(Al);

            IAlbum last = Al.First();
            MoveAlbumToPosition(last, !emptybefore);
            _PlayList.CurrentAlbumItem =last;

            _MusicPlayer.Mode = PlayMode.Play;
        }

        internal void AddAlbumAndPlay(IEnumerable<ITrack> trec)
        {
            bool emptybefore = _PlayList.Albums.Count == 0;
            var tr = trec.Distinct();
            var res = tr.Select(tra => tra.Album).Distinct();
            AddAlbum(res);

            var trlast = tr.First();
            var lastalb = trlast.Album;

            MoveAlbumToPosition(lastalb, !emptybefore);

            _PlayList.CurrentTrack = trlast;

            _MusicPlayer.Mode = PlayMode.Play;
        }

        internal bool IsPlaying
        {
            get { return _MusicPlayer.Mode == PlayMode.Play; }
        }

        public AlbumPlayer()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
            }
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            _MusicPlayer.Position = TimeSpan.FromMilliseconds(slider1.Value);
            UpdateTimeDisplay(slider1.Value, _MusicPlayer.MaxPosition.TotalMilliseconds);
        }

        private ICollectionView ImageCollectionView()
        {
            IAlbum al = _ViewAl.View.CurrentItem as IAlbum;

            return (al == null) ? null : CollectionViewSource.GetDefaultView(al.Images);
        }

        internal void RootGrid_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            ICollectionView view = ImageCollectionView();

            ListCollectionView lcv = view as ListCollectionView;
            

            int cur = lcv.CurrentPosition;

            switch (e.Key)
            {
                case Key.Right:

                    if (lcv.Count < 2)
                        return;

                    if (cur != lcv.Count - 1)
                    {
                        using (TransitionGrid.GetTransitionner())
                        {
                            view.MoveCurrentToNext();
                        }
                    }

                    break;

                case Key.Left:

                    if (lcv.Count < 2)
                        return;

                    if (cur != 0)
                    {
                        using (TransitionGrid.GetTransitionner())
                        {
                            view.MoveCurrentToPrevious();
                        }
                    }

                    break;


                case Key.F12:

                    if (lcv.Count < 2)
                        return;

                    if (cur != 0)
                    {
                        using (TransitionGrid.GetTransitionner())
                        {
                            view.MoveCurrentToFirst();
                        }
                    }

                    break;


                case Key.Up:
                    this.Session.MusicPlayer.Volume += 0.01;
                    break;

                case Key.Down:
                    this.Session.MusicPlayer.Volume -= 0.01;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tracks.SelectedIndex = 0;
        }


        public void OnEnd()
        {
            _MusicPlayer.Mode = PlayMode.Stopped;
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        void ChangeAlbumViewIfNeeded(object s, EventArgs ea)
        {
            if ((_ViewAl == null) || (_ViewAl.View == null))
                return;

            if ((!object.ReferenceEquals(_ViewAl.View.CurrentItem, _PlayList.CurrentAlbumItem)) && (_PlayList.CurrentAlbumItem != null))
                MoveAlbumToPosition(_PlayList.CurrentAlbumItem);
        }

        private void PreviousAlbum(object sender, RoutedEventArgs e)
        {
            MoveAlbumToPrevious();
        }


        private void NextAlbum(object sender, RoutedEventArgs e)
        {
            MoveAlbumToNext();
        }

        private void CurrentAlbum(object sender, RoutedEventArgs e)
        {
            var al = _Session.MusicPlayer.AlbumPlayList.CurrentAlbumItem;
            if (al == null)
                return;

            MoveAlbumToPosition(al);
        }

        private void OnAlbumPlay(object sender, RoutedEventArgs e)
        {

            var cual = _Session.MusicPlayer.AlbumPlayList.CurrentAlbumItem;

            if (object.ReferenceEquals(_ViewAl.View.CurrentItem,cual))
                return;

            this.Album.Checked -= OnAlbumPlay;

            _PlayList.CurrentAlbumItem = _ViewAl.View.CurrentItem as IAlbum;

            this.Album.Checked += OnAlbumPlay;
        }

        private void Window_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _Timer2.Stop();
            _Timer2.Start();
        }

        private ICollectionView TracksView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(Tracks.ItemsSource);
            }
        }

        private void InitTracksDisplay()
        {
            if (Tracks.ItemsSource == null)
                return;

            var res = Tracks.ItemsSource.Cast<ITrack>();
            bool multial = res.Select(tr => tr.DiscNumber).Distinct().Count() > 1;

            ICollectionView view = TracksView;
            view.SortDescriptions.Add(new SortDescription("DiscNumber", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("TrackNumber", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Path", ListSortDirection.Ascending));

            if (!multial)
                return;

            PropertyGroupDescription pgd = new PropertyGroupDescription("DiscNumber");
            view.GroupDescriptions.Add(pgd);

        }

        private void Tracks_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ICollectionView icv = TracksView;
            if (icv == null)
                return;

            using (var b = icv.DeferRefresh())
            {
                icv.GroupDescriptions.Clear();
                icv.SortDescriptions.Clear();
                InitTracksDisplay();
            }
        }


        private IList<IAlbum> _UnderEdit;
        public void EditEntity(IEnumerable<IMusicObject> al)
        {
            _UnderEdit = al.ConvertToAlbums().ToList();
        }

        public void Remove(IEnumerable<IMusicObject> al)
        {
        }

        public void CancelEdit()
        {
            _UnderEdit = null;
        }

        public void EndEdit()
        {
            if (_UnderEdit == null)
                return;

            if (_UnderEdit.Contains(_ViewAl.View.CurrentItem))
            {
                ICollectionView tr = TracksView;
                using (var b = tr.DeferRefresh())
                {
                    tr.GroupDescriptions.Clear();
                    tr.SortDescriptions.Clear();
                    InitTracksDisplay();
                }
            }

            _UnderEdit = null;

        }
    }
}
