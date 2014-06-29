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
    public static class MultiSelectorBehaviour
    {   
        #region SelectedItemsSource

        private static IDictionary<INotifyCollectionChanged, MultiSelector> _Mapped = new Dictionary<INotifyCollectionChanged, MultiSelector>();

        public static readonly DependencyProperty SelectedItemsSourceProperty = DependencyProperty.RegisterAttached("SelectedItemsSource",
               typeof(IList), typeof(MultiSelectorBehaviour), new PropertyMetadata(null, SelectedItemsSourceChanged));

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
            MultiSelector container = d as MultiSelector;

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

        private static void InitCollection(IList ilis, MultiSelector lb)
        {
            ilis.Cast<object>().Apply(o => lb.SelectedItems.Add(o));
        }

        private static void RegisterIfPossible(IList ientry, MultiSelector icontainer = null)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                inc.CollectionChanged += inc_CollectionChanged;
                if (icontainer != null)
                    _Mapped.Add(inc, icontainer);
            }
        }


        private static void UnregisterIfPossible(IList ientry, MultiSelector icontainer = null)
        {
            INotifyCollectionChanged inc = ientry as INotifyCollectionChanged;
            if (inc != null)
            {
                inc.CollectionChanged -= inc_CollectionChanged;
                if (icontainer != null)
                    _Mapped.Remove(inc);
            }
        }

        private static void Compute_CollectionChanged(MultiSelector lb, IList l, NotifyCollectionChangedEventArgs e)
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
            MultiSelector lb = null;
            if (!_Mapped.TryGetValue(sender as INotifyCollectionChanged, out lb))
                return;

            lb.SelectionChanged -= ListBox_SelectionChanged;
           
            Compute_CollectionChanged(lb, sender as IList, e);

            lb.SelectionChanged += ListBox_SelectionChanged;
        }


        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultiSelector container = sender as MultiSelector;

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


