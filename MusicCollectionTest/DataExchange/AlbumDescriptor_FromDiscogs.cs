using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionTest.TestObjects;
using System.IO;
//using MusicBrainz;
using System.Xml;
using MusicCollection.DataExchange;
using MusicCollection.ToolBox.Web;
using MusicCollection.Infra;
using MusicCollection.WebServices;
using System.Threading;
using System.Collections.Specialized;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    public class AlbumDescriptor_FromDiscogs
    {
        private IInternetProvider _OriginalProvider;
        private IInternetProvider _FakeInternetProvider;
        private IOAuthManager _FakeIOAuthManager;

        [SetUp]
        public void SetUp()
        {
            _OriginalProvider =InternetProvider.InternetHelper;
            _FakeInternetProvider = Substitute.For<IInternetProvider>();
            _FakeIOAuthManager = Substitute.For<IOAuthManager>();
            InternetProvider.InternetHelper = _FakeInternetProvider;
        }

        [TearDown]
        public void TearDown()
        {
            InternetProvider.InternetHelper = _OriginalProvider;
        }

        public class FoundTrack
        {
            public FoundTrack()
            {
                duration = title = position = "";
            }

            public string title  { get; set; }
            public string duration  { get; set; }
            public string position { get; set; }
        }

        public class FoundArtist
        {
            public string name { get; set; }
        }

        public class FoundDoImage
        {
            public string uri { get; set; }
        }

        public class FoundDo
        {
            public FoundDo()
            {
                images = new List<dynamic>();
                artists = new List<dynamic>();
                tracklist = new List<dynamic>();
                genres = new List<string>();
            }

            public List<dynamic> artists { get; private set; }
            public List<dynamic> images { get; private set; }
            public List<dynamic> tracklist { get; private set; }
            public List<string> genres { get; private set; }
            public int year { get; set; }
            public string title { get; set; }
        }


        [Test]
        public void Test()
        {
            //arrange
            FoundDo fdo = new FoundDo() {title="cacadanslaboue", year = 2000 };
            fdo.artists.Add(new FoundArtist() { name = "Esse cara sou eu" });
            fdo.tracklist.Add(new FoundTrack() { title = "Esse cara sou eu 1",duration="" });
            fdo.tracklist.Add(new FoundTrack() { title = "Esse cara sou eu 2" });
            fdo.images.Add(new FoundDoImage() { uri ="dans la lune"});
            fdo.genres.Add("Blues");

            IHttpWebRequest web = Substitute.For<IHttpWebRequest>();
            NameValueCollection nvc = new NameValueCollection();
            web.Headers.Returns(nvc);
            _FakeInternetProvider.CreateHttpRequest("dans la lune").Returns(web);

            IHttpWebResponse resp = Substitute.For<IHttpWebResponse>();
            Stream myimage = new MemoryStream();
            resp.GetResponseStream().Returns(myimage);

            web.GetResponse().Returns(resp);

            _FakeIOAuthManager.GenerateAuthzHeader("dans la lune", "GET").Returns("25");

            HttpContext hc = new HttpContext("ua", "r");

            CancellationToken ict = new CancellationTokenSource().Token;

            //act
            AlbumDescriptor res = AlbumDescriptor.FromDiscogs(fdo, true, _FakeIOAuthManager, hc, ict);

            //assert
            res.Should().NotBeNull();
            res.Images.Count.Should().Be(1);

            web.UserAgent.Should().Be("ua");
            web.Headers["Authorization"].Should().Be("25");
            web.PreAuthenticate.Should().BeTrue();

            res.Artist.Should().Be("Esse cara sou eu");
            res.Genre.Should().Be("Blues");
            res.Year.Should().Be(2000);
            res.Name.Should().Be("cacadanslaboue");
            res.TrackDescriptors.Count.Should().Be(2);

        }
    }
}
