using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using MusicCollection.Infra;

namespace MusicCollectionWPF.CustoPanel
{
    //public class ListBoxItemEventArgs: EventArgs
    //{
    //    public ListBoxItem ListBoxItem
    //    {
    //        get;
    //        private set;
    //    }

    //    public ListBoxItemEventArgs(ListBoxItem iListBoxItem)
    //    {
    //        ListBoxItem = iListBoxItem;
    //    }
    //}


    public abstract class CoreVirtualizingPanelBase : VirtualizingPanel
    {
        protected int _StartIndex = 0;
        protected int _VisibileIndexes = 0;

        protected ItemsControl _ItemsOwner { get; private set; }

        //public EventHandler<ListBoxItemEventArgs> OnListBoxItemCreated;

        public static readonly DependencyProperty Z_IndexProperty = DependencyProperty.RegisterAttached("Z_Index", typeof(int), typeof(CoreVirtualizingPanelBase), new PropertyMetadata(0, ZIndexPropertyChangedCallback));

        public static void SetZ_Index(DependencyObject element, int value)
        {
            element.SetValue(CoreVirtualizingPanelBase.Z_IndexProperty, value);
        }

        public static int GetZ_Index(DependencyObject element)
        {
            return (int)element.GetValue(CoreVirtualizingPanelBase.Z_IndexProperty);
        }

        static private void ZIndexPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBoxItem lbi = d as ListBoxItem;
            if (lbi == null)
                return;

            CoreVirtualizingPanelBase sm2 = VisualTreeHelper.GetParent(d) as CoreVirtualizingPanelBase;
            if (sm2 == null)
                return;

            sm2.ChangingPosition(lbi);
        }

        private void ChangingPosition(ListBoxItem lbi)
        {
            RemoveVisualChild(lbi);
            AddVisualChild(lbi);
        }

        //override get 
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var List = Children.Cast<UIElement>().OrberWithIndexBy((i, o) => (o == null) ? 0 : -i + CoreVirtualizingPanelBase.GetZ_Index(o));

            return List.ElementAt(index);

        }

        protected override void OnInitialized(EventArgs e)
        {
            _ItemsOwner = ItemsControl.GetItemsOwner(this) as ItemsControl;
        }

        protected IDisposable VirtualizeItems()
        {
            IItemContainerGenerator generator = _ItemsOwner.ItemContainerGenerator;

            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(_StartIndex);
            int childIndex = startPos.Offset == 0 ? startPos.Index : startPos.Index + 1;
            //using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            IDisposable res = generator.StartAt(startPos, GeneratorDirection.Forward, true);
            //{
                for (int i = _StartIndex; i < _StartIndex + _VisibileIndexes; i++, childIndex++)
                {
                    bool isNewlyRealized;
                    ListBoxItem child = generator.GenerateNext(out isNewlyRealized) as ListBoxItem;
                    if (isNewlyRealized)
                    {
                        //if (child.Style == null)
                        //{
                        //    child.Style = _ItemsOwner.ItemContainerStyle;
                        //}

                        if (childIndex >= InternalChildren.Count)
                        {
                            AddInternalChild(child);
                        }
                        else
                        {
                            InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                        //OnChildCreated(child);
                    }
                    else
                    {
                        //to handle update/remove scenario
                        int CurrentIndex = InternalChildren.IndexOf(child);
                        if ((CurrentIndex != -1) && (CurrentIndex != childIndex))
                        {
                            RemoveInternalChildRange(CurrentIndex, 1);
                            InsertInternalChild(childIndex, child);
                        }
                    }
                }
            //}

                return res;
        }

        protected void CleanupItems()
        {
            if (!this.IsLoaded)
                return;

            IItemContainerGenerator generator = _ItemsOwner.ItemContainerGenerator;
            for (int i = InternalChildren.Count - 1; i >= 0; i--)
            {
                GeneratorPosition position = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(position);
                if (itemIndex < _StartIndex || itemIndex > _StartIndex + _VisibileIndexes - 1)
                {
                    if (itemIndex != -1)
                        generator.Remove(position, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

       

       
    }
}
