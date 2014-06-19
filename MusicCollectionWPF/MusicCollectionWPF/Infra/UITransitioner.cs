using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.Infra
{
    public class UITransitioner
    {
        private Storyboard _Storyboard;
 
        public UITransitioner(UIElement oldelement, UIElement newelement, TimeSpan iduration)
        {

            _Storyboard = new Storyboard();

            DoubleAnimation animationSplash = new DoubleAnimation(1, 0, new Duration(iduration));
            Storyboard.SetTarget(animationSplash, oldelement);
            Storyboard.SetTargetProperty(animationSplash, new PropertyPath("Opacity"));
            _Storyboard.Children.Add(animationSplash);

            DoubleAnimation animationMain = new DoubleAnimation(0, 1, new Duration(iduration));
            Storyboard.SetTarget(animationMain, newelement);
            Storyboard.SetTargetProperty(animationMain, new PropertyPath("Opacity"));
            _Storyboard.Children.Add(animationMain);

            //EventHandler onend = null;

            //onend = (o, ev) =>
            //{
            //    _Storyboard.Completed -= onend;
            //    OnEnd();
            //};

            //_Storyboard.Completed += onend;
        }

        public Task RunAsync()
        {
            return _Storyboard.RunAsync();
        }

        //public Action OnCompleted;

        //public void Begin()
        //{
        //    _Storyboard.Begin();
        //}

        //private void OnEnd()
        //{
        //    if (OnCompleted != null)
        //        OnCompleted();

        //    OnCompleted = null;
        //}
    }
}
