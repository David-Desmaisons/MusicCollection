using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollectionTest.TestObjects;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;
using MusicCollection.DataExchange;
using MusicCollection.FileImporter;
using MusicCollection.Implementation;
using MusicCollection.Fundation;
using System.IO;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    internal class PathTrackDescriptorTest : TestBase
    {
        private string _Path;
        [SetUp]
        public void SetUp()
        {
            _Path = this.GetFileInName("5-NoName.mp3");
        }
        

        [Test]
        public void Test()
        {            
            IImportHelper iih = Substitute.For<IImportHelper>();
            IImportContext iic = Substitute.For<IImportContext>();
            iih.AlbumArtistClue.Returns("Artist");
            iih.AlbumNameClue.Returns("Name");

            PathTrackDescriptor target = new PathTrackDescriptor(_Path, iih, iic);       

            IAlbumDescriptor ta = target;
            ta.Artist.Should().Be("Artist");
            ta.Name.Should().Be("Name");
            target.TrackNumber.Should().Be(5);
            target.Name.Should().Be("NoName");

            using( Stream str = target.MusicStream())
            {
                str.Should().NotBeNull();
            }

            target.Dispose();
        }

        [Test]
        public void Test_Error()
        {
            string path =  this.GetFileInName("6-NoName.mp3");

            IImportHelper iih = Substitute.For<IImportHelper>();
            IImportContext iic = Substitute.For<IImportContext>();

            PathTrackDescriptor target = new PathTrackDescriptor(path, iih, iic);

            string res = null;
            Action wl = () => res= target.Name;
            wl.ShouldThrow<Exception>();

        }
    }
}
