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
using System.Collections;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for AlbumGroupedView.xaml
    /// </summary>
    public partial class AlbumGroupedView :UserControl
    {
        public AlbumGroupedView()
        {
            InitializeComponent();
        }

        //#region Dependency Properties


        ////public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
        ////        typeof(double), typeof(AlbumGroupedView), new FrameworkPropertyMetadata(200D,
        ////        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        ////public double ItemHeight
        ////{
        ////    get { return (double)GetValue(ItemHeightProperty); }
        ////    set { SetValue(ItemHeightProperty, value); }
        ////}

        ////public static readonly DependencyProperty SizerProperty = DependencyProperty.Register("Sizer",
        ////        typeof(int), typeof(AlbumGroupedView), new FrameworkPropertyMetadata(0,
        ////        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        ////public int Sizer
        ////{
        ////    get { return (int)GetValue(SizerProperty); }
        ////    set { SetValue(SizerProperty, value); }
        ////}

        ////public static readonly DependencyProperty IsNavigatingProperty = DependencyProperty.Register(
        ////      "IsNavigating", typeof(bool), typeof(AlbumGroupedView), new PropertyMetadata(false,IsNavigatingChangedCallback));

        ////public bool IsNavigating
        ////{
        ////    get { return (bool)GetValue(IsNavigatingProperty); }
        ////    set { SetValue(IsNavigatingProperty, value); }
        ////}

        ////static private void IsNavigatingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        ////{
        ////    AlbumGroupedView alp = d as AlbumGroupedView;
        ////    alp.IsNavigatingChanged((bool)e.NewValue);
        ////}

        ////private void IsNavigatingChanged(bool navigationvalue)
        ////{         
        ////}

        //#endregion

        private void Root_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Focus();
            IInputElement res = Keyboard.Focus(this);
        }

        private void ListDisc_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
