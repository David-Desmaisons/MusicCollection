using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.CustoPanel
{
    public partial class SmartPanel2
    {
        //private TranslateTransform _transform = new TranslateTransform();   
        
        private double BigDelta
        {
            //get { return ViewportWidth; } 1
            get { return ViewportWidth; } //2
        }

        private double SmallDelta
        {
            //get { return ItemHeight / 4; } 1
            get { return 1; }  //2
        }

        private double RealOffet(double Off)
        {
            return Off * ItemHeight / ItemByWidth;
        }

        public override void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + SmallDelta);
        }

        public override void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - SmallDelta);
        }

        public override void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - BigDelta);
        }

        public override void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + BigDelta);
        }

        public override void LineUp()
        {
        }

        public override void PageUp()
        {
        }

        public override void PageDown()
        {
        }

        public override void MouseWheelLeft()
        {
        }

        public override void MouseWheelRight()
        {
        }

        public override void LineDown()
        {
        }

        public override void MouseWheelUp()
        {
        }

        public override void MouseWheelDown()
        {
        }

        private bool _IsScrolling = false;

        private bool TrySetHorizontalOffset(double offset)
        {
            if (_IsScrolling)
                return false;

            double oldoff = HorizontalOffset;
            double newoffset = CalculateHorizontalOffset(offset);

            double delta = Math.Abs(oldoff - newoffset);

            if (delta == 0)
                return false;

            //_transform.X = -RealOffet(newoffset);//2

            HorizontalOffset = newoffset;
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            InvalidateMeasure();
            return true;
        }

        public override void SetHorizontalOffset(double offset)
        {
            TrySetHorizontalOffset(offset);
        }

        public override void SetVerticalOffset(double offset)
        {
        }

        //private void SetDeltaVertical(double offset)
        //{
        //}

        private double CalculateHorizontalOffset(double offset)
        {
            if (offset < 0 || ViewportWidth >= ExtentWidth)
            {
                offset = 0;
            }
            else
            {
                if (offset + ViewportWidth >= ExtentWidth)
                {
                    offset = ExtentWidth - ViewportWidth;
                }

                //if (offset > ExtentWidth)
                //{
                //    offset = ExtentWidth;
                //}
            }

            //return Math.Floor( (4 * offset) / ItemHeight) * ItemHeight/4;
            return Math.Floor(offset);
        }

        //public Rect MakeVisible(Visual visual, Rect rectangle)
        //{
        //    return rectangle;
        //}

        //public bool CanVerticallyScroll { get; set; }
        //public bool CanHorizontallyScroll { get; set; }
        //public double ExtentHeight { get; set; }
        //public double ExtentWidth { get; set; }
        //public double ViewportWidth { get; set; }
        //public double ViewportHeight { get; set; }
        //public double HorizontalOffset { get; set; }
        //public double VerticalOffset { get; set; }
        //public ScrollViewer ScrollOwner { get; set; }
    }
}
