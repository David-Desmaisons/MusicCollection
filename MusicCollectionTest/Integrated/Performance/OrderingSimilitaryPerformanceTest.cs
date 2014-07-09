using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using System.Runtime.CompilerServices;
using System.ComponentModel;

using FluentAssertions;

using MusicCollectionTest.Integrated.Tools;
using MusicCollection.Infra;
using MusicCollectionWPF.UserControls.AlbumPresenter;
using MusicCollection.Fundation;
using MusicCollection.Infra.Collection;
using MusicCollectionWPF.ViewModel.Element;
using MusicCollectionWPF.ViewModel.Interface;
using MusicCollectionWPF.ViewModel;
using MusicCollectionTest.ToolBox.FunctionListener;

namespace MusicCollectionTest.Integrated.Performance
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    public class OrderingSimilitaryPerformanceTest : IntegratedTestImprovedBase
    {

        //public class AfinityAlbumComparer : IUpdatableComparer<IAlbum>
        //{
        //    private IDictionary<IAlbum, int> _Dic;
        //    private IMusicSession _Session;

        //    public AfinityAlbumComparer(IMusicSession ims, IAlbum init)
        //    {
        //        _Session = ims;
        //        _Dic = new Dictionary<IAlbum, int>(ims.AllAlbums.Count);
        //        _All = init;

        //        _Artist = _All.Artists.ToHashSet();
        //        _ArtitsCount = _Artist.Count;
        //        _Genre = _All.MainGenre;

        //        //bufferize results
        //        _Session.AllGenres.Apply(g => g.Compare(_Genre));
        //        _RatioArtits = (int)Math.Floor((double)35000 / _ArtitsCount);

        //        var als = _Session.AllAlbums.ToList();

        //        Parallel.ForEach(als,
        //            () => new Dictionary<IAlbum, int>(),
        //            (el, lc, localdic) => { localdic.Add(el, RawCompare(el)); return localdic; },
        //            (ldic) => { lock (_Dic) _Dic.Import(ldic); });

        //        _Dic.Keys.Apply(al => al.PropertyChanged += al_PropertyChanged);
        //    }

        //    private HashSet<IArtist> _Artist;
        //    private int _ArtitsCount = 0;
        //    private IGenre _Genre;
        //    private int _RatioArtits = 0;

        //    private int RawCompare(IAlbum T1)
        //    {
        //        int res = 0;

        //        if (Object.ReferenceEquals(T1, _All))
        //            return res;

        //        res = 100000;

        //        if ((T1 == null) || (_All == null))
        //            return res;

        //        IGenre g = T1.MainGenre;

        //        if ((g != null) && (_Genre != null))
        //        {
        //            int vg = g.Compare(_Genre);
        //            res -= 1900 - Math.Min(vg, 19) * 100;
        //        }

        //        if ((T1.Year != 0) && (_All.Year != 0))
        //        {
        //            int Dist = Math.Min(2000, (Math.Abs(T1.Year - _All.Year)));
        //            res -= 10000 - 5 * Dist;
        //        }

        //        if (_ArtitsCount == 0)
        //            return res;

        //        IList<IArtist> arts = T1.Artists;
        //        int c = arts.Count;
        //        if (c == 0)
        //        {
        //            return res;
        //        }

        //        arts.Apply((a) => { if (_Artist.Contains(a)) res -= (int)Math.Floor((double)35000 / c); });

        //        _Artist.Apply((a) => { if (arts.Contains(a)) res -= this._RatioArtits; });

        //        return res;
        //    }

        //    private IAlbum _All;
        //    public IAlbum Album
        //    {
        //        get { return _All; }
        //    }

        //    private bool ResetAlbum(IAlbum al)
        //    {
        //        _Dic.Remove(al);
        //        _Dic.Add(al, RawCompare(al));

        //        return al == Album;
        //    }

        //    public int Compare(IAlbum xx, IAlbum yy)
        //    {
        //        if (object.ReferenceEquals(xx, yy))
        //            return 0;

        //        return Value(xx) - Value(yy);
        //    }

        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    private int Value(IAlbum al)
        //    {
        //        var trytofind = _Dic.FindOrCreate(al, c => RawCompare(c));
        //        if (trytofind.Item1 == false)
        //        {
        //            al.PropertyChanged += al_PropertyChanged;
        //        }
        //        return Math.Abs(trytofind.Item2);
        //    }

        //    private void al_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //    {
        //        var album = sender as IAlbum;
        //        if (ResetAlbum(album))
        //        {
        //            FireChanged();
        //        }
        //        else
        //        {
        //            FireElementChanged(album);
        //        }
        //    }

        //    private void FireChanged()
        //    {
        //        if (OnChanged != null) OnChanged(this, EventArgs.Empty);
        //    }

        //    private void FireElementChanged(IAlbum element)
        //    {
        //        if (OnChanged != null) OnElementChanged(this, ElementChangedArgs<IAlbum>.From(element));
        //    }

        //    public event EventHandler OnChanged;
        //    public event EventHandler<ElementChangedArgs<IAlbum>> OnElementChanged;

        //    public void Dispose()
        //    {
        //        foreach (IAlbum al in _Dic.Keys)
        //        {
        //            al.PropertyChanged -= al_PropertyChanged;
        //        }
        //        _Dic.Clear();
        //    }
        //}

        public OrderingSimilitaryPerformanceTest()
        {
        }

        #region test helpers

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            baseTestFixtureSetUp();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            baseTestFixtureTearDown();
        }

        [SetUp]
        public void SetUp()
        {
            baseSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            baseTearDown();
        }

        #endregion

        private void OrderTotal(IAlbum ial)
        {
            Console.WriteLine("Album Reference {0}", ial);
            //IUpdatableComparer<IAlbum> ct = new AfinityAlbumComparer(_IMusicSession, ial);

            IUpdatableComparer<IAlbum> ct = new AlbumDistanceComparerFactory(_IMusicSession).GetComparer(ial);

            for (int k = 0; k < 5; k++)
            {
                TimeTracer tr2 = TimeTracer.TimeTrack(string.Format("50 odering full with album {0}", ial));

                using (tr2)
                {
                    List<List<IAlbum>> ll = new List<List<IAlbum>>();
                    for (int i = 0; i < 50; i++)
                    {
                        var myres = _IMusicSession.AllAlbums.OrderBy(al => al, ct);
                        ll.Add(myres.ToList());
                    }
                }
            }
        }

        private void OrderTotalFast(IAlbum ial)
        {
            Console.WriteLine("Album Reference {0}", ial);
            //AfinityAlbumComparer ct = new AfinityAlbumComparer(_IMusicSession, ial);
            IUpdatableComparer<IAlbum> ct = new AlbumDistanceComparerFactory(_IMusicSession).GetComparer(ial);

            for (int k = 0; k < 5; k++)
            {
                TimeTracer tr2 = TimeTracer.TimeTrack(string.Format("50 odering parcial(50) with album {0}", ial));

                using (tr2)
                {
                    List<List<IAlbum>> ll = new List<List<IAlbum>>();
                    for (int i = 0; i < 50; i++)
                    {
                        var myres = _IMusicSession.AllAlbums.SortFirst(50, ct);
                        ll.Add(myres.ToList());
                    }
                }
            }
        }

        private void OrderTotalFastParellel(IAlbum ial)
        {
            Console.WriteLine("Album Reference {0}", ial);
            //AfinityAlbumComparer ct = new AfinityAlbumComparer(_IMusicSession, ial);
            IUpdatableComparer<IAlbum> ct = new AlbumDistanceComparerFactory(_IMusicSession).GetComparer(ial);

            for (int k = 0; k < 5; k++)
            {
                TimeTracer tr2 = TimeTracer.TimeTrack(string.Format("50 odering parcial(50) with album {0}", ial));

                using (tr2)
                {
                    List<List<IAlbum>> ll = new List<List<IAlbum>>();
                    for (int i = 0; i < 50; i++)
                    {
                        var myres = _IMusicSession.AllAlbums.SortFirstParallel(50, ct);
                        ll.Add(myres.ToList());
                    }
                }
            }
        }

        private void OrderTotalTrack(Func<IList<ITrack>, IComparer<ITrack>, ICollection<ITrack>> Filter, string Description)
        {

            IComparer<ITrack> RatingComparer = Comparer<ITrack>.Create((x, y) => (int)x.Rating - (int)y.Rating);
            IComparer<ITrack> LastPlayedComparer = Comparer<ITrack>.Create((x, y) =>


                (x.LastPlayed.HasValue ? (y.LastPlayed.HasValue ? x.LastPlayed.Value.CompareTo(y.LastPlayed.Value) : int.MinValue) :
                                         (y.LastPlayed.HasValue ? int.MaxValue : 0)));
            //for (int k = 0; k < 5; k++)
            //{
            using (TimeTracer.TimeTrack(string.Format("40 {0} with rating    ", Description)))
            {
                List<ICollection<ITrack>> ll = new List<ICollection<ITrack>>();
                for (int i = 0; i < 40; i++)
                {
                    var myres = Filter(_IMusicSession.AllTracks, RatingComparer);
                    ll.Add(myres);
                }
            }

            using (TimeTracer.TimeTrack(string.Format("40 {0} with last played", Description)))
            {
                List<ICollection<ITrack>> ll = new List<ICollection<ITrack>>();
                for (int i = 0; i < 40; i++)
                {
                    var myres = Filter(_IMusicSession.AllTracks, LastPlayedComparer);
                    ll.Add(myres);
                }
            }


            //}
        }




        [Test]
        public void TestPef_OrderTotalTrack_OrderBy()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 5; i++)
            {
                OrderTotalTrack((ts, c) => ts.OrderBy(t => t, c).ToList(), "Full Sort");
            }
        }

        [Test]
        public void TestPef_OrderTotalTrack_SortFirst()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 5; i++)
            {
                OrderTotalTrack((ts, c) => ts.SortFirst(50, c), "Parcial Sort 50");
            }
        }

        [Test]
        public void TestPef_OrderTotalTrack_SortFirstParallel()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 5; i++)
            {
                OrderTotalTrack((ts, c) => ts.SortFirstParallel(50, c), "Parcial Sort 50(parellel)");
            }
        }

        [Test]
        public void TestPef_OrderTotalAlbum_OrderBy()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            OrderTotal(_IMusicSession.AllAlbums[100]);
            OrderTotal(_IMusicSession.AllAlbums[500]);
            OrderTotal(_IMusicSession.AllAlbums[1000]);
        }

        [Test]
        public void Test_Genre_Should_beCoherent()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            foreach (IGenre g in _IMusicSession.AllGenres)
            {
                g.Albums.Should().BeEquivalentTo(_IMusicSession.AllAlbums.Where(al => al.MainGenre == g));
            }
        }

        [Test]
        public void ViewModel_ExtensionTest_Genre_Should_beCoherent()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);


            IAlbum al = _IMusicSession.AllAlbums[0];
            ITrack tr = al.Tracks[0];

            al.GetAlbumCollection().Should().Equal(al);
            al.MainGenre.GetAlbumCollection().Should().Equal(al.MainGenre.Albums);
            tr.GetAlbumCollection().Should().Equal(al);
            al.Artists[0].GetAlbumCollection().Should().Equal(al.Artists[0].Albums);

        }


        [Test]
        public void TestPef_OrderTotalAlbum_OrderTotalFast()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            OrderTotalFast(_IMusicSession.AllAlbums[100]);
            OrderTotalFast(_IMusicSession.AllAlbums[500]);
            OrderTotalFast(_IMusicSession.AllAlbums[1000]);
        }

        [Test]
        public void TestPef_OrderTotalAlbum_OrderTotalFastParellel()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            OrderTotalFastParellel(_IMusicSession.AllAlbums[100]);
            OrderTotalFastParellel(_IMusicSession.AllAlbums[500]);
            OrderTotalFastParellel(_IMusicSession.AllAlbums[1000]);
        }

        [Test]
        public void Test_functional_AfinityAlbumComparer()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            var first = _IMusicSession.AllAlbums.First();
            first.Artists.Should().HaveCount(3);

            //AfinityAlbumComparer target = new AfinityAlbumComparer(_IMusicSession, first);
            IUpdatableComparer<IAlbum> target = new AlbumDistanceComparerFactory(_IMusicSession).GetComparer(first);

            _IMusicSession.AllAlbums.MinBy(target).Should().Be(first);

            var second = _IMusicSession.AllAlbums.Skip(1).MinBy(target);

            second.Artists.Apply(a => first.Artists.Should().Contain(a));
            first.Artists.Apply(a => second.Artists.Should().Contain(a));

            target.MonitorEvents();

            using (var mod = second.GetModifiableAlbum())
            {
                mod.Artists.Clear();
                mod.Artists.Add(_IMusicSession.CreateArtist("Teste Artist"));
                mod.Commit();
            }

            second.Artists.Should().HaveCount(1);
            second.Artists.First().Name.Should().Be("Teste Artist");

            target.ShouldRaise("OnElementChanged").WithSender(target).WithArgs<ElementChangedArgs<IAlbum>>(a => a.Element == second);

            var newsecond = _IMusicSession.AllAlbums.Skip(1).MinBy(target);

            newsecond.Should().NotBe(second);
        }

        //[Test]
        //public void TestNewTrackView()
        //{
        //    _IMusicSession.AllAlbums.Should().HaveCount(3377);
        //    _IMusicSession.AllTracks.Should().HaveCount(23996);

        //    using (TimeTracer.TimeTrack(string.Format("Comparing NewTrackView and TrackView")))
        //    {
        //        foreach (ITrack track in _IMusicSession.AllTracks)
        //        {
        //            var res1 = TrackView.GetTrackView(track);
        //            var res2 = NewTrackViewer.GetTrackView(track);
        //            res1.ShouldHave().AllProperties().EqualTo(res2);
        //        }
        //    }
        //}

        [Test]
        public void TestSimpleNewTrackViewer()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            using (TimeTracer.TimeTrack(string.Format("Comparing NewTrackView and TrackView")))
            {
                foreach (ITrack track in _IMusicSession.AllTracks)
                {
                    var res1 = TrackView.GetTrackView(track);
                    var res2 = SimpleNewTrackViewer.GetTrackView(track);
                    res1.ShouldHave().AllProperties().EqualTo(res2);
                }
            }

            foreach (ITrack track in _IMusicSession.AllTracks)
            {
                var res1 = TrackView.GetTrackView(track);
                var res2 = SimpleNewTrackViewer.GetTrackView(track);
                res1.Dispose();
                res2.Dispose();
            }
        }

        [Test]
        public void PerfomanceComputeTrackView_Complete()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 10; i++)
            {
                List<TrackView> trs =null;
                using (TimeTracer.TimeTrack(string.Format("Computing TrackView {0}", i)))
                {
                    trs = _IMusicSession.AllTracks.Select(tr => TrackView.GetTrackView(tr)).ToList();
                    trs.Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
                    var lidt = trs.Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
                }

                trs.Apply(tv => tv.PropertyChanged -= tv_PropertyChanged);
            }

            foreach (ITrack track in _IMusicSession.AllTracks)
            {
                var res1 = TrackView.GetTrackView(track);
                res1.Dispose();
            }
        }

        //[Test]
        //public void PerfomanceComputeNewTrackViewer_Complete()
        //{
        //    _IMusicSession.AllAlbums.Should().HaveCount(3377);
        //    _IMusicSession.AllTracks.Should().HaveCount(23996);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        using (TimeTracer.TimeTrack(string.Format("Computing NewTrackViewer {0}", i)))
        //        {
        //            List<NewTrackViewer> trs = _IMusicSession.AllTracks.Select(tr => NewTrackViewer.GetTrackView(tr)).ToList();
        //            trs.Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
        //            var lidt = trs.Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
        //        }
        //    }
        //}


        [Test]
        public void PerfomanceComputeSimpleNewTrackViewer_Complete()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 10; i++)
            {
                List<SimpleNewTrackViewer> trs = null;
                using (TimeTracer.TimeTrack(string.Format("Computing SimpleNewTrackViewer {0}", i)))
                {
                    trs = _IMusicSession.AllTracks.Select(tr => SimpleNewTrackViewer.GetTrackView(tr)).ToList();
                    trs.Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
                    var lidt = trs.Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
                }

                
                trs.Apply(tv => tv.PropertyChanged -= tv_PropertyChanged);
            }

            foreach (ITrack track in _IMusicSession.AllTracks)
            {
                var res1 = SimpleNewTrackViewer.GetTrackView(track);
                res1.Dispose();
            }
        }
  
  

        [Test]
        public void PerfomanceComputeTrackView()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("Computing TrackView {0}", i)))
                {
                    List<TrackView> trs = _IMusicSession.AllTracks.Select(tr => TrackView.GetTrackView(tr)).ToList();
                }
            }

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("TrackView+Observing {0}", i)))
                {
                    _IMusicSession.AllTracks.Select(tr => TrackView.GetTrackView(tr)).Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
                }

                using (TimeTracer.TimeTrack(string.Format("TrackView+UnObserving {0}", i)))
                {
                    _IMusicSession.AllTracks.Select(tr => TrackView.GetTrackView(tr)).Apply(tv => tv.PropertyChanged -= tv_PropertyChanged);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("TrackView+GetInfo {0}", i)))
                {
                    var lidt = _IMusicSession.AllTracks.Select(tr => TrackView.GetTrackView(tr))
                        .Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
                }
            }

            foreach (ITrack track in _IMusicSession.AllTracks)
            {
                var res1 = TrackView.GetTrackView(track);
                res1.Dispose();
            }
        }


        //[Test]
        //public void PerfomanceComputeNewTrackViewer()
        //{
        //    _IMusicSession.AllAlbums.Should().HaveCount(3377);
        //    _IMusicSession.AllTracks.Should().HaveCount(23996);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        using (TimeTracer.TimeTrack(string.Format("Computing NewTrackViewer {0}", i)))
        //        {
        //            List<NewTrackViewer> trs = _IMusicSession.AllTracks.Select(tr => NewTrackViewer.GetTrackView(tr)).ToList();
        //        }
        //    }

        //    for (int i = 0; i < 10; i++)
        //    {
        //        using (TimeTracer.TimeTrack(string.Format("NewTrackViewer+Observing {0}", i)))
        //        {
        //            _IMusicSession.AllTracks.Select(tr => NewTrackViewer.GetTrackView(tr)).Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
        //        }

        //        using (TimeTracer.TimeTrack(string.Format("NewTrackViewer+UnObserving {0}", i)))
        //        {
        //            _IMusicSession.AllTracks.Select(tr => NewTrackViewer.GetTrackView(tr)).Apply(tv => tv.PropertyChanged -= tv_PropertyChanged);
        //        }
        //    }

        //    for (int i = 0; i < 10; i++)
        //    {
        //        using (TimeTracer.TimeTrack(string.Format("NewTrackViewer+GetInfo {0}", i)))
        //        {
        //            var lidt = _IMusicSession.AllTracks.Select(tr => NewTrackViewer.GetTrackView(tr))
        //                .Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
        //        }
        //    }
        //}

        [Test]
        public void PerfomanceComputeSimpleNewTrackViewer()
        {
            _IMusicSession.AllAlbums.Should().HaveCount(3377);
            _IMusicSession.AllTracks.Should().HaveCount(23996);

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("Computing SimpleNewTrackViewer {0}", i)))
                {
                    List<SimpleNewTrackViewer> trs = _IMusicSession.AllTracks.Select(tr => SimpleNewTrackViewer.GetTrackView(tr)).ToList();
                }
            }

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("SimpleNewTrackViewer+Observing {0}", i)))
                {
                    _IMusicSession.AllTracks.Select(tr => SimpleNewTrackViewer.GetTrackView(tr)).Apply(tv => tv.PropertyChanged += tv_PropertyChanged);
                }

                using (TimeTracer.TimeTrack(string.Format("SimpleNewTrackViewer+UnObserving {0}", i)))
                {
                    _IMusicSession.AllTracks.Select(tr => SimpleNewTrackViewer.GetTrackView(tr)).Apply(tv => tv.PropertyChanged -= tv_PropertyChanged);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                using (TimeTracer.TimeTrack(string.Format("SimpleNewTrackViewer+GetInfo {0}", i)))
                {
                    var lidt = _IMusicSession.AllTracks.Select(tr => SimpleNewTrackViewer.GetTrackView(tr))
                        .Select(t => new { al = t.AlbumGenre, al2 = t.AlbumName, alt3 = t.Name, alt4 = t.DiscNumber }).ToList();
                }
            }

            foreach (ITrack track in _IMusicSession.AllTracks)
            {
                var res1 = SimpleNewTrackViewer.GetTrackView(track);
                res1.Dispose();
            }
        }

        void tv_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}
