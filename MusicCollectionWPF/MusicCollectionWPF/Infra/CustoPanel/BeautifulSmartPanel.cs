using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace MusicCollectionWPF.CustoPanel
{

    public partial class BeautifulSmartPanel :
        CoreVirtualizingPanelBase
    {
        private double _R1 = 1;
        private double _R2= 1.7;
        private double _R3 = 2.5;
        private double _Fact = 0.7;

        private int _CircletoBeDrawn;

        public event EventHandler<EventArgs> OnDraw;

        public BeautifulSmartPanel()
        {
        }

        //private int _count = 0;
        private void DispatchDraw()
        {
            EventHandler<EventArgs> Draw = OnDraw;
            if (Draw == null)
                return;

            Draw(this, new EventArgs());
        }

        private double RayonFactor(int circleNumber)
        {  
            switch (circleNumber)
            {
                case 0:
                    return 0;

                case 1:
                    return _R1;

                case 2:
                    return _R2;

                case 3:
                    return _R3;
            }

            throw new ArgumentException();
        }

        private double Rayon(int circleNumber)
        {
            return RayonFactor(circleNumber) * ItemHeigthToUse;
        }

        private int CircleFrom(int index)
        {
            if (index == 0)
            {
                return 0;
            }
            else if (index <= ItemByCircle)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private int TotalItembyCircleLayer(int CN)
        {
            int res = 0;
            for (int i = 0; i <= CN; i++)
            {
                res += ItembyCircleLayer(i);
            }
            return res;
        }

        private int ItembyCircleLayer(int CN)
        {
            if (CN == 0)
                return 1;

            return (ItemByCircle * (int)Math.Pow(2, CN - 1));
        }


        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
        "ItemHeight", typeof(double), typeof(BeautifulSmartPanel),
        new FrameworkPropertyMetadata(200D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                FrameworkPropertyMetadataOptions.AffectsArrange));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }


        public static readonly DependencyProperty ItemByCircleProperty = DependencyProperty.Register(
        "ItemByCircle", typeof(int), typeof(BeautifulSmartPanel),
        new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                FrameworkPropertyMetadataOptions.AffectsArrange));

        public int ItemByCircle
        {
            get { return (int)GetValue(ItemByCircleProperty); }
            set { SetValue(ItemByCircleProperty, value); }
        }

        private double _ItemHeigthToUse = 0;
        private double ItemHeigthToUse
        {
            get
            {
                return (_ItemHeigthToUse!=0) ? _ItemHeigthToUse : ItemHeight;
            }
        }


        private Size ItemSize(int circle)
        {
            double s = Math.Pow(_Fact, circle) * ItemHeigthToUse;
            return new Size(s, s);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double needed = MaxHeigth;

            UpdateScrollInfo(constraint);

            double CWith = HeigthByCircle(_CircletoBeDrawn);
            double CH = HeigthByCircle(_CircletoBeDrawn);

            using (VirtualizeItems())
            {

                for (int index = 0; index < InternalChildren.Count; index++)
                {
                    InternalChildren[index].Measure(ItemSize(CircleFrom(index)));

                }
            }

           
          

            return new Size(CWith, CH);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateScrollInfo(finalSize);

            double ddangle1 = Math.PI * 2 / ItemByCircle,
                    angle1 = (ItemByCircle % 2 == 0) ? Math.PI / ItemByCircle : Math.PI / 2 - (Math.Floor(Math.PI / 2 / ddangle1) * ddangle1),
                    ddangle2 = Math.PI / ItemByCircle,
                    angle2 = 0;

            double x0 = finalSize.Width / 2;
            double y0 = finalSize.Height / 2;

            double intx0 = 0;
            double inty0 = 0;
   
            for (int index = 0; index < InternalChildren.Count; index++)
            {
                int c = CircleFrom(index);
                switch (c)
                {
                    case 0: 
                        intx0 = x0;
                        inty0 = y0;
                         break;

                    case 1:
                         intx0 = x0 + _R1 * ItemHeigthToUse * Math.Cos(angle1);
                         inty0 = y0 + _R1 * ItemHeigthToUse * Math.Sin(angle1);
                        angle1 +=ddangle1;
                        break;

                    case 2:
                        intx0 = x0 + _R2 * ItemHeigthToUse * Math.Cos(angle2);
                        inty0 = y0 + _R2 * ItemHeigthToUse * Math.Sin(angle2);
                        angle2 += ddangle2;
                        break;
                }

                Size s = ItemSize(c);
 
               Rect rect = new Rect(new Point(intx0 - s.Width / 2, inty0 - s.Height / 2), s);
               InternalChildren[index].Arrange(rect);
             }

            Action ac = CleanupItems;
            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, ac); // todo test

            DispatchDraw();

            return finalSize;
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //}


        private double HeigthByCircle(int CN)
        {
            return 2 * Rayon(CN) + ItemSize(CN).Height;
        }

        private double ReSizeToLength(double L, int CN)
        {
            return L / (2 * RayonFactor(CN) + Math.Pow(_Fact, CN));
        }

        private double MaxHeigth
        {
            get
            {
                return HeigthByCircle(2);
            }
        }

        private bool IsLengthValid(Size finalSize,int circleNumber)
        {
            return (finalSize.Height >= HeigthByCircle(circleNumber)) &&
                   (finalSize.Width >= HeigthByCircle(circleNumber));
        }

        private void UpdateScrollInfo(Size finalSize)
        {
            //_Second_Circle = IsLengthValid(finalSize,2);
            //_First_Circle = _Second_Circle || IsLengthValid(finalSize,1);

            _CircletoBeDrawn=0; 
            _ItemHeigthToUse = 0;

            for (int i = 3; i >= 2; i--)
            {
                if (IsLengthValid(finalSize, i))
                {
                    _CircletoBeDrawn = i;
                    break;
                }
            }

            if (_CircletoBeDrawn == 0)
            {
                _CircletoBeDrawn = 2;
                _ItemHeigthToUse = Math.Min(ReSizeToLength(finalSize.Height, _CircletoBeDrawn), ReSizeToLength(finalSize.Width, _CircletoBeDrawn));
            }

               

            //_CircletoBeDrawn = 2;//hehe

            _StartIndex = 0;

            int MaxPossibleItems = TotalItembyCircleLayer(_CircletoBeDrawn);

            //int MaxPossibleItems = 1 + (_Second_Circle ? ItemByCircle*2 :0) + (_First_Circle ? ItemByCircle :0);

            int ItemNumbers = _ItemsOwner.Items.Count;

            _VisibileIndexes = Math.Min(ItemNumbers - _StartIndex, MaxPossibleItems);  

        }

        protected override Visual GetVisualChild(int index)
        {
            //if (index < 0 || index >= Children.Count)
            //{
            //    throw new ArgumentOutOfRangeException();
            //}

            //return Children[Children.Count - 1 - index];

            return base.GetVisualChild(index);
        }

        
    }
}
