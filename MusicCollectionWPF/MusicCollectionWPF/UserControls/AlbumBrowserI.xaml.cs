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
using System.Windows.Controls.Primitives;// {System.Windows.Controls.ListBox}


using System.Timers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.UserControls.AlbumPresenter;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumBrowser.xaml
    /// </summary>
    public partial class AlbumBrowserI : UserControl        
    {
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(AlbumBrowserI), new PropertyMetadata(StatusPropertyChanged));

        private static void StatusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AlbumBrowserI ab = d as AlbumBrowserI;
            ab.statustext.AddMessage(e.NewValue as string);
        }
 

        public AlbumBrowserI()
        {
            InitializeComponent();
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;

            if (lbi.IsSelected)
            {
                e.Handled = true;
                return;
            }

             lbi.IsSelected = true;

            e.Handled = true;
        }


        public MusicCollection.Fundation.AlbumPresenter AlbumPresenter
        {
            get { return (MusicCollection.Fundation.AlbumPresenter)GetValue(AlbumPresenterProperty); }
            set { SetValue(AlbumPresenterProperty, value); }
        }

        public static readonly DependencyProperty AlbumPresenterProperty = DependencyProperty.Register("AlbumPresenter",
        typeof(MusicCollection.Fundation.AlbumPresenter), typeof(AlbumBrowserI), new PropertyMetadata(OnPresenterChanged));

        private static void OnPresenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AlbumBrowserI ab = d as AlbumBrowserI;
            ab.UpdatePresenter();
        }

        private void UpdatePresenter()
        {
            UserControl rawnext = this.FindName(AlbumPresenter.ToString()) as UserControl;
            UserControl old = transitionContainer.Current as UserControl;

            if (old == rawnext)
                return;

            transitionContainer.Current = rawnext;
        }
         
          

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Escape:
                    Finder.Reset();
                    break;

                case Key.Enter:
                    Finder.Commit();
                    break;
            }
        }

        private void searcher_Click(object sender, RoutedEventArgs e)
        {
            Finder.Reset();
        }

        private void Search_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
             if (!string.IsNullOrEmpty(Search.Text))
                 Finder.IsOpen = true;
             else
                 Finder.Reset();
        }
      
    }
}
