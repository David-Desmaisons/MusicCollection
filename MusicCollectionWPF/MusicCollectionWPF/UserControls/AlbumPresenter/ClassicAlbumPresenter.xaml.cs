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
using System.Windows.Media.Animation;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for ClassicAlbumPresenter.xaml
    /// </summary>
    public partial class ClassicAlbumPresenter : AlbumPresenterBase
        //AlbumPresenterUserControl
    {
        public ClassicAlbumPresenter()
        {
            InitializeComponent();
            //this.DataContextChanged+=OnDataContextChanged;
        }


        //private IMusicSession Session
        //{
        //    set
        //    {
        //        if (value == null)
        //            return;

        //        Sorter = value.AlbumSorter;
        //    }
        //}

        private void ListDisc_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        //private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Session = (e.NewValue as IMusicSession);
        //}

        //private void SizeContextMenu(ContentPresenter vb, FrameworkElement fe)
        //{
        //    ScaleTransform st = vb.RenderTransform as ScaleTransform;

        //    double f = 1;

        //    if (st != null)
        //    {
        //        f = st.ScaleX;
        //    }

        //    fe.ContextMenu.VerticalOffset = vb.ActualHeight * f / 2 - fe.ContextMenu.ActualHeight / 2;
        //}

        //private void DiscImage_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    FrameworkElement fe = sender as FrameworkElement;
        //    ContentPresenter vb = fe.FindAncestor<ContentPresenter>();

        //    if (fe.ContextMenu.ActualHeight != 0)
        //    {
        //        SizeContextMenu(vb, fe);
        //    }
        //    else
        //    {
        //        fe.ContextMenu.SizeChanged += (o, eb) => SizeContextMenu(vb, fe);
        //    }
        //}

        //public override ListBox MyDisc
        //{
        //    get { return ListDisc; }
        //}

        

        //private void Presenter_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        //{

        //}
      
     }
}
