using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicCollectionWPF.Infra
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Waiting : UserControl
    {
        public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register("FillBrush", typeof(Brush), typeof(Waiting),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register("Frequency", typeof(double), typeof(Waiting),
         new PropertyMetadata(0.05D));

        public double Frequency
        {
            get { return (double)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        private Nullable<bool> _Running = null;

        private Storyboard _SB;
        public Waiting()
        {
            InitializeComponent();
        }


        public void Start()
        {
            if (_Running == null)
            {
                _SB.Begin();
                _Running = true;
            }
        }

        public void Stop()
        {
            StopAsync();
        }

        public Task StopAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (_Running != true)
            {
                tcs.SetResult(null);
                return tcs.Task;
            }

            Storyboard SB = new Storyboard();

            _Running = false;

            double cangle = RotateTransform.Angle;

            int Nb = (int)Math.Round((360 - cangle) / 45) + 1;

            DoubleAnimationUsingKeyFrames dauk = new DoubleAnimationUsingKeyFrames();
            for (int i = 0; i < 17; i++)
            {
                DiscreteDoubleKeyFrame first = new DiscreteDoubleKeyFrame(cangle + 45 * i, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * Frequency)));
                dauk.KeyFrames.Add(first);
            }
            Storyboard.SetTarget(dauk, VB);
            Storyboard.SetTargetProperty(dauk, new PropertyPath("(RenderTransform).(RotateTransform.Angle)"));
            SB.Children.Add(dauk);

            for (int i = 0; i < 8; i++)
            {
                ObjectAnimationUsingKeyFrames oauk = new ObjectAnimationUsingKeyFrames();

                oauk.RepeatBehavior = new RepeatBehavior(1);

                DiscreteObjectKeyFrame first = new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(Frequency * Nb)));
                oauk.KeyFrames.Add(first);
                DiscreteObjectKeyFrame first_1 = new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds((i + Nb) * Frequency)));
                oauk.KeyFrames.Add(first_1);
                DiscreteObjectKeyFrame second = new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds((i + Nb) * Frequency)));
                oauk.KeyFrames.Add(second);

                Storyboard.SetTarget(oauk, this.FindName(string.Format("E{0}", i)) as DependencyObject);
                Storyboard.SetTargetProperty(oauk, new PropertyPath("Visibility"));

                SB.Children.Add(oauk);
            }

            _SB.Stop();
           
            EventHandler handler = null;
            handler = delegate
            {
                SB.Completed -= handler;
                tcs.SetResult(null);
            };
            SB.Completed += handler;
            SB.Begin();

            return tcs.Task;
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            _SB = new Storyboard();

            DoubleAnimationUsingKeyFrames dauk = new DoubleAnimationUsingKeyFrames();
            dauk.RepeatBehavior = RepeatBehavior.Forever;
            for (int i = 0; i < 9; i++)
            {
                DiscreteDoubleKeyFrame first = new DiscreteDoubleKeyFrame(45 * i, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * Frequency)));
                dauk.KeyFrames.Add(first);
            }
            Storyboard.SetTarget(dauk, VB);
            Storyboard.SetTargetProperty(dauk, new PropertyPath("(RenderTransform).(RotateTransform.Angle)"));
            _SB.Children.Add(dauk);



            for (int i = 1; i < 8; i++)
            {
                ObjectAnimationUsingKeyFrames oauk = new ObjectAnimationUsingKeyFrames();

                oauk.RepeatBehavior = new RepeatBehavior(1);

                DiscreteObjectKeyFrame first = new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
                oauk.KeyFrames.Add(first);
                DiscreteObjectKeyFrame first_1 = new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * Frequency)));
                oauk.KeyFrames.Add(first_1);
                DiscreteObjectKeyFrame second = new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * Frequency)));
                oauk.KeyFrames.Add(second);

                Storyboard.SetTarget(oauk, this.FindName(string.Format("E{0}", i)) as DependencyObject);
                Storyboard.SetTargetProperty(oauk, new PropertyPath("Visibility"));

                _SB.Children.Add(oauk);
            }

            Start();
        }
    }
}
