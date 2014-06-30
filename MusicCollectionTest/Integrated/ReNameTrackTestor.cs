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
    internal class ReNameTrackTestor : IntegratedTestImprovedBase
    {
        public ReNameTrackTestor()
        {
        }

        #region test helpers

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            baseTestFixtureSetUp();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            baseTestFixtureTearDown();
        }

        [SetUp]
        public  void SetUp()
        {
            baseSetUp();
        }

        protected override void PostOpen(IMusicSession ims)
        {
            ims.AllAlbums.Should().BeEmpty();
            ims.AllGenres.Count.Should().Be(25);
            ims.AllArtists.Should().BeEmpty();

            IDirectoryImporterBuilder imib = ims.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
            imib.Should().NotBeNull();
            imib.Directory = DirectoryIn;
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

            imib.IsValid.Should().BeTrue();
            IMusicImporter imi = imib.BuildImporter();
            imi.Should().NotBeNull();
            imi.Load();

            ims.ShouldHaveAlbumsLike(Albums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

        }

        [TearDown]
        public void TearDown()
        {
            baseTearDown(true);
        }
        #endregion



        [Test]
        public void Test()
        {
            ITrack tr = _IMusicSession.AllTracks.FirstOrDefault();
            tr.Should().NotBeNull();

            tr.MonitorEvents();

            using (IModifiableAlbum ima = tr.Album.GetModifiableAlbum())
            {
                IModifiableTrack imt = ima.Tracks.Where(t => t.TrackNumber == 1).FirstOrDefault();
                imt.Should().NotBeNull();

                imt.MonitorEvents();

                imt.DiscNumber.Should().Be(0);
                imt.Name.Should().Be("A - B");
                imt.TrackNumber.Should().Be(1);
                imt.State.Should().Be(ObjectState.Available);
        //         <TrackNumber>1</TrackNumber>
        //<Name>A - B</Name>

                imt.DiscNumber = 25;
                imt.DiscNumber.Should().Be(25);
                imt.ShouldRaisePropertyChangeFor(t => t.DiscNumber);

                imt.Name = "kilo papa";
                imt.Name.Should().Be("kilo papa");
                imt.ShouldRaisePropertyChangeFor(t => t.Name);

                imt.Rating = 5;
                imt.Rating.Should().Be(5);
                imt.ShouldRaisePropertyChangeFor(t => t.Rating);

                imt.TrackNumber = 345;
                imt.TrackNumber.Should().Be(345);
                imt.ShouldRaisePropertyChangeFor(t => t.TrackNumber);

                ima.Commit().Should().BeTrue();

            }

            tr.ShouldRaisePropertyChangeFor(t => t.Name);
            tr.ShouldRaisePropertyChangeFor(t => t.DiscNumber);
            tr.ShouldRaisePropertyChangeFor(t => t.Rating);
            tr.ShouldRaisePropertyChangeFor(t => t.TrackNumber);

            tr.DiscNumber.Should().Be(25);
            tr.Name.Should().Be("kilo papa");
            tr.Rating.Should().Be(5);
            tr.TrackNumber.Should().Be(345);

            _IMusicSession.ShouldHaveAlbumsLike(Albums[1], AlbumDescriptorCompareMode.AlbumandTrackMD);

        }


        [Test]
        public void Test_Remove()
        {
            ITrack tr = _IMusicSession.AllTracks.FirstOrDefault();
            tr.Should().NotBeNull();

            tr.MonitorEvents();

            using (IModifiableAlbum ima = tr.Album.GetModifiableAlbum())
            {
                IModifiableTrack imt = ima.Tracks.Where(t => t.TrackNumber == 1).FirstOrDefault();
                imt.Should().NotBeNull();

                imt.MonitorEvents();

                imt.DiscNumber.Should().Be(0);
                imt.Name.Should().Be("A - B");
                imt.TrackNumber.Should().Be(1);
                imt.State.Should().Be(ObjectState.Available);

                imt.Delete();

                ima.Commit().Should().BeTrue();

            }

            tr.State.Should().Be(ObjectState.Removed);

            _IMusicSession.ShouldHaveAlbumsLike(Albums[2], AlbumDescriptorCompareMode.AlbumandTrackMD);

        }
    }
}
