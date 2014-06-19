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
        //private string _Req;

        ////q=%22toto+asticot%22+%2B+%22titi+lidi%22
        //public InternetCoverFinder(IModifiableAlbum IAl)
        //{
        //    _Req = IAl.CreateSearchGoogleSearchString();
        //    InitializeComponent();
        //}

        public InternetCoverFinder()
        {
            InitializeComponent();
        }

        //private void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Uri uri = new Uri(@"http://www.google.com/images?q="+_Req, UriKind.RelativeOrAbsolute);
        //    webBrowser1.Navigate(uri);
        //}
    }
}
