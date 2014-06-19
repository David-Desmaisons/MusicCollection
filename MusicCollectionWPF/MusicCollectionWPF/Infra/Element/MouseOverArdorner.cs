using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MusicCollectionWPF.Infra
{
    public class MouseOverArdornerInfo
    {
        public MouseOverArdornerInfo()
        {
            Template = null;
            DelayClose = 0;
            VerticalOffset = 0;
            HorizontalOffset = 0;
        }

        public DataTemplate Template
        {
            get;
            set;
        }

        public int DelayClose
        {
            get;
            set;
        }

        public Double VerticalOffset
        {
            get;
            set;
        }

        public Double HorizontalOffset
        {
            get;
            set;
        }
    }


    public class MouseOverArdorner : DependencyObject
    {

        private static Dictionary<FrameworkElement, UIElementAdorner> _Adorners = new Dictionary<FrameworkElement, UIElementAdorner>();

        private static readonly DependencyProperty AttachedAdornerProperty = DependencyProperty
            .RegisterAttached(
            "AttachedAdorner", typeof(MouseOverArdornerInfo), typeof(MouseOverArdorner),
            new PropertyMetadata(OnUseHoverChanged));

        public static void SetAttachedAdorner(DependencyObject d, MouseOverArdornerInfo use)
        {
            d.SetValue(AttachedAdornerProperty, use);
        }

        public static MouseOverArdornerInfo GetAttachedAdorner(DependencyObject d)
        {
            return (MouseOverArdornerInfo)d.GetValue(AttachedAdornerProperty);
        }

        private static readonly DependencyProperty IsClosingAdornerProperty = DependencyProperty
            .RegisterAttached(
            "IsClosingAdorner", typeof(bool), typeof(MouseOverArdorner),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsClosingAdorner(DependencyObject d, bool use)
        {
            d.SetValue(IsClosingAdornerProperty, use);
        }

        public static bool GetIsClosingAdorner(DependencyObject d)
        {
            return (bool)d.GetValue(IsClosingAdornerProperty);
        }

        private static void OnUseHoverChanged(DependencyObject d,
                                              DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement lb = d as FrameworkElement;
            if (lb != null)
            {

                MouseOverArdornerInfo ai = e.NewValue as MouseOverArdornerInfo;
                if (ai != null)
                {
                    lb.MouseEnter += ListBox_MouseEnter;
                    lb.MouseLeave += ListBox_MouseLeave;
                }
                else
                {
                    lb.MouseEnter -= ListBox_MouseEnter;
                    lb.MouseLeave -= ListBox_MouseLeave;
                }
            }
        }

        //static void lb_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Adorner ad = sender as Adorner;
        //    FrameworkElement fe = ad.AdornedElement as FrameworkElement;
        //    fe.RaiseEvent(e);
        //}

        private static void ListBox_MouseEnter(object sender, MouseEventArgs e)
        {
            // Check that we are hovering on a ListBoxItem
            FrameworkElement lb = sender as FrameworkElement;
            MouseOverArdornerInfo ct = GetAttachedAdorner(lb);

            if (ct == null)
                return;

            if (_Adorners.ContainsKey(lb))
                return;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(lb);

            ContentPresenter cp = new ContentPresenter();
            cp.ContentTemplate = ct.Template;

            Binding b = new Binding();
            b.Path = new PropertyPath("DataContext");
            b.Source = lb;
            b.Mode = BindingMode.OneWay;
            cp.SetBinding(ContentPresenter.ContentProperty, b);
            UIElementAdorner uea = new UIElementAdorner(lb) { Child = cp };

            //uea.OffsetLeft = ct.VerticalOffset;
            //uea.OffsetTop = ct.HorizontalOffset;

            layer.Add(uea);
            _Adorners.Add(lb, uea);

            //uea.MouseLeave += new MouseEventHandler(uea_MouseLeave);
            //uea.MouseDown += lb_MouseDown;
            //uea.PreviewMouseDown += lb_MouseDown;
            //uea.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(lb_MouseDown);
            //uea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(lb_MouseDown);
            //uea.MouseLeftButtonUp += new MouseButtonEventHandler(lb_MouseDown);
            //uea.MouseLeftButtonDown += new MouseButtonEventHandler(lb_MouseDown);
            //uea.PreviewMouseRightButtonUp += new MouseButtonEventHandler(uea_PreviewMouseRightButtonDown);
            //uea.PreviewMouseRightButtonDown += new MouseButtonEventHandler(lb_MouseDown);
            //uea.MouseRightButtonUp += new MouseButtonEventHandler(lb_MouseDown);
            //uea.MouseRightButtonDown += new MouseButtonEventHandler(lb_MouseDown);

        }

        //static void uea_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    UIElementAdorner ad = sender as UIElementAdorner;
        //    FrameworkElement fe = ad.AdornedElement as FrameworkElement;

        //    ContextMenu cm = null;
        //  //  UIElement res = fe;

        //    while ((cm == null) && (fe != null))
        //    {
        //        //var res = VisualTreeHelper.GetParent(res);
        //        FrameworkElement nfe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
        //        if (nfe == null)
        //            nfe = fe.Parent as FrameworkElement;

        //        fe = nfe;

        //        if (fe != null)
        //            cm = fe.ContextMenu;
        //    }

        //    if (fe != null)
        //    {
        //        fe.ContextMenu.PlacementTarget = fe;
        //        fe.ContextMenu.IsOpen = true;
        //    }

        //    lb_MouseDown(sender, e);
        //}


        //static void uea_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    UIElementAdorner ad = sender as UIElementAdorner;
        //    FrameworkElement fe = ad.AdornedElement as FrameworkElement;
        //    if (fe.IsMouseOver)
        //        return;

        //    CleanUp(fe, ad);
        //}


        private static void ListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement lb = sender as FrameworkElement;
            UIElementAdorner ad = null;
            if (!_Adorners.TryGetValue(lb, out ad))
            {
                return;
            }

            ad.Child.RaiseEvent(e);

            //if (ad.IsMouseOver)
            //    return;


            CleanUp(lb, ad);
        }

        private static void CleanUp(FrameworkElement lb, UIElementAdorner ad)
        {
            if (!_Adorners.ContainsKey(lb))
                return;

            //ad.MouseLeave -= new MouseEventHandler(uea_MouseLeave);
            //ad.MouseDown -= lb_MouseDown;
            //ad.PreviewMouseDown -= lb_MouseDown;
            //ad.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.MouseLeftButtonUp -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.MouseLeftButtonDown -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.PreviewMouseRightButtonUp -= new MouseButtonEventHandler(uea_PreviewMouseRightButtonDown);
            //ad.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.MouseRightButtonUp -= new MouseButtonEventHandler(lb_MouseDown);
            //ad.MouseRightButtonDown -= new MouseButtonEventHandler(lb_MouseDown);
            _Adorners.Remove(lb);

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(lb);
            if (layer == null)
                return;

            int del = GetAttachedAdorner(lb).DelayClose;

            if (del == 0)
            {
                layer.Remove(ad);
                ad.Dispose();
            }
            else
            {
                ////bool res = GetIsClosingAdorner(ad);

                //SetIsClosingAdorner(ad.Child, true);

                //res = GetIsClosingAdorner(ad);
                DispatcherTimer d = new DispatcherTimer( DispatcherPriority.Normal,  Dispatcher.CurrentDispatcher){ Interval=TimeSpan.FromMilliseconds(del)};
  
                EventHandler ev = null;
                ev = (o, e) => { layer.Remove(ad); d.Tick -= ev; d.Stop(); ad.Dispose(); };
                d.Tick += ev;
               
                d.Start();
            }

        }



    }
}
