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
using System.Windows.Controls.Primitives;// {System.Windows.Controls.ListBox}


using System.Timers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.UserControls.AlbumPresenter;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumBrowser.xaml
    /// </summary>
    public partial class AlbumBrowserI : UserControl, INotifyPropertyChanged, IEditListener
        , IDisposable
    {
        private IMusicSession _Session;
 
        internal string Status
        {
            set 
            {
                statustext.AddMessage(value);
                //statustext.Content = value; 
            }
        }

        public AlbumBrowserI()
        {
            InitializeComponent();
        }

        public AlbumBrowserI(IMusicSession ims)
        {
            _Session = ims;

            InitializeComponent();

        }

        public static readonly DependencyProperty FilterViewProperty = DependencyProperty.Register("FilterView",
         typeof(FilterView), typeof(AlbumBrowserI), new PropertyMetadata(null));

        public FilterView FilterView
        {
            get { return (FilterView)GetValue(FilterViewProperty); }
            set { SetValue(FilterViewProperty, value); }
        }

        public static readonly DependencyProperty AlbumsProperty = DependencyProperty.Register("Albums",
         typeof(IList<IAlbum>), typeof(AlbumBrowserI), new PropertyMetadata(null));

        public IList<IAlbum> Albums
        {
            get { return (IList<IAlbum>)GetValue(AlbumsProperty); }
            set { SetValue(AlbumsProperty, value); }
        }

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register("Tracks",
        typeof(IList<ITrack>), typeof(AlbumBrowserI), new PropertyMetadata(null));

        public IList<ITrack> Tracks
        {
            get { return (IList<ITrack>)GetValue(TracksProperty); }
            set { SetValue(TracksProperty, value); }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             DataContext = _Session;

             Finder.FilterBuilder.PropertyChanged += this.Finder_PropertyChanged;
        }

        private void DisplayError(ImportExportErrorEventArgs Ev)
        {
            WindowFactory.GetWindowFromImporterror(Ev,_Session).ShowDialog();
        }


        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;

            if (lbi.IsSelected)
            {
                e.Handled = true;
                return;
            }

             lbi.IsSelected = true;

            e.Handled = true;

        }

        public bool InEdit
        {
            get { return this.Finder.IsOpen == false; }
        }



        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _Session = e.NewValue as IMusicSession;
            if (_Session == null)
                return;

            Settings = _Session.Setting;
            FilterView = new FilterView(_Session);
            Albums = _Session.AllAlbums.LiveWhere(FilterView.FilterAlbum);
            Tracks = _Session.AllTracks.LiveWhere(FilterView.FilterTrack);
        }

        private IMusicSettings _Settings;
        public IMusicSettings Settings
        {
            get { return _Settings; }
            set { _Settings = value; PropertyHasChanged("Settings"); UpdatePresenter();}
                //if (_Settings != null) _Settings.PropertyChanged += SettingsChanged; }
        }

        private void SettingsChanged(object sender, EventArgs pce)
        {
            //if (pce.PropertyName != "PresenterMode")
            //    return;

            UpdatePresenter();
        }

        private void UpdatePresenter()
        {
            if (_Settings == null)
                return;

            IAlbumPresenter next = this.FindName(_Settings.AparencyUserSettings.PresenterMode.ToString()) as IAlbumPresenter;

            IAlbumPresenter old = transitionContainer.Current as IAlbumPresenter;

            if (old == next)
                return;

            transitionContainer.Current = next as UIElement;

            if (old != null)
                next.SelectedAlbums = old.SelectedAlbums;  
        }

        private IAlbumPresenter CurrentPresenter
        {
            get
            {
                return transitionContainer.Current as IAlbumPresenter;
            }
        }

        private IEnumerable<IAlbumPresenter> Presenters
        {
            get
            {
                return transitionContainer.Children.Cast<IAlbumPresenter>();
            }
        }

        public void Dispose()
        {
            //if (_Settings != null)
            //    _Settings.PropertyChanged -= SettingsChanged;

            Presenters.Apply(p => p.Dispose());
        }

        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        public void EditEntity(IEnumerable<IMusicObject> al)
        {
            Presenters.Apply(p=> p.EditEntity(al));
        }


        public void Remove(IEnumerable<IMusicObject> al)
        {
            Finder.UpDatePopUpInfo();
            //Presenters.Apply(p => p.Remove(al));
        }

        public void EndEdit()
        {
            Finder.UpDatePopUpInfo();
            Presenters.Apply(p=> p.EndEdit());
        }

        public void CancelEdit()
        {
            Presenters.Apply(p => p.CancelEdit());
        }

      

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Escape:
                    Finder.Reset();
                    break;

                case Key.Enter:
                    Finder.Commit();
                    break;
            }
        }

        private void Finder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "FilterEntity")
                return;

            FilterView.FilteringObject = Finder.FilterBuilder.FilterEntity;

        }

        private void searcher_Click(object sender, RoutedEventArgs e)
        {
            Finder.Reset();
        }

        private void Search_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
             if (!string.IsNullOrEmpty(Search.Text))
                 Finder.IsOpen = true;
             else
                 Finder.Reset();
        }
      
    }
}
