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
                                              new PropertyMetadata(false, OnModeChanged));

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

            if (ddsm!=DragAndDropSourceMode.None)
            {
                ListDragAndDropSource ldad = new ListDragAndDropSource(lb,ddsm);
                DragDropManager.SetDragSourceAdvisor(lb, ldad);
            }
            else
            {
                DragDropManager.SetDragSourceAdvisor(lb, null);
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
            Do.SetData("RawUI", draggedElt);

            ListBoxItem listViewItem = draggedElt.FindAncestor<ListBoxItem>();
            if (listViewItem != null)
            {
                Do.SetData("ListBoxItem", listViewItem);

                if (this.Mode==DragAndDropSourceMode.Removable)
                    Do.SetData("OriginalSourceIndex", _ListBox.ItemsSource.Cast<object>().Index(listViewItem.DataContext));
            }

            return Do;
        }

        void IDragSourceAdvisor.FinishDrag(DataObject draggedElt, DragDropEffects finalEffects, bool DropOk)
        {
            if (this.Mode == DragAndDropSourceMode.ReadOnly)
                return;

            if (DropOk == false)
                return;

            object oindex = draggedElt.GetData("OriginalSourceIndex");
            if (oindex == null)
                return;

            IList l = _ListBox.ItemsSource as IList;
            if (l == null)
                return;

            l.RemoveAt((int)oindex);
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
            return _ListBox.FindAncestor<Window>();
        }

        #endregion  
    }
}
