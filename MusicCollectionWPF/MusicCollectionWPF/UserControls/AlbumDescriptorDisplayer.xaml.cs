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

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumDescriptorDisplayer.xaml
    /// </summary>
    public partial class AlbumDescriptorDisplayer : UserControl
    {
        private CollectionViewSource _ViewAl = null;

        public AlbumDescriptorDisplayer()
        {
            InitializeComponent();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewAl.View == null)
                return;

            _ViewAl.View.MoveCurrentToNext();
            if (_ViewAl.View.IsCurrentAfterLast)
                _ViewAl.View.MoveCurrentToFirst();
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewAl.View == null)
                return;

            _ViewAl.View.MoveCurrentToPrevious();
            if (_ViewAl.View.IsCurrentBeforeFirst)
                _ViewAl.View.MoveCurrentToLast();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ViewAl = ((System.Windows.Data.CollectionViewSource)(this.FindResource("iCoverViewSource")));

            if (_ViewAl == null)
                return;

            if (DataContext != null)
                _ViewAl.Source = (DataContext as IFullAlbumDescriptor).Images;

            if ( _ViewAl.View!=null)
                _ViewAl.View.MoveCurrentToFirst();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        { 
            if (_ViewAl == null)
                _ViewAl = ((System.Windows.Data.CollectionViewSource)(this.FindResource("iCoverViewSource")));

            if (_ViewAl == null)
                return;

            if (DataContext != null)
            {
                IFullAlbumDescriptor ifum= (DataContext as IFullAlbumDescriptor);
                if (ifum!=null)
                    _ViewAl.Source = ifum.Images;
            }

            if (_ViewAl.View != null)
                _ViewAl.View.MoveCurrentToFirst();

        }
    }
}
