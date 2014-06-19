using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for WebInfoChooserWindow.xaml
    /// </summary>
    [ViewModelBinding(typeof(WebAlbumSelectorViewModel))]
    public partial class WebInfoChooserWindow :  CustomWindow
    {
        public WebInfoChooserWindow()
        {
            InitializeComponent();
        }
    }
}
