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
using System.Windows.Shapes;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for AlbumContinueCancelWindow.xaml
    /// </summary>
    public partial class AlbumContinueCancelWindow : CustomWindow
    {
        public AlbumContinueCancelWindow()
        {
            InitializeComponent();
            Success = null;
        }

        public AlbumContinueCancelWindow(ToogleModelAlbum iToogleModelAlbum)
        {
          
            InitializeComponent();
            ToogleModelAlbum = iToogleModelAlbum;
            Success = null;
        }


        private ToogleModelAlbum _ToogleModelAlbum;
        public ToogleModelAlbum ToogleModelAlbum
        {
            get { return _ToogleModelAlbum; }
            set
            {
                _ToogleModelAlbum = value;
                DataContext = _ToogleModelAlbum;
                _ToogleModelAlbum.SelectedAlbums.Apply(al=>this.AlbumSelector.SelectedItems.Add(al));
                this.AlbumSelector.SelectionChanged += AlbumSelector_SelectionChanged;
            }
        }

        public bool? Success
        {
            get;
            private set;
        }

        private void AlbumSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (IMusicObject item in e.RemovedItems)
            {
                ToogleModelAlbum.SelectedAlbums.Remove(item);
            }

            foreach (IMusicObject item in e.AddedItems)
            {
                if (!ToogleModelAlbum.SelectedAlbums.Contains(item))
                    ToogleModelAlbum.SelectedAlbums.Add(item);
            }
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            Success = ToogleModelAlbum.SelectedAlbums.Count>0;
            DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
