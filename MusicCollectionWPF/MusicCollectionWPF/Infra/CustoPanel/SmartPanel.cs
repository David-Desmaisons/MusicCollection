using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;



using MusicCollectionWPF.CustoPanel;

namespace MusicCollectionWPF
{
    public partial class SmartPanel : VirtualRectPanelBase
    {

      
        //private double _SpaceHeigthY;
        //protected override double SpaceHeigthY
        //{
        //    get { return _SpaceHeigthY; }
        //}

        //private double MinimalSpaceHeigthX
        //{
        //    get { return base.SpaceHeigthX; }
        //}

        //private double MinimalSpaceHeigthY
        //{
        //    get { return base.SpaceHeigthY; }
        //}

        public SmartPanel()
        {
            CanVerticallyScroll = false;
            CanHorizontallyScroll = false;
        }

        public static readonly DependencyProperty CanContentScrollProperty = DependencyProperty.Register(
       "CanContentScroll", typeof(bool), typeof(SmartPanel),
       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                               FrameworkPropertyMetadataOptions.AffectsArrange));

        public bool CanContentScroll
        {
            get { return (bool)GetValue(CanContentScrollProperty); }
            set { SetValue(CanContentScrollProperty, value); }
        }

        private double _SpaceHeigthY;

        protected override Size ArrangeOverride(Size finalSize)
        {
            Tuple<int, int> Dimension = ItemPerWidth(finalSize.Width);

            //double w = (finalSize.Width - Dimension.Item1 * SpaceHeigthX) / 2;
            //double z = (finalSize.Height - _Maxlign * SpaceHeigthY) / 2 + InitialYGap;

        
            double w = (finalSize.Width - Dimension.Item1 * SpaceHeigthX) / 2;

            _SpaceHeigthY = finalSize.Height / _Maxlign;
            double z = (_SpaceHeigthY - SpaceHeigthY) / 2 + InitialYGap;

            Size size = ItemSize();

            int sindex = _StartIndex + _ScrollingOffset * Dimension.Item1;

            for (int index = 0; index < InternalChildren.Count; index++)
            {
                int x = (index + sindex) % Dimension.Item1;
                int y = (index + sindex) / Dimension.Item1;

                Rect rect = new Rect(new Point(w + x * SpaceHeigthX, z + y * _SpaceHeigthY), size);
                //Rect rect = new Rect(new Point( x * SpaceHeigthX, y * SpaceHeigthY), size);
                InternalChildren[index].Arrange(rect);

            }

            return finalSize;
        }

        private double TotalExtend(Tuple<int, int> Dimension, double ViewportHeight)
        {
            return Dimension.Item2;
        }

        protected override void ItemSizeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private int _Maxlign = 0;

        protected override void UpdateScrollInfo(Size finalSize)
        {
            bool HC = false;

            _Maxlign = (int)(Math.Truncate(((finalSize.Height) / SpaceHeigthY)));

            Tuple<int, int> Dimension = ItemPerWidth(finalSize.Width);
            if (ViewportHeight != _Maxlign)
            {
                ViewportWidth = finalSize.Width;
                HC = true;
            }

            ViewportHeight = _Maxlign;

            double newextend = TotalExtend(Dimension, ViewportHeight);

            if (ExtentHeight != newextend)// Dimension.Item2 * SpaceHeigthY)//ItemHeight)
            {
                ExtentHeight = newextend;// Dimension.Item2* SpaceHeigthY;// ItemHeight;
                HC = true;
            }

            if (HC)
            {
                _SpaceHeigthY = finalSize.Height / _Maxlign;
                
                VerticalOffset = CalculateVerticallOffset(Math.Truncate((double)(_StartIndex / Dimension.Item1)));

                _transform.Y = -1 * VerticalOffset * _SpaceHeigthY;

                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            int ItemNumbers = _ItemsOwner.Items.Count;

             _StartIndex = Math.Max(0, (int)(Math.Floor(VerticalOffset)) * Dimension.Item1);

            _Maxlign = (int)(Math.Truncate(((finalSize.Height) / SpaceHeigthY)));

            int MaxVisibleItem = _Maxlign * Dimension.Item1;

            _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, MaxVisibleItem);

        }
    }
}
