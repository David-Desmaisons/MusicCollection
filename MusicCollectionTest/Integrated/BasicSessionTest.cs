using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using NUnit;
using NUnit.Framework;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

using FluentAssertions;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class BasicSessionTest : IntegratedBase
    {
        public BasicSessionTest()
        {

        }

        [TearDown]
        public void TD()
        {
            base.CleanDirectories();
        }



        [SetUp]
        public void SetUp()
        {
            Init();
        }

        private void ImportError(object sender, ImportExportErrorEventArgs e)
        {
            OtherAlbumsConfirmationNeededEventArgs oa = e as OtherAlbumsConfirmationNeededEventArgs;
            if (oa!=null)
                oa.Continue = true;
        }

        [Test]
        public void Test()
        {

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                foreach (IGenre genreso in ms.AllGenres)
                {
                    genreso.Albums.Should().BeEmpty();
                    foreach (IGenre genresm in ms.AllGenres)
                    {
                        int res = genreso.Compare(genresm);
                        Console.WriteLine("{0} Compare to {1}: {2}", genreso, genresm, res);
                        if (genreso == genresm)
                        {
                            Assert.That(res, Is.EqualTo(0));
                        }
                    }
                }

                IGenre monde = ms.AllGenres.FirstOrDefault(g => g.Name == "Monde");
                monde.Should().NotBeNull();

                monde.SubGenres.Should().NotBeEmpty();

                foreach (IGenre sub in monde.SubGenres)
                {
                    sub.Father.Should().Be(monde);
                    sub.Compare(monde).Should().NotBe(int.MaxValue);
                }

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));

                AssertEnumerable(from a in ms.AllAlbums select a.Name, from a in OldAlbums[0] select a.Name, n => n);
                AssertEnumerable(from a in ms.AllAlbums select (a.Genre), from a in OldAlbums[0] select a.Genre, n => n);
                AssertEnumerable(from a in ms.AllAlbums select a.Year, from a in OldAlbums[0] select a.Year, n => n);

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);


                Console.WriteLine("Import Successful 5 Albums");

            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));

                AssertEnumerable(from a in ms.AllAlbums select a.Name, from a in OldAlbums[0] select a.Name, n => n);
                AssertEnumerable(from a in ms.AllAlbums select (a.Genre), from a in OldAlbums[0] select a.Genre, n => n);
                AssertEnumerable(from a in ms.AllAlbums select a.Year, from a in OldAlbums[0] select a.Year, n => n);

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

                IAlbum res = ms.AllAlbums.FirstOrDefault(a => a.Name == OldAlbums[0][0].Name);
                Assert.That(res, Is.Not.Null);
                ITrack tr = res.Tracks[0];
                Assert.That(tr, Is.Not.Null);
                Assert.That(tr.Rating, Is.EqualTo(0));
                tr.Rating = 5;
                Assert.That(tr.Rating, Is.EqualTo(5));

                Console.WriteLine("Albums restored");
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

                AlbumCollection ac = (ms as IInternalMusicSession).Albums;
                IEnumerable<MatchAlbum> res0 = ac.FindAlbums(OldAlbums[0][0]);

                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma = res0.First();
                Assert.That(ma, Is.Not.Null);
                Assert.That(ma.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma.Precision, Is.EqualTo(MatchPrecision.Exact));

                IAlbum res = ms.AllAlbums.FirstOrDefault(a => a.Name == OldAlbums[0][0].Name);
                Assert.That(res, Is.EqualTo(ma.FindItem));
                Assert.That(res, Is.Not.Null);
                ITrack tr = res.Tracks[0];
                Assert.That(tr.Rating, Is.EqualTo(5));

                IModifiableAlbum ima = res.GetModifiableAlbum();
                ima.Artists.Clear();
                ima.Artists.Add(ms.CreateArtist("C"));
                //ima.Author = "C";
                ima.Name = "C";
                Assert.That(res.Name, Is.Not.EqualTo("C"));
                Assert.That(res.Author, Is.Not.EqualTo("C"));
                ima.Commit(true);

                Assert.That(res.Name, Is.EqualTo("C"));
                Assert.That(res.Author, Is.EqualTo("C"));

                res0 = ac.FindAlbums(OldAlbums[0][0]);
                Assert.That(res0.Any(), Is.False);

                res0 = ac.FindAlbums(OldAlbums[1][0]);
                Assert.That(res0.Any(), Is.True);

                ma = res0.First();
                Assert.That(ma, Is.Not.Null);
                Assert.That(ma.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma.Precision, Is.EqualTo(MatchPrecision.Exact));
                Assert.That(res, Is.EqualTo(ma.FindItem));


                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumandTrackMD);
                ms.AllAlbums.ShouldBeCoherentWithAlbums(OldAlbums[1]);


                Console.WriteLine("Albums restored");
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumandTrackMD);

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(10));

                AlbumCollection ac = (ms as IInternalMusicSession).Albums;
                IEnumerable<MatchAlbum> res0 = ac.FindAlbums(OldAlbums[0][1]);

                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma = res0.First();
                Assert.That(ma, Is.Not.Null);
                Assert.That(ma.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma.Precision, Is.EqualTo(MatchPrecision.Exact));


                res0 = ac.FindAlbums(OldAlbums[1][0]);
                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));
                Assert.That(ma2.FindItem.Name, Is.EqualTo("C"));
                Assert.That(ma2.FindItem.Author, Is.EqualTo("C"));
                IAlbum album = ma2.FindItem;
                Assert.That(album.Tracks.Count, Is.EqualTo(1));

                IModifiableAlbum ima = (ma.FindItem as IAlbum).GetModifiableAlbum();
                SmartEventListener sel = new SmartEventListener();
                sel.SetExpectation(new OtherAlbumConfirmationNeededEventArgs(album), a => a.Continue = true);
                ima.Error += sel.Listener;
                ima.Artists.Clear();
                ima.Artists.Add(ms.CreateArtist("C"));
                //ima.Author = "C";
                ima.Name = "C";
                Assert.That(ma.FindItem.Name, Is.Not.EqualTo("C"));
                Assert.That(ma.FindItem.Author, Is.Not.EqualTo("C"));
                ima.Commit(true);

                Assert.That((ma.FindItem as IObjectState).State, Is.EqualTo(ObjectState.Removed));
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));


                Assert.That(sel.IsOk, Is.True);


                ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));
                Assert.That(ma2.FindItem.Name, Is.EqualTo("C"));
                Assert.That(ma2.FindItem.Author, Is.EqualTo("C"));
                album = ma2.FindItem;
                Assert.That(album.Tracks.Count, Is.EqualTo(2));

                AssertAlbums(ms, OldAlbums[2], AlbumDescriptorCompareMode.AlbumandTrackMD);
                ms.AllAlbums.ShouldBeCoherentWithAlbums(OldAlbums[2]);


                Console.WriteLine("Albums restored");
            }

            string toberemovedlater;

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[2], AlbumDescriptorCompareMode.AlbumandTrackMD);

                AlbumCollection ac = (ms as IInternalMusicSession).Albums;
                IEnumerable<MatchAlbum> res0 = ac.FindAlbums(OldAlbums[2][1]);
                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));

                toberemovedlater = ma2.FindItem.Tracks[0].Path;
                Assert.That(File.Exists(toberemovedlater), Is.True);

                _SK.Settings.CollectionFileSettings.DeleteRemovedFile = BasicBehaviour.No;
                IMusicRemover imr = ms.GetMusicRemover();
                Assert.That(imr, Is.Not.Null);
                imr.AlbumtoRemove.Add(ma2.FindItem);
                imr.Comit(true);

                Assert.That((ma2.FindItem as IObjectState).State, Is.EqualTo(ObjectState.Removed));
                Assert.That(File.Exists(toberemovedlater), Is.True);
                AssertAlbums(ms, OldAlbums[3], AlbumDescriptorCompareMode.AlbumandTrackMD);

                Console.WriteLine("Reimport Files");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Console.WriteLine("check that files correctly re-imported");
                AssertAlbums(ms, OldAlbums[2], AlbumDescriptorCompareMode.AlbumandTrackMD);


                Console.WriteLine("remove again");
                ac = (ms as IInternalMusicSession).Albums;
                res0 = ac.FindAlbums(OldAlbums[2][1]);
                Assert.That(res0.Any(), Is.True);
                ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));

                toberemovedlater = ma2.FindItem.Tracks[0].Path;
                Assert.That(File.Exists(toberemovedlater), Is.True);

                _SK.Settings.CollectionFileSettings.DeleteRemovedFile = BasicBehaviour.No;
                imr = ms.GetMusicRemover();
                Assert.That(imr, Is.Not.Null);
                imr.AlbumtoRemove.Add(ma2.FindItem);
                imr.Comit(true);

                Assert.That((ma2.FindItem as IObjectState).State, Is.EqualTo(ObjectState.Removed));
                Assert.That(File.Exists(toberemovedlater), Is.True);
                AssertAlbums(ms, OldAlbums[3], AlbumDescriptorCompareMode.AlbumandTrackMD);




            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[3], AlbumDescriptorCompareMode.AlbumandTrackMD);


                AlbumCollection ac = (ms as IInternalMusicSession).Albums;
                IEnumerable<MatchAlbum> res0 = ac.FindAlbums(OldAlbums[3][1]);
                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));

                string ipath = ma2.FindItem.Tracks[0].Path;
                Assert.That(File.Exists(ipath), Is.True);

                _SK.Settings.CollectionFileSettings.DeleteRemovedFile = BasicBehaviour.Yes;
                IMusicRemover imr = ms.GetMusicRemover();
                Assert.That(imr, Is.Not.Null);
                imr.AlbumtoRemove.Add(ma2.FindItem);
                imr.Comit(true);

                Assert.That((ma2.FindItem as IObjectState).State, Is.EqualTo(ObjectState.Removed));
                Assert.That(File.Exists(ipath), Is.False);
                AssertAlbums(ms, OldAlbums[4], AlbumDescriptorCompareMode.AlbumandTrackMD);
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[4], AlbumDescriptorCompareMode.AlbumandTrackMD);

                AlbumCollection ac = (ms as IInternalMusicSession).Albums;
                IEnumerable<MatchAlbum> res0 = ac.FindAlbums(OldAlbums[4][0]);
                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma2 = res0.First();
                Assert.That(ma2, Is.Not.Null);
                Assert.That(ma2.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma2.Precision, Is.EqualTo(MatchPrecision.Exact));

                res0 = ac.FindAlbums(OldAlbums[4][1]);
                Assert.That(res0.Any(), Is.True);
                MatchAlbum ma3 = res0.First();
                Assert.That(ma3, Is.Not.Null);
                Assert.That(ma3.Way, Is.EqualTo(FindWay.ByName));
                Assert.That(ma3.Precision, Is.EqualTo(MatchPrecision.Exact));
                Album al = ma3.FindItem;

                IModifiableAlbum ima = (ma2.FindItem as IAlbum).GetModifiableAlbum();
                ima.MergeFromMetaData(OldAlbums[4][1], ms.Strategy.OnlyIfDummy);
                ima.Commit(true);

                AssertAlbums(ms, OldAlbums[4], AlbumDescriptorCompareMode.AlbumandTrackMD);

                ima = (ma2.FindItem as IAlbum).GetModifiableAlbum();
                ima.MergeFromMetaData(OldAlbums[4][1], ms.Strategy.Get(IndividualMergeStategy.Always, IndividualMergeStategy.Always, IndividualMergeStategy.Always));


                SmartEventListener sel = new SmartEventListener();
                sel.SetExpectation(new OtherAlbumConfirmationNeededEventArgs(al), a => a.Continue = true);
                ima.Error += sel.Listener;

                ima.Commit(true);

                Assert.That(sel.IsOk, Is.True);

                AssertAlbums(ms, OldAlbums[5], AlbumDescriptorCompareMode.AlbumandTrackMD);
                ms.AllAlbums.ShouldBeCoherentWithAlbums(OldAlbums[5]);

            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[5], AlbumDescriptorCompareMode.AlbumandTrackMD);

                _SK.Settings.CollectionFileSettings.DeleteRemovedFile = BasicBehaviour.No;
                IMusicRemover imr = ms.GetMusicRemover();
                Assert.That(imr, Is.Not.Null);
                imr.AlbumtoRemove.AddCollection(ms.AllAlbums);
                imr.Comit(true);

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
            }

            File.Delete(toberemovedlater);

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Cheking Persistency");
                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[5], AlbumDescriptorCompareMode.AlbumandTrackMD);

                Console.WriteLine("Testing track remover");

                ITrack wbrd = ms.AllTracks.Where(tr => tr.Name == "One To Grow On").FirstOrDefault();
                Assert.That(wbrd, Is.Not.Null);

                IMusicRemover imr = ms.GetMusicRemover();
                imr.IncludePhysicalRemove = false;
                imr.TrackRemove.Add(wbrd);
                Assert.That(imr.TrackRemove.Count, Is.EqualTo(1));
                imr.Comit(true);


                Assert.That(wbrd.State, Is.EqualTo(ObjectState.Removed));
                string mp = wbrd.Path;

                Assert.That(File.Exists(mp), Is.True);
                AssertAlbums(ms, OldAlbums[6], AlbumDescriptorCompareMode.AlbumandTrackMD);
                Assert.That(ms.AllTracks.Count, Is.EqualTo(2));
 

                Console.WriteLine("Testing re-import");
                imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Error += ImportError;
                imi.Load();
                imi.Error -= ImportError;


                AssertAlbums(ms, OldAlbums[5], AlbumDescriptorCompareMode.AlbumandTrackMD);


                wbrd = ms.AllTracks.Where(tr => tr.Name == "One To Grow On").FirstOrDefault();
                Assert.That(wbrd, Is.Not.Null);
                Track twbrd = wbrd as Track;

                string newDir = TempDirectoryIn;
                //Path.Combine(_SK.InPath, MYIn, "TempDir");
                //Directory.CreateDirectory(newDir);

                string oldpath = wbrd.Path;

                IInternalMusicSession mss = ms as IInternalMusicSession;

                using (IImportContext Context = mss.GetNewSessionContext())
                {
                    using (IMusicTransaction imut = Context.CreateTransaction())
                    {
                        Context.AddForUpdate(twbrd);
                        //IMaturityUserSettings imm = Context.MaturityUserSettings;

                        bool OK = twbrd.ReRoot(newDir, Context);
                        imut.Commit();
                    }
                }

                string newpath = wbrd.Path;

                AssertAlbums(ms, OldAlbums[5], AlbumDescriptorCompareMode.AlbumandTrackMD);
                Assert.That(File.Exists(oldpath),Is.False);
                Assert.That(newpath.StartsWith(newDir), Is.False);

                DirectoryInfo di = new DirectoryInfo(newDir);
                FileInfo[] fis=di.GetFiles();

                Assert.That(fis, Is.Not.Null);
                Assert.That(fis.Length, Is.EqualTo(1));

                FileInfo fis0 = fis[0];
                Assert.That(fis0, Is.Not.Null);
                Assert.That(fis0.FullName.ToLower(), Is.EqualTo(newpath));

                    

            }
        }
    }


}
