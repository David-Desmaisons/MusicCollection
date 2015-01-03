using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Windows.Input;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF
{
    [ContentProperty("Children")]
    public class TransitionContainer : ContentControl
    {
        public event EventHandler TransitionCompleted;

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(TransitionBase), typeof(TransitionContainer), new PropertyMetadata(new FadeTransition()));

        private readonly Grid _childContainer;
        private readonly Grid _rootContainer;
        private readonly Grid _transitionContainer;

        private UIElement _nextChild;
        private UIElement _prevChild;

        private UIElement _CurrentElem;
        private UIElement CurrentElem
        {
            get { return _CurrentElem; }
            set
            {
                _CurrentElem = value;

                if (_CurrentElem != null)
                {
                    _CurrentElem.Focus();
                    Keyboard.Focus(_CurrentElem);
                }
            }
        }   

        public static readonly DependencyProperty ForceCurrentProperty = DependencyProperty.Register("Current",
        typeof(UIElement), typeof(TransitionContainer), new PropertyMetadata(null, ForceCurrentPropertyChangedCallback));

        public UIElement Current
        {
            get { return (UIElement)GetValue(ForceCurrentProperty); }
            set { SetValue(ForceCurrentProperty, value); }
        }

        static private void ForceCurrentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nv = e.NewValue as UIElement;
            if (nv == null)
                return;

            TransitionContainer tc = d as TransitionContainer;
            tc.ApplyTransition(nv);
        }

        public UIElementCollection Children
        {
            get { return _childContainer.Children; }
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            if (CurrentElem == null)
                CurrentElem = ((_childContainer.Children == null) || _childContainer.Children.Count == 0) ? null : _childContainer.Children[0];
            else
                ChangeChildrenStackOrder(CurrentElem);

            if (CurrentElem != null)
            {
                _nextChild = CurrentElem;
                FinishTransition();
            }
        }

        public TransitionBase Transition
        {
            get { return (TransitionBase)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }


        public TransitionContainer()
        {
            _childContainer = new Grid();
            _transitionContainer = new Grid();

            _rootContainer = new Grid();
            _rootContainer.Children.Add(_transitionContainer);
            _rootContainer.Children.Add(_childContainer);

            Content = _rootContainer;

            this.Loaded += Init;
        }

        public void ApplyTransition(UIElement nextChild)
        {
            ApplyTransition(CurrentElem, nextChild);
        }

        public void ApplyTransition(string nextChildName)
        {
            FrameworkElement nextChild = (FrameworkElement)FindName(nextChildName);
            ApplyTransition(nextChild);
        }


        public void ApplyTransition(string prevChildName, string nextChildName)
        {
            FrameworkElement prevChild = (FrameworkElement)FindName(prevChildName);
            FrameworkElement nextChild = (FrameworkElement)FindName(nextChildName);

            ApplyTransition(prevChild, nextChild);
        }

        public void ApplyTransition(UIElement prevChild, UIElement nextChild)
        {
            if ((prevChild == nextChild) && (_SB == null))
                return;

            if (prevChild == null)
            {
                CurrentElem = nextChild;
                return;
            }

            if (nextChild == null)
            {
                throw new ArgumentNullException("nextChild cannot be null");
            }

            _prevChild = prevChild;
            _nextChild = nextChild;


            if (_SB == null)
                StartTransition();
            else
            {
                if (_EA != null)
                {
                    _SB.Completed -= _EA;
                    _EA = null;
                }
                _SB.Stop();
                _SB.Remove();
                _SB = null;
                _UnderTransition = false;
                ChangeNoTransition(nextChild, true);
            }
        }

        private bool _UnderTransition = false;
        private Storyboard _SB = null;
        private EventHandler _EA = null;

        private void StartTransition()
        {
            if (_UnderTransition)
                return;

            _UnderTransition = true;

       
      
            // Make the children Visible, so that the VisualBrush will not be blank
            _prevChild.Visibility = Visibility.Visible;
            _nextChild.Visibility = Visibility.Visible;

            // Switch to transition-mode
            double h = this.ActualHeight;
            double w = this.ActualWidth;
            FrameworkElement root = Transition.SetupVisuals(_prevChild.CreateBrush(w, h), _nextChild.CreateBrush(w, h));

            _transitionContainer.Children.Add(root);
            _transitionContainer.Visibility = Visibility.Visible;
            _childContainer.Visibility = Visibility.Hidden;

            // Get Storyboard to play
            _SB = Transition.PrepareStoryboard(this);
            _EA = delegate
                        {
                            _SB.Completed -= _EA;
                            FinishTransition();
                            _EA = null;
                        };
            _SB.Completed += _EA;
            _SB.Begin(_transitionContainer);
        }

        public void ChangeNoTransition(UIElement final, bool force = false)
        {

            if (CurrentElem == null)
            {
                CurrentElem = final;
                return;
            }

            if ((!force) && (CurrentElem == final))
                return;

            final.Visibility = Visibility.Visible;

            ChangeChildrenStackOrder(final);

            _transitionContainer.Visibility = Visibility.Hidden;
            _childContainer.Visibility = Visibility.Visible;
            _transitionContainer.Children.Clear();
            CurrentElem = final;

            NotifyTransitionCompleted();

        }

        public void FinishTransition()
        {
            _nextChild.Visibility = Visibility.Visible;
            // Bring the next-child on top
            ChangeChildrenStackOrder(_nextChild);

            _transitionContainer.Visibility = Visibility.Hidden;
            _childContainer.Visibility = Visibility.Visible;
            _transitionContainer.Children.Clear();
            CurrentElem = _nextChild;
         

            NotifyTransitionCompleted();

            _UnderTransition = false;

            if (_SB != null)
            {
                _SB.Remove();
                _SB = null;
            }
        }



        private void ChangeChildrenStackOrder(UIElement target)
        {
            Panel.SetZIndex(target, 1);
            foreach (UIElement element in _childContainer.Children)
            {
                if (element != target)
                {
                    Panel.SetZIndex(element, 0);
                    element.Visibility = Visibility.Hidden;
                }
            }

        }

        private void NotifyTransitionCompleted()
        {
            if (TransitionCompleted != null)
            {
                TransitionCompleted(this, null);
            }
        }
    }
}