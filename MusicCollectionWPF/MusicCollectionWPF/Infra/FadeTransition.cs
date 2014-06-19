using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF
{
	public class FadeTransition : TransitionBase
	{
		private Rectangle _nextRect;
		private Rectangle _prevRect;
		private Grid _rectContainer;

		public override FrameworkElement SetupVisuals(VisualBrush prevBrush, VisualBrush nextBrush)
		{
			_prevRect = new Rectangle();
			_prevRect.Fill = prevBrush;

			_nextRect = new Rectangle();
			_nextRect.Fill = nextBrush;

			_rectContainer = new Grid();
			_rectContainer.ClipToBounds = true;
			_rectContainer.Children.Add(_nextRect);
			_rectContainer.Children.Add(_prevRect);

			return _rectContainer;
		}

		public override Storyboard PrepareStoryboard(TransitionContainer container)
		{
            return _prevRect.PrepareStoryboardTransitionTo(new Storyboard(),_nextRect, Duration);
		}
	}
}