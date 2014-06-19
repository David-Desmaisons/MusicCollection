using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollection.DataExchange;

namespace MusicCollectionTest.TestObjects
{
    public class AlbumComparer
    {
        public enum AlbumDescriptorCompareMode { AlbumMD, AlbumandTrackMD };

        private class AlbumMDComparer : IEqualityComparer<IFullAlbumDescriptor>
        {
            static public bool staticEquals(IFullAlbumDescriptor x, IFullAlbumDescriptor y, bool id)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if ((x == null) || (y == null))
                    return false;

                if (x.Name != y.Name)
                {
                    Console.WriteLine("Expected Album Name: {0}  was: {1}", x.Name, y.Name);
                    return false;
                }


                if ((!id) && (x.Year != y.Year))
                {
                    Console.WriteLine("Expected Album Year: {0}  was: {1}", x.Year, y.Year);
                    return false;
                }

                if (x.Artist != y.Artist)
                {
                    Console.WriteLine("Expected Album Artist: {0}  was: {1}", x.Artist, y.Artist);
                    return false;
                }

                if ((!id) && (x.Genre != y.Genre))
                {
                    Console.WriteLine("Expected Album Genre: {0}  was: {1}", x.Genre, y.Genre);
                    return false;
                }

                return true;
            }

            public bool Equals(IFullAlbumDescriptor x, IFullAlbumDescriptor y)
            {
                return staticEquals(x, y, _ID);
            }

            public static int staticGetHashCode(IFullAlbumDescriptor obj)
            {
                if (obj == null)
                    return 0;

                return obj.Artist.GetHashCode() ^ obj.Name.GetHashCode();
            }

            public int GetHashCode(IFullAlbumDescriptor obj)
            {
                return staticGetHashCode(obj);
            }

            private bool _ID;
            internal AlbumMDComparer(bool id)
            {
                _ID = id;
            }
        }

        private class AlbumTrackMDComparer : IEqualityComparer<IFullAlbumDescriptor>, IEqualityComparer<ITrackMetaDataDescriptor>
        {
            public bool Equals(IFullAlbumDescriptor x, IFullAlbumDescriptor y)
            {
                if (!AlbumMDComparer.staticEquals(x, y, _ID))
                    return false;

                if (object.ReferenceEquals(x.TrackDescriptors, y.TrackDescriptors))
                    return true;

                if ((x.TrackDescriptors == null) || (y.TrackDescriptors == null))
                    return false;

                if (x.TrackDescriptors.Count != y.TrackDescriptors.Count)
                {
                    Console.WriteLine("Expected Track Number: {0}  was: {1}", x.TrackDescriptors.Count, y.TrackDescriptors.Count);
                    return false;
                }

                return x.TrackDescriptors.OrderBy(o => o.TrackNumber).ThenBy(o => o.Name).SequenceEqual(y.TrackDescriptors.OrderBy(o => o.TrackNumber).ThenBy(o => o.Name), this);
            }

            public int GetHashCode(IFullAlbumDescriptor obj)
            {
                return AlbumMDComparer.staticGetHashCode(obj);
            }

