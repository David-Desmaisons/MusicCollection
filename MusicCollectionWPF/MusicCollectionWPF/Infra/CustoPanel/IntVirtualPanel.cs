using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MusicCollectionWPF.CustoPanel
{
    public class IntVirtualPanel : VirtualRectPanelBase
    {
        public static readonly DependencyProperty RangeNumberProperty = DependencyProperty.Register(
   "RangeNumber", typeof(int), typeof(IntVirtualPanel),
   new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                       FrameworkPropertyMetadataOptions.AffectsArrange));

        public int RangeNumber
        {
            get { return (int)GetValue(RangeNumberProperty); }
            set { SetValue(RangeNumberProperty, value); }
        }

        private double _OverrideHeight = 0;
        protected override double CurrentItemHeight
        {
            get { return (_OverrideHeight == 0) ? ItemHeight : _OverrideHeight; }
        }


        public IntVirtualPanel()
        {
            CanVerticallyScroll = false;
            CanHorizontallyScroll = false;
            ViewportWidth = 1;
        }

        protected override Size ItemSize()
        {
            return base.ItemSize();
        }

        private int GetNumberEntireLign(double Heigth)
        {
            int res = (int) Math.Floor(Heigth / SpaceHeigthY);
            return res;
        }

        private class CalcRes
        {
            public int ItemPerView{get{return LignPerView*ColPerView;}}
           
            public int LignPerView   {  set;  get;}
            public int ColPerView {  set;  get;}
          
            public int NbofViews{  set;  get;}          
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            CalcRes res = GetTotalNbOfLignItemByLignItembyPage(finalSize);

            double w = (finalSize.Width - res.ColPerView * SpaceHeigthX) / 2 + (HorizontalOffset+_ScrollingOffset) * SpaceHeigthX*res.ColPerView;
            //double w = (finalSize.Width - res.ColPerView * SpaceHeigthX) / 2 + (HorizontalOffset + _ScrollingOffset ) * SpaceHeigthX * res.ColPerView;
            double z = InitialYGap;

            if (this._NeedUpdateScrollbar)
            {
                _NeedUpdateScrollbar = false; 
                _transform.X = -1 * HorizontalOffset * CollPerView * SpaceHeigthX;         
                
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();

            }
  


            for (int index = 0; index < InternalChildren.Count; index++)
            {
                int myfilteredindex = index % ItemPerView;
                int x = (myfilteredindex) % res.ColPerView;
                int y = (myfilteredindex) / res.ColPerView;
                int zx = (index) / res.ItemPerView;

                Rect rect = new Rect(new Point(w + x * SpaceHeigthX + zx * SpaceHeigthX * res.ColPerView, z + y * SpaceHeigthY), ItemSize());
                InternalChildren[index].Arrange(rect);

            }

            return finalSize;
        }

        private CalcRes GetTotalNbOfLignItemByLignItembyPage(Size size)
        {
            CalcRes res = new CalcRes();

            Tuple<int,int> parres= ItemPerWidth(size.Width);
            res.ColPerView=parres.Item1;

            res.LignPerView = (RangeNumber != 0) ? RangeNumber : GetNumberEntireLign(size.Height);

            res.NbofViews = (int)Math.Ceiling((double)parres.Item2/res.LignPerView);
            int sbts = (int)Math.Ceiling((double)_ItemsOwner.Items.Count / res.ItemPerView); 

            return res; 
        }

        private int OffsetFromPosition(int pos,CalcRes status)
        {
            return pos / status.ItemPerView;
        }

        private int ItemPerView
        {
            get;
            set;
        }

        private int CollPerView
        {
            get;
            set;
        }


        private bool _NeedUpdateScrollbar = false;

        protected override void UpdateScrollInfo(Size finalSize)
        {
           
            if (RangeNumber != 0)
            {
                _OverrideHeight = (finalSize.Height / ((1 + YSpace) * RangeNumber)) ;
            }
            else
            {
                _OverrideHeight = 0;
            }

            CalcRes res = GetTotalNbOfLignItemByLignItembyPage(finalSize);
            //int Nblign = (RangeNumber == 0) ? res.LignPerView : RangeNumber;

            CollPerView = res.ColPerView;

            if (ExtentWidth != res.NbofViews)
            {
                ExtentWidth = res.NbofViews;

                var olh = HorizontalOffset;

                HorizontalOffset = Normalizeoffset(OffsetFromPosition(_StartIndex, res));

                _NeedUpdateScrollbar = true;
            }

            ItemPerView = res.ItemPerView;

            _StartIndex = (int)HorizontalOffset * res.ItemPerView;
            _VisibileIndexes = Math.Min(_ItemsOwner.Items.Count - _StartIndex, ItemPerView);

        }

        #region dummy mouse move

        public override void MouseWheelDown()
        {
        }

        public override void MouseWheelLeft()
        {
        }

        public override void MouseWheelRight()
        {
        }

        public override void MouseWheelUp()
        {
        }

        #endregion
     
        #region vertical Mouv

        public override void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 1,true);
        }

        public override void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + 1,true);
        }

        public override void PageLeft()
        {
            //SetHorizontalOffset(HorizontalOffset - 2);
            SetHorizontalOffset(HorizontalOffset - 1);
        }

        public override void PageRight()
        {
            //SetHorizontalOffset(HorizontalOffset + 2);
            SetHorizontalOffset(HorizontalOffset + 1);
        }

        private double Normalizeoffset(double offset)
        {
            if (offset + ViewportWidth > ExtentWidth)
                return Math.Round(ExtentWidth - ViewportWidth);

            else if (offset < 0)
                return 0;

            return Math.Round(offset);
        }

        public override void SetHorizontalOffset(double offset)
        {
            SetHorizontalOffset(offset, false);   
        }

        private void SetHorizontalOffset(double offset, bool DoScroll)
        {
            if (_IsScrolling)
                return;

            _IsScrolling = true;

            double old = HorizontalOffset;
            offset = Normalizeoffset(offset);
            double offsetx = -offset * CollPerView * SpaceHeigthX;
            double delta = old - offset;

            if ((Math.Abs(delta) > 1) || !DoScroll)
            {
                _transform.X = offsetx;
                HorizontalOffset = offset;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
  
                _IsScrolling = false;
                InvalidateMeasure(); //neededd
              
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation(offsetx, new Duration(TimeSpan.FromSeconds(0.3)));
                anim.AccelerationRatio = 0.2;
                anim.DecelerationRatio = 0.2;
                PropertyPath p = new PropertyPath("(0).(1)", RenderTransformProperty, TranslateTransform.XProperty);
                Storyboard.SetTargetProperty(anim, p);
                Storyboard sb = new Storyboard();
                sb.Children.Add(anim);
                EventHandler handler = null;
                handler = delegate
                {
                    sb.Completed -= handler;
                    sb.Remove(this);
                    _transform.X = offsetx;
                    _IsScrolling = false;
                    _ScrollingOffset = 0;
                    HorizontalOffset = offset;
                    InvalidateMeasure();

                    if (ScrollOwner != null)
                        ScrollOwner.InvalidateScrollInfo();
                };
                sb.Completed += handler;

                if (delta < 0)
                    _ScrollingOffset = (int)(delta);

                HorizontalOffset = offset;
                InvalidateMeasure();

                Action Ba = () => sb.Begin(this, true);
                this.Dispatcher.BeginInvoke(Ba, DispatcherPriority.Input);
            }


        }

        protected override bool InhibitCleanUp
        {
            get { return _IsScrolling; }
        }

        private bool _IsScrolling = false;
        private int _ScrollingOffset = 0; 

        #endregion 

      

        #region dummy horizontal mouv

        public override void PageDown()
        {
        }

        public override void PageUp()
        {
        }

        public override void SetVerticalOffset(double offset)
        {
        }

        public override void LineUp()
        {
        }

        public override void LineDown()
        {
        }

        #endregion
    }
}
