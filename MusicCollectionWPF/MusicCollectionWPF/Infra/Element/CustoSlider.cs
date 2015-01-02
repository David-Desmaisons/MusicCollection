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
using System.Windows.Shapes;


namespace MusicCollectionWPF.Infra
{
    public class CustoSlider : Slider
    {
        private Canvas _Matrix;
        private Image _Cursor;
        private Popup _autoToolTip;
        private TextBlock _TB;
        private Line _B;
        private DispatcherTimer _Time;
        private int _ImobileCount=0;

        public event EventHandler<EventArgs> DragStarted;
        public event EventHandler<EventArgs> DragCompleted;

        public CustoSlider()
            : base()
        {
            NeedTP = false;
            _Time = new DispatcherTimer();
            _Time.Interval = TimeSpan.FromMilliseconds(200);
            _Time.Tick += Tick;
            _Time.IsEnabled = false;
        }

        private void UpdateToolTip(double xp)
        {
            _TB.Text = AutoToolTipContent.Convert(ConvertToValue(xp), this.Minimum, this.Maximum);
            FrameworkElement ch = (this._autoToolTip.Child as FrameworkElement);
            bool force = false;
            if (ch.ActualWidth==0)
            {
                _TB.Visibility=Visibility.Hidden;
                _autoToolTip.IsOpen = true;
                force = true;
            }         

            this._autoToolTip.HorizontalOffset = xp - ch.ActualWidth  / 2;

            if (force)
            { 
                _autoToolTip.IsOpen = false;
                _TB.Visibility = Visibility.Visible;
            }     
        }

        private void UpdateToolTip(Point p)
        {
            UpdateToolTip(p.X);
        }

        private Point _Current;
        private void Tick(object s, EventArgs ea)
        {          
            Point nCurrent = Mouse.GetPosition(_Matrix);

            if (nCurrent == _Current)
            {
                _ImobileCount++;

                if (_ImobileCount==3)
                    OnImmobile(nCurrent);
            }
            else
            {
                _ImobileCount = 0;
                if (_autoToolTip.IsOpen)
                {
                    UpdateToolTip(nCurrent);
                }
            }

            _Current = nCurrent;
        }

        private void OnImmobile(Point p)
        {
            if (!this.NeedTP)
                return;

            if (this._Trig)
                return;

            UpdateToolTip(p);
            _autoToolTip.IsOpen = true;

        }

        private void ME(object s, EventArgs ea)
        {
            _Time.IsEnabled = true;
        }

        private void ML(object s, MouseEventArgs ea)
        {
            _Time.IsEnabled = false;
            _autoToolTip.IsOpen = false;
        }


        private double ConvertToValue(double delta)
        {
            return Math.Min(this.Maximum, Math.Max(this.Minimum, this.Minimum + (delta * (this.Maximum - this.Minimum)) / _Matrix.ActualWidth));
        }

        private double ConvertToRealValue(double delta)
        {
            return Math.Min(_Matrix.ActualWidth, Math.Max(0, delta));
        }


        public double LineThickness
        {
            get { return (double)GetValue(LineTicknessProperty); }
            set { SetValue(LineTicknessProperty, value); }
        }

        public static readonly DependencyProperty LineTicknessProperty = DependencyProperty.Register(
            "LineThickness",
            typeof(double),
            typeof(CustoSlider),
            new FrameworkPropertyMetadata(2D));

        public bool FillLineVisible
        {
            get { return (bool)GetValue(FillLineVisibleProperty); }
            set { SetValue(FillLineVisibleProperty, value); }
        }

        public static readonly DependencyProperty TickLineBrushProperty = DependencyProperty.Register(
            "TickLineBrush", typeof(Brush), typeof(CustoSlider));

        public Brush TickLineBrush
        {
            get { return (Brush)GetValue(TickLineBrushProperty); }
            set { SetValue(TickLineBrushProperty, value); }
        }

        public static readonly DependencyProperty FillLineVisibleProperty = DependencyProperty.Register(
            "FillLineVisible",
            typeof(bool),
            typeof(CustoSlider),
            new FrameworkPropertyMetadata(false));


        public ISliderMultiConverter AutoToolTipContent
        {
            get { return (ISliderMultiConverter)GetValue(AutoToolTipContentProperty); }
            set { SetValue(AutoToolTipContentProperty, value); }
        }

