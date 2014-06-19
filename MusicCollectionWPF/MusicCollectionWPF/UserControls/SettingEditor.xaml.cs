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

using MusicCollection.Fundation;
using MusicCollectionWPF.Windows;


namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for SettingEditor.xaml
    /// </summary>
    public partial class SettingEditor : UserControl
    {
        //public event EventHandler<EventArgs> RequestClose;

        public SettingEditor()
        {

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
            }
        }

        //private IMusicSettings _Settings;

        //public IMusicSettings Settings
        //{
        //    set
        //    {
        //        _Settings = value;
        //        //DataContext = _Settings;
        //    }
        //}

        ////private void DispatchRequestClose(EventArgs e)
        ////{
        ////    if (RequestClose != null)
        ////        RequestClose(this, null);
        ////}

        ////private void OK(object sender, RoutedEventArgs e)
        ////{
        ////    _Settings.CommitChanges();
        ////    DispatchRequestClose(e);
        ////}

        ////private void Cancel(object sender, RoutedEventArgs e)
        ////{
        ////    DispatchRequestClose(e);
        ////}

        //private void Password(object sender, RoutedEventArgs e)
        //{
        //    RarPasswordManagerWindow rpw = new RarPasswordManagerWindow(_Settings);
        //    rpw.ShowDialog();
        //}
    }
}
