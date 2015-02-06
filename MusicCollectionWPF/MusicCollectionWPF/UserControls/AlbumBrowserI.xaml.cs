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
using System.Windows.Controls.Primitives;


using System.Timers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.UserControls.AlbumPresenter;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumBrowser.xaml
    /// </summary>
    public partial class AlbumBrowserI : UserControl        
    {
        public AlbumBrowserI()
        {
            InitializeComponent();
        }

    }
}
