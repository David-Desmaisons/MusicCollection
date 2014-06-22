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
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary> 
    [ViewModelBinding(typeof(SettingsViewModel))]
    public partial class SettingsWindow : CustomWindow
    {
       
        public SettingsWindow()
        {
            InitializeComponent();
        }
        //public SettingsWindow(SettingsViewModel Session)
        //{
        //    ModelView = Session;          
        //    InitializeComponent();       
        //}
    }
}
