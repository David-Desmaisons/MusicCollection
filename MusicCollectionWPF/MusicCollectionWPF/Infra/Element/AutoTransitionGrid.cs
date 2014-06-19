using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Collections;
using System.Linq;
using System.Windows.Media.Imaging;

using MusicCollection.Infra;

namespace MusicCollectionWPF.Infra
{
    public interface ITransitioner : IDisposable
    {
    }

    internal interface IAbstractTransitor : ITransitioner
    {
        Storyboard CompleteSB(Storyboard animator);

        void PrepareEnd();
    }


    internal abstract class TransitionBase : IAbstractTransitor
    {
        public void Dispose()
        {
            PrepareEnd();
            Storyboard animator = new Storyboard();
            CompleteSB(animator).Begin();
        }

        public abstract Storyboard CompleteSB(Storyboard animator);

        public abstract void PrepareEnd();
    }



    public static class ITransitionerExt
    {
        private class AddTransitor : TransitionBase
        {
            private IAbstractTransitor _A1;
            private IAbstractTransitor _A2;
            public AddTransitor(IAbstractTransitor iA1, IAbstractTransitor iA2)
            {
                _A1 = iA1;
                _A2 = iA2;
            }

            override public Storyboard CompleteSB(Storyboard animator)
            {
                Storyboard res = _A1.CompleteSB(animator);
                res = _A2.CompleteSB(res);
                return res;
            }

            override public void PrepareEnd()
            {
                _A1.PrepareEnd(); _A2.PrepareEnd();
            }
        }

        public static ITransitioner Merge(this ITransitioner first, ITransitioner second)
        {
            IAbstractTransitor First = first as IAbstractTransitor;
            if (First == null)
                return second;

            IAbstractTransitor Second = second as IAbstractTransitor;

            if (Second == null)
                return first;

            return new AddTransitor(First, Second);
        }
    }


    public class AutoTransitionGrid : Grid
    {

        public Duration Duration
        {
            get;
            set;
        }

        public AutoTransitionGrid()
            : base()
        {
            this.Background = Brushes.Transparent;
            Duration = new Duration(TimeSpan.FromSeconds(1));
            FocusVisualStyle = null;
        }





        private class Transitor : TransitionBase
        {
            private AutoTransitionGrid _Father;
            public Transitor(AutoTransitionGrid Father)
            {
                _Father = Father;
                _Father.InitTransaction();
            }

            override public Storyboard CompleteSB(Storyboard animator)
            {
                return _Father.GetFinalTransition(animator);
            }

            override public void PrepareEnd()
            {
                _Father.PrepareEnd();
            }
        }

        private Transitor _CuT;

        public ITransitioner GetTransitionner()
        {
            if (_CuT != null)
            {
                return null;
            }

            _CuT = new Transitor(this);
            return _CuT;
        }

        //private Rectangle _Rec;
        private void InitTransaction()
        {
            this.Background = this.CreateFrozenBrush();
            Children.Cast<UIElement>().Apply(u => u.Visibility = Visibility.Hidden);
            this.IsHitTestVisible = false;
        }


        private void PrepareEnd()
        {
            Children.Cast<UIElement>().Apply(u => { u.Opacity = 0; u.Visibility = Visibility.Visible; });
        }

        private Storyboard GetFinalTransition(Storyboard animator)
        {
            WPFHelper.PrepareStoryboardTransitionTo(animator, Children.Cast<UIElement>(), Duration);

            DoubleAnimation nextAnim = new DoubleAnimation();
            Storyboard.SetTarget(nextAnim, this);
            Storyboard.SetTargetProperty(nextAnim, new PropertyPath("Background.Opacity"));
            nextAnim.Duration = Duration;
            nextAnim.From = 1;
            nextAnim.To = 0;
            animator.Children.Add(nextAnim);


            EventHandler handler = null;
            handler = delegate
            {
                animator.Completed -= handler;
                this.IsHitTestVisible = true;
                _CuT = null;
            };
            animator.Completed += handler;

            return animator;
        }
    }
}
