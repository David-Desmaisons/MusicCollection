using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.WebServices.Freedb;
using MusicCollection.WebServices;
using System.Threading;
using MusicCollection.ToolBox.Web;
using NSubstitute;
using System.Net;
using System.IO;
using FluentAssertions;

namespace MusicCollectionTest.Freedb
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("InternetCDInfoProvider")]
    [NUnit.Framework.Category("InternetConnectionDependant")]
    //[NUnit.Framework.Category("CDdependant")]

    internal class FreedbCDInfoProviderTester : TestBase
    {

        readonly int[] _Track_frame_offsets = new int[] { 150, 12605, 30573, 41185, 55648, 60551, 74777, 96470, 108986, 120410, 129958, 140632, 150262, 167157, 184114 };

        const int _Disc_length = 2557;

        const string _ID = "d009fb0f";

        [SetUp]
        public void SU()
        {

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Assert.Ignore("Internet Acess Mandatory.  Omitting.");
            }
        }



        private class Fake : ICDInfoHandler
        {
            private class FakeID : IDiscIDs
            {
                private int _Count;
                public int Count
                {
                    get { return _Count; }
                }

                internal FakeID(int[] tfo, int dl, string id)
                {
                    _Count = tfo.Length;
                    CDDB = id;
                    CDDBQueryString = string.Format("{0}+{1}+{2}+{3}", id, _Count, string.Join("+", tfo), dl);
                }

                public string Asin
                {
                    get { throw new NotImplementedException(); }
                }

                public string MusicBrainzID
                {
                    get { throw new NotImplementedException(); }
                }

                public string MusicBrainzCDId
                {
                    get { throw new NotImplementedException(); }
                }

                public string CDDB
                {
                    get;
                    private set;
                }

                public string CDDBQueryString
                {
                    get;
                    private set;
                }

                public bool IsEmpty
                {
                    get { return false; }
                }

                public DiscHash RawHash
                {
                    get { throw new NotImplementedException(); }
                }
            }

            private FakeID _DI;
            internal Fake(int[] tfo, int dl, string id)
            {
                _DI = new FakeID(tfo, dl, id);
            }

            public bool IsReady
            {
                get { return true; }
            }

            public int TrackNumbers
            {
                get { return _DI.Count; }
            }

            public IDiscIDs IDs
            {
                get { return _DI; }
            }

            public string Driver
            {
                get { throw new NotImplementedException(); }
            }


            public TimeSpan Duration(int traknumber)
            {
                throw new NotImplementedException();
            }


            public List<int> Tocs
            {
                get { throw new NotImplementedException(); }
            }
        }


        [Test]
        public void Test()
        {
            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);


            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();


            Assert.That(reses.Count, Is.EqualTo(1));
            this.AssertAlbumDescriptor(reses[0].FindItem, AlbumsOld[1][0], AlbumDescriptorCompareMode.AlbumandTrackMD);

            fdb.Dispose();

        }

        private IInternetProvider _Std;

        [SetUp]
        public void SetUp()
        {
            _Std = InternetProvider.InternetHelper;
        }

        [TearDown]
        public void TearDown()
        {
            InternetProvider.InternetHelper = _Std;
        }

        [Test]
        public void Test_LetsGoHaking_NullStreamResponse()
        {
            //arrange 
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            wr.GetRequestStream().Returns(new MemoryStream());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.BadRequest);
            wres.GetResponseStream().Returns((Stream)null);
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;


            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);


            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(0));
            fdb.ShouldRaise("OnInternetError").WithSender(fdb);

            fdb.Dispose();

        }

        [Test]
        public void Test_LetsGoHaking_NotNullStreamResponse()
        {
            //arrange 
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            wr.GetRequestStream().Returns(new MemoryStream());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.OK);
            wres.GetResponseStream().Returns(new MemoryStream());
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;


            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);


            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(0));
            fdb.ShouldRaise("OnInternetError").WithSender(fdb);

            fdb.Dispose();

        }

        private IHttpWebRequest IHttpWebRequestOKBuilder(Stream StreamResult)
        {
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            wr.GetRequestStream().Returns(new MemoryStream());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.OK);
            wres.GetResponseStream().Returns(StreamResult);
            wr.GetResponse().Returns(wres);
            return wr;
        }

        private IHttpWebRequest IHttpWebRequestOKBuilder(string StreamResult)
        {
            return IHttpWebRequestOKBuilder(new MemoryStream(Encoding.UTF8.GetBytes(StreamResult)));
        }

        private void SetUpMyFakeAnswer(string answer)
        {
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest req = IHttpWebRequestOKBuilder(answer);
            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(req);
            InternetProvider.InternetHelper = wrb;
        }

        [Test]
        public void Test_LetsGoHaking_NotNullStreamResponse_CorruptedFile()
        {
            //arrange 

            SetUpMyFakeAnswer("titi");

            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);

            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(0));
            fdb.ShouldRaise("OnInternetError").WithSender(fdb);

            fdb.Dispose();

        }

        [Test]
        public void Test_LetsGoHaking_NotNullStreamResponse_Sever500()
        {
            //arrange 

            SetUpMyFakeAnswer("500 ko");

            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);

            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(0));
  
            fdb.Dispose();

        }

        private Stream GetAlbumAnswer()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("210 ok");
            using (StreamReader sr = new StreamReader(this.GetFileInName("FreedbAnswer.txt")))
            {
                while (sr.Peek() >= 0)
                {
                    sb.AppendLine(sr.ReadLine());
                }
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }



        [Test]
        public void Test_LetsGoHaking_NotNullStreamResponse_ResponseOK()
        {
            //arrange 
            StringBuilder sb = new StringBuilder("210 ok.");
            sb.Append(Environment.NewLine);
            sb.Append("rock d009fb1f The Beatles / White Album");
            sb.Append(Environment.NewLine);
            sb.Append("rock d239fb1f The Beatles / White Albumss");

            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest first = IHttpWebRequestOKBuilder(sb.ToString());
            IHttpWebRequest second = IHttpWebRequestOKBuilder(GetAlbumAnswer());
            IHttpWebRequest third = IHttpWebRequestOKBuilder(GetAlbumAnswer());
            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(first, second, third);
            InternetProvider.InternetHelper = wrb;

            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);

            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(2));
            fdb.ShouldNotRaise("OnInternetError");

            fdb.Dispose();

        }

        [Test]
        public void Test_LetsGoHaking_NotNullStreamResponse_ResponseK0_MultipleReasons()
        {
            //arrange 
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("210 ok.");
            sb.AppendLine("rock d009fb1f The Beatles / White Album");
            sb.AppendLine("rock d239fb1f The Beatles / White Albumss");
            sb.AppendLine("rock d639fb1f The Beatles / White Albumss");
            sb.AppendLine("rock d839fb1f The Beatles / White Albumss");

            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest first = IHttpWebRequestOKBuilder(sb.ToString());
            IHttpWebRequest second = IHttpWebRequestOKBuilder("titi");
            IHttpWebRequest third = IHttpWebRequestOKBuilder((Stream)null);
            IHttpWebRequest fourth = IHttpWebRequestOKBuilder("500 ko");
            IHttpWebRequest finth = IHttpWebRequestOKBuilder("590 ko");

            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(first, second, third, fourth, finth);
            InternetProvider.InternetHelper = wrb;

            FreedbFinder fdb = new FreedbFinder(new ManualSettings().WebUserSettings);

            Fake cdif = new Fake(_Track_frame_offsets, _Disc_length, _ID);

            IWebQuery iwq = new WebQueryFactory(null).FromCDInfo(cdif);
            iwq.NeedCoverArt = false;

            fdb.MonitorEvents();

            //act
            List<Match<AlbumDescriptor>> reses = fdb.Search(iwq, new CancellationToken()).ToList();

            //assert
            Assert.That(reses.Count, Is.EqualTo(0));
            fdb.ShouldRaise("OnInternetError");

            fdb.Dispose();

        }
    }
}
