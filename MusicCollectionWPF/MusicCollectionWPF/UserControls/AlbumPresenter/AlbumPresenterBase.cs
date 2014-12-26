using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    public class AlbumPresenterBase : UserControl
    {
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
         typeof(double), typeof(AlbumPresenterBase), new FrameworkPropertyMetadata(200D,
             FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty SizerProperty = DependencyProperty.Register("Sizer",
            typeof(int), typeof(AlbumPresenterBase), new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public int Sizer
        {
            get { return (int)GetValue(SizerProperty); }
            set { SetValue(SizerProperty, value); }
        }   
    }
}
