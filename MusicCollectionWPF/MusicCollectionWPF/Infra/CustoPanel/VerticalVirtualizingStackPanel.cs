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
    public partial class VerticalVirtualizingStackPanel : PanelWithSize
    {


        public VerticalVirtualizingStackPanel()
        {
            CanVerticallyScroll = true;
            CanHorizontallyScroll = true;
        }

        private ListBox ListBoxOwner
        {
            get { return this._ItemsOwner as ListBox; }
        }


        protected override Size MeasureOverride(Size constraint)
        {
            double CWith = (constraint.Width == double.PositiveInfinity) ? 10000 : constraint.Width;
            double CH = Math.Min(_ItemsOwner.Items.Count * ItemHeight,constraint.Height);


            Size res = new Size(CWith, CH);
            Size ItemSize = new Size(CWith, ItemHeight);

            UpdateScrollInfo(res);

            using (VirtualizeItems())
            {
                foreach (UIElement child in InternalChildren)
                {
                    child.Measure(ItemSize);
                }
            }

            if (!InhibitCleanUp)
                CleanupItems(); // todo test

            return res;
        }
       

       
        //private int _FirstIntValue = 0;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_IsScrolling == false)
            {
                _transform.Y = 0;
            }
            //int intoffset = _FirstIntValue - ItemByWidth - StartedDecal;
            //bool NotFirst = false;
            //double w = (finalSize.Width - ItemHeight) / 2;
            //double Fact = ItemHeight;

            Size s = new Size(finalSize.Width, ItemHeight);

            //Console.WriteLine("Start Index {0} intoffset {1}", _StartIndex, intoffset);

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                ListBoxItem lbi = Children[index] as ListBoxItem;
                //if (NotFirst)
                //{
                //    intoffset += lbi.IsSelected ? ItemByWidth : 1;
                //}
                //else
                //{
                //    NotFirst = true;
                //}

                //Rect rect = new Rect(new Point(w, (index + _ScrollingOffset) * ItemHeight), s);
                Rect rect = new Rect(new Point(0,(index+_ScrollingOffset) * ItemHeight), s);

                lbi.Arrange(rect);
            }

            if ((_ItemsOwner.Items.Count * ItemHeight) < finalSize.Height)
            {
                return new Size(finalSize.Width, (_ItemsOwner.Items.Count * ItemHeight));
            }

            return finalSize;
        }

        private int ItemPerWidth(double iHeight)
        {
            if (double.IsInfinity(iHeight))
                return int.MaxValue;

            return Math.Max(1, (int)(iHeight/ (ItemHeight)));
        }

        protected override void UpdateScrollInfo(Size finalSize)
        {
            bool HC = false;

            int Dimension = ItemPerWidth(finalSize.Height);

            //if (ViewportWidth != Dimension)
            //{
            //    ViewportHeight = finalSize.Height;
            //    ViewportWidth = Dimension;
            //    HC = true;
            //}

            if (ViewportHeight != Dimension)
            {
                ViewportHeight = Dimension;
                ViewportWidth = finalSize.Width;
                HC = true;
            }

            int ItemNumbers = _ItemsOwner.Items.Count;

            ListBox lb = this.ListBoxOwner;
            if (lb == null)
                throw new InvalidOperationException();

            if (ExtentHeight != ItemNumbers)
            {
                ExtentHeight = ItemNumbers;
                HC = true;
            }

            if (HC)
            {
                VerticalOffset = CalculateVerticalOffset(VerticalOffset);

                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            _StartIndex = StartedDecal;
            //_VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension + 2); ItemByWidth
            _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension ); 

        }

        private int StartedDecal
        {
            get { return (int)Math.Floor(VerticalOffset); }
        }

        private void RefreshDisplay()
        {
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            InvalidateMeasure();
        }

        
    }
}
