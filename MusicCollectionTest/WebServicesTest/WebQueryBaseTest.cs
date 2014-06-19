using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using System.Net;
using System.IO;
using FluentAssertions;
using MusicCollection.WebServices;
using MusicCollection.Fundation;

namespace MusicCollectionTest.WebServicesTest
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("WebServices")]
    public class WebQueryBaseTest
    {
        [Test]
        public void WebQueryBaseTest_Album()
        {
            IAlbumDescriptor iad = Substitute.For<IAlbumDescriptor>();
            AlbumDescriptorQuery target = new AlbumDescriptorQuery(iad);
            target.CDInfo.Should().BeNull();
            
            target.MaxResult = 0;
            target.MaxResult.Should().Be(-1);

            target.MaxResult = -2;
            target.MaxResult.Should().Be(-1);

            target.MaxResult = 10;
            target.MaxResult.Should().Be(10);

            target.AlbumDescriptor.Should().Be(iad);
        }

        [Test]
        public void WebQueryBaseTest_CD()
        {
            ICDInfoHandler icdh = Substitute.For<ICDInfoHandler>();
            CDInfoQuery target = new CDInfoQuery(icdh);
            target.CDInfo.Should().Be(icdh);

            target.MaxResult = 0;
            target.MaxResult.Should().Be(-1);

            target.MaxResult = -2;
            target.MaxResult.Should().Be(-1);

            target.MaxResult = 10;
            target.MaxResult.Should().Be(10);

            target.AlbumDescriptor.Should().BeNull();
        }
    }
}
