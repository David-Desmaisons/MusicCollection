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
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModel.Filter;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for FindItemsControl.xaml
    /// </summary>
    public partial class FindItemsControl : UserControl
    {
        private IEntityFinder<IAlbum> _AlbumFinder;
        private IEntityFinder<IArtist> _IArtistFinder;
        private IEntityFinder<ITrack> _TrackFinder;


        public FindItemsControl()
        {
            InitializeComponent();
            InitLists();
        }

        private static List<IAlbum> _EmptyAlbum = new List<IAlbum>();
        private static List<IArtist> _EmptyArtist = new List<IArtist>();
        private static List<ITrack> _EmptyTrack = new List<ITrack>();

        private void InitLists()
        {
            Albums = _EmptyAlbum;
            Artists = _EmptyArtist;
            Tracks = _EmptyTrack;
        }


        private FilterBuilder _FB=new FilterBuilder();
        public FilterBuilder FilterBuilder
        {
            get { return _FB; }
        }

        public IMusicSession Session
        {
            set
            {
                var e = value.EntityFinder;
                    //new EntityFinder(value);
                _AlbumFinder = e.AlbumFinder;
                _IArtistFinder = e.ArtistFinder;
                _TrackFinder = e.TrackFinder;
            }
        }

        public IList<IArtist> Artists
        {
            get { return (IList<IArtist>)GetValue(ArtistsProperty); }
            private set { SetValue(ArtistsProperty, value); }
        }

        public static readonly DependencyProperty ArtistsProperty = DependencyProperty.Register("Artists", typeof(IList<IArtist>), typeof(FindItemsControl), new PropertyMetadata(null));


        public IList<IAlbum> Albums
        {
            get { return (IList<IAlbum>)GetValue(AlbumsProperty); }
            private set { SetValue(AlbumsProperty, value); }
        }

        public static readonly DependencyProperty AlbumsProperty = DependencyProperty.Register("Albums", typeof(IList<IAlbum>), typeof(FindItemsControl),new PropertyMetadata(null));

        public IList<ITrack> Tracks
        {
            get { return (IList<ITrack>)GetValue(TracksProperty); }
            private set { SetValue(TracksProperty, value); }
        }

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register("Tracks", typeof(IList<ITrack>), typeof(FindItemsControl), new PropertyMetadata(null));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(FindItemsControl), new PropertyMetadata(false, IsOpenPropertyChangedCallback));


        static private void IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FindItemsControl @this = d as FindItemsControl;
            @this.IsOpenPropertyChanged(e);
        }

        private void IsOpenPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (_Automaticmode)
                {
                    string curr = this.Filter;
                    BindingOperations.ClearBinding(this, FilterProperty);
                    Filter = curr;
                    this._Automaticmode = false;
                }
            }
        }
    
        
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            private set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(FindItemsControl),new PropertyMetadata(string.Empty,FilterPropertyChangedCallback));

        static private void FilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FindItemsControl @this = d as FindItemsControl;
            @this.FilterPropertyChanged(e);
        }

        private bool _Automaticmode = false;

        private void UpdatePopUpInfo(string nv)
        {
            if ((string.IsNullOrEmpty(nv)) || (nv.Length < _AlbumFinder.MinimunLengthForSearch))
            {
                IsOpen = false;
                InitLists();
                FilterBuilder.FilterEntity = new MusicCollectionWPF.ViewModel.Filter.NoFilter();
                return;
            }

            IsOpen = true;

            Albums = _AlbumFinder.Search(nv).ToList();
            Artists = _IArtistFinder.Search(nv).Where(ar=>ar.Albums.Count>0).ToList();
            Tracks = _TrackFinder.Search(nv).ToList();
        }

        private void FilterPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_Automaticmode)
                return;

            UpdatePopUpInfo( e.NewValue as string);
        }

        public void UpDatePopUpInfo()
        {
            if (_Automaticmode)
                return;

            if (IsOpen == false)
                return;

            UpdatePopUpInfo(Filter);
        }


        private void Root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Session = e.NewValue as IMusicSession;
        }

    

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChangedUIOnly(string PropertyName)
        {
            PropertyChangedEventHandler pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(PropertyName));
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Commit();
                    break;

                case Key.Escape:
                    Reset();
                    break;
            }
        }

        public void Commit()
        {
            if (FilterBuilder.FilterObject == null)
            {
                if (Albums == null)
                    return;

                if (Albums.Count + Artists.Count + Tracks.Count != 1)
                    return;

                FilterBuilder.FilterObject = this.Albums.Cast<object>().Concat(this.Artists).Concat(this.Tracks).FirstOrDefault();
            }



            Binding binding = new Binding();
            binding.Path = new PropertyPath("FilterBuilder.FilterEntity.DisplayName");
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(FilterProperty, binding);
            _Automaticmode = true;

            //this.Filter = FilterBuilder.FilterEntity.DisplayName;
            this.IsOpen = false; 
        }

        public void Reset()
        {
            _Automaticmode = false;
            BindingOperations.ClearBinding(this, FilterProperty);
            this.Filter = string.Empty;
            IsOpen = false;
        }

        private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount==2)
                Commit();
        }

    }


  
}
