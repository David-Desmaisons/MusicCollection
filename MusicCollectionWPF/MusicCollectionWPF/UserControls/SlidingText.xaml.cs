using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SlidingText : UserControl
    {

        public static readonly DependencyProperty ShowAllProperty = DependencyProperty.RegisterAttached("ShowAll", typeof(bool), typeof(SlidingText), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, ShowAllPropertyPropertyChangedCallback));


        public static void SetShowAll(DependencyObject element, bool value)
        {
            element.SetValue(SlidingText.ShowAllProperty, value);
        }


        public static bool GetShowAll(DependencyObject element)
        {
            return (bool)element.GetValue(SlidingText.ShowAllProperty);
        }


        static private void ShowAllPropertyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidingText st = d as SlidingText;
            if (st == null)
                return;

            if (st.TextOwner == null)
                return;

            st.ChangedShowAlll((bool)e.NewValue);
        }

        public static readonly DependencyProperty BlankProperty = DependencyProperty.RegisterAttached("Blank", typeof(double), typeof(SlidingText), new PropertyMetadata(10D));


        public static void SetBlank(DependencyObject element, double value)
        {
            element.SetValue(SlidingText.BlankProperty, value);
        }


        public static double GetBlank(DependencyObject element)
        {
            return (double)element.GetValue(SlidingText.BlankProperty);
        }

        private class SliderConverter
        {
            private double _Dest;
            private TimeSpan _TS;
            private TimeSpan _Begin = TimeSpan.FromSeconds(0);
            private TimeSpan _DeltaBegin;
            private double _Speed = 75;
            private double _Current;

            internal double Speed
            {
                get { return _Speed; }
                set { _Speed = value; }
            }


            internal TimeSpan InitialWait
            {
                get { return _Begin; }
                set { _Begin = value; }
            }

            internal TimeSpan Begin
            {
                get
                {
                    if (_DeltaBegin.TotalSeconds == 0)
                        return _Begin;

                    return _DeltaBegin;
                }
            }

            private SliderConverter()
            {
                _Dest = 0;
                _TS = TimeSpan.FromSeconds(0);
            }

            private SliderConverter(double Current, double Max, bool GoToEnd)
            {
                _Dest = GoToEnd ? Max : 0;
                _Current = GoToEnd ? 0 : Max;

                _TS = TimeSpan.FromSeconds(Math.Abs(Max) / Speed);

                double Decal = Math.Abs((Current - _Current) * _TS.TotalSeconds / Max);

                _DeltaBegin = TimeSpan.FromSeconds(-Decal);
            }

            //TextBlock
            static internal SliderConverter Get(FrameworkElement iTB, double max, double iBlank, double current, bool GoToEnd)
            {
                Double Max = iTB.ActualWidth;

                if (Max < max)
                {
                    return new SliderConverter();
                }

                return new SliderConverter(current, -(Max - max + iBlank), GoToEnd);
            }

            internal DoubleAnimation InitFromData()
            {
                DoubleAnimation res = new DoubleAnimation
                {
                    Duration = new Duration(Time),
                    From = _Current,
                    To = Destination,
                    BeginTime = Begin,


                };

                return res;
            }


            internal Double Destination
            {
                get
                {
                    return _Dest;
                }
            }

            internal TimeSpan Time
            {
                get
                {
                    return _TS;
                }
            }
        }

        public SlidingText()
        {
            InitializeComponent();
        }

        public double Decal
        {
            get { return (double)GetValue(DecalProperty); }
            set { SetValue(DecalProperty, value); }
        }


        public static readonly DependencyProperty DecalProperty = DependencyProperty.Register("Decal", typeof(double), typeof(SlidingText), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsMeasure |
                                           FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsRender));




        //public double Blank
        //{
        //    get { return (double)GetValue(BlankProperty); }
        //    set { SetValue(BlankProperty, value); }
        //}


        //public static readonly DependencyProperty BlankProperty = DependencyProperty.Register("Blank", typeof(double), typeof(SlidingText), new PropertyMetadata(10D));



        public object Text
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        //public static readonly DependencyProperty ShowAllProperty = DependencyProperty.Register("ShowAll", typeof(bool), typeof(SlidingText), new PropertyMetadata(false, ShowAllPropertyPropertyChangedCallback));

        //static private void ShowAllPropertyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //    SlidingText st = d as SlidingText;
        //    if (st == null)
        //        return;

        //    st.ChangedShowAlll((bool)e.NewValue);
        //}

        private void ChangedShowAlll(bool newv)
        {
            if (newv)
            {
                Vai(true, true);
            }
            else
            {
                Vai(false);
            }

            _MSE = false;
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(object), typeof(SlidingText),new PropertyMetadata(string.Empty,TextChangedCB));

        private static void TextChangedCB(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private Storyboard _Sb;
        //private bool _Sense = true;

        private void Vai(bool Sense, bool Reverse = false)
        {
            if (_Sb != null)
            {
                _Sb.Completed -= End;
                _Sb.Pause();
                _Sb = null;
            }

            SliderConverter sc = SliderConverter.Get(this.TextOwner, this.ActualWidth, SlidingText.GetBlank(this), Decal, Sense);
            sc.InitialWait = TimeSpan.FromSeconds(0.5);

            if (sc.Time.TotalMilliseconds != 0)
            {
                try
                {

                    _Sb = new Storyboard();

                    DoubleAnimation animation = sc.InitFromData();

                    if ((sc.Time == TimeSpan.Zero) || (sc.Time == Duration.Automatic) || (sc.Time == Duration.Forever))
                    {
                        Trace.WriteLine(string.Format("Sliding Text Animation Problem"));
                        return;
                    }

                    animation.AutoReverse = Reverse;

                    // Set the target of the animation
                    Storyboard.SetTarget(animation, this);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("Decal"));

                    // Kick the animation off
                    _Sb.Children.Add(animation);
                    _Sb.Completed += End;


                    _Sb.Begin();
                }
                catch (Exception e)
                {
                    _Sb.Stop();
                    _Sb.Completed -= End;
                    _Sb = null;
                    Trace.WriteLine(string.Format("Sliding Text Animation Problem{0} ", e));
                }

            }
        }

        private void End(object sender, EventArgs ea)
        {
            _Sb = null;
        }

        private bool _MSE = false;

        private void TextOwner_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_Sb == null)
            {
                Vai(true);
                _MSE = true;
            }
        }

        private void TextOwner_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_MSE)
                return;

            _MSE = false;

            if (_Sb != null)
            {
                _Sb.Completed -= End;
                _Sb.Pause();
                _Sb = null;
            }

            Vai(false);
        }

  
    }
}
