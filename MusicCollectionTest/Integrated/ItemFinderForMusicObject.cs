using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MusicCollection.Fundation;
using MusicCollectionTest.Integrated.Tools;
using MusicCollection.Infra;
using MusicCollection.Implementation;
using MusicCollection.ToolBox.Collection;
using MusicCollectionTest.TestObjects;

using FluentAssertions;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder(null, "SQLiteClean")]
    public class ItemFinderForMusicObject : IntegratedBase
    {
        private IMusicSession _MS;

        [TestFixtureSetUp]
        public void TD()
        {
            Init();

            var tr2 = TimeTracer.TimeTrack("Session Building");
            using (tr2)
            {
                _MS = MusicSessionImpl.GetSession(_SK.Builder);
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            Assert.That(_MS.AllAlbums.Count, Is.EqualTo(0));
            Assert.That(_MS.AllGenres.Count, Is.EqualTo(0));
            Assert.That(_MS.AllArtists.Count, Is.EqualTo(0));

            var tr = TimeTracer.TimeTrack("Load");
            using (tr)
            {
                IMusicImporter imi = _MS.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        [TestFixtureTearDown]
        public void SetUp()
        {
            _MS.Dispose();
            base.CleanDirectories();
        }

        [Test]
        public void TesterForackComparableImplementation()
        {
            SortedSet<ITrack> sd = new SortedSet<ITrack>(); 
            _MS.AllTracks.Apply(a=>sd.Add(a));
            sd.Count.Should().Be(Perf_ListenedCollection._TrackNumber);
        }

        [Test]
        public void TesterForOrdered()
        {
            IEntityFinder<IArtist> artf = new ItemFinder<IArtist>(_MS.AllArtists, ar => ar.Name);

            var no = artf.Search("bra").ToList();
            var o = artf.SearchOrdered("bra").ToList();
            var r = _MS.AllArtists.Where(ar => ar.Name.Normalized().Contains("bra")).ToList();

            o.ShouldHaveSameElements(r);
            no.ShouldHaveSameElements(r);
            no.ShouldHaveSameElements(o);

            for (int u = 0; u < 10; u++)
            {
                using (TimeTracer.TimeTrack("Not Ordered (x10000)"))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        var res = artf.Search("bra").ToList();
                        res.Count.Should().Be(28);
                    }
                }

                using (TimeTracer.TimeTrack("Ordered (x10000)"))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        var res = artf.SearchOrdered("bra").ToList();
                        res.Count.Should().Be(28);
                    }
                }

                using (TimeTracer.TimeTrack("Without Optimization (x100)"))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var res = _MS.AllArtists.Where(ar => ar.Name.Normalized().Contains("bra")).ToList();
                        res.Count.Should().Be(28);
                    }
                }
            }

            var nullresult = artf.SearchOrdered("a");
            nullresult.Should().BeNull();


            var BrFind = artf.SearchOrdered("br").ToList();
            var BrFind2 = _MS.AllArtists.Where(ar => ar.Name.Normalized().Contains("br")).ToList();
            BrFind.ShouldHaveSameElements(BrFind2);

            artf.Dispose();
        }


        [Test]
        public void TesterForSimiliraty()
        {
            IEntityFinder<IArtist> artf = new ItemFinder<IArtist>(_MS.AllArtists, ar => ar.Name);


            IArtist arn = _MS.AllArtists.Where(al=>al.Name=="Anthony Braxton").FirstOrDefault();
            arn.Should().NotBeNull();

            using (TimeTracer.TimeTrack("Without TesterForSimiliraty"))
            {
                var similirities = artf.FindSimilarMatches("anthony baxton").ToList();
                similirities.Should().Contain(arn);
            }

            using (TimeTracer.TimeTrack("Without TesterForSimiliraty"))
            {
                var similirities = artf.FindSimilarMatches("anthony braxtone").ToList();
                similirities.Should().Contain(arn);
            }

            using (TimeTracer.TimeTrack("Without TesterForSimiliraty"))
            {
                var similirities = artf.FindSimilarMatches("anthoni braxton").ToList();
                similirities.Should().Contain(arn);
            }
            

            


            using (TimeTracer.TimeTrack("Without TesterForSimiliraty"))
            {
                var similirities = artf.FindPotentialMisname().ToList();
                similirities.Should().HaveCount(27);
            }

          

            artf.Dispose();
        }
    }
}
