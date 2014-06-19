using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.FileConverter;
using iTunesLib;
using System.Collections;
using MusicCollection.Implementation.Modifier;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    internal class AlbumDescriptorTestor : TestBase
    {
        [SetUp]
        public void SetUp()
        {
        }


        [Test]
        public void Test()
        {
            int i=0;
            foreach (IFullAlbumDescriptor ial in Albums[0])
            {
                var res = ial.SplitOnDiscNumber().ToList();
                Assert.IsNotNull(res);
                Assert.AreEqual(ial.TrackDescriptors.Count,res.Sum(e =>e.TrackDescriptors.Count));
                foreach (IFullAlbumDescriptor al in res)
                {
                    Assert.AreEqual(ial.Name, al.Name);
                    Assert.AreEqual(ial.Artist, al.Artist);
                    Assert.AreEqual(ial.Year, al.Year);
                    Assert.AreEqual(ial.Genre, al.Genre);
                }

                if (i != 2)
                {
                    Assert.AreEqual(1, res.Count);
                    Assert.IsTrue(ial.MatchTrackNumberOnDisk(ial.TrackDescriptors.Count));
                }
                else
                {
                    Assert.AreEqual(2, res.Count);
                    Assert.IsTrue(ial.MatchTrackNumberOnDisk(9));
                    Assert.IsTrue(ial.MatchTrackNumberOnDisk(6));
                    Assert.IsFalse(ial.MatchTrackNumberOnDisk(15));
                }

                    i++;
            }
        }

        [Test]
        public void Test_IITAudioCDPlaylist_With_Tracks()
        {
            //arrange
            IITAudioCDPlaylist iit = Substitute.For<IITAudioCDPlaylist>();

            iit.Artist.Returns("Artist");
            iit.Year.Returns(1943);
            iit.Genre.Returns("Blues");
            iit.Name.Returns("Agathe");

            IITTrack t1 = Substitute.For<IITTrack>();
            t1.Name.Returns("t1");
            t1.TrackNumber.Returns(1);
            t1.DiscNumber.Returns(1);

            IITTrack t2 = Substitute.For<IITTrack>();
            t2.Name.Returns("t2");
            t2.TrackNumber.Returns(2);
            t2.DiscNumber.Returns(2);

            IITTrack[] tracks = new IITTrack[2]{t1,t2};

            IITTrackCollection td = Substitute.For<IITTrackCollection>();
            ((IEnumerable)td).GetEnumerator().Returns(tracks.GetEnumerator());
            td.Count.Returns(2);

            iit.Tracks.Returns(td);

            //act
            IFullAlbumDescriptor ifa = AlbumDescriptor.FromiTunes(iit);

            //assert
            ifa.Artist.Should().Be("Artist");
            ifa.Year.Should().Be(1943);
            ifa.Genre.Should().Be("Blues");
            ifa.Name.Should().Be("Agathe");

            ifa.TracksNumber.Should().Be(2);
            var tt1 = ifa.TrackDescriptors[0];

            tt1.Name.Should().Be("t1");
            tt1.TrackNumber.Should().Be(1);
            tt1.DiscNumber.Should().Be(1);

            var tt2 = ifa.TrackDescriptors[1];

            tt2.Name.Should().Be("t2");
            tt2.TrackNumber.Should().Be(2);
            tt2.DiscNumber.Should().Be(2);
        }


        [Test]
        public void Test_IITAudioCDPlaylist_Without_Track()
        {
            //arrange
            IITAudioCDPlaylist iit = Substitute.For<IITAudioCDPlaylist>();

            iit.Artist.Returns("Artist");
            iit.Year.Returns(1943);
            iit.Genre.Returns("Blues");
            iit.Name.Returns("Agathe");

            IITTrack[] tracks = new IITTrack[0] {};

            IITTrackCollection td = Substitute.For<IITTrackCollection>();
            td.GetEnumerator().Returns(tracks.GetEnumerator());
            td.Count.Returns(0);

            iit.Tracks.Returns(td);

            //act
            IFullAlbumDescriptor ifa = AlbumDescriptor.FromiTunes(iit);

            //assert
            ifa.Artist.Should().Be("Artist");
            ifa.Year.Should().Be(1943);
            ifa.Genre.Should().Be("Blues");
            ifa.Name.Should().Be("Agathe");

            ifa.TracksNumber.Should().Be(0);

            ifa.ToString().Should().Be("Artist - Agathe");
        }

        [Test]
        public void Test_ICDInfoHandler()
        {
            //arrange
            ICDInfoHandler iit = Substitute.For<ICDInfoHandler>();
            IImportContext iic = Substitute.For<IImportContext>();
            IDiscIDs iid = Substitute.For<IDiscIDs>();

            iit.TrackNumbers.Returns(2);
            iit.IDs.Returns(iid);
            iit.Duration(0).Returns(TimeSpan.FromMinutes(5));
            iit.Duration(1).Returns(TimeSpan.FromMinutes(4));

            iic.FindNewUnknownNameAlbumForArtist(Arg.Any<string>()).Returns("Fake Name");

            //act

            AlbumDescriptor target = AlbumDescriptor.CreateBasicFromCD(iit, iic);

            //assert
            target.Artist.Should().Be("Unknown Artist");
            target.Name.Should().Be("Fake Name");
            target.TracksNumber.Should().Be(2);

            var tt1 = target.TrackDescriptors[0];

            tt1.Name.Should().Be("Track 1");
            tt1.TrackNumber.Should().Be(1);
            tt1.Duration.Should().Be(TimeSpan.FromMinutes(5));

            var tt2 = target.TrackDescriptors[1];

            tt2.Name.Should().Be("Track 2");
            tt2.TrackNumber.Should().Be(2);
            tt2.Duration.Should().Be(TimeSpan.FromMinutes(4));

        }

        [Test]
        public void TestGetEditable()
        {
            AlbumDescriptor ad = new AlbumDescriptor() 
            { Artist="ATeste",Name="Name1" };

            List<TrackDescriptor> lt = new List<TrackDescriptor>();
            lt.Add(new TrackDescriptor(){Name="T1",TrackNumber=47,Duration=TimeSpan.FromMinutes(2)});
            ad.RawTrackDescriptors = lt;

            IFullEditableAlbumDescriptor target = ad.GetEditable();


            target.Name.Should().Be("Name1");
            target.Artist.Should().Be("ATeste");
            target.EditableTrackDescriptors.Should().NotBeNull();
            target.EditableTrackDescriptors.Count.Should().Be(1);
            target.EditableTrackDescriptors[0].Name.Should().Be("T1");
            target.EditableTrackDescriptors[0].TrackNumber.Should().Be(47);
            target.EditableTrackDescriptors[0].Duration.Should().Be(TimeSpan.FromMinutes(2));
        }

         [Test]
        public void TestInjectImages_Trivial()
        {
            AlbumDescriptor ad = new AlbumDescriptor() 
            { Artist="ATeste",Name="Name1" };

            List<TrackDescriptor> lt = new List<TrackDescriptor>();
            lt.Add(new TrackDescriptor(){Name="T1",TrackNumber=47,Duration=TimeSpan.FromMinutes(2)});
            ad.RawTrackDescriptors = lt;

            lt.ToString().Should().NotBeNull();

            AlbumDescriptor ad2 = new AlbumDescriptor();
            ad.InjectImages(ad2,false);
            ad.RawImages.Should().BeNull();


            ad.InjectImages(null, false);
            ad.RawImages.Should().BeNull();
        }

        [Test]
        public void TestMergeIDsFromCDInfos()
        {
            //arrange
            AlbumDescriptor ad = new AlbumDescriptor() { Artist = "ATeste", Name = "Name1" };

            ICDInfoHandler ih = Substitute.For<ICDInfoHandler>();
            IDiscIDs dis = Substitute.For<IDiscIDs>();
            ih.IDs.Returns(dis);

            dis.CDDB.Returns("CDDB");
            dis.MusicBrainzID.Returns("MusicBrainzID");
            dis.MusicBrainzCDId.Returns("MusicBrainzCDId");
            dis.Asin.Returns("Asin");

            //act
            ad.MergeIDsFromCDInfos(ih);

            //assert
            ad.CDDB.Should().Be("CDDB");
            ad.MusicBrainzID.Should().Be("MusicBrainzID");
            ad.MusicBrainzCDId.Should().Be("MusicBrainzCDId");
            ad.Asin.Should().Be("Asin");

        }

        [Test]
        public void SimpleAlbumDescriptorFromAlbumModifier_Test()
        {
            IInternalAlbumModifier iam = Substitute.For<IInternalAlbumModifier>();
            IDiscIDs ivds = Substitute.For<IDiscIDs>();
            ivds.CDDB.Returns("29");
            IArtist ar = Substitute.For<IArtist>();
            ar.Name.Returns("Toto");
            var lis = new List<IArtist>();
            lis.Add(ar);
            iam.Year.Returns(1000);
            iam.Genre = "Genre";
            iam.Name = "Name";
            iam.Artists.Returns(lis);
            iam.CDIDs.Returns(ivds);

            IAlbumDescriptor target = AlbumDescriptor.SimpleAlbumDescriptorFromAlbumModifier(iam);
            target.Artist.Should().Be("Toto");
            target.Year.Should().Be(1000);
            target.Genre.Should().Be("Genre");
            target.Name.Should().Be("Name");
            IDiscIDs rdi = target.IDs;
            rdi.Should().NotBeNull();
            rdi.CDDB.Should().Be("29");
        }
    }

    
}
