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
using System.IO;
using MusicCollection.Fundation;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF
{
    /// <summary>
    /// Interaction logic for InternetCoverFinder.xaml
    /// </summary>
    [ViewModelBinding(typeof(InternetFinderViewModel))]
    public partial class InternetCoverFinder : CustomWindow
    {
        public InternetCoverFinder()
        {
            InitializeComponent();
        }
    }
}
