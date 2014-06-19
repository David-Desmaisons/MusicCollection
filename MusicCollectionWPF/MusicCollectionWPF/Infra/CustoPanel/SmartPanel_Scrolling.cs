using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MusicCollectionWPF
{
    public partial class SmartPanel
    {
        //TranslateTransform _transform = new TranslateTransform();

        private double LineUpHeigth
        {
            get
            {
                return 1;
            }
        }

        #region Vertical movement

        public override void LineUp()
        {
            if (CanContentScroll)
            {
                PageUp();
                return;
            }

            PrivateSetVerticalOffset(VerticalOffset - LineUpHeigth);// - ItemHeight / 2);
        }

        public override void LineDown()
        {
            if (CanContentScroll)
            {
                PageDown();
                return;
            }

            PrivateSetVerticalOffset(VerticalOffset + LineUpHeigth);//+ ItemHeight / 2);
        }

        public override void PageUp()
        {
            PrivateSetVerticalOffset(VerticalOffset - ViewportHeight);///2);
        }

        public override void PageDown()
        {
            PrivateSetVerticalOffset(VerticalOffset + ViewportHeight);//2);

        }

        public override void MouseWheelUp()
        {
            LineUp();
        }

        public override void MouseWheelDown()
        {
            LineDown();
        }

        private bool _IsScrolling = false;


        public override void SetVerticalOffset(double offset)
        {
            PrivateSetVerticalOffset(Math.Truncate(offset));
        }

        //private void PrivateSetVerticalOffset(double offset)
        //{
        //    if (_IsScrolling)
        //        return;

        //    //if (CanContentScroll)
        //    //    offset = Math.Floor((offset - this.InitialYGap) / LineUpHeigth) * LineUpHeigth;

        //    //offset = Math.Truncate(offset / LineUpHeigth) * LineUpHeigth;

        //    double oldoff = VerticalOffset;
        //    double newoffset = CalculateVerticallOffset(offset);

        //    double delta = Math.Abs(oldoff - newoffset);

        //    if (delta == 0)
        //        return;

        //    //if (delta <= ViewportHeight / 2)
        //    //{
        //    //    _IsScrolling = true;



        //    //    //DoubleAnimation anim = new DoubleAnimation(-newoffset, new Duration(TimeSpan.FromMilliseconds(500*delta/ItemHeight+1)));
        //    //    DoubleAnimation anim = new DoubleAnimation(-newoffset, new Duration(TimeSpan.FromMilliseconds(300)));
        //    //    PropertyPath p = new PropertyPath("(0).(1)",
        //    //    RenderTransformProperty, TranslateTransform.YProperty);
        //    //    Storyboard.SetTargetProperty(anim, p);
        //    //    Storyboard sb = new Storyboard();
        //    //    sb.Children.Add(anim);
        //    //    EventHandler handler = null;
        //    //    handler = delegate
        //    //    {
        //    //        sb.Completed -= handler;
        //    //        sb.Remove(this);
        //    //        _transform.Y = -newoffset;
        //    //        VerticalOffset = newoffset;
        //    //        _IsScrolling = false;

        //    //        if (ScrollOwner != null)
        //    //            ScrollOwner.InvalidateScrollInfo();

        //    //        //InvalidateMeasure();
        //    //    };
        //    //    sb.Completed += handler;
        //    //    sb.Begin(this, true);
        //    //}
        //    //else
        //    //{
        //    //_transform.Y = -newoffset * SpaceHeigthY;
        //    _transform.Y = -newoffset * _SpaceHeigthY;
        //    VerticalOffset = newoffset;
        //    if (ScrollOwner != null)
        //        ScrollOwner.InvalidateScrollInfo();
        //    //}



        //    //_transform.Y = -VerticalOffset;

        //    // Force us to realize the correct children
        //    InvalidateMeasure(); //needed for virtualization
        //}

        protected override bool InhibitCleanUp
        {
            get { return _IsScrolling; }
        }

        private int _ScrollingOffset = 0;

        private void PrivateSetVerticalOffset(double offset)
        {
            if (_IsScrolling)
                return;

            double oldoff = VerticalOffset;
            double newoffset = CalculateVerticallOffset(offset);

            double delta = Math.Abs(oldoff - newoffset);

            if (delta == 0)
                return;

            //if (delta> 1)
            if (delta > ViewportHeight)
            {
                _transform.Y = -newoffset * _SpaceHeigthY;
                VerticalOffset = newoffset;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();

                _IsScrolling = false;
                _ScrollingOffset = 0;
      
                InvalidateMeasure(); //needed for virtualization
                return;
            }


            DoubleAnimation anim = new DoubleAnimation(-newoffset * _SpaceHeigthY, new Duration(TimeSpan.FromSeconds(0.2)));
            PropertyPath p = new PropertyPath("(0).(1)", RenderTransformProperty, TranslateTransform.YProperty);
            Storyboard.SetTargetProperty(anim, p);
            Storyboard sb = new Storyboard();
            sb.Children.Add(anim);
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;
                sb.Remove(this);
                _transform.Y = -newoffset * _SpaceHeigthY;
                VerticalOffset = newoffset;
                _IsScrolling = false;
                _ScrollingOffset = 0;
                //CleanupItems();
                InvalidateMeasure();

                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();

                //InvalidateMeasure();
            };
            sb.Completed += handler;
            //sb.Begin(this, true);
            _IsScrolling = true;
            //if (oldoff - newoffset == -1)
            //    _ScrollingOffset = -1;
            if (oldoff - newoffset <0)
                _ScrollingOffset = (int)(oldoff - newoffset);

            VerticalOffset = newoffset;
            InvalidateMeasure();
            //sb.Begin(this, true);
            ////this.Dispatcher.BeginInvoke((Action)InvalidateMeasure,null);

            Action Ba = () => sb.Begin(this, true);
            this.Dispatcher.BeginInvoke(Ba, DispatcherPriority.Input);
        }


        private double CalculateVerticallOffset(double offset)
        {
            if (offset < 0 || ViewportHeight >= ExtentHeight)
            {
                offset = 0;
            }
            else
            {
                if (offset + ViewportHeight >= ExtentHeight)
                {
                    offset = ExtentHeight - ViewportHeight;
                }
            }
            return offset;
        }


        #endregion

        #region Horizontal movement

        public override void LineLeft()
        {
        }

        public override void LineRight()
        {
        }

        public override void PageLeft()
        {
        }

        public override void PageRight()
        {
        }

        public override void MouseWheelLeft()
        {
        }

        public override void MouseWheelRight()
        {
        }

        public override void SetHorizontalOffset(double offset)
        {

        }


        //public Rect MakeVisible(Visual visual, Rect rectangle)
        //{
        //    return rectangle;
        //}

        #endregion

    }
}
