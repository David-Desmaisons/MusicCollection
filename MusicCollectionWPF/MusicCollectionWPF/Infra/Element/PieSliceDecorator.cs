using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicCollectionWPF.Infra
{
    public class PieSliceDecorator : Decorator
    {
        #region Dependency property

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(PieSliceDecorator),
       new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty ChildrenSizeProperty = DependencyProperty.Register("ChildrenSize", typeof(double), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public double ChildrenSize
        {
            get { return (double)GetValue(ChildrenSizeProperty); }
            set { SetValue(ChildrenSizeProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(45D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty BigRayProperty = DependencyProperty.Register("BigRay", typeof(double), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(100D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public double BigRay
        {
            get { return (double)GetValue(BigRayProperty); }
            set { SetValue(BigRayProperty, value); }
        }

        public static readonly DependencyProperty SmallRayProperty = DependencyProperty.Register("SmallRay", typeof(double), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public double SmallRay
        {
            get { return (double)GetValue(SmallRayProperty); }
            set { SetValue(SmallRayProperty, value); }
        }

        public static readonly DependencyProperty ChildRelativePositionProperty = DependencyProperty.Register("ChildRelativePosition", typeof(double), typeof(PieSliceDecorator), new FrameworkPropertyMetadata(0.4D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public double ChildRelativePosition
        {
            get { return (double)GetValue(ChildRelativePositionProperty); }
            set { SetValue(ChildRelativePositionProperty, value); }
        }

        #endregion

        private Size GetChildrenSize()
        {
            return new Size(ChildrenSize, ChildrenSize);
        }

        private SlicerHelper _SH = null;
        private Size _Size;
        private Pen _Pen;
        private double _Y;

        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                Child.Measure(GetChildrenSize());
            }

            //double angle = Angle / 180 * Math.PI;

            _SH = new SlicerHelper(new Point(0, BigRay + BorderThickness), SmallRay, BigRay, Math.PI / 2 + Angle / 2, Math.PI / 2 - Angle / 2);
            _Pen = new Pen(BorderBrush, BorderThickness);
            Rect BB = _SH.GetPathGeometry(true, true).GetRenderBounds(_Pen);
            _Size = BB.Size;
            _Y = BB.Top;

            return _Size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size cz = GetChildrenSize();
            double half = cz.Height / 2;
            this.RenderTransform = new TranslateTransform(_Size.Width / 2, -_Y);

            if (Child != null)
            {
                Vector Trans = new Vector(0 - half, (this.BigRay - this.SmallRay) * this.ChildRelativePosition - half);
                Child.Arrange(new Rect((Point)Trans, cz));
                Child.ClipToBounds = true;
                var clipper = _SH.GetPathGeometry(true, true);
                clipper.Transform = new TranslateTransform(-Trans.X, -Trans.Y);
                Child.Clip = clipper;
            }

            return _Size;
        }


        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawGeometry(Background, _Pen, _SH.GetPathGeometry(true, true));
        }
    }
}
