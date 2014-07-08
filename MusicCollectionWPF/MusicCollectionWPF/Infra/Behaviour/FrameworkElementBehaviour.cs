using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class FrameworkElementBehaviour
    {
        public static readonly DependencyProperty FocusOnLoadedProperty = DependencyProperty.RegisterAttached("FocusOnLoaded",
                typeof(bool), typeof(FrameworkElementBehaviour), new PropertyMetadata(false, FocusOnLoadedPropertyChanged));

         public static bool GetFocusOnLoaded(DependencyObject element)
        {
            return (bool)element.GetValue(FocusOnLoadedProperty);
        }

         public static void SetFocusOnLoaded(DependencyObject element, bool value)
        {
            element.SetValue(FocusOnLoadedProperty, value);
        }

        private static void FocusOnLoadedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement  ui = d as FrameworkElement ;
            if (ui == null)
                return;

            if ((bool)e.NewValue)
            {
                ui.Loaded += ui_Loaded;
            }
            else
            {
                ui.Loaded -= ui_Loaded;
            }
        }

        static private void ui_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ui = sender as FrameworkElement;
            if (ui == null)
                return;

            ui.Focus();
        }
    }
}
