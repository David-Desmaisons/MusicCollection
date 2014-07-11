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

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class ListDragAndDrop : IDragSourceAdvisor, IDropTargetAdvisor
    {
        private ListBox _ListBox;
        private ListDragAndDrop(ListBox ilb)
        {
            _ListBox = ilb;
        }

        #region Attached Property
        public static readonly DependencyProperty IsAutoProperty =
          DependencyProperty.RegisterAttached("IsAuto", typeof(bool),typeof(ListDragAndDrop),
                                              new PropertyMetadata(false,OnIsAutoChanged));

        public static void SetIsAuto(DependencyObject depObj, bool advisor)
        {
            depObj.SetValue(IsAutoProperty, advisor);
        }

        public static bool GetIsAuto(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(IsAutoProperty);
        }

        #endregion

        private static void OnIsAutoChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            ListBox lb = depObj as ListBox;
            if (lb == null)
                return;

            ListDragAndDrop ldad = null;

            if ((bool)args.NewValue)
            {
                ldad = new ListDragAndDrop(lb);
                DragDropManager.SetDragSourceAdvisor(lb, ldad);
                DragDropManager.SetDropTargetAdvisor(lb, ldad);
            }
            else
            {
                DragDropManager.SetDragSourceAdvisor(lb, null);
                DragDropManager.SetDropTargetAdvisor(lb, null);
            }
           
        }

        #region IDropTargetAdvisor

        private UIElement _DropTraget;
        UIElement IDropTargetAdvisor.TargetUI
        {
            get { return _DropTraget; }
            set { _DropTraget = value;}
        }

        bool IDropTargetAdvisor.ApplyMouseOffset
        {
            get { return true; }
        }

        bool IDropTargetAdvisor.IsValidDataObject(IDataObject obj)
        {
            return obj.GetDataPresent("ListBoxItem");
        }

        bool IDropTargetAdvisor.OnDropCompleted(IDataObject obj, System.Windows.Point dropPoint, object Originalsource)
        {
            FrameworkElement fe = Originalsource as FrameworkElement;
            ListBoxItem lbitarget = fe.FindAncestor<ListBoxItem>();
            if (lbitarget == null)
                return false;

            int newindex = _ListBox.ItemContainerGenerator.IndexFromContainer(lbitarget);

            ListBoxItem lbi = obj.GetData("ListBoxItem") as ListBoxItem;

            //new
            object oindex = obj.GetData("OriginalIndex");
            int oldindex = -1;

            if (oindex != null)
            {
                oldindex = (int)oindex;
            }   
            //new

            //int oldindex = _ListBox.ItemContainerGenerator.IndexFromContainer(lbi);

            var item = lbi.Content;

            IList itemsource = _ListBox.ItemsSource as IList;
            if (itemsource == null)
                return false;

            if (newindex == oldindex)
                return false;

            if (oldindex!=-1)
            {
                dynamic isrce = itemsource;
                Swap(isrce, oldindex, newindex, item);;
            }
            else
            {
                (itemsource as IList).Insert(newindex, item);
            }

            return true;
        }

        private static void Swap(IList il, int oldindex, int newindex, object item)
        {
            il.RemoveAt(oldindex);
            il.Insert(newindex, item);
        }

        private static void Swap<T>(ObservableCollection<T> il, int oldindex, int newindex, object item)
        {
            il.Move(oldindex, newindex);
        }

        UIElement IDropTargetAdvisor.GetVisualFeedback(IDataObject obj)
        {

            UIElement elt = obj.GetData("RawUI") as UIElement;
            System.Windows.Shapes.Rectangle rect = elt.CreateSnapshot();
            rect.Opacity = 0.5;
            rect.IsHitTestVisible = false;
            return rect;
        }

        UIElement IDropTargetAdvisor.GetTopContainer()
        {
            return _ListBox;
        }

        #endregion

        #region IDragSourceAdvisor

        private UIElement _SourceDrag;
        UIElement IDragSourceAdvisor.SourceUI
        {
            get { return _SourceDrag; }
            set { _SourceDrag = value; }
        }

        DragDropEffects IDragSourceAdvisor.SupportedEffects
        {
            get { return DragDropEffects.Move; }
        }

        DataObject IDragSourceAdvisor.GetDataObject(UIElement draggedElt)
        {
            DataObject Do = new DataObject();
            Do.SetData("RawUI", draggedElt);

            ListBoxItem listViewItem = draggedElt.FindAncestor<ListBoxItem>();
            if (listViewItem != null)
            {
                Do.SetData("ListBoxItem", listViewItem);
                Do.SetData("OriginalIndex", _ListBox.ItemsSource.Cast<object>().Index(listViewItem.DataContext));
            }

            return Do;
        }

        void IDragSourceAdvisor.FinishDrag(DataObject draggedElt, DragDropEffects finalEffects, bool DropOk)
        {
        }

        bool IDragSourceAdvisor.IsDraggable(UIElement dragElt)
        {
            if (dragElt == _ListBox)
                return false;

            ListBoxItem listViewItem = dragElt.FindAncestor<ListBoxItem>();
            return (listViewItem != null);
        }

        UIElement IDragSourceAdvisor.GetTopContainer()
        {
            return _ListBox;
            //.FindAncestor<Window>();
        }

        #endregion  
    }
}
