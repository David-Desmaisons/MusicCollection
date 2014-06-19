using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using MusicCollectionWPF.CustoPanel;

namespace MusicCollectionWPF.Infra.Behaviour
{
    class CircularContextMenuItemBehaviour
    {
        public static readonly DependencyProperty IsCircularProperty = DependencyProperty.RegisterAttached("IsCircular",
             typeof(bool), typeof(CircularContextMenuItemBehaviour), new PropertyMetadata(false, BehaviourChanged));

        public static bool GetIsCircular(DependencyObject element)
        {
            return (bool)element.GetValue(IsCircularProperty);
        }

        public static void SetIsCircular(DependencyObject element, bool value)
        {
            element.SetValue(IsCircularProperty, value);
        }

        private static void BehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem mi = d as MenuItem;

            if (mi == null)
                return;

            if (!(bool)e.OldValue)
            {
                mi.MouseEnter -= Grid_MouseEnter;
                mi.MouseLeave -= Grid_MouseLeave;
            }

            if ((bool)(e.NewValue))
            {
                mi.MouseEnter += Grid_MouseEnter;
                mi.MouseLeave += Grid_MouseLeave;
            }
        }

        private static Tuple<ContentPresenter, ContentPresenter, FrameworkElement> GetCP(object sender)
        {
            MenuItem mi = sender as MenuItem;
            if (mi == null)
                return null;

            Grid p = WPFHelper.FindAncestor<Grid>(mi);

            return new Tuple<ContentPresenter, ContentPresenter, FrameworkElement>
                (p.FindName("CurrentHeaderPlace") as ContentPresenter,
                 p.FindName("OldHeaderPlace") as ContentPresenter,
                  p.FindName("Wheel") as FrameworkElement);
        }

        private static double normaliseangle(double angle)
        {
            return (angle - 180) % 360 + 180;
        }

        private static void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var cp = GetCP(sender);

            if (cp == null)
            {
                return;
            }

            MenuItem mi = (sender as MenuItem);

            cp.Item1.Content = mi.Header;

            cp.Item1.Opacity = 0;

            Storyboard sb = new Storyboard();


            double currentangle = normaliseangle((cp.Item3.RenderTransform as RotateTransform).Angle);
            double targetangle = -SimpleCirclePanel.GetDecalAngle(mi);

            targetangle = normaliseangle(targetangle - currentangle) + currentangle;

            double delta = targetangle - currentangle;

            if (delta > 180)
            {
                targetangle = targetangle - 360;
            }
            else if (delta < -180)
            {
                targetangle = targetangle + 360;
            }

            //Console.WriteLine(targetangle);

            DoubleAnimation da0 = new DoubleAnimation(currentangle, targetangle, new Duration(TimeSpan.FromSeconds(0.3)));
            da0.AccelerationRatio = 0.2; da0.DecelerationRatio = 0.2;
            Storyboard.SetTarget(da0, cp.Item3);
            Storyboard.SetTargetProperty(da0, new PropertyPath("(Path.RenderTransform).(RotateTransform.Angle)"));
            sb.Children.Add(da0);

            DoubleAnimationUsingKeyFrames myAnimation = new DoubleAnimationUsingKeyFrames();

            DiscreteDoubleKeyFrame opacityChange1 = new DiscreteDoubleKeyFrame();
            opacityChange1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            opacityChange1.Value = 0;
            myAnimation.KeyFrames.Add(opacityChange1);

            DiscreteDoubleKeyFrame opacityChange2 = new DiscreteDoubleKeyFrame();
            opacityChange2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4));
            opacityChange2.Value = 0;
            myAnimation.KeyFrames.Add(opacityChange2);

            LinearDoubleKeyFrame opacityChange3 = new LinearDoubleKeyFrame();
            opacityChange3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.9));
            opacityChange3.Value = 1;
            myAnimation.KeyFrames.Add(opacityChange3);

            Storyboard.SetTarget(myAnimation, cp.Item1);
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath("Opacity"));
            sb.Children.Add(myAnimation);

            sb.Begin();

        }

        private static void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var cp = GetCP(sender);

            if (cp == null)
                return;

            cp.Item1.Content = null;
            cp.Item2.Content = (sender as MenuItem).Header;

            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(da, cp.Item2);
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            sb.Children.Add(da);
            sb.Begin();
        }
    }
}
