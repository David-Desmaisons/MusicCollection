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

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Nhibernate.Session;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder("BasicSessionTest")]
    class ArtitsCleanerTestor : IntegratedBase
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

        protected override SessionKeys GetKeys()
        {
            return new SessionKeys(DirectoryOut);
        }
      

        [Test]
        public void Test()
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);


                Console.WriteLine("Import Successful 5 Albums");


                IAlbum al = ms.AllAlbums[0];
                IList<IArtist> la = al.Artists.ToList();

                Assert.That(la.All(a => a.Albums.Count == 1));
                Assert.That(la.All(a => a.Albums[0] == al));

                IGenre g = al.MainGenre;

                g.Should().NotBeNull();
                g.Albums.Should().Contain(al);

                IMusicRemover imr = ms.GetMusicRemover();
                imr.AlbumtoRemove.Add(al);
                imr.IncludePhysicalRemove = true;
                imr.Comit(true);

                ms.AllAlbums.Should().HaveCount(4);
                ms.AllAlbums.Should().NotContain(al);

                la.All(a => a.Albums.Count == 0).Should().BeTrue();

                g.Albums.Should().NotContain(al);

                Console.WriteLine("Delete Successful 1 Album");
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(9));
            }
        }
    }
}
