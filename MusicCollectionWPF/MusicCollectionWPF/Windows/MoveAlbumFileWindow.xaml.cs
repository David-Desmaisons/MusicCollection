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

using MusicCollectionWPF.Infra;
using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for MoveAlbumFileWindow.xaml
    /// </summary>
    public partial class MoveAlbumFileWindow : CustomWindow
    {
        public MoveAlbumFileWindow()
        {
            InitializeComponent();
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
