using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicCollectionWPF.ViewModelHelper;
using System.Windows.Controls.Primitives;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class ScrollViewerBehaviour
    {

        #region  HorizontalWheelScroll

        public static readonly DependencyProperty HorizontalWheelScrollProperty = DependencyProperty.RegisterAttached("HorizontalWheelScroll",
                typeof(bool), typeof(ScrollViewerBehaviour), new PropertyMetadata(false, HorizontalWheelScrollPropertyChanged));

        public static bool GetHorizontalWheelScroll(DependencyObject element)
        {
            return (bool)element.GetValue(HorizontalWheelScrollProperty);
        }

        public static void SetHorizontalWheelScroll(DependencyObject element, bool value)
        {
            element.SetValue(HorizontalWheelScrollProperty, value);
        }

        private static void HorizontalWheelScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement sv = d as FrameworkElement;
            if (sv == null)
                return;

            if (!(bool)e.OldValue)
            {
                sv.PreviewMouseWheel -= container_PreviewMouseWheel;
            }

            if ((bool)(e.NewValue))
            {
                sv.PreviewMouseWheel += container_PreviewMouseWheel;
            }
        }

        static void container_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer sv = WPFHelper.GetFirstVisualChild<ScrollViewer>(sender as DependencyObject);
            if (sv == null)
                return;

            if (e.Delta < 0)
                sv.LineLeft();
            else
                sv.LineRight();

            e.Handled = true;
        }

        #endregion


        #region  SmoothScrolling

        public static readonly DependencyProperty SmoothScrollingProperty = DependencyProperty.RegisterAttached("SmoothScrolling",
                typeof(Nullable<double>), typeof(ScrollViewerBehaviour), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, VerticalWheelScrollPropertyChanged));

        public static Nullable<double> GetSmoothScrolling(DependencyObject element)
        {
            return (Nullable<double>)element.GetValue(SmoothScrollingProperty);
        }

        public static void SetSmoothScrolling(DependencyObject element, Nullable<double> value)
        {
            element.SetValue(SmoothScrollingProperty, value);
        }

        private static void VerticalWheelScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement sv = d as FrameworkElement;
            if (sv == null)
                return;

            if (((Nullable<double>)e.NewValue).HasValue)
            {
                sv.PreviewMouseWheel += container_PreviewMouseWheel_Vertical;
            }
            else
            {
                sv.PreviewMouseWheel -= container_PreviewMouseWheel_Vertical;
            }
        }

        static void container_PreviewMouseWheel_Vertical(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            DependencyObject DependencyObjectsender = sender as DependencyObject;

            Nullable<double> sov =  ScrollViewerBehaviour.GetSmoothScrolling(DependencyObjectsender);
            if (sov == null)
                return;

            ScrollViewer sv = WPFHelper.GetFirstVisualChild<ScrollViewer>(DependencyObjectsender);
            if (sv == null)
                return;

            double dir = (e.Delta < 0) ? 1 : -1;

            sv.SmoothToVertical(sv.VerticalOffset + dir *sov.Value, TimeSpan.FromSeconds(0.05));

            e.Handled = true;
        }

        #endregion



        #region SmoothDown

        public static readonly DependencyProperty SmoothDownProperty = DependencyProperty.RegisterAttached("SmoothDown",
            typeof(Nullable<double>), typeof(ScrollViewerBehaviour), new PropertyMetadata(null, SmoothDownPropertyChanged));

        public static Nullable<double> GetSmoothDown(DependencyObject element)
        {
            return (Nullable<double>)element.GetValue(SmoothDownProperty);
        }

        public static void SetSmoothDown(DependencyObject element, Nullable<double> value)
        {
            element.SetValue(SmoothDownProperty, value);
        }

        private static void SmoothDownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonBase container = d as ButtonBase;
            if (container == null)
                return;

            Nullable<double> newvalue = (Nullable<double>)e.NewValue;

            ScrollViewer sv = d.FindAncestor<ScrollViewer>();
            if (sv == null)
                return;

            if (newvalue != null)
            {
                
                container.Command = RelayCommand.Instanciate(() => { SmoothDownPropertyChangedAction(sv, newvalue.Value); });
            }
            else
            {
                container.Command = null;
            }
        }

        private static void SmoothDownPropertyChangedAction(ScrollViewer sv, double value)
        {
            Nullable<double> smcsrvalue = ScrollViewerBehaviour.GetSmoothScrolling(sv);
            if (smcsrvalue.HasValue)
            {
                value = value * smcsrvalue.Value;
            }
             sv.SmoothToVertical(sv.VerticalOffset + value, TimeSpan.FromSeconds(0.05));
        }

        #endregion


    }
}
