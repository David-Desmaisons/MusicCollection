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

using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.FileConverter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated
{
    
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class Itunes: IntegratedBase
    {
        [TearDown]
        public void TD()
        {
            base.CleanDirectories();
        }


        [SetUp]
        public void SetUp()
        {
            Init();
        }

        [Test]
        public void Test()
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IiTunesImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.iTunes) as IiTunesImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.ItunesDirectory = DirectoryIn;
                imib.ImportBrokenTracks = true;
                Assert.That(imib.IsValid, Is.True);

                imi = imib.BuildImporter();
                imi.Load();

                AssertAlbums(ms,OldAlbums[0],AlbumDescriptorCompareMode.AlbumandTrackMD);

            }
        }

        [Test]
        public void Test2()
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IiTunesImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.iTunes) as IiTunesImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.ItunesDirectory = DirectoryIn;
                imib.ImportBrokenTracks = false;
                Assert.That(imib.IsValid, Is.True);

                imi = imib.BuildImporter();
                imi.Load();

                AssertAlbums(ms,OldAlbums[1],AlbumDescriptorCompareMode.AlbumandTrackMD);

            }
        }
    }
}
