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

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.Integrated;
using MusicCollectionTest.TestObjects;
using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;

using MusicCollection.WindowsPhone;


namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [NUnit.Framework.Category("DataExchange")]
    [TestFolder("BasicSessionTest")]
    public class Phone8HelperTest : IntegratedBase
    {
        private MusicSessionImpl _Session;
        private IMusicSession _ISession;

        public Phone8HelperTest()
        {
        }

        [TearDown]
        public void TD()
        {
            base.CleanDirectories();
            _Session.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            Init();
            _ISession = MusicSessionImpl.GetSession(_SK.Builder);
            _Session = _ISession as MusicSessionImpl;

            IMusicImporter imi = _ISession.GetDBImporter();
            imi.Load();

            IDirectoryImporterBuilder imib = _ISession.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
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
            var transf = _ISession.AllAlbums.Select(all => AlbumDescriptor.CopyAlbum(all as Album, false));

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
        }
    }
}
