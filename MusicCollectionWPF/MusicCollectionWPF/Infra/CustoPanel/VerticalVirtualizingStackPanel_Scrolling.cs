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

namespace MusicCollectionWPF.CustoPanel
{
    public partial class VerticalVirtualizingStackPanel
    {
        #region DependencyProperty

        public static readonly DependencyProperty TimeToTransitionProperty = DependencyProperty.Register("TimeToTransition",
                typeof(TimeSpan), typeof(VerticalVirtualizingStackPanel), new PropertyMetadata(TimeSpan.FromSeconds(0.2)));

        public TimeSpan TimeToTransition
        {
            get { return (TimeSpan)GetValue(TimeToTransitionProperty); }
            set { SetValue(TimeToTransitionProperty, value); }
        }

        #endregion

        private double BigDelta
        {
            get { return ViewportHeight; }
        }

        private double SmallDelta
        {
            get { return 1; }
        }

        private double RealOffet(double Off)
        {
            return Off * ItemHeight;
        }

        public override void LineRight()
        {
            //SetHorizontalOffset(HorizontalOffset + SmallDelta);
        }

        public override void LineLeft()
        {
            //SetHorizontalOffset(HorizontalOffset - SmallDelta);
        }

        public override void PageLeft()
        {
            //SetHorizontalOffset(HorizontalOffset - BigDelta);
        }

        public override void PageRight()
        {
            //SetHorizontalOffset(HorizontalOffset + BigDelta);
        }

        public override void LineUp()
        {
            SetVerticalOffset(VerticalOffset - SmallDelta);
        }

        public override void PageUp()
        {
            SetVerticalOffset(VerticalOffset - BigDelta);
        }

        public override void PageDown()
        {
            SetVerticalOffset(VerticalOffset + BigDelta);
        } 
        
        public override void LineDown()
        {
            SetVerticalOffset(VerticalOffset + SmallDelta);
        }

        public override void MouseWheelLeft()
        {
        }

        public override void MouseWheelRight()
        {
        }

        public override void MouseWheelUp()
        {
            LineUp();
        }

        public override void MouseWheelDown()
        {
            LineDown();
        }
      
        protected override bool InhibitCleanUp
        {
            get { return _IsScrolling; }
        }

        private bool _IsScrolling = false;
        private int _ScrollingOffset = 0;

        private bool TrySetVerticalOffset(double offset)
        {
            if (_IsScrolling)
                return false;

            _IsScrolling = true;

            double oldoff = VerticalOffset;
            double newoffset = CalculateVerticalOffset(offset);

            double delta = Math.Abs(oldoff - newoffset);

            if (delta == 0)
            {
                _IsScrolling = false;
                return false;
            }

            if (delta > ViewportHeight)
            {
                VerticalOffset = newoffset;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
 
                _IsScrolling = false;
                _ScrollingOffset = 0;
                InvalidateMeasure();
               
            }
            else
            {
                double deltatrans =  (oldoff - newoffset) * ItemHeight;
                DoubleAnimation anim = new DoubleAnimation(deltatrans, new Duration(TimeToTransition));
                PropertyPath p = new PropertyPath("(0).(1)", RenderTransformProperty, TranslateTransform.YProperty);
                Storyboard.SetTargetProperty(anim, p);
                Storyboard sb = new Storyboard();
                sb.Children.Add(anim);
                EventHandler handler = null;
                handler = delegate
                {
                    sb.Completed -= handler;
                    sb.Remove(this);
                    _transform.Y = deltatrans;
                    VerticalOffset = newoffset;

                    _IsScrolling = false;
                    _ScrollingOffset = 0;

                    InvalidateMeasure();
                    //_transform.X = 0;

                    if (ScrollOwner != null)
                        ScrollOwner.InvalidateScrollInfo();
                };
                sb.Completed += handler;
                _IsScrolling = true;

                if (oldoff - newoffset > 0)
                    _ScrollingOffset = (int)(newoffset - oldoff);

                VerticalOffset = newoffset;
                InvalidateMeasure();

                Action Ba = () => sb.Begin(this, true);
                this.Dispatcher.BeginInvoke(Ba, DispatcherPriority.Input);
            }
            return true;
        }

        public override void SetHorizontalOffset(double offset)
        {
            //TrySetHorizontalOffset(offset);
        }

        public override void SetVerticalOffset(double offset)
        {
            TrySetVerticalOffset(offset);
        }

        private double CalculateVerticalOffset(double offset)
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

            return Math.Floor(offset);
        }
    }
}
