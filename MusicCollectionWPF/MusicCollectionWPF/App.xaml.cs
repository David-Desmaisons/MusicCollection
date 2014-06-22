using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.Infra;
using MusicCollection.Implementation.Session;
using System.Windows.Interop;
using MusicCollectionWPF.ViewModel;
using PyBinding;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using MusicCollectionWPF.ViewModel.Interface;
using System.Deployment.Application;
using System.ServiceModel;
using MusicCollection.Infra.Communication;

namespace MusicCollectionWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
        }

        private static ThreadProperties _CurrentProperty;
        private static MusicCollectionWPF.Windows.SplashScreen _SplashScreen;

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main(string[] args)
        {

            _CurrentProperty = ThreadProperties.FromCurrentThread();

            using (var mutex = new Mutex(false, "MusicCollection SingleApplication"))
            {
                // Wait a few seconds if contended, in case another instance
                // of the program is still in the process of shutting down.
                if (!mutex.WaitOne(TimeSpan.FromSeconds(3), false))
                {
                    Trace.WriteLine("Another instance of the MusicCollection is running!");

                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        string activationData = GetInputFilePath();
                        if (activationData != null)
                        {
                            try
                            {
                                IMusicFileImporter imf = new IPCClient<IMusicFileImporter>().GetService();
                                imf.ImportCompactedFileAsync(activationData).Wait();
                            }
                            catch { }
                        }
                    }

                    return;
                }

                Execute(args);
            }
        }

        private static string GetInputFilePath()
        {
            string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            if ((activationData != null) && (activationData.Length > 0))
            {
                return new Uri(activationData[0]).LocalPath;
            }

            return null;
        }


        private static bool _EventDone = false;
        private static void OnException(UnhandledExceptionEventArgs e, IMusicSession Ims)
        {
            if (_EventDone)
                return;

            ExceptionManager em = new ExceptionManager(Ims, e.ExceptionObject);
            em.Deal(MessageBox.Show(
                string.Format("Music Collection encountered a fatal error and needs to close.{0}Click Ok to allow music collection to communicate details about the error to administator.{0}This will help improving Music collection quality. {0}", Environment.NewLine)
                , "Fatal Error Detected", MessageBoxButton.OKCancel) == MessageBoxResult.OK);

            _EventDone = true;
        }

        private class WPFHwndProvider : IMainWindowHwndProvider
        {
            public IntPtr MainWindow
            {
                get { return SafeMainWindow; }
            }

            private IntPtr SafeMainWindow
            {
                get
                {
                    Func<IntPtr> del = GetUnSafeSafeMainWindow;
                    return (IntPtr)Application.Current.Dispatcher.Invoke(del, null);
                }
            }

            private IntPtr GetUnSafeSafeMainWindow()
            {
                if (App.Current.MainWindow == null) return (IntPtr)null;

                return new WindowInteropHelper(App.Current.MainWindow).Handle;
            }
        }

        private static IMusicSession _IS;

        private static void Execute(string[] args)
        {
            MusicCollectionWPF.App app = new MusicCollectionWPF.App();

            using (_IS = MusicSession.GetSession(new WPFHwndProvider()))
            {
                AppDomain.CurrentDomain.UnhandledException += (o, e) => OnException(e, _IS);

                _SplashScreen = new MusicCollectionWPF.Windows.SplashScreen(new SplashScreenViewModel(_IS.SplashScreen));
                _SplashScreen.Loaded += slaphscreen_Loaded;
                app.Run(_SplashScreen);

                if (_MusicImporterService != null)
                {
                    _MusicImporterService.Dispose();
                    _MusicImporterService = null;
                }
            }
        }


        static async private void slaphscreen_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadProperties TP = new ThreadProperties(ThreadPriority.Highest, null);

            new ThreadProperties(ThreadPriority.Normal, ProcessPriorityClass.High).SetCurrentThread();

            using (var tt = TimeTracer.TimeTrack("Load Time"))
            {
                IMusicImporter DB = _IS.GetDBImporter();

                await Task.WhenAll(DB.LoadAsync(TP), ScriptConverter.LoadAsync(TP));
            }

            MusicCollectionWPF.Windows.MainWindow window = new MusicCollectionWPF.Windows.MainWindow(_IS);
            window.Opacity = 0;
            window.Loaded += window_Loaded;
            App.Current.MainWindow = window;

            window.Show();
        }

        private static IPCServer<IMusicFileImporter> _MusicImporterService;

        static async private void window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(3);

            _CurrentProperty.SetCurrentThread();

            MusicCollectionWPF.Windows.MainWindow win = sender as MusicCollectionWPF.Windows.MainWindow;

            await new UITransitioner(_SplashScreen, win, TimeSpan.FromSeconds(1)).RunAsync();
            _SplashScreen.Close();

            IMusicFileImporter imf = win;
            _MusicImporterService = new IPCServer<IMusicFileImporter>(imf);

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string path = GetInputFilePath();
                if (path != null)
                {
                    await imf.ImportCompactedFileAsync(path);
                }
            }

        }
    }
}
