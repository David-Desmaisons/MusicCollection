using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.Infra
{

    public class VolumeSlider : Slider
    {
        private Nullable<Point> _MousePosition;
        private UIElement _Potar;
        private Point _Center = new Point(60, 72);
        private Grid _MotherGrid;

        public double Ratio
        {
            get { return (double)GetValue(RatioProperty); }
            set { SetValue(RatioProperty,value); }
        }

        public readonly static DependencyProperty RatioProperty = DependencyProperty.Register("Ratio", typeof(double), typeof(VolumeSlider));

        private Point GetPoint(MouseEventArgs e)
        {
            Point P = e.GetPosition(_Potar);
            return new Point(P.X - _Center.X, P.Y - _Center.X);
        }

        private Point GetGridPoint(MouseEventArgs e)
        {
            Point P = e.GetPosition(_MotherGrid);
            return new Point(P.X - 100, P.Y - 100);
        }

        private void PotarMouseRightPressed(Object sender, MouseButtonEventArgs e)
        {
            this.Value = Math.Max(Minimum, Value - (Maximum - Minimum) / 10);
        }


        private void PotarPreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Value = Math.Min(Maximum, Value + (Maximum - Minimum) / 10);
                _Potar.ReleaseMouseCapture();
                return;
            }

            _Potar.CaptureMouse();

            _MousePosition = GetPoint(e);

            e.Handled = true;
        }



        private void PotarPreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            _MousePosition = null;
            _Potar.ReleaseMouseCapture();
        }


        private void PotarMouseMove(Object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;

            if (_MousePosition == null)
                return;

            Point old = _MousePosition.Value;
            Point newPos = GetPoint(e);

            double deltaangle = old.GetAngle(newPos);

            double value = Value + ((deltaangle * (Maximum - Minimum)) / 270);
            Value = Math.Max(Minimum, Math.Min(Maximum, value));
        }

        private void PotarMouseLeave(Object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;

            _MousePosition = null;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Potar = (UIElement)GetTemplateChild("Potar");


            _Potar.MouseRightButtonDown += PotarMouseRightPressed;
            _Potar.PreviewMouseLeftButtonDown += PotarPreviewMouseLeftButtonDown;
            _Potar.PreviewMouseMove += PotarMouseMove;
            _Potar.PreviewMouseLeftButtonUp += PotarPreviewMouseLeftButtonUp;
            _Potar.MouseLeave += PotarMouseLeave;

            _MotherGrid = (Grid)GetTemplateChild("MotherGrid");
            _MotherGrid.MouseLeftButtonDown += MotherGrid_MouseLeftButtonDown;
            
        }

        private void MotherGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double deltaangle = new Point(-100, 100).GetFullAngle(GetGridPoint(e));

            if (deltaangle > 270)
                return;

            double value = (((deltaangle) * (Maximum - Minimum)) / 270) ;
            //double targetvalue = Math.Max(Minimum, Math.Min(Maximum, value));
            //Value = Math.Max(Minimum, Math.Min(Maximum, value));

            this.SmoothSet(ValueProperty, Math.Max(Minimum, Math.Min(Maximum, value)), TimeSpan.FromSeconds(0.15));

            //DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(TimeSpan.FromSeconds(0.15)));
            //PropertyPath p = new PropertyPath("(0)", ValueProperty);
            //Storyboard.SetTargetProperty(anim, p);
            //Storyboard sb = new Storyboard();
            //sb.Children.Add(anim);
            //EventHandler handler = null;
            //handler = delegate
            //{
            //    sb.Completed -= handler;
            //    sb.Remove(this);
            //    Value = targetvalue;
            //};
            //sb.Completed += handler;
            //sb.Begin(this, true);
        }
    }
}
