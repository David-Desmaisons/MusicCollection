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
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;


namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for RarPasswordManagerWindow.xaml
    /// </summary>
    [ViewModelBinding(typeof(RarFileManagementModelView))]
    public partial class RarPasswordManagerWindow : CustomWindow
    {
        public RarPasswordManagerWindow()
        {
            InitializeComponent();
        }

        //public RarPasswordManagerWindow(IMusicSettings Settings)
        //{
        //    InitializeComponent();

        //    Password.Settings=Settings;

        //    Password.RequestClose += (o,e)=>Close();
        //}
       
        //private void CustomWindow_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    this.SizeToContent = SizeToContent.WidthAndHeight;
        //}
    }
}
