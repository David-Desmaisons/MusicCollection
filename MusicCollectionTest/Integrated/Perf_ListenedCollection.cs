using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;

using NUnit;
using NUnit.Framework;

using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Utilities;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModel.Filter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.Collection;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder(null, "SQLiteClean")]
    class Perf_ListenedCollection : IntegratedBase
    {
        private TimeSpan _MaxTimeToBuildSession = TimeSpan.FromSeconds(50);
        private TimeSpan _MaxTimeToOpenBefore = TimeSpan.FromSeconds(20);
        private TimeSpan _MaxMinToLiveSelectMany = TimeSpan.FromSeconds(2.2);
        private TimeSpan _MaxMinFilterAlbum = TimeSpan.FromSeconds(0.18);
        private TimeSpan _MaxMinFilterTrack = TimeSpan.FromSeconds(1);
        private TimeSpan _MaxMinFinderTrack = TimeSpan.FromSeconds(1.5);

        private TimeSpan _MaxMinFilterAlbumInvertedStandard = TimeSpan.FromSeconds(1.5);
        private TimeSpan _MaxMinFilterAlbumInverted = TimeSpan.FromSeconds(3.5);


        private int _AlbumNumber = 2860;
        private int _AlbumNumberFiltera = 2669;
        private int _AlbumNumberFilter2 = 1311;
        private int _AlbumNumberFilter3 = 280;
        private int _AlbumNumberFilter4 = 110;

        public const int _TrackNumber = 19707;
        private int _TrackNumberFiltera = 19168;
        private int _TrackNumberFilterb = 11090;
        private int _TrackNumberFilterba = 2546;
        private int _TrackNumberFilterbra = 662;





        private int _ArtistNumber = 2261;
        private int _MaxCount = 10;

        private IMusicSession _MS;
        private IMusicSession _msi;
        private TimeSpan _OpenTime;
        private TimeSpan _BuildSessionTime;


        [TestFixtureSetUp]
        public void TD()
        {
            Init();

            
            
            ThreadProperties TP = new ThreadProperties( ThreadPriority.Highest,ProcessPriorityClass.High);
            TP.SetCurrentThread();

            //Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            var tr2 = TimeTracer.TimeTrack("Session Building");
            using (tr2)
            {
                _MS = MusicSessionImpl.GetSession(_SK.Builder);
                _msi = _MS as MusicSessionImpl;
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            _BuildSessionTime = (TimeSpan)tr2.EllapsedTimeSpent;

            Assert.That(_MS.AllAlbums.Count, Is.EqualTo(0));
            Assert.That(_MS.AllGenres.Count, Is.EqualTo(0));
            Assert.That(_MS.AllArtists.Count, Is.EqualTo(0));



            var tr = TimeTracer.TimeTrack("Load before vacuum");
            using (tr)
            {
                IMusicImporter imi = _MS.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
            }

            _OpenTime = (TimeSpan)tr.EllapsedTimeSpent;


            Assert.That(_MS.AllAlbums.Count, Is.EqualTo(_AlbumNumber));
            Assert.That(_MS.AllArtists.Count, Is.EqualTo(_ArtistNumber));

            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        [Test]
        public void TestBuildingSession()
        {
            Assert.That(this._BuildSessionTime, Is.LessThan(this._MaxTimeToBuildSession));
        }

        [Test]
        public void TestOpenTime()
        {
            Assert.That(_OpenTime, Is.LessThan(_MaxTimeToOpenBefore));
        }

        [TestFixtureTearDown]
        public void SetUp()
        {
            _MS.Dispose();
            base.CleanDirectories();

        }


        private void TestPerfo(string name, Action<IMusicSession> Tobeevaluated, IMusicSession ms, int nboftime, TimeSpan MinimunMaxValue, Action<IMusicSession> Clean = null)
        {
            Console.WriteLine("Test Perfo:{0} Target:{1}", name, MinimunMaxValue);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TimeSpan First = TimeSpan.FromHours(1);
            for (int i = 0; i < nboftime; i++)
            {
                var tr = TimeTracer.TimeTrack(string.Format(name));
                using (tr)
                {
                    Tobeevaluated(ms);
                    using (TimeTracer.TimeTrack(name, "Garbage Collection"))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                TimeSpan curr = (TimeSpan)tr.EllapsedTimeSpent;
                First = TimeSpan.FromTicks(Math.Min(First.Ticks, curr.Ticks));
                if (Clean != null)
                {
                    Clean(ms);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            Assert.That(First, Is.LessThan(MinimunMaxValue));
        }



        //private string _Genre = "Free Jazz";


        private void PerFilter(IMusicSession ms, bool PerformTest = false)
        {

            FilterView fv = new FilterView(ms);


            var Res = ms.AllAlbums.LiveWhere(fv.FilterAlbum);
            IInvariant iiv = Res as IInvariant;

            Assert.That(Res.Count, Is.EqualTo(_AlbumNumber));

            //fv.SetFilterAll("a");
            ////= FilterType.All;
            ////fv.FilterValue = "a";

            //Assert.That(Res.Count, Is.EqualTo(_AlbumNumberFiltera));
            //if (PerformTest) Assert.That(iiv.Invariant);

            //AllFilter af = fv.FilterAll;

            //af.Filter = "b";
            //Assert.That(Res.Count, Is.EqualTo(_AlbumNumberFilter2));
            //if (PerformTest) Assert.That(iiv.Invariant);


            //af.Filter = "ba";
            //Assert.That(Res.Count, Is.EqualTo(_AlbumNumberFilter3));
            //if (PerformTest) Assert.That(iiv.Invariant);


            //af.Filter = "bra";
            //Assert.That(Res.Count, Is.EqualTo(_AlbumNumberFilter4));
            //if (PerformTest) Assert.That(iiv.Invariant);


            //af.Filter = "";
            //Assert.That(Res.Count, Is.EqualTo(_AlbumNumber));
            //if (PerformTest) Assert.That(iiv.Invariant);
        }

        private int FilterTrackCount(string RealFilterValue, IFullObservableCollection<ITrack> all)
        {
            var l = all.Where(t => t.Album.NormalizedName.Contains(RealFilterValue) || t.Album.Artists.Any(ar => ar.Name.Normalized().Contains(RealFilterValue)) || t.Name.ToLower().WithoutAccent().FastContains(RealFilterValue)).ToList();
            return l.Count;
        }

        private void DummyL(object sender, ObjectModifiedArgs oma)
        {
        }

        private void RawPerFilterTrack(IMusicSession ms)
        {

            ////Assert.That(ms.AllTracks.Count, Is.EqualTo(_TrackNumber));

            //using (TimeTracer.TimeTrack(string.Format("Raw filter empty")))
            //{
            Assert.That(FilterTrackCount("", ms.AllTracks), Is.EqualTo(_TrackNumber));
            //}

            //using (TimeTracer.TimeTrack(string.Format("Raw filter a")))
            //{
            Assert.That(FilterTrackCount("a", ms.AllTracks), Is.EqualTo(_TrackNumberFiltera));
            //}

            //using (TimeTracer.TimeTrack(string.Format("Raw filter b")))
            //{
            Assert.That(FilterTrackCount("b", ms.AllTracks), Is.EqualTo(_TrackNumberFilterb));
            //}

            //using (TimeTracer.TimeTrack(string.Format("Raw filter ba")))
            //{
            Assert.That(FilterTrackCount("ba", ms.AllTracks), Is.EqualTo(_TrackNumberFilterba));
            //}

            //using (TimeTracer.TimeTrack(string.Format("Raw filter bra")))
            //{
            Assert.That(FilterTrackCount("bra", ms.AllTracks), Is.EqualTo(_TrackNumberFilterbra));
            //}

            //using (TimeTracer.TimeTrack(string.Format("Raw filter empty")))
            //{
            Assert.That(FilterTrackCount("", ms.AllTracks), Is.EqualTo(_TrackNumber));
            //}
        }

        private class ToClean : IDisposable
        {
            //public IObservableLookup<string, IAlbum> Albumname;
            //public IObservableLookup<string, IAlbum> authours;
            public IObservableLookup<string, ITrack> tracks;

            public void Dispose()
            {
                //if (Albumname != null)
                //    Albumname.Dispose();

                //if (authours != null)
                //    authours.Dispose();

                if (tracks != null)
                    tracks.Dispose();
            }
        }




        private ToClean PerFilterTrackWithinvertedDictionary(IMusicSession ms)
        {
            ToClean tc = new ToClean();

            //IExtendedObservableCollection<IAlbum> Res = null;


            //using (TimeTracer.TimeTrack(string.Format("Album Dictionaries creation")))
            //{
            //    tc.Albumname = ms.AllAlbums.LiveSelectManyTuple(a => a.NormalizedName.GetSubstring().ToList()).LiveToLookUp(t => t.Item2, t => t.Item1);
            //    tc.authours = ms.AllAlbums.LiveSelectManyTuple(a => a.Author.ToLower().WithoutAccent().GetSubstring().ToList()).LiveToLookUp(t => t.Item2, t => t.Item1);
            //}

            using (TimeTracer.TimeTrack(string.Format("Track Dictionary raw creation")))
            {
                var res = ms.AllTracks.SelectMany(a => a.Name.ToLower().WithoutAccent().GetSubstring(), (al, s) => new Tuple<ITrack, string>(al, s)).ToLookup(t => t.Item2, t => t.Item1);
            }

            IExtendedObservableCollection<Tuple<ITrack, string>> Inter = null;
            using (TimeTracer.TimeTrack(string.Format("Track Dictionary creation- 1 select many")))
            {
                Inter = ms.AllTracks.LiveSelectManyTuple(a => a.Name.ToLower().WithoutAccent().GetSubstring().ToList());
            }


            using (TimeTracer.TimeTrack(string.Format("Track Dictionary creation- 2 look-up")))
            {
                tc.tracks = Inter.LiveToLookUp(t => t.Item2, t => t.Item1);
            }




            //using (TimeTracer.TimeTrack(string.Format("Using dictionnary")))
            //{
            //    Res = (tc.Albumname[""] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours[""] as IObservableGrouping<string, IAlbum>).Collection;

            //    Res = (tc.Albumname["a"] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours["a"] as IObservableGrouping<string, IAlbum>).Collection;

            //    Res = (tc.Albumname["b"] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours["b"] as IObservableGrouping<string, IAlbum>).Collection;

            //    Res = (tc.Albumname["ba"] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours["ba"] as IObservableGrouping<string, IAlbum>).Collection;

            //    Res = (tc.Albumname["bra"] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours["bra"] as IObservableGrouping<string, IAlbum>).Collection;

            //    Res = (tc.Albumname[""] as IObservableGrouping<string, IAlbum>).Collection;
            //    Res = (tc.authours[""] as IObservableGrouping<string, IAlbum>).Collection;

            //}

            return tc;

        }


        private class ToClean2 : IDisposable
        {
            public IObservableLookup<char, IAlbum> Albumname;
            public IObservableLookup<char, IAlbum> authours;
            public IObservableLookup<char, ITrack> tracks;

            public void Dispose()
            {
                if (Albumname != null)
                    Albumname.Dispose();

                if (authours != null)
                    authours.Dispose();

                if (tracks != null)
                    tracks.Dispose();
            }
        }


        private ToClean2 PerFilterTrackWithStandardInvertedDictionary(IMusicSession ms)
        {
            ToClean2 tc = new ToClean2();

            //IExtendedObservableCollection<IAlbum> Res = null;


            using (TimeTracer.TimeTrack(string.Format("Album Dictionaries creation")))
            {
                tc.Albumname = ms.AllAlbums.LiveSelectManyTuple(a => a.NormalizedName.ToList()).LiveToLookUp(t => t.Item2, t => t.Item1);
                tc.authours = ms.AllAlbums.LiveSelectManyTuple(a => a.Author.ToLower().WithoutAccent().ToList()).LiveToLookUp(t => t.Item2, t => t.Item1);
            }


            IExtendedObservableCollection<Tuple<ITrack, char>> Inter = null;
            using (TimeTracer.TimeTrack(string.Format("Track Dictionary creation- 1 select many")))
            {
                Inter = ms.AllTracks.LiveSelectManyTuple(a => a.Name.ToLower().WithoutAccent().ToList());
            }


            using (TimeTracer.TimeTrack(string.Format("Track Dictionary creation- 2 look-up")))
            {
                tc.tracks = Inter.LiveToLookUp(t => t.Item2, t => t.Item1);
            }


            //IDictionary<string, HashSet<ITrack>> Myres = null;
            //using (TimeTracer.TimeTrack(string.Format("Track Dictionary dede algo string 4")))
            //{
            //    Myres = new Dictionary<string, HashSet<ITrack>>();

            //    foreach (ITrack t in ms.AllTracks)
            //    {
            //        foreach (string s in t.Name.ToLower().WithoutAccent().GetSubstrings(4))
            //        {
            //            HashSet<ITrack> r = Myres.FindOrCreateEntity(s, (st) => new HashSet<ITrack>());
            //            r.Add(t);
            //        }
            //    }
            //}



            return tc;

        }

        private IEnumerable<string> ForTest
        {
            get
            {
                //yield return "";
                //yield return "b";
                yield return "ba";
                yield return "bra";
                yield return "music";
                yield return "jean";
                yield return "lonely woman";
                //yield return "c";
                yield return "braxton";
                yield return "love";
                yield return "love for sale";
                yield return "coleman";
                //yield return "";
                yield return "toto l`asticot";
                yield return "21";
                yield return "remember";
            }
        }

        [Test]
        public void TestFinderFunctionality()
        {
            Finder f = new Finder(_MS.AllTracks, 2);
            using (TimeTracer.TimeTrack(string.Format("TestFinderFunctionality")))
            {
                foreach (string s in this.ForTest)
                {
                    IEnumerable<ITrack> Res1 = f.ComputeForSearch(s).OrderBy(t=>t.ID);
                    IEnumerable<ITrack> Res2 = this.FilterCount2(s, _MS.AllTracks).OrderBy(t => t.ID);
                    Assert.That(Res1.SequenceEqual(Res2));
                }
            }
        }

        private void PerFilterTrack2(IMusicSession ms)
        {
            using (TimeTracer.TimeTrack(string.Format("Finder")))
            {
                Finder f = null;
                using (TimeTracer.TimeTrack(string.Format("1-Finder creation")))
                {
                    f = new Finder(ms.AllTracks, 2);
                }

                IEnumerable<ITrack> Res = null;

                using (TimeTracer.TimeTrack(string.Format("2-Finder usage")))
                {
                    foreach (string s in this.ForTest)
                    {
                        Res = f.ComputeForSearch(s).ToList();
                    }
                }
            }

            using (TimeTracer.TimeTrack(string.Format("Raw")))
            {
                foreach (string s in this.ForTest)
                {
                    IEnumerable<ITrack> Res2 = this.FilterCount2(s, ms.AllTracks).ToList();
                }
            }
        }

        private void PerFilterTrackGeneric(IMusicSession ms)
        {
            using (TimeTracer.TimeTrack(string.Format("Generic Finder")))
            {
                ItemFinder<ITrack> f = null;
                using (TimeTracer.TimeTrack(string.Format("1-Finder creation")))
                {
                    f = new ItemFinder<ITrack>(ms.AllTracks, (t) => t.Name);
                }

                IEnumerable<ITrack> Res = null;

                using (TimeTracer.TimeTrack(string.Format("Generic Finder usage")))
                {
                    foreach (string s in this.ForTest)
                    {
                        Res = f.Search(s).ToList();
                    }
                }
            }

            using (TimeTracer.TimeTrack(string.Format("Raw")))
            {
                foreach (string s in this.ForTest)
                {
                    IEnumerable<ITrack> Res2 = this.FilterCount2(s, ms.AllTracks).ToList();
                }
            }
        }

        private void PerFilterTrack(IMusicSession ms)
        {
            //Trace.WriteLine("Begin<<<<<<<<<<<<");
            FilterView fv = null;

            //using (TimeTracer.TimeTrack(string.Format("Filter creation")))
            //{
            fv = new FilterView(ms);
            //}

            IExtendedObservableCollection<ITrack> Res = null;

            //using (TimeTracer.TimeTrack(string.Format("Collection creation")))
            //{
            Res = ms.AllTracks.LiveWhere(fv.FilterTrack);
            //}

            Assert.That(Res.Count, Is.EqualTo(_TrackNumber));

            //fv.FilteringEntity = FilterType.All;

            ////Trace.WriteLine("Filter a");
            //fv.SetFilterAll("a");
            //Assert.That(Res.Count, Is.EqualTo(_TrackNumberFiltera));

            ////Trace.WriteLine("Filter b");
            //fv.SetFilterAll("b");
            //Assert.That(Res.Count, Is.EqualTo(_TrackNumberFilterb));

            ////Trace.WriteLine("Filter ba");
            //fv.SetFilterAll("ba");
            //Assert.That(Res.Count, Is.EqualTo(_TrackNumberFilterba));

            ////Trace.WriteLine("Filter bra");
            //fv.SetFilterAll("bra");
            //Assert.That(Res.Count, Is.EqualTo(_TrackNumberFilterbra));


            //Trace.WriteLine("Filter toto");
            //fv.FilterValue = "toto kkpo hhhh";
            //Assert.That(Res.Count, Is.EqualTo(0));

            //Trace.WriteLine("Filter empy");
            //fv.SetFilterAll("");
            //Assert.That(Res.Count, Is.EqualTo(_TrackNumber));
            //Trace.WriteLine("End<<<<<<<<<<<<");

            Res.Dispose();

            fv.Dispose();
        }

        private int FilterCount(string RealFilterValue, IFullObservableCollection<IAlbum> all)
        {
            //return all.Where((a) => a.NormalizedName.FastContains(RealFilterValue) || a.Author.FastContains(RealFilterValue)).Count();

            return all.Where((a) => a.NormalizedName.FastContains(RealFilterValue) || a.Author.ToLower().WithoutAccent().FastContains(RealFilterValue)).Count();
        }

        private IEnumerable<ITrack> FilterCount2(string RealFilterValue, IFullObservableCollection<ITrack> all)
        {

            return all.Where((a) => a.Name.ToLower().WithoutAccent().Contains(RealFilterValue));
        }

        private void RawtestFilter(IMusicSession ms)
        {
            IFullObservableCollection<IAlbum> all = ms.AllAlbums;

            int Count = FilterCount("", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumber));

            Count = FilterCount("a", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumberFiltera));

            Count = FilterCount("b", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumberFilter2));

            Count = FilterCount("ba", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumberFilter3));

            Count = FilterCount("bra", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumberFilter4));

            Count = FilterCount("", all);
            Assert.That(Count, Is.EqualTo(_AlbumNumber));
        }


        private class Finder
        {
            private IDictionary<string, HashSet<ITrack>> _Myres = null;
            private int _Depth;

            internal Finder(IEnumerable<ITrack> tracks, int Depth)
            {
                _Depth = Depth;
                //_Myres = new Dictionary<string, HashSet<ITrack>>(20000);
                _Myres = new Dictionary<string, HashSet<ITrack>>(2000);
                foreach (ITrack t in tracks)
                {
                    foreach (string s in t.Name.ToLower().WithoutAccent().GetSubstrings(_Depth))
                    {
                        HashSet<ITrack> r = _Myres.FindOrCreateEntity(s, (st) => new HashSet<ITrack>());
                        r.Add(t);
                    }
                }

                _Myres.Add(string.Empty, new HashSet<ITrack>(tracks));
            }

            private IEnumerable<ITrack> Candidate(string sear)
            {
                IEnumerable<string> strings = sear.GetExactSubstrings(_Depth);
                //List<HashSet<ITrack>> candidate = new List<HashSet<ITrack>>();
                IEnumerable<ITrack> Res = null;

                foreach (string s in strings)
                {
                    HashSet<ITrack> r = null;
                    if (!_Myres.TryGetValue(s, out r))
                        yield break;

                    if (Res == null)
                    {
                        Res = r;
                    }
                    else
                    {
                        Res = Res.Where(t => r.Contains(t));
                    }
                }

                Res = Res.Where(t => t.Name.ToLower().WithoutAccent().Contains(sear));;
            
                foreach (var h in Res)
                {
                    yield return h;
                }
            }

            internal IEnumerable<ITrack> ComputeForSearch(string searh)
            {
                string sear = searh.ToLower().WithoutAccent();
                if (sear.Length <= _Depth)
                {
                    HashSet<ITrack> r = null;
                    _Myres.TryGetValue(sear, out r);
                    return r;
                }

                return Candidate(sear);

                //return Candidate(sear).Where(t => t.Name.ToLower().WithoutAccent().Contains(sear));
            }
        }

        private void PerfRun(IMusicSession ms, List<IExtendedObservableCollection<IObservableGrouping<IArtist, IAlbum>>> l)
        {
            for (int i = 0; i < _MaxCount; i++)
            {
                l.Add(ms.AllAlbums.LiveSelectManyTuple(a => a.Artists).LiveToLookUp((t) => t.Item2, (t) => t.Item1).LiveOrderBy((a) => a.Key.Name));

            }
        }


        [Test]
        public void TestLiveSelectMany()
        {
            List<IExtendedObservableCollection<IObservableGrouping<IArtist, IAlbum>>> l = new List<IExtendedObservableCollection<IObservableGrouping<IArtist, IAlbum>>>();
            TestPerfo("LiveSelectMany", (ms) => PerfRun(ms, l), _MS, 7, _MaxMinToLiveSelectMany, (ms) => l.Apply(el => el.Dispose()));
        }

        [Test]
        public void FunctionalFilterAlbum()
        {
            PerFilter(_MS, true);
        }

        [Test]
        public void TestPerFilterTrackWithinvertedDictionary()
        {
            IDisposable id = null;
            TestPerfo("Inverted Dictionary", (ms) => id = PerFilterTrackWithinvertedDictionary(ms), _MS, 9, _MaxMinFilterAlbumInverted, (ms) => id.Dispose());
        }


        [Test]
        public void TestPerFilterTrackWithStandardinvertedDictionary()
        {
            IDisposable id = null;
            TestPerfo("Inverted Dictionary", (ms) => id = PerFilterTrackWithStandardInvertedDictionary(ms), _MS, 9, _MaxMinFilterAlbumInvertedStandard, (ms) => id.Dispose());
        }

        [Test]
        public void TestFilterAlbum()
        {
            TestPerfo("Filter Album", (ms) => PerFilter(ms, false), _MS, 9, _MaxMinFilterAlbum);
        }

        [Test]
        public void TestFilterTrack()
        {
            TestPerfo("Filter Track", PerFilterTrack, _MS, 9, _MaxMinFilterTrack);
            //, (ms) => StringExtension.ResetCache());
        }

        [Test]
        public void TestFilterTrackFinder()
        {
            TestPerfo("Filter Track with Finder", PerFilterTrack2, _MS, 9, _MaxMinFinderTrack);
            //, (ms) => StringExtension.ResetCache());
        }

        [Test]
        public void TestFilterTrackFinderGeneric()
        {
            TestPerfo("Filter Track with Finder", PerFilterTrackGeneric, _MS, 9, _MaxMinFinderTrack);
            //, (ms) => StringExtension.ResetCache());
        }




        [Test]
        public void TestFilterRawForReference()
        {
            TestPerfo("Filter Raw Album For Reference", RawtestFilter, _MS, 9, _MaxMinFilterAlbum);
        }

        //[Test]
        //public void a()
        //{
        //    var res = _MS.AllTracks.Where(t => t.Name.ToLower().WithoutAccent().Contains("a") && t.Name.IndexOf("a", StringComparison.OrdinalIgnoreCase) < 0).ToList();
        //    var res2 = _MS.AllTracks.Where(t => !t.Name.ToLower().WithoutAccent().Contains("a") && t.Name.IndexOf("a", StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        //}

        [Test]
        public void TestFilterRawTrackForReference()
        {
            TestPerfo("Filter Raw Track For Reference", RawPerFilterTrack, _MS, 9, _MaxMinFilterTrack);
        }
    }
}
