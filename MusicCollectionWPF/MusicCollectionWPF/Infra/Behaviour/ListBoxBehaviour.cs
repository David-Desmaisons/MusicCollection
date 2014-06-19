using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Collections;

using MusicCollection.Infra;
using System.Collections.Specialized;

namespace MusicCollectionWPF.Infra.Behaviour
{


    public static class ListBoxBehaviour
    {
        #region DependencyProperty

        public static readonly DependencyProperty ScrollOnDragDropProperty = DependencyProperty.RegisterAttached("ScrollOnDragDrop",
                typeof(bool), typeof(ListBoxBehaviour), new PropertyMetadata(false, HandleScrollOnDragDropChanged));

        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            return (bool)element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            element.SetValue(ScrollOnDragDropProperty, value);
        }

        public static readonly DependencyProperty ScrollToSelectItemProperty = DependencyProperty.RegisterAttached("ScrollToSelectItem",
                typeof(bool), typeof(ListBoxBehaviour), new PropertyMetadata(false, ScrollToSelectItemChanged));

        public static bool GetScrollToSelectItem(DependencyObject element)
        {
            return (bool)element.GetValue(ScrollToSelectItemProperty);
        }

        public static void SetScrollToSelectItem(DependencyObject element, bool value)
        {
            element.SetValue(ScrollToSelectItemProperty, value);
        }

        public static double Sensibility
        {
            get;
            set;
        }

        public static double offset
        {
            get;
            set;
        }

        #endregion

        # region drag and drop

        static ListBoxBehaviour()
        {
            Sensibility = 35;
            offset = 15;
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement container = d as FrameworkElement;

            if (!(bool)e.OldValue)
            {
                container.PreviewDragOver -= OnContainerPreviewDragOver;
            }

            if ((bool)(e.NewValue))
            {
                container.PreviewDragOver += OnContainerPreviewDragOver;
            }
        }

