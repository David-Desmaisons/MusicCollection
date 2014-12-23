using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    public class AlbumPresenterUserControl : AlbumPresenterBase
        //, IAlbumPresenter, IDisposable
    {
        #region attached property

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
            typeof(double), typeof(AlbumPresenterUserControl), new FrameworkPropertyMetadata(200D,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty SizerProperty = DependencyProperty.Register("Sizer",
            typeof(int), typeof(AlbumPresenterUserControl), new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));
        //, SizeChangedCallback));

        public int Sizer
        {
            get { return (int)GetValue(SizerProperty); }
            set { SetValue(SizerProperty, value); }
        }

        #endregion

        //#region Size 
        
        
        ////static private void SizeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        ////{

        ////    AlbumPresenterUserControl apuc = d as AlbumPresenterUserControl;
        ////    //apuc.OnSizeChanged(e);
        ////}

        ////protected virtual void OnSizeChanged(DependencyPropertyChangedEventArgs e)
        ////{
        ////}

        //#endregion
       
        //#region CleanUp

        ////public override void Dispose()
        ////{
        ////    base.Dispose();
        ////    //Filter = null;
        ////    Sorter = null;
        ////}

        ////private void Shut(object sender, EventArgs ev)
        ////{
        ////    //Dispose();
        ////    this.Dispatcher.ShutdownStarted -= Shut;
        ////}

        ////public AlbumPresenterUserControl()
        ////{
        ////    this.Dispatcher.ShutdownStarted += Shut;
        ////}

        //#endregion


        protected bool IsCtrlPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl)
                    || Keyboard.IsKeyDown(Key.RightCtrl);
            }
        }

       

        protected virtual void ApplyChanges(ListBoxItem origin, Changes ichanges)
        {
            if (ichanges == null)
                return;

            ichanges.ApplyChanges();
       }


        protected Changes GetChanges(ListBoxItem lbi)
        {
            if (lbi == null)
                return null;

            Changes ch = new Changes(MyDisc);

            if (IsCtrlPressed)
            {
                if (lbi.IsSelected)
                    ch.AddRemoved(lbi);
                else
                    ch.AddSelected(lbi);

                return ch;
            }

            if (MyDisc.SelectedItems.Contains(lbi.DataContext))
            {
                if (MyDisc.SelectedItems.Count == 1)
                {
                    ch.AddRemoved(lbi);
                }
                else
                {
                    var needtobeclean = MyDisc.SelectedItems.Cast<object>().Where(u => u != lbi.DataContext).ToList();
                    needtobeclean.Apply(a => ch.AddRemoved(a));
                }
                return ch;
            }

            ch.AddSelected(lbi);
            var needtobeclean2 = MyDisc.SelectedItems.Cast<object>().Where(u => u != lbi.DataContext).ToList();
            needtobeclean2.Apply(a => ch.AddRemoved(a));

            return ch;
        }


        protected void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ListBoxItem lbi = sender as ListBoxItem;
            ApplyChanges(lbi, GetChanges(lbi));
        }


        protected void ListDisc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BasicListDisc_SelectionChanged(sender, e);
            SelectionChanged(sender, e);
        }


        //protected void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;

        //    ListBoxItem lbi = sender as ListBoxItem;

        //    if (lbi == null)
        //        return;
        //    //ListDisc

        //    if (IsCtrlPressed)
        //    {
        //        lbi.IsSelected = !lbi.IsSelected;
        //        return;
        //    }

        //    if (MyDisc.SelectedItems.Contains(lbi.DataContext))
        //    {
        //        if (MyDisc.SelectedItems.Count == 1)
        //        {
        //            lbi.IsSelected = false;
        //        }
        //        else
        //        {
        //            var needtobeclean = MyDisc.SelectedItems.Cast<object>().Where(u => u != lbi.DataContext).ToList();
        //            needtobeclean.Apply(a => MyDisc.SelectedItems.Remove(a));
        //        }
        //        return;
        //    }

        //    MyDisc.SelectedItems.Clear();
        //    lbi.IsSelected = true;
        //}

        private void BasicListDisc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlockedReentrance)
                return;

            if (IsCtrlPressed)
                return;

            ListBox lb = sender as ListBox;

            object o = (e.AddedItems.Count == 1) ? e.AddedItems[0] : (e.RemovedItems.Count == 1) ? e.RemovedItems[0] : null;

            if (o == null)
                return;


            lb.SelectionChanged -= ListDisc_SelectionChanged;

            bool needtoselectitem = lb.SelectedItems.Count != 0;
            bool AlreadySelected = lb.SelectedItems.Contains(o);

            lb.SelectedItems.Clear();

            if (needtoselectitem)
            {
                lb.SelectedItems.Add(o);
            }         

            lb.SelectionChanged += ListDisc_SelectionChanged;
         
        }

        protected virtual void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

     

        protected void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;

            if (lbi.IsSelected)
            {
                lbi.Focus();
                e.Handled = true;
                return;
            }

            lbi.Focus();

            if (!IsCtrlPressed)
            {
                MyDisc.SelectedItems.Clear();
            }

            lbi.IsSelected = true;

            e.Handled = true;

        }

     
    }
}
