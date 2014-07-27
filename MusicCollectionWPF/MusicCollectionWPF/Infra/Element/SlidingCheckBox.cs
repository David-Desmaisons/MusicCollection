using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace MusicCollectionWPF.Infra
{
    public class SlidingCheckBox : CheckBox
    {

        private Image _Im;
        private TranslateTransform _Transf;
        private Grid _Wholle;
        private Border _Border;
        private Border _OnBorder;
        private Border _OffBorder;
        private bool _Init = false;

        public SlidingCheckBox()
            : base()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.LayoutUpdated += new EventHandler(SlidingCheckBox_LayoutUpdated);
            }
        }

        private double _CurrentActualWidth = 0;

        private void SlidingCheckBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (_CurrentActualWidth != this.ActualWidth)
            {
                this.UpdateDisplay(this.IsChecked);
                _CurrentActualWidth = this.ActualWidth;
            }
        }

        #region Dependency Property

        public Brush ForegroundCheckedColor
        {
            get { return (Brush)GetValue(ForegroundCheckedColorProperty); }
            set { SetValue(ForegroundCheckedColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundCheckedColorProperty = DependencyProperty.Register("ForegroundCheckedColor", typeof(Brush), typeof(SlidingCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public Brush CheckedColor
        {
            get { return (Brush)GetValue(CheckedColorProperty); }
            set { SetValue(CheckedColorProperty, value); }
        }
        public static readonly DependencyProperty CheckedColorProperty = DependencyProperty.Register("CheckedColor", typeof(Brush), typeof(SlidingCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Brush ForegroundNoneCheckedColor
        {
            get { return (Brush)GetValue(ForegroundNoneCheckedColorProperty); }
            set { SetValue(ForegroundNoneCheckedColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundNoneCheckedColorProperty = DependencyProperty.Register("ForegroundNoneCheckedColor", typeof(Brush), typeof(SlidingCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush NoneCheckedColor
        {
            get { return (Brush)GetValue(NoneCheckedColorProperty); }
            set { SetValue(NoneCheckedColorProperty, value); }
        }
        public static readonly DependencyProperty NoneCheckedColorProperty = DependencyProperty.Register("NoneCheckedColor", typeof(Brush), typeof(SlidingCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.Red)));


        public string NoneCheckedText
        {
            get { return (string)GetValue(NoneCheckedTextProperty); }
            set { SetValue(NoneCheckedTextProperty, value); }
        }
        public static readonly DependencyProperty NoneCheckedTextProperty = DependencyProperty.Register("NoneCheckedText", typeof(string), typeof(SlidingCheckBox), new PropertyMetadata("OFF"));


        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }
        public static readonly DependencyProperty CheckedTextProperty = DependencyProperty.Register("CheckedText", typeof(string), typeof(SlidingCheckBox), new PropertyMetadata("ON"));

        public Double AnimationSpeed
        {
            get { return (Double)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }
        public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register("AnimationSpeed", typeof(double), typeof(SlidingCheckBox), new PropertyMetadata(150D));


        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                _Init = true;

                _Im = (Image)GetTemplateChild("GB");

                _Transf = (TranslateTransform)GetTemplateChild("Transf");

                _Wholle = (Grid)GetTemplateChild("Wthing");
                _Im.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_Im_PreviewMouseLeftButtonDown);
                _Im.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_Im_MouseLeftButtonUp);
                _Im.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(_Im_MouseMove);

                _Border = (Border)GetTemplateChild("VBorder");

                _OnBorder = (Border)GetTemplateChild("B1");
                _OffBorder = (Border)GetTemplateChild("B2");

                _OnBorder.MouseLeftButtonDown += new MouseButtonEventHandler(_OnBorder_MouseLeftButtonDown);
                _OffBorder.MouseLeftButtonDown += new MouseButtonEventHandler(_OffBorder_MouseLeftButtonDown);


            }
            catch (Exception)
            {
            }
        }

        void _OffBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
            e.Handled = true;
        }

        void _OnBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
        }

        private Point? _MousePosition = null;
        private double _TransXOr = 0;

        private double Min
        {
            get { return -_Border.ActualWidth + 27; }
        }

        private double Frontier
        {
            get { return _Border.ActualWidth / 2 - this._OnBorder.ActualWidth; }
        }

        private bool _Br = false;
        private void ChangeCheckFromPos()
        {
            _Br = true;
            IsChecked = !(_Transf.X == 0);
            _Br = false;
        }

        private double DoubleTransFromChecked(bool? C)
        {
            if (C == null)
                return Frontier;

            return C.Value ? Min : 0;
        }

        private bool ComputeboolTransDestination()
        {
            bool Destination = false;
            if (IsChecked != null)
            {
                Destination = !IsChecked.Value;
            }
            else
            {
                Destination = _Transf.X < Frontier;
            }

            return Destination;
        }

        private void UpdateTrans(double delta)
        {
            double t = delta + _Transf.X;

            if (t > 0)
            {
                t = 0;
            }

            if ((t < Min))
            {
                t = Min;
            }

            _Transf.X = t;
        }

        private void AnimateToDestination(bool? Destination, double SpeedRation = 1)
        {
            if ((_Init == false) || (ActualWidth == 0))
                return;

            _OffBorder.Visibility = Visibility.Visible;
            _OnBorder.Visibility = Visibility.Visible;

            double Dest = DoubleTransFromChecked(Destination);
            double dist = Math.Abs(_Transf.X - Dest);



            if (dist != 0)
            {
                this._Im.MouseLeftButtonUp -= _Im_MouseLeftButtonUp;
                Storyboard sb = new Storyboard();

                DoubleAnimation db = new DoubleAnimation();

                db.To = Dest;

                db.Duration = TimeSpan.FromMilliseconds(dist * 100 / (SpeedRation * this.AnimationSpeed));

                Storyboard.SetTarget(db, this._Wholle);
                Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.X"));

                sb.Children.Add(db);
                EventHandler handler = null;
                handler = delegate
                {
                    sb.Completed -= handler;
                    sb.Remove();
                    _Transf.X = Dest;
                    ChangeCheckFromPos();
                    this._Im.MouseLeftButtonUp += _Im_MouseLeftButtonUp;
                };

                sb.Completed += handler;

                sb.Begin();
            }
            else
            {
                ChangeCheckFromPos();
            }
        }

        private void OnEndManualPush()
        {
            if ((_Transf.X == 0) || (_Transf.X == Min))
            {
                ChangeCheckFromPos();
                return;
            }

            AnimateToDestination(ComputeboolTransDestination());
        }

        void _Im_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _MousePosition = null;
                return;
            }

            if (_MousePosition == null)
                return;

            UpdateTrans(-_MousePosition.Value.X + e.GetPosition(_Wholle).X);
        }

        void _Im_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _Im.ReleaseMouseCapture();
            _MousePosition = null;
            OnEndManualPush();
        }

        void _Im_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool res = _Im.CaptureMouse();
            _MousePosition = e.GetPosition(_Wholle);
            _TransXOr = _Transf.X;
            _OnBorder.Visibility = Visibility.Visible;
            _OffBorder.Visibility = Visibility.Visible;

        }

        private void UpdateDisplay(bool? iChecked)
        {
            if ((_Init == false) || (_Transf == null) || (ActualWidth == 0))
                return;

            _Transf.X = DoubleTransFromChecked(iChecked);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            if (CanNotAnimate)
                return;

            AnimateToDestination(true);

        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            if (CanNotAnimate)
                return;

            AnimateToDestination(false);
        }


        protected override void OnIndeterminate(RoutedEventArgs e)
        {
            base.OnIndeterminate(e);

            if (CanNotAnimate)
                return;

            AnimateToDestination(null);
        }

        private bool CanNotAnimate
        {
            get { return _Br || this.IsLoaded == false; }
        }
    }
}
