using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.FileConverter;
using MusicCollection.WebServices.Amazon;
using MusicCollection.WebServices;
using System.Threading;
using MusicCollection.Implementation.Session;

namespace MusicCollectionTest.AmazonWebService
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("AmazonWebServices")]
    [NUnit.Framework.Category("InternetConnectionDependant")]
    internal class AmazonWebServices : TestBase
    {
        private AmazonFinder _Amf;

        [SetUp]
        public void Se()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            { 
                Assert.Ignore("Internet Acess Mandatory.  Omitting.");
            }

            IMusicSettings isf = SettingsBuilder.FromUserSetting();

             _Amf = new AmazonFinder(isf.WebUserSettings);
        }

        [TearDown]
        public void TearDown()
        {
            _Amf.Dispose();
        }

        [Test]
        public void TestAsin()
        {
            WebQueryFactory wqf = new WebQueryFactory(null);
            IWebQuery wq = wqf.FromAlbumDescriptor(Albums[0][0]);
            wq.NeedCoverArt = true;
            var res = _Amf.Search(wq, new CancellationToken ()).ToList();
            Assert.That(res.Count, Is.EqualTo(1));
            var ress = res.FirstOrDefault();
            Assert.That(ress, Is.Not.Null);
            AssertAlbumDescriptor(ress.FindItem,Albums[1][0],  AlbumDescriptorCompareMode.AlbumandTrackMD);
        }

        [Test]
        public void TestName1()
        {
            WebQueryFactory wqf = new WebQueryFactory(null);
            IWebQuery wq = wqf.FromAlbumDescriptor(Albums[0][1]);
            wq.NeedCoverArt = false;
            var res = _Amf.Search(wq, new CancellationToken()).ToList();
            Assert.That(res.Count, Is.AtLeast(1));
            //var ress = res.FirstOrDefault(a => a.FindItem.Asin == "B000038I5D");

            var ress = res.FirstOrDefault(
                a => a.FindItem.Asin == "B000038I5D" ||
                a.FindItem.Asin == "B00006BNF2" ||
                a.FindItem.Asin == "B00F5AH8QY");

            Assert.That(ress, Is.Not.Null);
            //AssertAlbumDescriptor(ress.FindItem,Albums[1][1],  AlbumDescriptorCompareMode.AlbumandTrackMD);

        }

        [Test]
        public void TestName2()
        {
            WebQueryFactory wqf = new WebQueryFactory(null);
            IWebQuery wq = wqf.FromAlbumDescriptor(Albums[0][2]);
            wq.NeedCoverArt = false;
            var res = _Amf.Search(wq, new CancellationToken()).ToList();
            Assert.That(res.Count, Is.AtLeast(1));
            //a.FindItem.Asin == "B000002ADT" ||
            var ress = res.Where(a=>a.FindItem.TrackDescriptors.Count==5).FirstOrDefault(a =>  a.FindItem.Asin == "B00000DS40" || a.FindItem.Asin == "B003R9PDT4");
            Assert.That(ress, Is.Not.Null);
            //AssertAlbumDescriptor(ress.FindItem, Albums[1][2], AlbumDescriptorCompareMode.AlbumandTrackMD);

            AssertAlbumDescriptor(ress.FindItem, Albums[1][3], AlbumDescriptorCompareMode.AlbumandTrackMD);
        }


    }

}
