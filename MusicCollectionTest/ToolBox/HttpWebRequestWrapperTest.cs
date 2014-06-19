using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using FluentAssertions;
using NSubstitute;
using System.Net;
using MusicCollection.ToolBox.Web;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class HttpWebRequestWrapperTest
    {
        private HttpWebRequest _HttpWebRequest;

        [SetUp]
        public void SU()
        {
            _HttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.google.com.br/");
        }

        [Test]
        public void Test_RequestWrapper()
        {
            HttpWebRequestWrapper target = new HttpWebRequestWrapper(_HttpWebRequest);

            target.Timeout = 10000;
            _HttpWebRequest.Timeout.Should().Be(10000);
            target.Timeout.Should().Be(10000);

            target.ReadWriteTimeout = 10000;
            _HttpWebRequest.ReadWriteTimeout.Should().Be(10000);
            target.ReadWriteTimeout.Should().Be(10000);


            target.Accept = "application/json";
            _HttpWebRequest.Accept.Should().Be("application/json");
            target.Accept.Should().Be("application/json");

            target.Method = "Post";
            _HttpWebRequest.Method.Should().Be("POST");
            target.Method.Should().Be("POST");


            target.Referer = "ererrereerrre";
            _HttpWebRequest.Referer.Should().Be("ererrereerrre");
            target.Referer.Should().Be("ererrereerrre");

            target.ContentType = "text/plain";
            _HttpWebRequest.ContentType.Should().Be("text/plain");
            target.ContentType.Should().Be("text/plain");


             target.Credentials = CredentialCache.DefaultCredentials;
            _HttpWebRequest.Credentials.Should().Be(CredentialCache.DefaultCredentials);
    

            target.UserAgent = "ererrtttrrereerrre";
            _HttpWebRequest.UserAgent.Should().Be("ererrtttrrereerrre");
            target.UserAgent.Should().Be("ererrtttrrereerrre");

            var h = target.Headers;
            ((object)_HttpWebRequest.Headers).Should().Be(h);
        }
    }
}
