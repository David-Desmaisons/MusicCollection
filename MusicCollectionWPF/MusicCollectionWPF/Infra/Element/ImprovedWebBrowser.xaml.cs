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

namespace MusicCollectionWPF.Infra
{
    /// <summary>
    /// Interaction logic for ImprovedWebBrowser
    /// </summary>
    public partial class ImprovedWebBrowser : UserControl
    {
        public ImprovedWebBrowser()
        {
            InitializeComponent();
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

         public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(ImprovedWebBrowser), new PropertyMetadata(null, SourcePropertyChanged));

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImprovedWebBrowser wb = d as ImprovedWebBrowser;
            if (wb == null)
                return;

            string gotourl = e.NewValue as string;
            if (gotourl != null)
                wb.WebBrowser1.Navigate(gotourl);
        }

        private async void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            await Waiting1.StopAsync();
            WebBrowser1.Visibility = Visibility.Visible;
        }
    }
}