        public static readonly DependencyProperty AutoToolTipContentProperty = DependencyProperty.Register(
            "AutoToolTipContent",
            typeof(ISliderMultiConverter),
            typeof(CustoSlider),
            new FrameworkPropertyMetadata(null, AutoToolTipPropertyChangedCallback));

        private static void AutoToolTipPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustoSlider cs = d as CustoSlider;
            cs.NeedTP = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                _Matrix = (Canvas)GetTemplateChild("Matrix");
                this.MouseDown += Canvas_MouseDown;

                _Cursor = (Image)GetTemplateChild("Cursor");
                _Cursor.PreviewMouseLeftButtonDown += Image_PreviewMouseDown;
                _Cursor.MouseLeftButtonUp += Image_MouseUp;
                _Cursor.MouseMove += Image_MouseMove;

                _B = (Line)GetTemplateChild("Bar");
                _B.MouseEnter += ME;
                _B.MouseLeave += ML;

                _autoToolTip = GetTemplateChild("AutoToolTip") as Popup;

                _TB = (TextBlock)GetTemplateChild("AutoToolTipContentTextBox");
            }
            catch (Exception)
            {
            }
       }

        protected override void OnRender(DrawingContext dc)
        {
            if (TickPlacement != TickPlacement.None)
            {
                Size size = new Size(base.ActualWidth, base.ActualHeight);
                double MidHeight = size.Height / 2;
                double BigCircle = size.Height / 5;
                double SmallCircle = (BigCircle * 2) / 3;

                dc.DrawEllipse(FillLineVisible? this.TickLineBrush: this.BorderBrush, new Pen(), new Point(10, MidHeight), BigCircle, BigCircle);

                dc.DrawEllipse(this.BorderBrush, new Pen(), new Point(size.Width + 10, MidHeight), BigCircle, BigCircle);
         
                int tickCount = (int)((this.Maximum - this.Minimum) / this.TickFrequency) -1;
                Double tickFrequencySize = (size.Width * this.TickFrequency / (this.Maximum - this.Minimum));

                for (int i = 0; i <tickCount; i++)
                {
                    dc.DrawEllipse(this.BorderBrush, new Pen(), new Point(10 + (i + 1) * tickFrequencySize, MidHeight), 
                                    SmallCircle, SmallCircle);
                }
            }

            base.OnRender(dc);
        }

        private async void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_Trig)
                return;

            Point p = Mouse.GetPosition(_Matrix);
             
            _autoToolTip.IsOpen = false;

            _Trig = true;

            var target = ConvertToValue(p.X);
            await this.SmoothSetAsync(ValueProperty, target, TimeSpan.FromSeconds(0.1));
            Value = target;

            OnThumbDragCompleted(new DragCompletedEventArgs(0, 0, false));

            if (DragCompleted != null)
                DragCompleted(this, new EventArgs());

            _Trig = false;

        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_Trig)
                return;

            OnThumbDragDelta(e);
        }

        private bool _Trig = false;
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _Cursor.CaptureMouse();
            e.Handled = true;
            _Trig = true;
            _autoToolTip.IsOpen = false;

            OnThumbDragStarted(e);

            if (DragStarted != null)
                DragStarted(this, new EventArgs());
        }


        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_Trig)
                return;

            _Cursor.ReleaseMouseCapture();
            e.Handled = true;
            _Trig = false;

            OnThumbDragCompleted(e);

            if (DragCompleted != null)
                DragCompleted(this, new EventArgs());
        }



        private void OnThumbDragStarted(MouseEventArgs e)
        {
            if (NeedTP)
            {
                _autoToolTip.IsOpen = true;
                UpdateToolTip(e.GetPosition(_Matrix));
            }
        }

        private void OnThumbDragDelta(MouseEventArgs e)
        {
            Point p = e.GetPosition(_Matrix);
            Value = ConvertToValue(p.X);

            if (NeedTP)
            {
                UpdateToolTip(e.GetPosition(_Matrix));
            }
        }

        public bool InDrag
        {
            get{ return _Trig; }
        }

        private void OnThumbDragCompleted(MouseEventArgs e)
        {
            Point p = e.GetPosition(_Matrix);
            Value = ConvertToValue(p.X);

            if (NeedTP)
            {
                _autoToolTip.IsOpen = false;
            }
        }

        private bool NeedTP
        {
            get;
            set;
        }

        protected void OnPropertyChanged(string pn)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(pn));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
