using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using NUnit.Framework;
using System.Linq.Expressions;
using MusicCollectionTest.TestObjects;
using System.Collections.ObjectModel;

using FluentAssertions;
using MusicCollection.PlayList;
using NSubstitute;
using MusicCollection.Fundation;
using MusicCollection.Implementation;

namespace MusicCollectionTest.PlayList
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("PlayList")]
    public class PlayListFactoryTestor
    {
        public PlayListFactoryTestor()
        {
        }

        [Test]
        public void Test_Basic()
        {
            PlayListFactory plf = new PlayListFactory(null);
            plf.GetPlayList("Test").Should().BeNull();

            IAlbumPlayList iplf = plf.CreateAlbumPlayList("Test");
            iplf.Should().NotBeNull();
            iplf.PlayListname.Should().Be("Test");

            IReadOnlyPlayList target = plf.GetPlayList("Test");
            target.ShouldBeSameAs(iplf);

            IAlbumPlayList target2 = plf.CreateAlbumPlayList("Test");
            target2.ShouldBeSameAs(iplf);

            plf.Dispose();
        }
    }
}
