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
    public partial class HorizontalVirtualizingStackPanel : PanelWithSize
    {


        public HorizontalVirtualizingStackPanel()
        {
            CanVerticallyScroll = true;
            CanHorizontallyScroll = true;
        }

        private ListBox ListBoxOwner
        {
            get { return this._ItemsOwner as ListBox; }
        }

       
        //private int _FirstIntValue = 0;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_IsScrolling == false)
            {
                _transform.X = 0;
            }
            //int intoffset = _FirstIntValue - ItemByWidth - StartedDecal;
            //bool NotFirst = false;
            double w = (finalSize.Height - ItemHeight) / 2;
            //double Fact = ItemHeight;

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

                Rect rect = new Rect(new Point((index+_ScrollingOffset) * ItemHeight, w), ItemSize());

                lbi.Arrange(rect);
            }

            return finalSize;
        }

        private int ItemPerWidth(double Width)
        {
            if (double.IsInfinity(Width))
                return int.MaxValue;

            return Math.Max(1, (int)(Width/ (ItemHeight)));
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

            ListBox lb = this.ListBoxOwner;
            if (lb == null)
                throw new InvalidOperationException();

            if (ExtentWidth != ItemNumbers)
            {
                ExtentWidth = ItemNumbers;
                HC = true;
            }

            if (HC)
            {
                HorizontalOffset = CalculateHorizontalOffset(HorizontalOffset);

                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            _StartIndex = StartedDecal;
            //_VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension + 2); ItemByWidth
            _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, Dimension ); 

        }

        private int StartedDecal
        {
            get { return (int)Math.Floor(HorizontalOffset); }
        }

        private void RefreshDisplay()
        {
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            InvalidateMeasure();
        }

        
    }
}
