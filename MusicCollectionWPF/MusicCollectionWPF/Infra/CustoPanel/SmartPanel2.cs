using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

using MusicCollection.Infra;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.CustoPanel
{
    public partial class SmartPanel2 : PanelWithSize
    {
        private class StoryBoardControlable
        {
            public bool IsCancelled
            {
                get;
                private set;
            }

            public Storyboard Storyboard
            {
                get;
                private set;
            }

            public StoryBoardControlable(Storyboard sb)
            {
                Storyboard = sb;
                IsCancelled = false;
            }

            public void Cancel()
            {
                IsCancelled = true;
            }
        }

        public SmartPanel2()
        {
            //this.RenderTransform = _transform;
            CanVerticallyScroll = true;
            CanHorizontallyScroll = true;

        }

        //     public static readonly DependencyProperty ItemByWidthProperty = DependencyProperty.Register(
        //"ItemByWidth", typeof(double), typeof(SmartPanel2),
        //new FrameworkPropertyMetadata(4D, FrameworkPropertyMetadataOptions.AffectsMeasure |
        //                                        FrameworkPropertyMetadataOptions.AffectsArrange));

        //     public double ItemByWidth
        //     {
        //         get { return (double)GetValue(ItemByWidthProperty); }
        //         set { SetValue(ItemByWidthProperty, value); }
        //     }

        public static readonly DependencyProperty ItemByWidthProperty = DependencyProperty.Register(
"ItemByWidth", typeof(int), typeof(SmartPanel2),
new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                      FrameworkPropertyMetadataOptions.AffectsArrange));

        public int ItemByWidth
        {
            get { return (int)GetValue(ItemByWidthProperty); }
            set { SetValue(ItemByWidthProperty, value); }
        }


        private ListBox ListBoxOwner
        {
            get { return this._ItemsOwner as ListBox; }
        }

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    int Dimension = ItemPerWidth(finalSize.Width);
        //    double w = (finalSize.Height - ItemHeight) / 2;

        //    double offset = 0;

        //    bool CanIdent = (ListBoxOwner != null) && (ItemByWidth >= 1);

        //    int Realstart = (int)Math.Floor(HorizontalOffset)-  _StartIndex;

        //    if (Realstart != 0)
        //    {
        //        if ((!CanIdent) || (ItemByWidth <= 1))
        //        {
        //            offset = (-ItemHeight * (Realstart + ItemByWidth - 1)) / ItemByWidth;
        //        }
        //        else
        //        {
        //             for (int i = 0; i < Realstart; i++)
        //            {
        //                ListBoxItem child = InternalChildren[i] as ListBoxItem;
        //                offset = offset - (child.IsSelected ? ItemHeight : ItemHeight / ItemByWidth);
        //            }
        //        }
        //    }

        //    for (int index = 0; index < InternalChildren.Count; index++)
        //    {
        //        UIElement child = InternalChildren[index];
        //        if (CanIdent && (_StartIndex + index) > 0)
        //        {
        //            ListBoxItem lbi = child as ListBoxItem;
        //            if (lbi.IsSelected)
        //            {
        //                offset += ItemHeight * (ItemByWidth - 1) / ItemByWidth;
        //            }
        //        }

        //        Rect rect = new Rect(new Point(((index) * ItemHeight) / ItemByWidth + offset, w), ItemSize());

        //        child.Arrange(rect);
        //        OnChildCreated(child as ListBoxItem);
        //    }

        //    return finalSize;
        //}

        //private int ItemPerWidth(double Width)
        //{
        //    if (double.IsInfinity(Width))
        //        return int.MaxValue;

        //    return Math.Max(1, (int)(Width * ItemByWidth / (ItemHeight)));
        //}

        //protected override void UpdateScrollInfo(Size finalSize)
        //{
        //    bool HC = false;

        //    int Dimension = ItemPerWidth(finalSize.Width);

        //    if (ViewportWidth != Dimension)               
        //    {
        //        ViewportHeight = finalSize.Height;
        //        ViewportWidth = Dimension;
        //        HC = true;
        //    }

        //    int ItemNumbers = _ItemsOwner.Items.Count;

        //    int val = ItemNumbers - 1 + Dimension;

        //    if (ExtentWidth != val) 
        //    {
        //        ExtentWidth = val;
        //        HC = true;
        //    }

        //    if (HC)
        //    {
        //        HorizontalOffset = CalculateHorizontalOffset(HorizontalOffset);

        //        if (ScrollOwner != null)
        //            ScrollOwner.InvalidateScrollInfo();
        //    }

        //    _StartIndex = (int)Math.Max(0, Math.Floor(HorizontalOffset) - this.ItemByWidth +1);
        //    _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension + this.ItemByWidth);
        //}

        private int StartedDecal
        {
            get { return (int)Math.Floor(HorizontalOffset); }
        }



        private int _FirstIntValue = 0;

        protected override Size ArrangeOverride(Size finalSize)
        {
            int intoffset = _FirstIntValue - ItemByWidth - StartedDecal;
            bool NotFirst = false;
            double w = (finalSize.Height - ItemHeight) / 2;
            double Fact = ItemHeight / ItemByWidth;

            //Console.WriteLine("Start Index {0} intoffset {1}", _StartIndex, intoffset);

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                ListBoxItem lbi = Children[index] as ListBoxItem;
                if (NotFirst)
                {
                    intoffset += lbi.IsSelected ? ItemByWidth : 1;
                }
                else
                {
                    NotFirst = true;
                }

                Rect rect = new Rect(new Point(intoffset * Fact, w), ItemSize());

                lbi.Arrange(rect);
                OnChildCreated(lbi);
            }

            return finalSize;
        }

        private int ItemPerWidth(double Width)
        {
            if (double.IsInfinity(Width))
                return int.MaxValue;

            return Math.Max(1, (int)(Width * ItemByWidth / (ItemHeight)));
        }

        protected override void UpdateScrollInfo(Size finalSize)
        {
            bool HC = false;

            int Dimension = ItemPerWidth(finalSize.Width);

            if (ViewportWidth != Dimension)
            {
                ViewportHeight = finalSize.Height;
                ViewportWidth = Dimension;
                HC = true;
            }

            int ItemNumbers = _ItemsOwner.Items.Count;

            int val = ItemNumbers + ItemByWidth - 1;
   

            ListBox lb = this.ListBoxOwner;
            if (lb == null)
                throw new InvalidOperationException();

            val = val + lb.SelectedItems.Count * (ItemByWidth - 1);

            if (ExtentWidth != val)
            {
                ExtentWidth = val;
                HC = true;
            }

            if (HC)
            {
                HorizontalOffset = CalculateHorizontalOffset(HorizontalOffset);

                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            int StartIndex = 0;
            _FirstIntValue = ItemByWidth;

            while ((_FirstIntValue < StartedDecal) && (StartIndex < ItemNumbers))
            {
                StartIndex++;
                _FirstIntValue += lb.SelectedItems.Contains(lb.Items[StartIndex]) ? ItemByWidth : 1;

            }

            _StartIndex = StartIndex;
            //_VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension + 2); ItemByWidth
            _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension + ItemByWidth + 2); 

            //Console.WriteLine("Start Index {0}", _StartIndex);
        }


        private int IntIndexDeb(int LBIindex, Func<object, bool> IsSelected)
        {
            if (LBIindex == 0)
                return 0;

            ListBox lb = this.ListBoxOwner;
            int Del = 0;
            for (int i = 1; i <= LBIindex; i++)
            {
                Del += IsSelected(lb.Items[i]) ? ItemByWidth : 1;
            }
            return Del;
        }

        private void RefreshDisplay()
        {
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            InvalidateMeasure();
        }

        internal void UpdateDisplay(ListBoxItem lbi, Changes ichanges)
        {
            ListBox lb = this.ListBoxOwner;
            if (lb == null)
            {
                RefreshDisplay();
                return;
            }

            var Fact = lb.ItemContainerGenerator;
            int UnchangedObjectIndex = Fact.IndexFromContainer(lbi);

            HashSet<object> Currentselected = lb.SelectedItems.Cast<object>().ToHashSet();
            HashSet<object> OldSelected = Currentselected.Where(o => !ichanges.AddeddSelected.Contains(o)).Union(ichanges.RemovedSelected).ToHashSet();

            int OldIndex = IntIndexDeb(UnchangedObjectIndex, o => OldSelected.Contains(o));
            int NewIndex = IntIndexDeb(UnchangedObjectIndex, o => Currentselected.Contains(o));

            //Console.WriteLine("Olindex {0} newindex {1}", OldIndex , NewIndex);
            if (NewIndex != OldIndex)
            {
                if (!TrySetHorizontalOffset(HorizontalOffset + NewIndex - OldIndex))
                    RefreshDisplay();
                //Console.WriteLine("Decale {0}",  NewIndex-OldIndex);
                //InvalidateMeasure();
            }
            else
            {
                RefreshDisplay();
                //Console.WriteLine("Decale 0");
            }
        }


        #region Animation


        private double Dist
        {
            get { return (-(ItemHeight * (ItemByWidth - 1)) / ItemByWidth); }
        }


        private ListBoxItem GetListBoxItemMouseOver()
        {
            FrameworkElement fe = Mouse.DirectlyOver as FrameworkElement;
            if (fe == null)
                return null;

            return fe.FindAncestor<ListBoxItem>();
        }

        private void OnChildCreated(ListBoxItem lbic)
        {
            ListBox lb = this.ListBoxOwner;

            ListBoxItem lbi = GetListBoxItemMouseOver();
            if ((lbi == null) || (lbi.IsSelected))
                return;

            int MouseOverIndex = lb.ItemContainerGenerator.IndexFromContainer(lbi);
            int NewLBIIndex = lb.ItemContainerGenerator.IndexFromContainer(lbic);

            if (NewLBIIndex < MouseOverIndex)
            {
                TranslateTransform tt = lbic.RenderTransform as TranslateTransform;
                if (tt.Value == Matrix.Identity)
                {
                    lbic.RenderTransform = new TranslateTransform() { X = Dist };

                }
            }
        }


        private IEnumerable<ListBoxItem> GetPrecedent(ListBoxItem li)
        {
            ListBox lb = ListBoxOwner as ListBox;
            int res = lb.ItemContainerGenerator.IndexFromContainer(li);

            for (int i = 0; i < res; i++)
            {
                ListBoxItem lbi = lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (lbi != null)
                {
                    yield return lbi;
                }
            }
        }

        //private double GetTranslateLI(ListBoxItem lbi)
        //{
        //    TranslateTransform ts = lbi.RenderTransform as TranslateTransform;
        //    return ts.X;
        //}

        private void SetTranslateLI(ListBoxItem lbi, double dist)
        {
            TranslateTransform ts = lbi.RenderTransform as TranslateTransform;

            if (ts.IsFrozen)
            {
                Console.WriteLine("Problem: lbi frozen {0} Animated Prop {1} Dist {2}", lbi, ts.HasAnimatedProperties, dist);
                lbi.RenderTransform = new TranslateTransform() { X = Dist };
            }
            else
            {
                //Console.WriteLine("lbi normal {0} Animated Prop {1} Dist {2}", lbi, ts.HasAnimatedProperties, dist);
                ts.X = dist;
            }
        }

        //private Storyboard Build(List<ListBoxItem> liss, double Distance, TimeSpan dur, TimeSpan ts)
        //{
        //    Storyboard sb = new Storyboard();

        //    foreach (ListBoxItem lis in liss)
        //    {
        //        DoubleAnimation db = new DoubleAnimation();
        //        db.To = Distance;
        //        db.Duration = dur;
        //        db.BeginTime = ts;
        //        db.AccelerationRatio = 0.1;
        //        db.DecelerationRatio = 0.1;

        //        Storyboard.SetTarget(db, lis);
        //        Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.X"));
        //        sb.Children.Add(db);
        //        //lis.IsHitTestVisible = false;
        //    }

        //     //EventHandler handler = null;
        //     //handler = delegate
        //     //{
        //     //    sb.Completed -= handler;
        //     //    var Dist = liss.Select(l => new Tuple<ListBoxItem, double>(l, this.GetTranslateLI(l))); sb.Remove(this); Dist.Apply(li => SetTranslateLI(li.Item1, li.Item2)); liss.Apply(ll => ll.IsHitTestVisible = true); 
        //     //};
        //     //sb.Completed += handler;

        //    sb.Completed += (o, e) => { var Dist = liss.Select(l => new Tuple<ListBoxItem, double>(l, this.GetTranslateLI(l))); sb.Remove(this); Dist.Apply(li => SetTranslateLI(li.Item1, li.Item2)); liss.Apply(ll => ll.IsHitTestVisible = true); };
        //    return sb;
        //}

        private double _MaxFactor = 1;
        private int _RangeFactor = 8;

        private TimeSpan FromOriginalAndDistance(TimeSpan iOr, TimeSpan iDuration, int Distance)
        {
            if (Distance < 0)
                return iOr;

            if (Distance > _RangeFactor)
                return TimeSpan.FromMilliseconds(iOr.TotalMilliseconds + iDuration.TotalMilliseconds * _MaxFactor);

            return TimeSpan.FromMilliseconds(iOr.TotalMilliseconds + ((iDuration.TotalMilliseconds * _MaxFactor * (Distance) / _RangeFactor)));
        }

        private StoryBoardControlable Build(ListBoxItem liss, double Distance, TimeSpan dur, TimeSpan ts)
        {
            Storyboard sb = new Storyboard();

            ListBox lb = ListBoxOwner as ListBox;
            int res = lb.ItemContainerGenerator.IndexFromContainer(liss);
            List<Tuple<ListBoxItem, double>> tobe = new List<Tuple<ListBoxItem,double>>();

            for (int i = _StartIndex; i < _StartIndex+InternalChildren.Count; i++)
            {
                ListBoxItem lis = lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (lis == null)
                    continue;

                double target = (i < res) ? Distance : 0;
                
                DoubleAnimation db = new DoubleAnimation();
                db.To = target;
                db.Duration = dur;
                //db.BeginTime = ts;
                db.BeginTime = FromOriginalAndDistance(ts,dur,res - i);
                db.AccelerationRatio = 0.1;
                db.DecelerationRatio = 0.1;

                tobe.Add(new Tuple<ListBoxItem,double>(lis,target));

                Storyboard.SetTarget(db, lis);
                Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.X"));
                sb.Children.Add(db);
            }

            StoryBoardControlable resfinal = new StoryBoardControlable(sb);

            EventHandler handler = null;
            handler = delegate
            {
                resfinal.Storyboard.Completed -= handler;
                resfinal.Storyboard.Remove(this);

                if (!resfinal.IsCancelled)
                {
                    if (object.ReferenceEquals(resfinal,resfinal.Storyboard))
                        _EnterSB = null;

                    tobe.Apply(li => SetTranslateLI(li.Item1, li.Item2));
                }
            };
            sb.Completed += handler;

            return resfinal;
        }

        private StoryBoardControlable _EnterSB;

        private void StopCurrentAnimation()
        {
            if (_EnterSB != null)
            {
                _EnterSB.Storyboard.Stop(this);
                _EnterSB.Storyboard.Remove(this);
                _EnterSB.Cancel();
                _EnterSB = null;
            }
        }

        //private void StopAnimation(ListBoxItem lbi)
        //{
        //    TranslateTransform tt = lbi.RenderTransform as TranslateTransform;
        //    bool res = tt.HasAnimatedProperties;
        //    Console.WriteLine("Has animation {0}", res);
        //    try
        //    {
        //        tt.BeginAnimation(TranslateTransform.XProperty, null);
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Echec Stop animation {0}", lbi);
        //    }
        //}

        //private void StopAnimationAndSetOffsets()
        //{
        //    ListBoxItem lbi = GetListBoxItemMouseOver();
        //    var chills = GetPrecedent(lbi).ToList();
        //    //chills.Apply(StopAnimation);
        //    chills.Apply(l => this.SetTranslateLI(l, Dist));
        //    SetTranslateLI(lbi, 0);
        //}

        internal void OnEnter(object sender, EventArgs ea)
        {
            if (ItemByWidth == 1)
                return;

            ListBoxItem li = sender as ListBoxItem;

            //Console.WriteLine("OnEnter {0} - {1}",li,li.IsSelected);

            if (li.IsSelected)
                return;

            //Console.WriteLine("Enter {0}", li);

            StopCurrentAnimation();

            //if (_EnterSB != null)
            //{
            //    _EnterSB.Stop(this);
            //    _EnterSB.Remove(this);
            //    _EnterSB = null;
            //}
            
            //var res = GetPrecedent(li).ToList();
            //_EnterSB = Build(res, Dist, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));
            ////_EnterSB.Completed += (o, e) => { _EnterSB = null; };

            _EnterSB = Build(li, Dist, TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(300));
  
            //EventHandler handler = null;
            //handler = delegate
            //{
            //    _EnterSB = null;
            //};
            //_EnterSB.Completed += handler;

            _EnterSB.Storyboard.Begin(this,true);

        }

        internal void OnLeave(object sender, EventArgs ea)
        {
            if (ItemByWidth == 1)
                return;

            ListBoxItem li = sender as ListBoxItem;

            //Console.WriteLine("OnLeave {0} - {1}", li, li.IsSelected);

            if (li.IsSelected)
                return;

            //Console.WriteLine("Leave {0}", li);

            FrameworkElement fel = VisualTreeHelper.GetChild(li, 0) as FrameworkElement;

            while ((fel != null) && (fel.ContextMenu == null))
            {
                fel = VisualTreeHelper.GetChild(fel, 0) as FrameworkElement;
            }


            if (fel != null)
            {
                if (fel.ContextMenu.IsOpen)
                {
                    fel.ContextMenuClosing += new ContextMenuEventHandler(fel_ContextMenuClosing);
                    return;
                }
            }

            StopCurrentAnimation();

            //if (_EnterSB != null)
            //{
            //    _EnterSB.Stop(this);
            //    _EnterSB.Remove(this);
            //    _EnterSB = null;
            //}

            //var res = GetPrecedent(li).ToList();

            //Storyboard sb = Build(res, 0, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
            _EnterSB = Build(li, 0, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));

            _EnterSB.Storyboard.Begin(this, true);


            //_EnterSB = sb;
        }

        private void fel_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement fel = sender as FrameworkElement;
            fel.ContextMenuClosing -= new ContextMenuEventHandler(fel_ContextMenuClosing);

            ListBoxItem lbi = WPFHelper.FindAncestor<ListBoxItem>(fel);
            if (lbi == null)
                return;

            OnLeave(lbi, EventArgs.Empty);
        }

        private void OnSelect(ListBoxItem lbi)
        {
            if (lbi == null)
                return;

            //Console.WriteLine("OnSelect {0} - {1}", lbi, lbi.IsSelected);

            //Console.WriteLine("OnSelect {0}", lbi);

            GetPrecedent(lbi).Apply(li => this.SetTranslateLI(li, 0));
        }

        private void OnUnSelect(ListBoxItem lbi)
        {
            if (lbi == null)
                return;

            //Console.WriteLine("OnUnSelect {0}", lbi);

            if (lbi.IsMouseOver)
            {
                GetPrecedent(lbi).Apply(li => this.SetTranslateLI(li, Dist));
            }
        }


        internal void ApplyChanges(ListBoxItem lbi, Changes ichanges)
        {
            StopCurrentAnimation();

            //if (_EnterSB == null)
            //{
                SynchronizeApplyChanges(lbi, ichanges);
            //}
            //else
            //{
            //    //////Console.WriteLine("No Wait end animation");
            //    ////_EnterSB.Stop(this);
            //    ////_EnterSB.Remove(this);
            //    ////_EnterSB = null;

            //    //StopAnimationAndSetOffsets();
            //    //SynchronizeApplyChanges(lbi, ichanges);
            //    EventHandler handler = null;
            //    handler = delegate
            //    {
            //        SynchronizeApplyChanges(lbi, ichanges);
            //    };

            //    _EnterSB.Completed += handler;
            //    //StopAnimationAndSetOffsets();
            //    //SynchronizeApplyChanges(lbi, ichanges);
            //}
        }

        private void SynchronizeApplyChanges(ListBoxItem lbi, Changes ichanges)
        {
            UpdateDisplay(lbi, ichanges);

            if (ichanges.AddedSelectedListBoxItem.Contains(lbi))
            {
                OnSelect(lbi);
            }
            else if (ichanges.RemovedSelectedListBoxItem.Contains(lbi))
            {
                OnUnSelect(lbi);
            }
            else
            {
                if (lbi.IsSelected)
                    OnSelect(lbi);
            }
        }


        #endregion

    }
}
