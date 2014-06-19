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

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Utilies.Edition;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

using FluentAssertions;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class ReNameTestor2 : IntegratedBase
    {

        public ReNameTestor2()
        {
        }

        [TestFixtureTearDown]
        public void TDO()
        {
            base.CleanDirectories();
        }

        [TearDown]
        public void TD()
        {
            base.BigClean();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            Init();
        }

        [Test]
        public void Test()
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                ms.AllAlbums.Count.Should().Be(0);
                ms.AllGenres.Count.Should().Be(0);
                ms.AllArtists.Count.Should().Be(0);
               
                IMusicImporter imi = ms.GetDBImporter();
                imi.Should().NotBeNull();
                imi.Load();

                ms.AllAlbums.Count.Should().Be(0);
                ms.AllGenres.Count.Should().Be(25);
                ms.AllArtists.Count.Should().Be(0);

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                imib.Should().NotBeNull();
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                imib.IsValid.Should().BeTrue();
                imi = imib.BuildImporter();
                imi.Should().NotBeNull();
                imi.Load();

                ms.ShouldHaveAlbumsLike(Albums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);
                ms.AllAlbums.ShouldBeCoherentWithAlbums(Albums[0]);

                IAlbum al = ms.AllAlbums.FirstOrDefault(a => ( (a.Name == "A") && (a.Author == "Bb")) );
                al.Should().NotBeNull();

                using (IModifiableAlbum ima = al.GetModifiableAlbum())
                {
                    ima.Name = "Aa";
                    ima.Artists.Clear();
                    ima.Artists.Add(ms.CreateArtist("B"));
                    ima.Commit(true);
                }

                ms.ShouldHaveAlbumsLike(Albums[1], AlbumDescriptorCompareMode.AlbumandTrackMD);
                ms.AllAlbums.ShouldBeCoherentWithAlbums(Albums[1]);
            }
            Console.WriteLine("In Session sucessfull");
        }



    }
}
