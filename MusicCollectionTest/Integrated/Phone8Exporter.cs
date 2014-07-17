using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.FileConverter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

using MusicCollection.WindowsPhone;
using MusicCollection.Implementation.Session;
using System.Windows;
using System.Windows.Interop;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder("BasicSessionTest")]
    [Ignore("Ignore as need windows phone connected")]
    public class Phone8Exporter : IntegratedBase
    {
        private MusicSessionImpl _Session;
        private IMusicSession _ISession;

        [TearDown]
        public void TD()
        {
            base.CleanDirectories();
            _Session.Dispose();
        }

        private WPFTester _wpfTester;
        private Window _window;


        private class WPFHwndProvider : IMainWindowHwndProvider
        {
            Window _window;
            public WPFHwndProvider(Window w)
            {
                _window = w;
            }

            public IntPtr MainWindow
            {
                get { return new WindowInteropHelper(_window).Handle; }
            }
        }

        [SetUp]
        [STAThread]
        public void SetUp()
        {
           
            _wpfTester = new WPFTester();

            _window = new Window();

            _wpfTester.ShowWindow(_window);

            Init();
            _ISession = MusicSessionImpl.GetSession(_SK.Builder, new WPFHwndProvider(_window));
            _Session = _ISession as MusicSessionImpl;

           IMusicImporter imi = _ISession.GetDBImporter();
           imi.Load();

           IDirectoryImporterBuilder imib = _ISession.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
           Assert.That(imib, Is.Not.Null);
           imib.Directory = DirectoryIn;
           imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

           Assert.That(imib.IsValid, Is.True);
           imi = imib.BuildImporter();
           Assert.That(imi, Is.Not.Null);
           imi.Load();
        }

        [Test]
        public void Test()
        {
            var transf = _ISession.AllAlbums.Select(all => AlbumDescriptor.CopyAlbum(all as Album,false));

            transf.Count().Should().Be(5);
            foreach (TrackDescriptor ial in transf.SelectMany(al => al.TrackDescriptors))
            {
                DataExchanger<TrackDescriptor> det = new DataExchanger<TrackDescriptor>(ial);
                Dictionary<string, object> res = new Dictionary<string, object>();
                det.Describe(DataExportImportType.WindowsPhone, res.ToObserver());
                res.Should().ContainKey("Artist");
                res.Should().ContainKey("Album");
                res.Should().ContainKey("Genre");
                res.Should().ContainKey("Path");
                res.Should().ContainKey("Name");
            }

            WindowsPhoneExporter wpe = new WindowsPhoneExporter(_Session);
            wpe.AlbumToExport = _ISession.AllAlbums.FirstOrDefault().SingleItemCollection();
            wpe.Export(true);
        }

        [TearDown]
        public void TearDown()
        {
            _wpfTester.Close();
        }
    }
}
