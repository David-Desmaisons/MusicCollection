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
using FluentAssertions;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Utilies.Edition;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class AlbumModifierTest : IntegratedBase
    {
        public AlbumModifierTest()
        {

        }

        [TestFixtureTearDown]
        public void TDO()
        {
            base.CleanDirectories();
        }

        [TearDown]
        public void TD()
        {
            base.BigClean();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            Init();
        }


        private void Tester(int IndexCompare, Func<IMusicSession, IList<Track>> Trackers, Func<AlbumInfoEditor, IMusicSession, IProgress<ImportExportError>> Initializer)
            //, Action<IList<Track>,IMusicSession> F)
        {
            TesterBasic(IndexCompare,

                (ms) =>
                {
                    using (AlbumInfoEditor aie = new AlbumInfoEditor(Trackers(ms), ms))
                    {
                        IProgress<ImportExportError> res = Initializer(aie, ms);
                        aie.Commit(res);
                        //aie.CommitChanges(true);
                    }
                }

                );
        }

        private void TesterSingle(int IndexCompare, Func<IMusicSession, Track> Tracker, Action<SingleTrackUpdater, IMusicSession> Initializer)
        //, Action<IList<Track>,IMusicSession> F)
        {
            TesterBasic(IndexCompare,

                (ms) =>
                {
                    using (SingleTrackUpdater aie = new SingleTrackUpdater(Tracker(ms), ms))
                    {
                        Initializer(aie, ms);
                        aie.Commit();
                    }
                }

                );
        }




//, Func<IMusicSession, IList<Track>> Trackers, Action<AlbumInfoEditor, IMusicSession> Initializer, 
        private void TesterBasic(int IndexCompare,Action<IMusicSession> F)
        {



            //Load and Change
            Console.WriteLine("Load and Change");
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

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);
                //etat initial correct maintenant on va modifier

                F(ms);

                //using (AlbumInfoEditor aie = new AlbumInfoEditor(Trackers(ms), ms))
                //{
                //    Initializer(aie, ms);
                //    aie.CommitChanges();
                //}


                AssertAlbums(ms, OldAlbums[IndexCompare], AlbumDescriptorCompareMode.AlbumandTrackMD);
            }
            Console.WriteLine("In Session sucessfull");


            //Reload and clean
            Console.WriteLine("ReOpen and Clean");
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[IndexCompare], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[IndexCompare], AlbumDescriptorCompareMode.AlbumandTrackMD);


                IMusicRemover imr = ms.GetMusicRemover();
                imr.IncludePhysicalRemove = false;
                imr.AlbumtoRemove.AddCollection(ms.AllAlbums);
                imr.Comit(true);

                ms.AllGenres.Apply(g=> g.Albums.Should().BeEmpty());

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
            }
            Console.WriteLine("Reopen sucessfull");


            //Reimport
            Console.WriteLine("ReImport and test");
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

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[IndexCompare], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[IndexCompare], AlbumDescriptorCompareMode.AlbumandTrackMD,true);
            }
            Console.WriteLine("Reimport sucessfull");
        }



        [Test]
        public void Test()
        {
            Console.WriteLine("AlbumInfoEditor tester #1");

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

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(7));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(13));

                AssertEnumerable(from a in ms.AllAlbums select a.Name, from a in OldAlbums[0] select a.Name, n => n);
                AssertEnumerable(from a in ms.AllAlbums select (a.MainGenre == null ? null : a.MainGenre.FullName), from a in OldAlbums[0] select a.Genre, n => n);
                AssertEnumerable(from a in ms.AllAlbums select a.Year, from a in OldAlbums[0] select a.Year, n => n);

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);


                Console.WriteLine("Import Successful 7 Albums(8 Tracks)");

                IList<Artist> ar = Artist.GetArtistFromName("Toto", ms as MusicSessionImpl).ToList();

                IList<AlbumTarget> AlbumTargets = AlbumTarget.FromListAndTargets(ms.AllTracks.Cast<Track>().ToList(), ar, "Africa");

                Assert.That(AlbumTargets.Count, Is.EqualTo(1));

                Assert.That(AlbumTargets[0].AlbumName, Is.EqualTo("Africa"));
                Assert.That(AlbumTargets[0].Artists.SequenceEqual(ar), Is.True);

                Assert.That(AlbumTargets[0].OrderedTrack.Count, Is.EqualTo(ms.AllAlbums.Count));
                Assert.That(AlbumTargets[0].OrderedTrack.Select(ot => ot.OriginAlbum).OrderBy(a => a.Name).SequenceEqual(ms.AllAlbums.OrderBy(a => a.Name)));
                Assert.That(AlbumTargets[0].OrderedTrack.SelectMany(ot => ot.Tracks).Count(), Is.EqualTo(ms.AllTracks.Count()));

                Assert.That(AlbumTargets.All(e => e.OrderedTrack.All(at => at.Complete)), Is.True);

                IList<ITrack> atrs = ms.AllTracks;

                List<Track> trs = new List<Track>();
                trs.Add(atrs[0] as Track);
                trs.Add(atrs[1] as Track);

                AlbumTargets = AlbumTarget.FromListAndTargets(trs, ar, null);

                Assert.That(AlbumTargets.Count, Is.EqualTo(2));
                Assert.That(AlbumTargets.All(at => at.Artists.SequenceEqual(ar)), Is.True);
                Assert.That(AlbumTargets.SelectMany(at => at.OrderedTrack.SelectMany(ot => ot.Tracks)).Count(), Is.EqualTo(2));



                AlbumTargets = AlbumTarget.FromListAndTargets(trs, null, "Afreica");

                Assert.That(AlbumTargets.Count, Is.EqualTo(2));
                Assert.That(AlbumTargets.All(at => at.AlbumName == "Afreica"), Is.True);
                Assert.That(AlbumTargets.SelectMany(at => at.OrderedTrack.SelectMany(ot => ot.Tracks)).Count(), Is.EqualTo(2));
                Assert.That(AlbumTargets.All(e => e.OrderedTrack.All(at => at.Complete)), Is.True);

                var arc = Artist.GetArtistFromName("Los Hombres Calientes", ms as MusicSessionImpl).ToList();

                var resartcalien = ms.AllAlbums.Where(a => a.Artists.Any(art => art.Name == "Los Hombres Calientes")).Select(a => a);
                var resnamecalien = ms.AllAlbums.Where(a => a.Name == "LOS HOMBRES CALIENTES VOL. IV").Select(a => a);

                AlbumTargets = AlbumTarget.FromListAndTargets(resartcalien.SelectMany(a => (a as Album).RawTracks).ToList(), null, "la mano nell luco");

                Assert.That(AlbumTargets.Count, Is.EqualTo(1));
                Assert.That(AlbumTargets[0].AlbumName, Is.EqualTo("la mano nell luco"));
                Assert.That(AlbumTargets[0].Artists.SequenceEqual(arc), Is.True);
                Assert.That(AlbumTargets[0].OrderedTrack.Count, Is.EqualTo(2));
                Assert.That(AlbumTargets.SelectMany(at => at.OrderedTrack.SelectMany(ot => ot.Tracks)).Count(), Is.EqualTo(3));
                Assert.That(AlbumTargets.All(e => e.OrderedTrack.All(at => at.Complete)), Is.True);

                var arf = Artist.GetArtistFromName("Des Menches Gefriezert", ms as MusicSessionImpl).ToList();


                AlbumTargets = AlbumTarget.FromListAndTargets(resnamecalien.SelectMany(a => (a as Album).RawTracks).ToList(), arf, null);

                Assert.That(AlbumTargets.Count, Is.EqualTo(1));
                Assert.That(AlbumTargets[0].AlbumName, Is.EqualTo("LOS HOMBRES CALIENTES VOL. IV"));
                Assert.That(AlbumTargets[0].Artists.SequenceEqual(arf), Is.True);
                Assert.That(AlbumTargets[0].OrderedTrack.Count, Is.EqualTo(2));
                Assert.That(AlbumTargets.SelectMany(at => at.OrderedTrack.SelectMany(ot => ot.Tracks)).Count(), Is.EqualTo(3));
                Assert.That(AlbumTargets.All(e => e.OrderedTrack.All(at => at.Complete)), Is.True);

                var unic = ms.AllAlbums.Where(a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "LOS HOMBRES CALIENTES VOL. IV").Select(a => a);

                Assert.That(unic.Count(), Is.EqualTo(1));

                Album all = unic.First() as Album;
                Assert.That(all, Is.Not.Null);
                Assert.That(all.RawTracks.Count(), Is.EqualTo(2));


                AlbumTargets = AlbumTarget.FromListAndTargets(all.RawTracks.ToList()[0].SingleItemCollection().ToList(), arf, "toto");

                Assert.That(AlbumTargets.Count, Is.EqualTo(1));
                Assert.That(AlbumTargets[0].AlbumName, Is.EqualTo("toto"));
                Assert.That(AlbumTargets[0].Artists.SequenceEqual(arf), Is.True);
                Assert.That(AlbumTargets[0].OrderedTrack.Count, Is.EqualTo(1));
                Assert.That(AlbumTargets.SelectMany(at => at.OrderedTrack.SelectMany(ot => ot.Tracks)).Count(), Is.EqualTo(1));
                Assert.That(AlbumTargets[0].OrderedTrack[0].Complete, Is.False);


                using (AlbumInfoEditor aie = new AlbumInfoEditor(all.RawTracks.ToList()[0].SingleItemCollection().ToList(), ms))
                {
                    aie.AlbumName = "toto";
                    aie.Author = Artist.AuthorName( arf);
                    aie.Commit();
                    //aie.CommitChanges(true);
                }


                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumandTrackMD, true);





                //<Artist>Los Hombres Calientes</Artist>
                //<Name>LOS HOMBRES CALIENTES VOL. IV</Name>




            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumandTrackMD);


                IMusicRemover imr = ms.GetMusicRemover();
                imr.IncludePhysicalRemove = false;
                imr.AlbumtoRemove.AddCollection(ms.AllAlbums);
                imr.Comit(true);

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));

            }

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

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumMD);
                AssertAlbums(ms, OldAlbums[1], AlbumDescriptorCompareMode.AlbumandTrackMD, true);

            }
        }




    //    <ExportAlbum>
    //  <Artist>Titi</Artist>
    //  <Name></Name>
    //  <Year>2005</Year>
    //  <Genre>Blues</Genre>
    //  <Tracks>
    //    <ExportTrack>
    //      <TrackNumber>2</TrackNumber>
    //      <Name>ooooo</Name>
    //    </ExportTrack>
    //  </Tracks>
    //</ExportAlbum>

        [Test]
        public void SingleUpdate()
        {
            TesterSingle(12, ms => (ms.AllAlbums.Where((a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "LOS HOMBRES CALIENTES VOL. IV")).First() as Album).RawTracks.Where(t => t.TrackNumber == 27).First(),
                (aie, ms) => { aie.AlbumName = "Gros Minet"; aie.Author = "Titi"; aie.Genre = "Blues"; aie.Year = 2005; aie.Name = "ooooo"; aie.TrackNumber = 2; });
        }

        
      //<ExportAlbum>
      //  <Artist>Los Hombres Calientes</Artist>
      //  <Name>Uni</Name>
      //  <Year>2008</Year>
      //  <Genre>Outer</Genre>
      //  <Tracks>
      //    <ExportTrack>
      //      <TrackNumber>1</TrackNumber>
      //      <Name>Enigma</Name>
      //    </ExportTrack>
      //  </Tracks>
      //</ExportAlbum>

        [Test]
        public void SingleUpdate1()
        {
            TesterSingle(14, ms => (ms.AllAlbums.Where((a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "Uni")).First() as Album).RawTracks.First(),
                (aie, ms) => { aie.Author = "Titi"; aie.Genre = "Blues";});
        }


        [Test]
        public void SingleUpdate2()
        {
            TesterSingle(13, ms => (ms.AllAlbums.Where((a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "LOS HOMBRES CALIENTES VOL. IV")).First() as Album).RawTracks.Where(t => t.TrackNumber == 27).First(),
                (aie, ms) => { aie.Name = "ooooo"; aie.TrackNumber = 2; });
        }

        [Test]
        public void SingleUpdate3()
        {
            TesterSingle(0, ms => (ms.AllAlbums.Where((a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "LOS HOMBRES CALIENTES VOL. IV")).First() as Album).RawTracks.Where(t => t.TrackNumber == 27).First(),
                (aie, ms) => { aie.Genre = "Blues"; });
        }


        [Test]
        public void Test2()
        {
            Tester(2, ms => ms.AllAlbums.SelectMany(a => (a as Album).RawTracks).ToList(),
                (aie, ms) => { aie.AlbumName = "One"; aie.Author = "The One"; aie.Genre = "Rock"; aie.Year = 10; return null; });
        }

        [Test]
        public void Test7()
        {
            Tester(7, ms => ms.AllAlbums.SelectMany(a => (a as Album).RawTracks).ToList(),
                (aie, ms) => 
                { 
                    aie.Genre = "Rock";
                    return null; 
                });
        }

        [Test]
        public void Test3()
        {
            Tester(3, ms => ms.AllAlbums.Where(a => a.Artists.Any(art => art.Name == "Los Hombres Calientes")).SelectMany(a => (a as Album).RawTracks).ToList(),
                 (aie, ms) => { aie.AlbumName = "New album"; aie.Genre = "Blues"; aie.Year = 2010; return null; });
        }

        [Test]
        public void Test4()
        {
            Tester(4, ms => ms.AllAlbums.Where(a => a.Artists.Any(art => art.Name == "Los Hombres Calientes")).SelectMany(a => (a as Album).RawTracks).ToList(),
                 (aie, ms) => { aie.Author = "Kraftwerk"; return null; });
        }

        [Test]
        public void Test5()
        {
            Tester(5, ms => ms.AllAlbums.Where(a => a.Artists.Any(art => art.Name == "Los Hombres Calientes")).SelectMany(a => (a as Album).RawTracks).ToList(),
                     (aie, ms) => { aie.Author = "Kraftwerk"; aie.Genre = "Blues"; return null; });

        }

        [Test]
        public void Test6()
        {
            Tester(6, ms => ms.AllAlbums.Where((a => a.Name == "LOS HOMBRES CALIENTES VOL. IV")).SelectMany(a => (a as Album).RawTracks).ToList(),
                        (aie, ms) => { aie.Author = "Kraftwerk"; aie.Genre = "Impro"; aie.Year = 1975; return null; });

        }

        [Test]
        public void Test8()
        {
            Tester(0, ms => (ms.AllAlbums.Where((a => a.Artists.Any(art => art.Name == "Los Hombres Calientes") && a.Name == "LOS HOMBRES CALIENTES VOL. IV")).First() as Album).RawTracks.First().SingleItemCollection().ToList(),
                        (aie, ms) => { aie.Genre = "Dub"; return null; });

        }

        [Test]
        public void Testa()
        {
            Tester(9, ms => ms.AllAlbums.Where((a => a.Name == "LOS HOMBRES CALIENTES VOL. IV")).SelectMany(a => (a as Album).RawTracks).ToList(),
                                (aie, ms) => { aie.Author = "Los Hombres Calientes"; return null; });

        }

        [Test]
        public void Test0()
        {

            bool needtc = false;
            Tester(11, ms => ms.AllAlbums.Where(a => a.Name == "LOS HOMBRES CALIENTES VOL. IV").Cast<Album>().SelectMany(at => at.RawTracks).ToList(),
                                (aie, ms) =>
                                {
                                    aie.Author = "Los Hombres Calientes";
                                    aie.AlbumName = "LOS HOMBRES CALIENTES VOL. IV";
                                    aie.Year = 2010;
                                    aie.Genre = "Blues";

                                    return new WPFSynchroneProgress<ImportExportError>
                                    ( (e) =>
                                        {
                                            OtherAlbumsConfirmationNeededEventArgs oo = e as OtherAlbumsConfirmationNeededEventArgs;
                                            if (oo != null)
                                            {
                                                needtc = true;
                                                oo.Continue = false;
                                            }
                                        });


                                });

            Assert.That(needtc, Is.False);

        }

        [Test]
        public void Test10()
        {

            bool needtc = false;
            Tester(10, ms => (ms.AllAlbums.Where(a => a.Name == "Field Recordings Vol. 1 The Birthday").First() as Album).RawTracks.ToList(),
                                (aie, ms) => {
                                    aie.Author = "Los Hombres Calientes";
                                     aie.AlbumName = "LOS HOMBRES CALIENTES VOL. IV";

                                     return new WPFSynchroneProgress<ImportExportError>
                                  ((e) =>
                                  {
                                      OtherAlbumsConfirmationNeededEventArgs oo = e as OtherAlbumsConfirmationNeededEventArgs;
                                      if (oo != null)
                                      {
                                          needtc = true;
                                          oo.Continue = true;
                                      }
                                  });

                                    //aie.Error += (o, e) => { OtherAlbumsConfirmationNeededEventArgs oo = e as OtherAlbumsConfirmationNeededEventArgs;
                                    //if (oo != null)
                                    //{
                                    //    needtc = true;
                                    //    oo.Continue = true;
                                    //}

                                    //};
                                
                                
                                });

            Assert.That(needtc, Is.True);

        }

        [Test]
        public void Testa00()
        {
            Tester(9, ms => ms.AllAlbums.Where((a => a.Name == "LOS HOMBRES CALIENTES VOL. IV")).SelectMany(a => (a as Album).RawTracks).ToList(),
                                (aie, ms) => { aie.Author = "Los Hombres Calientes"; return null; });

        }

        [Test]
        public void Testa0()
        {
            Tester(0, ms => (ms.AllAlbums.Where(a => a.Name == "Field Recordings Vol. 1 The Birthday").First() as Album).RawTracks.ToList(),
                                (aie, ms) => { aie.Author = "Los Hombres Calientes"; aie.AlbumName = "LOS HOMBRES CALIENTES VOL. IV"; return null; });

        }





    }
}
