using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace MusicCollectionWPF.CustoPanel
{
    public class SimpleCirclePanel : Panel
    {
        public SimpleCirclePanel()
        {
        }

        #region Dependency Properties

        public static readonly DependencyProperty ExternalRayProperty = DependencyProperty.Register("ExternalRay", typeof(double), typeof(SimpleCirclePanel));
        public double ExternalRay { get { return (double)GetValue(ExternalRayProperty); } set { SetValue(ExternalRayProperty, value); } }

        public static readonly DependencyProperty ListBoxExternalRayProperty = DependencyProperty.RegisterAttached("ListBoxExternalRay", typeof(double), typeof(SimpleCirclePanel));
        public static double GetListBoxExternalRay(DependencyObject io)
        {
            return (double)io.GetValue(ListBoxExternalRayProperty);
        }
        public static void SetListBoxExternalRay(DependencyObject io, double value)
        {
            io.SetValue(ListBoxExternalRayProperty, value);
        }

        public static readonly DependencyProperty ListBoxInternallRayProperty = DependencyProperty.RegisterAttached("ListBoxInternallRay", typeof(double), typeof(SimpleCirclePanel));
        public static double GetListBoxInternallRay(DependencyObject io)
        {
            return (double)io.GetValue(ListBoxInternallRayProperty);
        }
        public static void SetListBoxInternallRay(DependencyObject io, double value)
        {
            io.SetValue(ListBoxInternallRayProperty, value);
        }

        public static readonly DependencyProperty ChildrenSizeProperty = DependencyProperty.RegisterAttached("ChildrenSize", typeof(double), typeof(SimpleCirclePanel));
        public static double GetChildrenSize(DependencyObject io)
        {
            return (double)io.GetValue(ChildrenSizeProperty);
        }
        public static void SetChildrenSize(DependencyObject io, double value)
        {
            io.SetValue(ChildrenSizeProperty, value);
        }




        public static readonly DependencyProperty AllocatedAngleProperty = DependencyProperty.RegisterAttached("AllocatedAngle", typeof(double), typeof(SimpleCirclePanel), new PropertyMetadata(0D));
        public static double GetAllocatedAngle(DependencyObject io)
        {
            return (double)io.GetValue(AllocatedAngleProperty);
        }

        private static void SetAllocatedAngle(DependencyObject io, double value)
        {
            io.SetValue(AllocatedAngleProperty, value);
        }

        public static readonly DependencyProperty DecalAngleProperty = DependencyProperty.RegisterAttached("DecalAngle", typeof(double), typeof(SimpleCirclePanel), new PropertyMetadata(0D));
        public static double GetDecalAngle(DependencyObject io)
        {
            return (double)io.GetValue(DecalAngleProperty);
        }

        private static void SetDecalAngle(DependencyObject io, double value)
        {
            io.SetValue(DecalAngleProperty, value);
        }



        #endregion

        private double MidRay
        {
            get;
            set;
        }

        private double Thesholder(double value, double Tres)
        {
            return double.IsPositiveInfinity(value) ? Tres : value;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double heigth = 0;

            foreach (UIElement child in this.Children)
            {
                SetAllocatedAngle(child, AngleIncrement);
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                heigth = child.DesiredSize.Height;
            }

            MidRay = ExternalRay - heigth / 2;

            return new Size(ExternalRay * 2, ExternalRay * 2);
        }

        private double DegreeAngle(double radianangle)
        {
            return (radianangle / Math.PI) * 180;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            double CenterY = finalSize.Height / 2;
            double CenterX = finalSize.Width / 2;

            double angle = StartingAngle;

            foreach (UIElement child in this.Children)
            {
                if (child != null)
                {
                    Size desired = child.DesiredSize;
                    double halfw = desired.Width / 2;
                    double halfh = desired.Height / 2;

                    double x = CenterX + MidRay * Math.Cos(angle) - halfw;
                    double y = CenterY - MidRay * Math.Sin(angle) - halfh;

                    child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                    child.RenderTransformOrigin = new Point(0.5, 0.5);
                    double degreeangle = -DegreeAngle(angle - Math.PI / 2);
                    child.RenderTransform = new RotateTransform(degreeangle);

                    SetDecalAngle(child, -degreeangle);

                    angle += AngleIncrement;
                }
            }

            return finalSize;
        }

        private double RealAngleAvailable
        {
            get { return 2 * Math.PI; }
        }

        private double AngleMort
        {
            get { return 0; }
        }


        private double AngleIncrement
        {
            get
            {
                return -RealAngleAvailable / this.Children.Count;
            }
        }

        private double StartingAngle
        {
            get
            {
                return (this.Children.Count % 2 == 0) ? Math.PI / 2 : Math.PI / 2 - AngleIncrement / 2;
            }
        }
    }
}
