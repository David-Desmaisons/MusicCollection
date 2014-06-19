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
using System.Windows.Shapes;
using MusicCollection.Infra;

using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public SplashScreen(SplashScreenViewModel imssh)
        {
            DataContext = imssh;
            InitializeComponent();
            
            //this.Version.Text = string.Format("V {0}", VersionInfo.GetVersionInfo().ToString());
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                e.Handled = false;
            }
        }

        //public void Close(TimeSpan iTs)
        //{

        //   Storyboard sb = new Storyboard();


           

        //    DoubleAnimation animation = new DoubleAnimation(0, new Duration(iTs));
        //    Storyboard.SetTarget(animation, WholeImage);
        //    Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
        //    sb.Children.Add(animation);

        //    EventHandler onend = null;

        //    onend = (o, e) =>
        //        {
        //            sb.Completed -= onend;
        //            this.Close();
        //        };

        //    sb.Completed += onend;
        //    sb.Begin();
            
            
        //}
        //private void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //this.SizeToContent = SizeToContent.WidthAndHeight;
        //}
    }
}
