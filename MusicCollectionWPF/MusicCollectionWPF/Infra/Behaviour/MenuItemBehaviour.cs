using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class MenuItemBehaviour
    {
        public static readonly DependencyProperty ClickableProperty = DependencyProperty.RegisterAttached("Clickable",
             typeof(bool), typeof(MenuItemBehaviour), new PropertyMetadata(false, ClickableChanged));

         public static bool GetClickable(DependencyObject element)
        {
            return (bool)element.GetValue(ClickableProperty);
        }

         public static void SetClickable(DependencyObject element, bool value)
        {
            element.SetValue(ClickableProperty, value);
        }

        private static void ClickableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem cmi = d as MenuItem;
            if (cmi == null)
                return;

            if ((bool)e.NewValue)
            {
                cmi.StaysOpenOnClick = true;
                cmi.Click += MenuItem_Click;
            }
            else
            {
                cmi.StaysOpenOnClick = false;
                cmi.Click -= MenuItem_Click;
            }
        }

        static void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem me = sender as MenuItem;
            me.IsChecked = !me.IsChecked;
        }
    }
}