            public bool Equals(ITrackMetaDataDescriptor x, ITrackMetaDataDescriptor y)
            {
                if (x.Name != y.Name)
                {
                    Console.WriteLine("Track Comparison failed {0}  and {1}", x, y);
                    Console.WriteLine("Expected Track Name: {0}  was: {1}", x.Name, y.Name);
                    return false;
                }

                if (x.TrackNumber != y.TrackNumber)
                {
                    Console.WriteLine("Track Comparison failed {0}  and {1}", x, y);
                    Console.WriteLine("Expected Track Number: {0}  was: {1}", x.TrackNumber, y.TrackNumber);
                    return false;
                }

                if (y.Duration.TotalMilliseconds > 0)
                {
                    if (x.Duration.TotalMilliseconds != y.Duration.TotalMilliseconds)
                    {
                        Console.WriteLine("Track Comparison failed {0}  and {1}", x, y);
                        Console.WriteLine("Expected Track Duration: {0}  was: {1}", x.Duration.TotalMilliseconds, y.Duration.TotalMilliseconds);
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(ITrackMetaDataDescriptor obj)
            {
                if (obj == null)
                    return 0;

                return obj.TrackNumber.GetHashCode() ^ obj.Name.GetHashCode();
            }

            private bool _ID;
            internal AlbumTrackMDComparer(bool id)
            {
                _ID = id;
            }
        }

        static private IEqualityComparer<IFullAlbumDescriptor> AlbumMDComp(bool ignoredate)
        {
            return new AlbumMDComparer(ignoredate);
        }

        static private IEqualityComparer<IFullAlbumDescriptor> AlbumTrackMDComp(bool ignoredate)
        {
            return new AlbumTrackMDComparer(ignoredate);
        }


        static internal IEqualityComparer<IFullAlbumDescriptor> GetComparer(AlbumDescriptorCompareMode iMode, bool ignoredate)
        {
            switch (iMode)
            {
                case AlbumDescriptorCompareMode.AlbumandTrackMD:
                    return AlbumTrackMDComp(ignoredate);

                case AlbumDescriptorCompareMode.AlbumMD:
                    return AlbumMDComp(ignoredate);
            }

            return null;
        }

        protected void AssertEnumerable<T, TK>(IEnumerable<T> un, IEnumerable<T> deux)
        {
            Assert.That(un.SequenceEqual(deux), Is.True);
        }

        protected void AssertEnumerable<T, TK>(IEnumerable<T> un, IEnumerable<T> deux, Func<T, TK> Comp)
        {
            Assert.That(un.OrderBy(Comp).SequenceEqual(deux.OrderBy(Comp)), Is.True);
        }

        protected void AssertEnumerable<T, TK, TK2>(IEnumerable<T> un, IEnumerable<T> deux, Func<T, TK> Main, Func<T, TK2> Undrower)
        {
            Assert.That(un.OrderBy(Main).ThenBy(Undrower).SequenceEqual(deux.OrderBy(Main).ThenBy(Undrower)), Is.True);
        }

        protected static void AssertEnumerable<T, TK, TK2>(IEnumerable<T> un, IEnumerable<T> deux, Func<T, TK> Main, Func<T, TK2> Undrower, IEqualityComparer<T> Comp)
        {
            Assert.That(un.OrderBy(Main).ThenBy(Undrower).SequenceEqual(deux.OrderBy(Main).ThenBy(Undrower), Comp), Is.True);
        }

        protected void AssertAlbumDescriptors(IEnumerable<IFullAlbumDescriptor> un, IEnumerable<IFullAlbumDescriptor> deux, AlbumDescriptorCompareMode mode, bool ignoredate = false)
        {
            AssertEnumerable(un, deux, a => a.Name, a => a.Artist, GetComparer(mode, ignoredate));
        }

        protected void AssertAlbums(IList<IAlbum> AllAlbums, IEnumerable<IFullAlbumDescriptor> deux, AlbumDescriptorCompareMode mode, bool ignoredate = false)
        {
            var res2 = from al in AllAlbums select ExportAlbum.CopyAlbum(al as Album);
            AssertEnumerable(res2, deux, a => a.Name, a => a.Artist, GetComparer(mode, ignoredate));
        }

        public static void AssertAlbums(IMusicSession Session, IEnumerable<IFullAlbumDescriptor> deux, AlbumDescriptorCompareMode mode, bool AdditionalTest = false, bool ignoredate = false)
        {
            IInvariant mi = Session as IInvariant;

            Assert.That(mi.Invariant, Is.True);

            var res2 = from al in Session.AllAlbums select ExportAlbum.CopyAlbum(al as Album);
            AssertEnumerable(res2, deux, a => a.Name, a => a.Artist, GetComparer(mode, ignoredate));

            foreach (ITrack tr in Session.AllTracks)
            {
                AlbumDescriptor ad = AlbumDescriptor.DuplicateFromAlbum(tr.Album as Album);
                TrackDescriptor td = ad.AddExportedTrack(tr as Track, tr.Path);

                IEnumerable<Match<Track>> res = (Session as IInternalMusicSession).Tracks.Find(td);

                Assert.That(res, Is.Not.Null);
                Assert.That(res.Count(), Is.EqualTo(1));
                Match<Track> first = res.First();
                Assert.That(first.FindItem, Is.EqualTo(tr));
                Assert.That(first.Precision, Is.EqualTo(MatchPrecision.Exact));

                if (AdditionalTest)
                {
                    string op = tr.Path;
                    string pt = Path.Combine(Path.GetDirectoryName(op), Path.GetFileNameWithoutExtension(op) + "_2" + Path.GetExtension(op));
                    File.Copy(tr.Path, pt);

                    ad = AlbumDescriptor.DuplicateFromAlbum(tr.Album as Album);
                    td = ad.AddExportedTrack(tr as Track, pt);
                    res = (Session as IInternalMusicSession).Tracks.Find(td);

                    Assert.That(res, Is.Not.Null);
                    Assert.That(res.Count(), Is.EqualTo(1));
                    first = res.First();
                    Assert.That(first.FindItem, Is.EqualTo(tr));
                    Assert.That(first.Precision, Is.EqualTo(MatchPrecision.Exact));

                    File.Delete(op);

                    td = ad.AddExportedTrack(tr as Track, pt);
                    res = (Session as IInternalMusicSession).Tracks.Find(td);

                    Assert.That(res, Is.Not.Null);
                    Assert.That(res.Count(), Is.EqualTo(1));
                    first = res.First();
                    Assert.That(first.FindItem, Is.EqualTo(tr));
                    Assert.That(first.Precision, Is.EqualTo(MatchPrecision.Suspition));
                    Assert.That(tr.FileExists, Is.EqualTo(FileStatus.FileNotFoundDriverFoundFixed));
                    Assert.That(tr.State, Is.EqualTo(ObjectState.FileNotAvailable));
                    File.Move(pt, op);

                    IObjectStateCycle of = tr as IObjectStateCycle;
                    Assert.That(of.UpdatedState, Is.EqualTo(ObjectState.Available));
                }

                foreach (IAlbum al in Session.AllAlbums)
                {
                    AlbumDescriptor add = AlbumDescriptor.DuplicateFromAlbum(al as Album);

                    IEnumerable<MatchAlbum> ress = (Session as IInternalMusicSession).Albums.FindAlbums(add);

                    Assert.That(ress, Is.Not.Null);
                    Assert.That(ress.Count(), Is.EqualTo(1));
                    MatchAlbum firstt = ress.First();
                    Assert.That(firstt.FindItem, Is.EqualTo(al));
                    Assert.That(firstt.Precision, Is.EqualTo(MatchPrecision.Exact));
                    Assert.That(firstt.Way, Is.EqualTo(FindWay.ByName));

                }



            }

        }

        protected void AssertAlbumDescriptor(IFullAlbumDescriptor un, IFullAlbumDescriptor deux, AlbumDescriptorCompareMode mode, bool ignoredate = false)
        {
            Assert.That(GetComparer(mode, ignoredate).Equals(un, deux), Is.True);
        }

        protected void AssertAlbum(IAlbum un, IFullAlbumDescriptor deux, AlbumDescriptorCompareMode mode, bool ignoredate = false)
        {
            var res2 = ExportAlbum.CopyAlbum(un as Album);
            Assert.That(GetComparer(mode, ignoredate).Equals(res2, deux), Is.True);
        }

    }

    public static class AlbumsComparerExtension
    {
        public static void ShouldHaveAlbumsLike(this IMusicSession Session, IEnumerable<IFullAlbumDescriptor> deux, AlbumComparer.AlbumDescriptorCompareMode mode, bool AdditionalTest = false, bool ignoredate = false)
        {
            AlbumComparer.AssertAlbums(Session, deux, mode, AdditionalTest, ignoredate);
        }

        public static void ShouldBeCoherentWithAlbums(this IFullObservableCollection<IAlbum> albumcollection, IEnumerable<IFullAlbumDescriptor> deux)
        {
            AlbumCollection ac = albumcollection as AlbumCollection;
            foreach (IFullAlbumDescriptor all in deux)
            {
                IEnumerable<MatchAlbum> ress = ac.FindAlbums(all);
                MatchAlbum res = ress.Where(r => r.Way == FindWay.ByName).FirstOrDefault();
                res.Should().NotBeNull();
                Album al = res.FindItem;
                al.Should().NotBeNull();
                al.Name.Should().Be(all.Name);
                al.Author.Should().Be(all.Artist);

            }
        }
    }
}
