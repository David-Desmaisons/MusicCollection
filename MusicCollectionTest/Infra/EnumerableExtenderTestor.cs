using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using FakeItEasy;
using NSubstitute;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class EnumerableExtenderTestor
    {
        [Test]
        public void Test_FromAlbum_Basic()
        {
            IEnumerable<IMusicObject> target = A.CollectionOfFake<IAlbum>(10);
            IEnumerable<IAlbum> res = target.ConvertToAlbums();
            res.Should().BeEquivalentTo(target);
            res.ShouldAllBeEquivalentTo(target);

            IEnumerable<ITrack> res2 = target.ConvertMusicObject<ITrack>();
            res2.Should().BeEmpty();

            IEnumerable<IAlbum> res3 = target.ConvertMusicObject<IAlbum>();
            res3.Should().Equal(target);
        }

        private IEnumerable<IAlbum> GetFakeAlbum(List<ITrack> iadd)
        {
            for(int k=0; k<10; k++)
            {
                CompleteObservableCollectionImpl<ITrack> coci = new CompleteObservableCollectionImpl<ITrack>();
                Enumerable.Range(0, 10).Select(i => Substitute.For<ITrack>()).Apply(t => coci.Add(t));
                iadd.AddCollection(coci);
                var res = Substitute.For<IAlbum>();
                res.Tracks.Returns(coci);
                res.Genre.Returns("Blues");
                yield return res;
               
            }
        }

        [Test]
        public void Test_FromAlbum_ConvertTracks()
        {
            List<ITrack> Expected = new List<ITrack>();
            IEnumerable<IMusicObject> target = GetFakeAlbum(Expected).ToList();
            IEnumerable<ITrack> res = target.ConvertToTracks();

            res.Should().Equal(Expected);
        }


        private IEnumerable<ITrack> GetFakeTracks(IList<IAlbum> als)
        {
            for (int k = 0; k < 10; k++)
            {
                IAlbum resa = Substitute.For<IAlbum>();
                als.Add(resa);
                for (int l = 0; l < 10; l++)
                { 
                    ITrack res = Substitute.For<ITrack>();
                    res.Album.Returns(resa);
                    yield return res;
                }
            }
        }

        [Test]
        public void Test_FromAlbum_ConvertAlbums()
        {
            List<IAlbum> Expected = new List<IAlbum>();
            IEnumerable<IMusicObject> target = GetFakeTracks(Expected).ToList();
            
            IEnumerable<IAlbum> res = target.ConvertToAlbums();

            res.Should().Equal(Expected);
        }

        [Test]
        public void Test_From_Track_Basic()
        {
            IEnumerable<IMusicObject> target0 = null;
            IEnumerable<ITrack> t1 = target0.ConvertMusicObject<ITrack>();
            t1.Should().NotBeNull();
            t1.Should().BeEmpty();


            IEnumerable<IMusicObject> target = A.CollectionOfFake<ITrack>(10);
            IEnumerable<ITrack> res = target.ConvertToTracks();
            res.Should().BeEquivalentTo(target);
            res.ShouldAllBeEquivalentTo(target);

            IEnumerable<ITrack> res2 = target.ConvertMusicObject<ITrack>();
            res2.Should().Equal(target);

            IEnumerable<IAlbum> res3 = target.ConvertMusicObject<IAlbum>();
            res3.Should().BeEmpty();
        }

        [Test]
        public void Test_From_Track_Null()
        {
            IEnumerable<IMusicObject> target = null;
            IEnumerable<ITrack> res = target.ConvertToTracks();
            res.Should().BeNull();
        }

        [Test]
        public void Test_From_Track_NotKnown()
        {
            IEnumerable<IMusicObject> target = A.CollectionOfFake<IMusicObject>(10);
            IEnumerable<ITrack> res = target.ConvertToTracks();
            res.Should().BeNull();
        }
    }
}
