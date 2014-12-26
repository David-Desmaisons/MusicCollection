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

        //public virtual ListBox MyDisc
        //{
        //    get { return null; }
        //}

        protected ScrollViewer ScrollViewer { get; set; }


        protected void OnKeyBoardEvent(object sender, KeyEventArgs e)
        {
            ScrollViewer isc = ScrollViewer;
            if (isc == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                    isc.LineUp();
                    e.Handled = true;
                    break;

                case Key.Down:
                    isc.LineDown();
                    e.Handled = true;
                    break;

                case Key.Left:
                     isc.LineLeft();

                    break;

                case Key.Right:
                    isc.LineRight();
                    break;

                case Key.PageUp:
                     isc.PageUp();
                     e.Handled = true;
                    break;

                case Key.PageDown:
                    isc.PageDown();
                    e.Handled = true;
                    break;
            }
        }

        protected void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer = sender as ScrollViewer;
        }
    }
}
