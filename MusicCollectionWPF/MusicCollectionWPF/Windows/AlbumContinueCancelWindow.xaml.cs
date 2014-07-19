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
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for AlbumContinueCancelWindow.xaml
    /// </summary>
    [ViewModelBinding(typeof(ConfirmationAlbumViewModel))]
    public partial class AlbumContinueCancelWindow : CustomWindow
    {
        public AlbumContinueCancelWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
