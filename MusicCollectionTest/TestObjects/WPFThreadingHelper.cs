using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Threading;
using System.Windows;

namespace MusicCollectionTest.TestObjects
{
    public static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }

    internal class WPFThreadingHelper
    {
        
        private Thread _UIThread;
        private AutoResetEvent _ARE;
        private Window _window;
        private WPFTester _wpfTester;
        private CancellationTokenSource _CTS;

        protected Thread UIThread
        {
            get { return _UIThread; }
        }

        protected Window MainWindow
        {
            get { return _window; }
        }


        internal void BaseSetUp()
        {
            _CTS = new CancellationTokenSource();
            _ARE = new AutoResetEvent(false);
            Thread thread = new Thread(InitUIinSTA);
            thread.Name = "Simulated UI Thread";
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();

            _ARE.WaitOne();
            _UIThread = _window.Dispatcher.Thread;
        }

        protected virtual Window CreateMainWindow()
        {
            return new Window();
        }

        internal void BaseTearDown()
        {
            _CTS.Cancel();
            _UIThread.Join();
            _ARE.Dispose();
            _ARE = null;
            _CTS.Dispose();
            _CTS = null;
        }

        private void InitUIinSTA()
        {
            _wpfTester = new WPFTester();
            _window = CreateMainWindow();
            _wpfTester.ShowWindow(_window);
            _ARE.Set();
      
            while (_CTS.IsCancellationRequested == false)
            {
                DispatcherHelper.DoEvents();
            }
        }
    }
}
