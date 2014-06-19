using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.Infra
{
    public static class FrameworkElementExtender
    {
        public static void SmoothSet(this FrameworkElement @this, DependencyProperty dp, double targetvalue, TimeSpan iDuration)
        {
            DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
            PropertyPath p = new PropertyPath("(0)", dp);
            Storyboard.SetTargetProperty(anim, p);
            Storyboard sb = new Storyboard();
            sb.Children.Add(anim);
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;
                sb.Remove(@this);
                @this.SetValue(dp, targetvalue);
            };
            sb.Completed += handler;
            sb.Begin(@this, true);
        }

        public static Task SmoothSetAsync(this FrameworkElement @this, DependencyProperty dp, double targetvalue, TimeSpan iDuration)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
            PropertyPath p = new PropertyPath("(0)", dp);
            Storyboard.SetTargetProperty(anim, p);
            Storyboard sb = new Storyboard();
            sb.Children.Add(anim);
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;
                sb.Remove(@this);
                @this.SetValue(dp, targetvalue);
                tcs.SetResult(null);
            };
            sb.Completed += handler;
            sb.Begin(@this, true);

            return tcs.Task;
        }

        public static Task SmoothSetAsync(this FrameworkElement @this, DependencyProperty dp, double targetvalue,
            TimeSpan iDuration, CancellationToken iCancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
            PropertyPath p = new PropertyPath("(0)", dp);
            Storyboard.SetTargetProperty(anim, p);
            Storyboard sb = new Storyboard();
            sb.Children.Add(anim);
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;
                sb.Remove(@this);
                @this.SetValue(dp, targetvalue);
                tcs.TrySetResult(null);
            };
            sb.Completed += handler;
            sb.Begin(@this, true);

            iCancellationToken.Register(() =>
            {
                double v = (double)@this.GetValue(dp);  
                sb.Stop(); 
                sb.Remove(@this); 
                @this.SetValue(dp, v);
                tcs.TrySetCanceled();
            });

            return tcs.Task;
        }


        //public static void SmoothSet(this DependencyObject @this, DependencyProperty dp, FrameworkElement father,double targetvalue, TimeSpan iDuration)
        //{
        //    Console.WriteLine(string.Format("Begin Set to {0}", targetvalue));
        //    DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
        //    PropertyPath p = new PropertyPath("(0)", dp);
        //    Storyboard.SetTargetProperty(anim, p);
        //    Storyboard.SetTarget(anim, @this);
        //    Storyboard sb = new Storyboard();
        //    sb.Children.Add(anim);
        //    EventHandler handler = null;
        //    handler = delegate
        //    {
        //        sb.Completed -= handler;
        //        sb.Remove(father);
        //        @this.SetValue(dp, targetvalue);
        //        Console.WriteLine(string.Format("Set to {0}", targetvalue));
        //    };
        //    sb.Completed += handler;
        //    sb.Begin(father,true);
        //}


        public static Task SmoothSet(this DependencyObject @this, DependencyProperty dp, FrameworkElement father, double targetvalue,
            TimeSpan iDuration, CancellationToken iCancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            //Console.WriteLine(string.Format("Begin Set to {0}", targetvalue));
            DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
            //anim.en
            //{
            //    EnableDependentAnimation = true
            //};
            PropertyPath p = new PropertyPath("(0)", dp);
            Storyboard.SetTargetProperty(anim, p);
            Storyboard.SetTarget(anim, @this);
            Storyboard sb = new Storyboard();
            sb.FillBehavior = FillBehavior.Stop;
            sb.Children.Add(anim);
            EventHandler handler = null;
            handler = delegate
            {
                sb.Completed -= handler;             
                sb.Stop();
                sb.Remove(father);
                @this.SetValue(dp, targetvalue);
                tcs.TrySetResult(null);
                //if (tcs.TrySetResult(null))
                //    Console.WriteLine(string.Format("Set to {0}", targetvalue));
            };
            sb.Completed += handler;
            sb.Begin(father, true);

            iCancellationToken.Register(() =>
            {
                double v = (double)@this.GetValue(dp);
                sb.Stop();
                sb.Remove(father);
                @this.SetValue(dp, v);
                tcs.TrySetCanceled();
            //    if (tcs.TrySetCanceled())
            //        Console.WriteLine(string.Format("Cancelled to {0}", targetvalue));
            });

            return tcs.Task;
        }

        public static async Task SafeSmoothSet(this DependencyObject @this, DependencyProperty dp, FrameworkElement father, double targetvalue,
           TimeSpan iDuration, CancellationTokenSource iCancellationToken)
        {
            await SmoothSet(@this, dp, father, targetvalue, iDuration, iCancellationToken.Token).WithTimeout(iDuration.Milliseconds + 50, iCancellationToken);
            @this.SetValue(dp, targetvalue);
        }

        // public static void SmoothSet(this DependencyObject @this, DependencyProperty dp, double targetvalue,
        //      TimeSpan iDuration, CancellationToken iCancellationToken)
        //{
        //    DoubleAnimation anim = new DoubleAnimation(targetvalue, new Duration(iDuration));
        //    PropertyPath p = new PropertyPath("(0)", dp);
        //    Storyboard.SetTargetProperty(anim, p);
        //    Storyboard.SetTarget(anim, @this);
        //    Storyboard sb = new Storyboard();
        //    sb.Children.Add(anim);
        //    EventHandler handler = null;
        //    handler = delegate
        //    {
        //        sb.Completed -= handler;
        //        sb.Remove();
        //        @this.SetValue(dp, targetvalue);
        //    };
        //    sb.Completed += handler;
        //    sb.Begin();
        

        //    iCancellationToken.Register(() =>
        //    {
        //        double v = (double)@this.GetValue(dp);
        //        sb.Stop();
        //        @this.SetValue(dp, v);
        //    });
        //}
    }
}
