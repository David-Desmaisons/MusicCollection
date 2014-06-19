using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web.Script.Serialization;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.Web;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class DynaJSONConv:TestBase
    {
        private string _Parse1;
        private string _Parse2;
        private string _Parse3;
        //private JavaScriptSerializer _JSC;

        [SetUp]
        public void setUp()
        {
            _Parse1 = "{\"pagination\": {\"per_page\": 50, \"items\": 0, \"page\": 1, \"urls\": {}, \"pages\": 1}, \"results\": []}";
            _Parse2 = "{\"pagination\": {\"per_page\": 50, \"items\": 1, \"page\": 1, \"urls\": {}, \"pages\": 1}, \"results\": [{\"style\": [\"Free Jazz\", \"Contemporary Jazz\", \"Free Improvisation\"], \"thumb\": \"http://api.discogs.com/image/R-90-2892161-1305987328.jpeg\", \"format\": [\"CD\"], \"country\": \"UK\", \"title\": \"Marilyn Crispell, Fritz Hauser, Hildegard Kleeb, Urs Leimgruber, Elvira Plenar - Behind The Night\", \"uri\": \"/Marilyn-Crispell-Fritz-Hauser-Hildegard-Kleeb-Urs-Leimgruber-Elvira-Plenar-Behind-The-Night/release/2892161\", \"label\": \"B&W, X-Talk\", \"catno\": \"BW049\", \"year\": \"1995\", \"genre\": [\"Jazz\"], \"resource_url\": \"http://api.discogs.com/releases/2892161\", \"type\": \"release\", \"id\": 2892161}]}";
            _Parse3 = "{\"status-1\": \"S1\", \"status_2\": \"S2\"}";
            //_JSC = DynamicJsonConverter.DynamicSerializer;
        }

        [Test]
        public void Test()
        {
            dynamic data = DynamicJsonConverter.DynamicDeSerialize(_Parse1);

            Assert.That(data.results.Count,Is.EqualTo(0));
            Assert.That(data.pagination.per_page,Is.EqualTo(50));
            Assert.That(data.pagination.items,Is.EqualTo(0));


            data = DynamicJsonConverter.DynamicDeSerialize(_Parse2);

            Assert.That(data.results.Count,Is.EqualTo(1));
            Assert.That(data.pagination.per_page,Is.EqualTo(50));
            Assert.That(data.pagination.items,Is.EqualTo(1));
            Assert.That(data.results[0].thumb, Is.EqualTo("http://api.discogs.com/image/R-90-2892161-1305987328.jpeg"));
            Assert.That(data.results[0].resource_url, Is.EqualTo("http://api.discogs.com/releases/2892161"));
            Assert.That(data.results[0].id, Is.EqualTo(2892161));

            string res = data.ToString();
            object res_object =data.NoneExistingProperty;
            res_object.Should().Be(null);

            object data2 = DynamicJsonConverter.DynamicDeSerialize("jskjsa  asbasbmas");
            data2.Should().BeNull();


        }

        [Test]
        public void Test2()
        {
            dynamic data = DynamicJsonConverter.DynamicDeSerialize(_Parse3);
            string s1 = data.status_1;
            s1.Should().Be("S1");

            string s2 = data.status_2;
            s2.Should().Be("S2");

            string s3 = data.status_3;
            s3.Should().BeNull();
        }





    }
}