        private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
        {
            FrameworkElement container = sender as FrameworkElement;

            if (container == null)
                return;

            ScrollViewer scrollViewer = WPFHelper.GetFirstVisualChild<ScrollViewer>(container);

            if (scrollViewer == null)
            {
                return;
            }


            double verticalPos = e.GetPosition(container).Y;
            double horizontalPos = e.GetPosition(container).X;

            if ((scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) || (scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto))
            {
                if (verticalPos < Sensibility) // Top of visible list? 
                {
                    scrollViewer.LineUp();
                }
                else if (verticalPos > container.ActualHeight - Sensibility) //Bottom of visible list? 
                {
                    scrollViewer.LineDown();//Scroll down.     
                }
            }

            if ((scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) || (scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto))
            {
                if (horizontalPos < Sensibility) // Top of visible list? 
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - offset); //Scroll up. 
                }
                else if (horizontalPos > container.ActualWidth - Sensibility) //Bottom of visible list? 
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + offset); //Scroll down.     
                }
            }
        }

        #endregion

        #region scroll to item

        private static void ScrollToSelectItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox container = d as ListBox;
            if (d == null)
                return;

            if (container.SelectionMode != SelectionMode.Single)
                return;

            if (!(bool)e.OldValue)
            {
                container.SelectionChanged -= container_SelectionChanged;
            }

            if ((bool)(e.NewValue))
            {
                container.SelectionChanged += container_SelectionChanged;
            }
        }

        static private void container_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox container = sender as ListBox;

            if (e.AddedItems.Count == 0)
                return;

            Action Up = () => ItemContainerGenerator_StatusChanged(e.AddedItems[0], container);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, Up);
        }

        static void ItemContainerGenerator_StatusChanged(object selected, ListBox container)
        {
            ListBoxItem lbi = container.ItemContainerGenerator.ContainerFromItem(selected) as ListBoxItem;
            if (lbi == null)
            {
                return;
            }

            if (lbi.IsLoaded == false)
            {
                lbi.Loaded += new RoutedEventHandler(lbi_Loaded);
                return;
            }

            container_SelectionChanged(container, lbi);
        }

        static void lbi_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.Loaded -= lbi_Loaded;

            if (!lbi.IsSelected)
                return;

            ListBox lb = lbi.FindAncestor<ListBox>();

            if (lb == null)
                return;

            container_SelectionChanged(lb, lbi);
        }


        static private void container_SelectionChanged(ListBox container, ListBoxItem lbi)
        {
            ScrollViewer sv = container.GetFirstVisualChild<ScrollViewer>();
            if (sv == null)
                return;

            if (sv.ComputedVerticalScrollBarVisibility != Visibility.Visible)
                return;

            ScrollContentPresenter sp = sv.GetFirstVisualChild<ScrollContentPresenter>();

            GeneralTransform childTransform = lbi.TransformToAncestor(sp);//sv
            Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), lbi.RenderSize));

            //Check if the elements Rect intersects with that of the scrollviewer's
            Rect result = Rect.Intersect(new Rect(new Point(0, 0), sp.RenderSize), rectangle);
            //if result is Empty then the element is not in view
            if (result != rectangle)
            {
                double Heigth = rectangle.Top;
                sv.SmoothToVertical(Heigth + sv.VerticalOffset, TimeSpan.FromSeconds(0.5));
            }

        }


        #endregion

        #region select all onchange

        public static readonly DependencyProperty SelectAllOnchangeProperty = DependencyProperty.RegisterAttached("SelectAllOnchange",
               typeof(bool), typeof(ListBoxBehaviour), new PropertyMetadata(false, SelectAllOnchangeChanged));

        public static bool GetSelectAllOnchange(DependencyObject element)
        {
            return (bool)element.GetValue(SelectAllOnchangeProperty);
        }

        public static void SetSelectAllOnchange(DependencyObject element, bool value)
        {
            element.SetValue(SelectAllOnchangeProperty, value);
        }

        private static void SelectAllOnchangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox container = d as ListBox;

            if (!(bool)e.OldValue)
            {
                container.TargetUpdated -= container_TargetUpdated;
            }

            if ((bool)(e.NewValue))
            {
                container.TargetUpdated += container_TargetUpdated;
            }
        }

        static void container_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ListBox container = sender as ListBox;
            if (container.ItemsSource == null)
                return;

            foreach (object al in container.ItemsSource)
                container.SelectedItems.Add(al);
        }

        #endregion

        #region SelectedItemsSource

        private static IDictionary<INotifyCollectionChanged, ListBox> _Mapped = new Dictionary<INotifyCollectionChanged, ListBox>();

        public static readonly DependencyProperty SelectedItemsSourceProperty = DependencyProperty.RegisterAttached("SelectedItemsSource",
               typeof(IList), typeof(ListBoxBehaviour), new PropertyMetadata(null, SelectedItemsSourceChanged));

        public static IList GetSelectedItemsSource(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsSourceProperty);
        }

        public static void SetSelectedItemsSource(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsSourceProperty, value);
        }

        private static void SelectedItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox container = d as ListBox;

            if (d == null)
                return;

            IList l = e.NewValue as IList;

            if (l == null)
            {
                UnregisterIfPossible(e.OldValue as IList, container);
                container.SelectionChanged -= ListBox_SelectionChanged;
            }
            else
            {
                InitCollection(l, container);
                RegisterIfPossible(l, container);
                container.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        private static void InitCollection( IList ilis, ListBox lb)
        {
            ilis.Cast<object>().Apply(o => lb.SelectedItems.Add(o));
        }

        private static void RegisterIfPossible(IList ientry, ListBox icontainer = null)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                inc.CollectionChanged += inc_CollectionChanged;
                if (icontainer != null)
                    _Mapped.Add(inc, icontainer);
            }
        }


        private static void UnregisterIfPossible(IList ientry, ListBox icontainer = null)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                inc.CollectionChanged -= inc_CollectionChanged;
                if (icontainer != null)
                    _Mapped.Remove(inc);
            }
        }

        private static void Compute_CollectionChanged(ListBox lb, IList l, NotifyCollectionChangedEventArgs e)
        {        
            if (l == null)
                return;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                lb.SelectedItems.Clear();
                InitCollection(l, lb);
                return;
            }

            if (e.NewItems != null)
            {
                e.NewItems.Cast<object>().Apply(o => lb.SelectedItems.Add(o));
            }

            if (e.OldItems != null)
            {
                e.OldItems.Cast<object>().Apply(o => lb.SelectedItems.Remove(o));
            }
        }

        private static void inc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListBox lb = null;
            if (!_Mapped.TryGetValue(sender as INotifyCollectionChanged, out lb))
                return;

            lb.SelectionChanged -= ListBox_SelectionChanged;
           
            Compute_CollectionChanged(lb, sender as IList, e);

            lb.SelectionChanged += ListBox_SelectionChanged;
        }


        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox container = sender as ListBox;

            IList list = GetSelectedItemsSource(container);
            if (list == null)
                return;

            if (container.SelectedItems == null)
            {
                list.Clear();
                return;
            }

            UnregisterIfPossible(list);
            e.AddedItems.Cast<object>().Apply(o => list.Add(o));
            e.RemovedItems.Cast<object>().Apply(o => list.Remove(o));
            RegisterIfPossible(list);

        }
        #endregion
    }
}


