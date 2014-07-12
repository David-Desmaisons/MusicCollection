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
    public enum DragAndDropSourceMode
    {
        None,
        ReadOnly,
        Removable
    }

    public class ListDragAndDropSource : IDragSourceAdvisor
    //, IDropTargetAdvisor
    {
        private ListBox _ListBox;
        private ListDragAndDropSource(ListBox ilb, DragAndDropSourceMode iDragAndDropSourceMode)
        {
            _ListBox = ilb;
            Mode = iDragAndDropSourceMode;
        }

        private DragAndDropSourceMode Mode { get; set; }


        #region Attached Property
        public static readonly DependencyProperty ModeProperty =
          DependencyProperty.RegisterAttached("Mode", typeof(DragAndDropSourceMode), typeof(ListDragAndDropSource),
                                              new PropertyMetadata(DragAndDropSourceMode.None, OnModeChanged));

        public static void SetMode(DependencyObject depObj, DragAndDropSourceMode advisor)
        {
            depObj.SetValue(ModeProperty, advisor);
        }

        public static DragAndDropSourceMode GetMode(DependencyObject depObj)
        {
            return (DragAndDropSourceMode)depObj.GetValue(ModeProperty);
        }

        #endregion

        private static void OnModeChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            ListBox lb = depObj as ListBox;
            if (lb == null)
                return;

            DragAndDropSourceMode ddsm = (DragAndDropSourceMode)args.NewValue;

            if (ddsm != DragAndDropSourceMode.None)
            {
                ListDragAndDropSource ldad = new ListDragAndDropSource(lb, ddsm);
                DragDropManager.SetDragSourceAdvisor(lb, ldad);
                //DragDropManager.SetDropTargetAdvisor(lb, ldad);
            }
            else
            {
                DragDropManager.SetDragSourceAdvisor(lb, null);
                //DragDropManager.SetDropTargetAdvisor(lb, null);
            }

        }

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
          
            ListBoxItem listViewItem = draggedElt.FindAncestor<ListBoxItem>();
            Do.SetData("RawUI", listViewItem);
            if (listViewItem != null)
            {
                listViewItem.IsSelected = true;

                if (_ListBox.SelectedItems.Count == 1)
                {
                    Do.SetData("ListBoxItem", listViewItem);

                    if (this.Mode == DragAndDropSourceMode.Removable)
                        Do.SetData("OriginalSourceIndex", _ListBox.ItemsSource.Cast<object>().Index(listViewItem.DataContext));
                }
                else
                {
                    var items = _ListBox.SelectedItems.Cast<object>().ToList();
                    Do.SetData("SelectedItems", items);

                    if (this.Mode == DragAndDropSourceMode.Removable)
                    {
                        var all = _ListBox.ItemsSource.Cast<object>().ToList();
                        Do.SetData("OriginalSourceIndexes", items.Select(i => all.Index(i)).ToList());
                    }
                }
            }

            return Do;
        }

        void IDragSourceAdvisor.FinishDrag(DataObject draggedElt, DragDropEffects finalEffects, bool DropOk)
        {
            if (this.Mode == DragAndDropSourceMode.ReadOnly)
                return;

            if (DropOk == false)
                return;

            IList l = _ListBox.ItemsSource as IList;
            if (l == null)
                return;

            object oindex = draggedElt.GetData("OriginalSourceIndex");
            if (oindex != null)
            {
                l.RemoveAt((int)oindex);
                return;
            }

            List<int> indexes = draggedElt.GetData("OriginalSourceIndexes") as List<int>;
            if (indexes == null)
                return;

            indexes.OrderByDescending(i => i).Apply(ind => l.RemoveAt(ind));
        }

        bool IDragSourceAdvisor.IsDraggable(UIElement dragElt)
        {
            if (dragElt == _ListBox)
                return false;

            ListBoxItem listViewItem = dragElt.FindAncestor<ListBoxItem>();
            if (listViewItem == null)
                return false;

            return true;
        }

        UIElement IDragSourceAdvisor.GetTopContainer()
        {
            return _ListBox;
            //.FindAncestor<Window>();
        }

        #endregion

        //public UIElement TargetUI { get; set; }

        //public bool ApplyMouseOffset
        //{
        //    get { return false; }
        //}

        //public bool IsValidDataObject(IDataObject obj)
        //{
        //    return false;
        //}

        //public bool OnDropCompleted(IDataObject obj, Point dropPoint, object TargetOriginalsource)
        //{
        //    return false;
        //}

        //public UIElement GetVisualFeedback(IDataObject obj)
        //{
        //    return null;
        //}

        //UIElement IDropTargetAdvisor.GetTopContainer()
        //{
        //    return null;
        //}
    }
}
