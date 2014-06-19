using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MusicCollectionWPF.CustoPanel
{
    public abstract class PanelWithSize:SmartPanelBase
    {
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
    "ItemHeight", typeof(double), typeof(PanelWithSize),
    new FrameworkPropertyMetadata(200D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                        FrameworkPropertyMetadataOptions.AffectsArrange,SizeChangedCallBack));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        protected virtual void ItemSizeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        static private void SizeChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PanelWithSize).ItemSizeChanged(e);
        }

        protected override Size ItemSize()
        {
            return new Size(ItemHeight, ItemHeight);
        }
    }

    public abstract class VirtualRectPanelBase : PanelWithSize
    {
        protected override Size ItemSize()
        {
            return new Size(CurrentItemHeight, CurrentItemHeight);
        }

        protected virtual double CurrentItemHeight
        {
            get { return ItemHeight; }
        }

        protected double SpaceHeigthX
        {
            get { return (1 + XSpace) * CurrentItemHeight; }
        }

        protected double SpaceHeigthY
        {
            get { return (1 + YSpace) * CurrentItemHeight; }
        }

        protected double InitialYGap
        {
            get { return YSpace * CurrentItemHeight; }
        }

        public static readonly DependencyProperty XSpaceProperty = DependencyProperty.Register(
      "XSpace", typeof(double), typeof(VirtualRectPanelBase),
      new FrameworkPropertyMetadata(0.25D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsArrange));

        public double XSpace
        {
            get { return (double)GetValue(XSpaceProperty); }
            set { SetValue(XSpaceProperty, value); }
        }

        public static readonly DependencyProperty YSpaceProperty = DependencyProperty.Register(
       "YSpace", typeof(double), typeof(VirtualRectPanelBase),
       new FrameworkPropertyMetadata(0.25D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                               FrameworkPropertyMetadataOptions.AffectsArrange));

        public double YSpace
        {
            get { return (double)GetValue(YSpaceProperty); }
            set { SetValue(YSpaceProperty, value); }
        }


        public static readonly DependencyProperty HorizontalMarginProperty = DependencyProperty.Register(
      "HorizontalMargin", typeof(double), typeof(VirtualRectPanelBase),
      new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsArrange));

        public double HorizontalMargin
        {
            get { return (double)GetValue(HorizontalMarginProperty); }
            set { SetValue(HorizontalMarginProperty, value); }
        }


        protected Tuple<int, int> ItemPerWidth(double Width)
        {
            int ItemPerLign = Math.Max(1, (int)((Width - 2 * HorizontalMargin) / (SpaceHeigthX)));
            int NumberOfElem = _ItemsOwner.Items.Count;

            //int Lign = -(int)Math.Floor(-(double)NumberOfElem / (double)ItemPerLign);

            int Lign = (int)Math.Ceiling((double)NumberOfElem / (double)ItemPerLign);

            return new Tuple<int, int>(ItemPerLign, Lign);
        }

     

    }
}
