//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using MusicCollectionWPF.Infra;
//using NUnit.Framework;
//using FluentAssertions;
//using NSubstitute;
//using MusicCollectionTest.TestObjects;
//using System.Threading;
//using System.Windows;

//namespace MusicCollectionTest.MusicCollectionWPF
//{
//    [TestFixture, RequiresSTA]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("MusicCollectionWPF.Infra")]
//    internal class FrameworkElementExtenderTestor : WPFThreadingHelper
//    {
//        [SetUp]
//        public void SetUp()
//        {
//            BaseSetUp();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            BaseTearDown();
//        }

//        private ManualResetEvent _MRE = new ManualResetEvent(false);
        
//        [Test]
//        [STAThread]
//        public void TestBasic()
//        {
//            MainWindow.GotFocus += MainWindow_MouseDoubleClick;

//            Action Raise = () =>
//                {
//                    RoutedEventArgs newEventArgs = new RoutedEventArgs(FrameworkElement.GotFocusEvent);       
//                    MainWindow.RaiseEvent(newEventArgs);
//                };

//            MainWindow.Dispatcher.BeginInvoke(Raise);


//            _MRE.WaitOne();

//            MainWindow.MouseDown -= MainWindow_MouseDoubleClick;
//        }

//        async void MainWindow_MouseDoubleClick(object sender, RoutedEventArgs e)
//        {
//            FrameworkElement fe = new FrameworkElement();
//            MainWindow.Opacity = 0;
//            MainWindow.Opacity.Should().Be(0);
//            await fe.SmoothSetAsync(FrameworkElement.OpacityProperty, 1, TimeSpan.FromSeconds(2));
//            fe.Width.Should().Be(1);
//            _MRE.Set();
            
//        }

        
//    }
//}
