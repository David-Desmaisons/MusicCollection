using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public static class ScrollViewerAnimator 
        //: FrameworkElement
    {
        //private ScrollViewer _ScrollViewer;

        public static readonly DependencyProperty ScrollOffsetVerticalProperty = DependencyProperty.RegisterAttached("ScrollOffsetVertical", typeof(double?),
            typeof(ScrollViewerAnimator), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnScrollOffsetVerticalPropertyChanged)));


        public static Nullable<double> GetScrollOffsetVertical(DependencyObject element)
        {
            return (Nullable<double>)element.GetValue(ScrollOffsetVerticalProperty);
        }

        public static void SetScrollOffsetVertical(DependencyObject element, Nullable<double> value)
        {
            element.SetValue(ScrollOffsetVerticalProperty, value);
        }

        //public static double ScrollOffsetVertical
        //{
        //    get { return (double)GetValue(ScrollOffsetVerticalProperty); }
        //    set { SetValue(ScrollOffsetVerticalProperty, value); }
        //}

        private static void OnScrollOffsetVerticalPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            //ScrollViewerAnimator myObj = obj as ScrollViewerAnimator;

            //if (myObj != null)
            //{
            //    myObj._ScrollViewer.ScrollToVerticalOffset((double)args.NewValue);
            //}

            ScrollViewer myObj = obj as ScrollViewer;
            Nullable<double> target = (Nullable<double>)args.NewValue;

            if ((myObj != null) && (target.HasValue))
            {
                myObj.ScrollToVerticalOffset(target.Value);
            }
        }

        public static readonly DependencyProperty ScrollOffsetHorizontalProperty = DependencyProperty.RegisterAttached("ScrollOffsetHorizontal", typeof(double?),
           typeof(ScrollViewerAnimator), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnScrollOffsetHorizontalPropertyChanged)));


        //public double ScrollOffsetHorizontal
        //{
        //    get { return (double)GetValue(ScrollOffsetHorizontalProperty); }
        //    set { SetValue(ScrollOffsetHorizontalProperty, value); }
        //}

        public static Nullable<double> GetScrollOffsetHorizontal(DependencyObject element)
        {
            return (Nullable<double>)element.GetValue(ScrollOffsetVerticalProperty);
        }

        public static void SetScrollOffsetHorizontal(DependencyObject element, Nullable<double> value)
        {
            element.SetValue(ScrollOffsetVerticalProperty, value);
        }

        private static void OnScrollOffsetHorizontalPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            //ScrollViewerAnimator myObj = obj as ScrollViewerAnimator;

            //if (myObj != null)
            //{
            //    myObj._ScrollViewer.ScrollToHorizontalOffset((double)args.NewValue);
            //}

            ScrollViewer myObj = obj as ScrollViewer;
            Nullable<double> target = (Nullable<double>)args.NewValue;

            if ((myObj != null) && (target.HasValue))
            {
                myObj.ScrollToHorizontalOffset(target.Value);
            }
        }

        public static void SmoothToVertical(this ScrollViewer sv, double iTarget, TimeSpan ts)
        {
            Nullable<double> smcsrvalue = ScrollViewerBehaviour.GetSmoothScrolling(sv);
            if (smcsrvalue.HasValue)
            {
                iTarget = Math.Round(iTarget / smcsrvalue.Value) * smcsrvalue.Value;
            }

            ScrollViewerAnimator.SetScrollOffsetVertical(sv,sv.VerticalOffset);
            sv.SmoothSet(ScrollViewerAnimator.ScrollOffsetVerticalProperty, iTarget, ts);
        }

        //public ScrollViewerAnimator(ScrollViewer scrollViewer)
        //{
        //    _ScrollViewer = scrollViewer;
        //    ScrollOffsetVertical = _ScrollViewer.VerticalOffset;
        //    ScrollOffsetHorizontal = _ScrollViewer.HorizontalOffset;
        //}
    }
}
