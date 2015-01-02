using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Effects;
using System.Threading.Tasks;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.UserControls;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel.Interface;
using System.ServiceModel;
using System.ComponentModel;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    [ViewModelBinding(typeof(AplicationViewModel))]
    public partial class MainWindow : CustomWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        protected override void OnLoaded()
        {
        }

    }
}
