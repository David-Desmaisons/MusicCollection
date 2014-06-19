using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MusicCollectionWPF.Infra
{
    public class ScrollViewerAttribute
    {
        public static readonly DependencyProperty ArrowStyleSmallProperty = DependencyProperty.RegisterAttached("ArrowStyleSmall", typeof(bool), typeof(ScrollViewerAttribute), new FrameworkPropertyMetadata(true,FrameworkPropertyMetadataOptions.Inherits));

        public static void SetArrowStyleSmall(DependencyObject element, bool value)
        {
            element.SetValue(ScrollViewerAttribute.ArrowStyleSmallProperty, value);
        }

        public static bool GetArrowStyleSmall(DependencyObject element)
        {
            return (bool)element.GetValue(ScrollViewerAttribute.ArrowStyleSmallProperty);
        }
    }
}
