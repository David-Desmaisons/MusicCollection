using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web.Script.Serialization;

using NUnit;
using NUnit.Framework;

using FluentAssertions;
using NSubstitute;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.Web;
using System.Net;
using System.IO;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class HttpJsonInterpretorTestor
    {
        [SetUp]
        public void setUp()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Assert.Ignore("Internet Acess Mandatory.  Omitting.");
            }
        }

        private IHttpWebRequest GetWebRequest(string iConnection)
        {
            return new HttpWebRequestWrapper( (HttpWebRequest)WebRequest.Create(iConnection));
        }

        [Test]
        public void Test_NullConstructor()
        {
            HttpJsonInterpretor target = null;
            Action st = () => target = new HttpJsonInterpretor((IHttpWebRequest)null);
            st.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Test_NoneJsonSite()
        {
            HttpJsonInterpretor target = new HttpJsonInterpretor(GetWebRequest("https://www.google.com.br/"));
            ////IInternetServiceListener

            object Response = target.GetObjectResponse();
            Response.Should().BeNull();

        }

        [Test]
        public void Test_Json_Response()
        {
            HttpJsonInterpretor target = new HttpJsonInterpretor(GetWebRequest("http://echo.jsontest.com/Name/David/Age/38/Nationality/French"));
           
            dynamic res =  target.GetObjectResponse();
            object Response = res;
            Response.Should().NotBeNull();
            string Name = res.Name;
            Name.Should().Be("David");

            string Age = res.Age;
            Age.Should().Be("38");

            string Nationality = res.Nationality;
            Nationality.Should().Be("French");

            object NoneExistingParameter = res.NoneExistingParameter;
            NoneExistingParameter.Should().BeNull();

        }

         [Test]
         public void Test_wrongsite()
         {
             HttpJsonInterpretor target = new HttpJsonInterpretor(GetWebRequest("http://dedemaisons.com"));
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);
             Response.Should().BeNull();

             listener.Received().OnWebExeption(Arg.Any<Exception>());

         }

        
         [Test]
         public void Test_wrong_Link()
         {
             HttpJsonInterpretor target = new HttpJsonInterpretor(GetWebRequest("http://www.google.com/barbapapa"));
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);
             Response.Should().BeNull();

             listener.Received().OnWebExeption(Arg.Any<Exception>());

         }
        
         [Test]
        [Ignore]
         public void Test_Musicbrainz_Link()
         {
             HttpJsonInterpretor target = new HttpJsonInterpretor(GetWebRequest("http://musicbrainz.org/ws/2/release/59211ea4-ffd2-4ad9-9a4e-941d3148024a?inc=artist-credits+labels+discids+recordings&fmt=json"));
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);
             //Response.Should().BeNull();

             listener.Received().OnWebExeption(Arg.Any<Exception>());

         }


         [Test]
         public void Test_Json_Response_Error_Case()
         {
             IHttpWebRequest WebRequest = Substitute.For<IHttpWebRequest>();
             HttpJsonInterpretor target = new HttpJsonInterpretor(WebRequest);
             IHttpWebResponse WebResponse = Substitute.For<IHttpWebResponse>();
             WebRequest.GetResponse().Returns(WebResponse);
             WebResponse.StatusCode.Returns(HttpStatusCode.Forbidden);
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);
            
             Response.Should().BeNull();
             listener.Received().OnStatusCodeKO(HttpStatusCode.Forbidden);
         }

         [Test]
         public void Test_Json_Response_Error_Case_Stream_Null()
         {
             IHttpWebRequest WebRequest = Substitute.For<IHttpWebRequest>();
             HttpJsonInterpretor target = new HttpJsonInterpretor(WebRequest);
             IHttpWebResponse WebResponse = Substitute.For<IHttpWebResponse>();
             WebRequest.GetResponse().Returns(WebResponse);
             WebResponse.StatusCode.Returns(HttpStatusCode.OK);
             WebResponse.GetResponseStream().Returns((Stream)null);
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);

             Response.Should().BeNull();
             listener.Received().OnUnExpectedUnreadableResult();
         }

         [Test]
         public void Test_Json_Response_Error_Case_Throw_Web_Exception()
         {
             IHttpWebRequest WebRequest = Substitute.For<IHttpWebRequest>();
             HttpJsonInterpretor target = new HttpJsonInterpretor(WebRequest);
             WebException we = new WebException();
             WebRequest.GetResponse().Returns(_ => { throw we; });
          
             IInternetServiceListener listener = Substitute.For<IInternetServiceListener>();

             object Response = target.GetObjectResponse(listener);

             Response.Should().BeNull();
             listener.Received().OnWebExeption(we);
         }


        
    }
}
