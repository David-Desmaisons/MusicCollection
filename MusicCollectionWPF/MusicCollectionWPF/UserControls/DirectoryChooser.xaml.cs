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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for DirectoryChooser.xaml
    /// </summary>
    public partial class DirectoryChooser : UserControl
    {
        public DirectoryChooser()
        {
            InitializeComponent();
        }

        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set { SetValue(DirectoryPathProperty, value); }
        }

        public static readonly DependencyProperty DirectoryPathProperty = DependencyProperty.Register("DirectoryPath", typeof(string), typeof(DirectoryChooser));

    }
}
