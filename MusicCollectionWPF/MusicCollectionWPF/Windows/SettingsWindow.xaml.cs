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

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : CustomWindow
    {
        //private IMusicSettings _Settings;
        //private SettingsViewModel _Settings;

        public SettingsWindow(SettingsViewModel Session)
        {
            ////_Settings = Session;
            //DataContext = Session;
            //Session.Window = this;

            ModelView = Session;          
            InitializeComponent();

            //Settings.Settings = _Settings;
            ////Settings2.Settings = _Settings;         
         }

        //private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.SizeToContent = SizeToContent.WidthAndHeight;
        //}

        //private void Cancel(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        //private void OK(object sender, RoutedEventArgs e)
        //{
        //    _Settings.CommitChanges();
        //    this.Close();
        //}
    }
}
