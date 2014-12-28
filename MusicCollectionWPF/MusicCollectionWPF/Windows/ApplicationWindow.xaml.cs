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
            transitionContainer1.Transition = new FadeTransition();

            transitionContainer1.Associate(GotoPlay, albumPlayer1);
            transitionContainer1.Associate(Browse, albumBrowser1);
        }

        protected override void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Browse.Visibility = Visibility.Collapsed;
        }

        protected override void OnLoaded()
        {
        }

    }
}
