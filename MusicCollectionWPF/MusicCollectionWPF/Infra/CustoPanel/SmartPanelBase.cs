using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Specialized;

namespace MusicCollectionWPF.CustoPanel
{
    public abstract class SmartPanelBase :
        CoreVirtualizingPanelBase,
        IScrollInfo
    {
        protected TranslateTransform _transform = new TranslateTransform();

        public SmartPanelBase()
        {
            this.RenderTransform = _transform;
        }

        protected abstract Size ItemSize();
        abstract protected void UpdateScrollInfo(Size fz);

        protected virtual bool InhibitCleanUp
        {
            get {return false;}
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double CWith = (constraint.Width == double.PositiveInfinity) ? 10000 : constraint.Width;
            double CH = (constraint.Height == double.PositiveInfinity) ? 10000 : constraint.Height;


            Size res = new Size(CWith, CH);

            UpdateScrollInfo(res);

            using (VirtualizeItems())
            {
                foreach (UIElement child in InternalChildren)
                {
                    child.Measure(ItemSize());
                }
            }

            if (!InhibitCleanUp)
                CleanupItems(); // todo test

            return res;
        }

        public bool CanHorizontallyScroll
        {
            get;
            set;
        }

        public bool CanVerticallyScroll
        {
            get;
            set;
        }

        public double ExtentHeight
        {
            get;
            protected set;
        }

        public double ExtentWidth
        {
            get;
            protected set;
        }

        public double HorizontalOffset
        {
            get;
            protected set;
        }

        public abstract void LineDown();

        public abstract void LineLeft();

        public abstract void LineRight();

        public abstract void LineUp();

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        public abstract void MouseWheelDown();

        public abstract void MouseWheelLeft();

        public abstract void MouseWheelRight();

        public abstract void MouseWheelUp();

        public abstract void PageDown();

        public abstract void PageLeft();

        public abstract void PageRight();

        public abstract void PageUp();

        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }

        public abstract void SetHorizontalOffset(double offset);

        public abstract void SetVerticalOffset(double offset);

        public double VerticalOffset
        {
            get;
            protected set;
        }

        public double ViewportHeight
        {
            get;
            protected set;
        }

        public double ViewportWidth
        {
            get;
            protected set;
        }
    }
}
