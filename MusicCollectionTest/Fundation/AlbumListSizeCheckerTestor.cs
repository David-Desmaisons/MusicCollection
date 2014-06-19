using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using FluentAssertions;
using MusicCollection.Fundation;
using NSubstitute;
using MusicCollection.Implementation;

namespace MusicCollectionTest.Fundation
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Implementation")]
    public class AlbumListSizeCheckerTestor
    {
        [Test]
        public void Test_0()
        {
            AlbumListSizeChecker target = new AlbumListSizeChecker(@"C:\");
            target.IsPertinent.Should().BeTrue();
            target.SpaceCheck.Should().BeNull();
        }

        [Test]
        public void Test_1()
        {
            AlbumListSizeChecker target = new AlbumListSizeChecker(@"C:\");
            target.IsPertinent.Should().BeTrue();
            target.SpaceCheck.Should().BeNull();
            IInternalAlbum al = Substitute.For<IInternalAlbum>();
            target.Albums = new IAlbum[1] { al};
            target.SpaceCheck.Should().NotBeNull();
        }

        [Test]
        public void Test_2()
        {
            AlbumListSizeChecker target = new AlbumListSizeChecker(@"C:\totto\lasticot");
            target.IsPertinent.Should().BeFalse();
            target.SpaceCheck.Should().BeNull();
        }
    }
}
