using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollection.WebServices.MuzicBrainz;
using MusicCollection.Implementation;
using MusicCollection.WebServices;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.Fundation;
using System.Threading;


namespace MusicCollectionTest.MusicBrainzTest
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [NUnit.Framework.Category("MusicBrainz")]
    class MuzicBrainz2FinderTest
    {
        [Test]
        public void Test()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            ICDInfoHandler icd = Substitute.For<ICDInfoHandler>();
            icd.IsReady.Returns(true);

            IDiscIDs iIDiscIDs = Substitute.For<IDiscIDs>();
            iIDiscIDs.MusicBrainzCDId.Returns("XzPS7vW.HPHsYemQh0HBUGr8vuU-");
            icd.IDs.Returns(iIDiscIDs);


            CDInfoQuery cdiq = new CDInfoQuery(icd);
            cdiq.NeedCoverArt=false;

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(cdiq, new CancellationToken());
            res.Should().NotBeNull();

            List<Match<AlbumDescriptor>> resl = res.ToList();
            resl.Should().HaveCount(2);
            resl[1].FindItem.HasImage().Should().BeTrue();

            mbf.Dispose();


        }

        [Test]
        public void Test_2()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            IAlbumDescriptor icd = Substitute.For<IAlbumDescriptor>();
            icd.Name.Returns("Houses of the Holy");
            icd.Artist.Returns("Led Zeppelin");

            IWebQuery cdiq = Substitute.For<IWebQuery>();
            cdiq.NeedCoverArt = true;
            cdiq.AlbumDescriptor.Returns(icd);
            cdiq.Type.Returns(QueryType.FromAlbumInfo);

            IDiscIDs iIDiscIDs = Substitute.For<IDiscIDs>();
            iIDiscIDs.MusicBrainzID.Returns((string)null);
            icd.IDs.Returns(iIDiscIDs);
            cdiq.MaxResult = 2;

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(cdiq, new CancellationToken());
            res.Should().NotBeNull();

            List<Match<AlbumDescriptor>> resl = res.ToList();
            resl.Should().HaveCount(2);
            resl[0].FindItem.Images.Should().NotBeEmpty();

            mbf.Dispose();
        }


        [Test]
        public void Test_3()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            IAlbumDescriptor icd = Substitute.For<IAlbumDescriptor>();
  
            IWebQuery cdiq = Substitute.For<IWebQuery>();
            cdiq.NeedCoverArt = false;
            cdiq.AlbumDescriptor.Returns(icd);
            cdiq.Type.Returns(QueryType.FromAlbumInfo);

            IDiscIDs iIDiscIDs = Substitute.For<IDiscIDs>();
            iIDiscIDs.MusicBrainzID.Returns("10dd792c-567e-3cea-9046-f640116f92c5");
            icd.IDs.Returns(iIDiscIDs);
            cdiq.MaxResult = 10;

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(cdiq, new CancellationToken());
            res.Should().NotBeNull();

            List<Match<AlbumDescriptor>> resl = res.ToList();
            resl.Should().HaveCount(1);
            resl[0].FindItem.Images.Should().BeNull();

            mbf.Dispose();
        }

        [Test]
        public void Failed_Test_Null()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(null, new CancellationToken());
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            mbf.Dispose();
        }

        [Test]
        public void Failed_Test_IWebQuery_null()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            IWebQuery wq = null;

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(wq, new CancellationToken());
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            mbf.Dispose();
        }

        [Test]
        public void Failed_Test_CDNotReady()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            ICDInfoHandler icd = Substitute.For<ICDInfoHandler>();
            icd.IsReady.Returns(false);
            CDInfoQuery cdiq = new CDInfoQuery(icd);

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(cdiq, new CancellationToken());
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            mbf.Dispose();
        }

        [Test]
        public void Failed_Test_CDIsNull()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            CDInfoQuery cdiq = new CDInfoQuery(null);

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(cdiq, new CancellationToken());
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            mbf.Dispose();
        }


        [Test]
        public void Failed_Test_WebQueryIncoherent()
        {
            IWebUserSettings iwsm = Substitute.For<IWebUserSettings>();
            MuzicBrainzFinder mbf = new MuzicBrainzFinder(iwsm);

            IWebQuery wq = Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromCD);

            IEnumerable<Match<AlbumDescriptor>> res = mbf.Search(wq, new CancellationToken());
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            mbf.Dispose();
        }
    }
}
