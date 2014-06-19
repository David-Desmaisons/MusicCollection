using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using NSubstitute;
using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;
using MusicCollection.ToolBox.Web;
using MusicCollection.Fundation;
using System.Threading;
using System.Net;
using System.IO;

namespace MusicCollectionTest.Discogs
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Discogs2")]
    public class Discogs2FinderTester
    {
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
        public void Discogs2FinderTester_ProblemWithInternet()
        {
            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(true);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.BadRequest);
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;

            IWebQuery wq =  Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromAlbumInfo);

            IAlbumDescriptor iad = Substitute.For<IAlbumDescriptor>();
            wq.AlbumDescriptor.Returns(iad);

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); ;
            
            //Assert
            var res = target.Search(wq, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldRaise("OnInternetError").WithSender(target);
  
        }

        [Test]
        public void Discogs2FinderTester_Deactivated()
        {
            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(false);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.BadRequest);
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;

            IWebQuery wq = Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromAlbumInfo);

            IAlbumDescriptor iad = Substitute.For<IAlbumDescriptor>();
            wq.AlbumDescriptor.Returns(iad);

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); 

            //Assert
            var res = target.Search(wq, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldNotRaise("OnInternetError");

        }

        [Test]
        public void Discogs2FinderTester_Query_Null()
        {
            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(true);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.BadRequest);
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); ;

            //Assert
            var res = target.Search(null, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldNotRaise("OnInternetError");

        }


        [Test]
        public void Discogs2FinderTester_NoAlbum()
        { 
            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(true);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            IHttpWebRequest wr = Substitute.For<IHttpWebRequest>();
            wr.Headers.Returns(new System.Collections.Specialized.NameValueCollection());

            IHttpWebResponse wres = Substitute.For<IHttpWebResponse>();
            wres.StatusCode.Returns(HttpStatusCode.BadRequest);
            wr.GetResponse().Returns(wres);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wr);

            InternetProvider.InternetHelper = wrb;

            IWebQuery wq = Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromAlbumInfo);
            wq.AlbumDescriptor.Returns((IAlbumDescriptor)null);

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); ;

            //Assert
            var res = target.Search(wq, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldNotRaise("OnInternetError");

        }

        private string _FakeAnswer= @"{""results"":[{""resource_url"": ""path1""}, {""resource_url"": ""path2""}]}";

        [Test]
        public void Discogs2FinderTester_PartialResult()
        {         
            //new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(myPage));

            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(true);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            
            IHttpWebRequest wrFirst = Substitute.For<IHttpWebRequest>();
            wrFirst.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            IHttpWebResponse wresok = Substitute.For<IHttpWebResponse>();
            wresok.StatusCode.Returns(HttpStatusCode.OK);       
            wresok.GetResponseStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(_FakeAnswer)));
            wrFirst.GetResponse().Returns(wresok);

            IHttpWebRequest wrSecond = Substitute.For<IHttpWebRequest>();
            wrSecond.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            IHttpWebResponse wresko = Substitute.For<IHttpWebResponse>();
            wresko.StatusCode.Returns(HttpStatusCode.BadRequest);         
            wresko.GetResponseStream().Returns((Stream)null); 
            wrSecond.GetResponse().Returns(wresko);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wrFirst, wrSecond, wrSecond);

            InternetProvider.InternetHelper = wrb;

            IWebQuery wq = Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromAlbumInfo);

            IAlbumDescriptor iad = Substitute.For<IAlbumDescriptor>();
            wq.AlbumDescriptor.Returns(iad);

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); ;

            //Assert
            var res = target.Search(wq, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldRaise("OnInternetError").WithSender(target);

        }

        private string _FakeEmptyAnswer = @"{""results"":[]}";


         [Test]
        public void Discogs2FinderTester_NoResult()
        {         
            //new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(myPage));

            //Arrange
            IWebUserSettings iwsb = Substitute.For<IWebUserSettings>();
            iwsb.DiscogsActivated.Returns(true);
            IInternetProvider wrb = Substitute.For<IInternetProvider>();
            
            IHttpWebRequest wrFirst = Substitute.For<IHttpWebRequest>();
            wrFirst.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            IHttpWebResponse wresok = Substitute.For<IHttpWebResponse>();
            wresok.StatusCode.Returns(HttpStatusCode.OK);
            wresok.GetResponseStream().Returns(new MemoryStream(Encoding.UTF8.GetBytes(_FakeEmptyAnswer)));
            wrFirst.GetResponse().Returns(wresok);

            IHttpWebRequest wrSecond = Substitute.For<IHttpWebRequest>();
            wrSecond.Headers.Returns(new System.Collections.Specialized.NameValueCollection());
            IHttpWebResponse wresko = Substitute.For<IHttpWebResponse>();
            wresko.StatusCode.Returns(HttpStatusCode.BadRequest);         
            wresko.GetResponseStream().Returns((Stream)null); 
            wrSecond.GetResponse().Returns(wresko);


            wrb.CreateHttpRequest(Arg.Any<string>()).Returns(wrFirst, wrSecond, wrSecond);

            InternetProvider.InternetHelper = wrb;

            IWebQuery wq = Substitute.For<IWebQuery>();
            wq.Type.Returns(QueryType.FromAlbumInfo);

            IAlbumDescriptor iad = Substitute.For<IAlbumDescriptor>();
            wq.AlbumDescriptor.Returns(iad);

            //Act
            CancellationToken ct = new CancellationToken(false);
            Discogs2Finder target = new Discogs2Finder(iwsb);
            target.MonitorEvents(); ;

            //Assert
            var res = target.Search(wq, ct);
            res.Should().NotBeNull();
            var listres = res.ToList();

            listres.Should().BeEmpty();

            target.ShouldNotRaise("OnInternetError");

        }




        

    }
}
