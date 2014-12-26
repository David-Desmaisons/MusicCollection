using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class UIElementBehaviour
    {

        public static readonly DependencyProperty AutoFocusProperty = DependencyProperty.RegisterAttached("AutoFocus",
                typeof(bool), typeof(UIElementBehaviour), new PropertyMetadata(false, AutoFocusPropertyChanged));

        public static bool GetAutoFocus(DependencyObject element)
        {
            return (bool)element.GetValue(AutoFocusProperty);
        }

        public static void SetAutoFocus(DependencyObject element, bool value)
        {
            element.SetValue(AutoFocusProperty, value);
        }

        private static void AutoFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiel = d as UIElement;
            if (uiel == null)
                return;

            if ((bool)e.NewValue == true)
            {
                uiel.IsVisibleChanged += UIElement_IsVisibleChanged;
            }
            else
            {
                uiel.IsVisibleChanged -= UIElement_IsVisibleChanged;
            }
        }


        private static void UIElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement el = sender as UIElement;

            if ((bool)e.NewValue)
            {
                el.Dispatcher.BeginInvoke( DispatcherPriority.ContextIdle,
                new Action(delegate() { el.Focus(); }));
            }
        }  
    }
}


