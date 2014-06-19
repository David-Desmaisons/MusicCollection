using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF
{
	public abstract class TransitionBase
	{
		private Duration _duration = new Duration(TimeSpan.FromSeconds(1));

		public Duration Duration
		{
			get { return _duration; }
			set { _duration = value; }
		}

		public abstract Storyboard PrepareStoryboard(TransitionContainer container);
		public abstract FrameworkElement SetupVisuals(VisualBrush prevBrush, VisualBrush nextBrush);
	}
}