using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using MusicCollection.Infra;
using System.Windows.Input;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public static class DragOnDropReady
    {
        #region Attached Property
        public static readonly DependencyProperty DragOnDropReadyProperty =
          DependencyProperty.RegisterAttached("IsReady", typeof(bool), typeof(DragOnDropReady),
                                              new PropertyMetadata(false, IsReadyReadyChanged));

        public static void SetIsReady(DependencyObject depObj, bool advisor)
        {
            depObj.SetValue(DragOnDropReadyProperty, advisor);
        }

        public static bool GetIsReady(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(DragOnDropReadyProperty);
        }

        #endregion

        private static void IsReadyReadyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            ListBoxItem lb = depObj as ListBoxItem;
            if (lb == null)
                return;

            if ((bool)(args.NewValue))
            {
                lb.PreviewMouseLeftButtonDown+=ListBoxItem_PreviewMouseLeftButtonDown;
                lb.PreviewMouseLeftButtonUp +=ListBoxItem_PreviewMouseLeftButtonUp;
            }
            else
            {
                lb.PreviewMouseLeftButtonDown-=ListBoxItem_PreviewMouseLeftButtonDown;
                lb.PreviewMouseLeftButtonUp -=ListBoxItem_PreviewMouseLeftButtonUp;
            } 
        }

        static private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lb = sender as ListBoxItem;
            if (lb == null)
                return; 
            
            if (lb.IsSelected)
            {
                _NeedToReset = true;
                e.Handled = true;
            }
        }

        private static bool _NeedToReset = false;

        static private void ListBoxItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lb = sender as ListBoxItem;
            if (lb == null)
                return;

            if (_NeedToReset)
            {
                if ( Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    ListBox lbx = lb.FindAncestor<ListBox>();
                    var selected = lbx.SelectedItems.Cast<object>().ToList();
                    selected.Apply(s => { if (s != lb.DataContext) lbx.SelectedItems.Remove(s); });
                }
                else
                {
                    lb.IsSelected = false;
                }
            }

           _NeedToReset = false;
        }
    }
}
