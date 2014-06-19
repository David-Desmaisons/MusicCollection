using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class WebBrowserBehaviour
    {

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source",
                typeof(string), typeof(WebBrowserBehaviour), new PropertyMetadata(null, SourcePropertyChanged));

        public static string GetSource(DependencyObject element)
        {
            return (string)element.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject element, string value)
        {
            element.SetValue(SourceProperty, value);
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser wb = d as WebBrowser;
            if (wb == null)
                return; 
                 
            string gotourl = e.NewValue as string;
            if (gotourl != null)
                wb.Navigate(gotourl);
        }

        //if (!wb.IsLoaded)
        //{
        //    wb.Loaded += wb_Loaded;
            
        //static void wb_Loaded(object sender, RoutedEventArgs e)
        //{
        //    WebBrowser wb = sender as WebBrowser;
        //    if (wb == null)
        //        return;

        //    wb.Loaded -= wb_Loaded;
        //    string gotourl = GetSource(wb);
        //    if (gotourl != null)
        //        wb.Navigate(new Uri(gotourl));
        //}
    }
}
