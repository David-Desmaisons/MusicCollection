﻿using System;
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
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;


namespace MusicCollectionWPF.Infra.Behaviour
{
    public static class ListBoxBehaviour
    {
        #region ScrollOnDragDrop

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

            if (!(bool)e.NewValue)
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

        private static IDictionary<INotifyCollectionChanged, HashSet<ISelectionableControl>> _Mapped = new Dictionary<INotifyCollectionChanged, HashSet<ISelectionableControl>>();

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
            ISelectionableControl container = SelectionableControlFactory.Get(d);

            if (container == null)
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

        private static void InitCollection(IList ilis, ISelectionableControl lb)
        {
            ilis.Cast<object>().Apply(o => lb.SelectedItems.Add(o));
        }

        private static void RegisterIfPossible(IList ientry, ISelectionableControl icontainer)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                HashSet<ISelectionableControl> listeners = null;
                if (!_Mapped.TryGetValue(inc, out listeners))
                {
                    listeners = new HashSet<ISelectionableControl>();
                    _Mapped.Add(inc, listeners);
                    inc.CollectionChanged += inc_CollectionChanged;
                }

                listeners.Add(icontainer);
            }
        }


        private static void UnregisterIfPossible(IList ientry, ISelectionableControl icontainer)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                HashSet<ISelectionableControl> listeners = _Mapped[inc];
                listeners.Remove(icontainer);

                if (listeners.Count==0)
                {
                    _Mapped.Remove(inc);
                    inc.CollectionChanged -= inc_CollectionChanged;
                }      
            }
        }

        private static void Compute_CollectionChanged(ISelectionableControl lb, IList l, NotifyCollectionChangedEventArgs e)
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
            HashSet<ISelectionableControl> lb = null;
            if (!_Mapped.TryGetValue(sender as INotifyCollectionChanged, out lb))
                return;

            var list = sender as IList;

            lb.Apply(l =>
                {
                    l.SelectionChanged -= ListBox_SelectionChanged;
                    Compute_CollectionChanged(l, list, e);
                    l.SelectionChanged += ListBox_SelectionChanged;
                });
        }


        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISelectionableControl container = SelectionableControlFactory.Get(sender);
            IList list = GetSelectedItemsSource(sender as DependencyObject);

            if ((list == null) || (container==null))
                return;

            if (container.SelectedItems == null)
            {
                list.Clear();
                return;
            }

            UnregisterIfPossible(list, container);
            e.RemovedItems.Cast<object>().Apply(o => list.Remove(o));
            e.AddedItems.Cast<object>().Apply(o => list.Add(o));
            RegisterIfPossible(list, container);

        }
        #endregion

        #region NotUnselected

        public static readonly DependencyProperty NotUnselectedProperty = DependencyProperty.RegisterAttached("NotUnselected",
               typeof(bool), typeof(ListBoxBehaviour), new PropertyMetadata(false, NotUnselectedchangeProperty));

        public static bool GetNotUnselected(DependencyObject element)
        {
            return (bool)element.GetValue(NotUnselectedProperty);
        }

        public static void SetNotUnselected(DependencyObject element, bool value)
        {
            element.SetValue(NotUnselectedProperty, value);
        }

        private static void NotUnselectedchangeProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox container = d as ListBox;

            if (!(bool)e.NewValue)
            {
                container.SelectionChanged-=container_SelectionChanged_NotSelected;
            }

            if ((bool)(e.NewValue))
            {
                container.SelectionChanged += container_SelectionChanged_NotSelected;
            }
        }

        static void container_SelectionChanged_NotSelected(object sender, SelectionChangedEventArgs e)
        {
            ListBox container = sender as ListBox;
            if (container.ItemsSource == null)
                return;

            if (container.SelectedItems.Count > 0)
                return;

            if (container.Items.Count == 0)
                return;

            var newnotnull = (e.RemovedItems.Count > 0) ? e.RemovedItems[0] : container.Items[0];
            container.SelectedItems.Add(newnotnull);
        }

        #endregion

        #region keyboardnavigation

        public static readonly DependencyProperty KeyboardNavigationProperty = DependencyProperty.RegisterAttached("KeyboardNavigation",
               typeof(bool), typeof(ListBoxBehaviour), new PropertyMetadata(false, KeyboardNavigationPropertychanged));

        public static bool GetKeyboardNavigation(DependencyObject element)
        {
            return (bool)element.GetValue(KeyboardNavigationProperty);
        }

        public static void SetKeyboardNavigation(DependencyObject element, bool value)
        {
            element.SetValue(KeyboardNavigationProperty, value);
        }

        private static void KeyboardNavigationPropertychanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            if (listBox == null)
                return;

            if ((bool)e.NewValue)
            {
                listBox.PreviewKeyDown += listBox_PreviewKeyDown;
            }
            else
            {
                listBox.PreviewKeyDown -= listBox_PreviewKeyDown;
            }
        }

        static private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
                return;

            ScrollViewer isc = listBox.GetFirstVisualChild<ScrollViewer>();
            if (isc == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                    isc.LineUp();
                    e.Handled = true;
                    break;

                case Key.Down:
                    isc.LineDown();
                    e.Handled = true;
                    break;

                case Key.Left:
                    isc.LineLeft();

                    break;

                case Key.Right:
                    isc.LineRight();
                    break;

                case Key.PageUp:
                    isc.PageUp();
                    e.Handled = true;
                    break;

                case Key.PageDown:
                    isc.PageDown();
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Sorter Grouper

        public static readonly DependencyProperty GrouperProperty = DependencyProperty.RegisterAttached("Grouper",
               typeof(ListGrouper), typeof(ListBoxBehaviour), new PropertyMetadata(null, GrouperPropertychanged));

        public static ListGrouper GetGrouper(DependencyObject element)
        {
            return (ListGrouper)element.GetValue(GrouperProperty);
        }

        public static void SetGrouper(DependencyObject element, ListGrouper value)
        {
            element.SetValue(GrouperProperty, value);
        }

        private static void GrouperPropertychanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = dpo as ItemsControl;
            if (itemsControl == null)
                return;

            DependencyPropertyDescriptor dependencyPropertyDescriptor =
                    DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl));
            if (dependencyPropertyDescriptor == null)
                return;

            if ((ListGrouper)e.NewValue !=null)
            {
                UpdateCurrentList(itemsControl);
                dependencyPropertyDescriptor.AddValueChanged(itemsControl, ItemsSourceChanged);
            }
            var old = (ListGrouper)e.OldValue;
            if (old != null)
            {
                dependencyPropertyDescriptor.RemoveValueChanged(itemsControl, ItemsSourceChanged);
                old.Dispose();
            }
        }
        
        private static void UpdateCurrentList(ItemsControl itemscontrol)
        {
            ListGrouper lgs = GetGrouper(itemscontrol);
            if (lgs == null)
                return;
            var source = itemscontrol.ItemsSource;
            if (source == null)
                return;
            ICollectionView icv = (source as ICollectionView)?? CollectionViewSource.GetDefaultView(source);
            lgs.Apply(icv);
        }

        static private void ItemsSourceChanged(object sender, EventArgs e)
        {
            UpdateCurrentList(sender as ItemsControl);       
        }

        #endregion

    }
}



