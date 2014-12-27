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

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class PopupBehaviour
    {

        public static readonly DependencyProperty ClosingOnWindowsChangeProperty = DependencyProperty.RegisterAttached("ClosingOnWindowsChange",
                typeof(bool), typeof(PopupBehaviour), new PropertyMetadata(false, SourcePropertyChanged));

        public static bool GetClosingOnWindowsChange(DependencyObject element)
        {
            return (bool)element.GetValue(ClosingOnWindowsChangeProperty);
        }

        public static void SetClosingOnWindowsChange(DependencyObject element, bool value)
        {
            element.SetValue(ClosingOnWindowsChangeProperty, value);
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup pup = d as Popup;
            if (pup == null)
                return;

            if ((bool)e.NewValue == true)
            {
                pup.Opened += pup_opened;
            }
            else
            {
                pup.Opened -= pup_opened;
            }
        }

        private static void pup_opened(object sender, EventArgs e)
        {
            Popup pup = sender as Popup;
            if (pup == null)
                return;
            Window win = Window.GetWindow(pup);
            win.LocationChanged += (o, ev) => pup.IsOpen = false; ;
            win.SizeChanged += (o, ev) => pup.IsOpen = false; ;
        }
    }
}
