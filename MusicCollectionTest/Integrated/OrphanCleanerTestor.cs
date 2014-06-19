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

using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Nhibernate.Session;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder("BasicSessionTest")]
    public class OrphanCleanerTestor : IntegratedBase
    {
        private ArtistComp _AC = new ArtistComp();

        private class ArtistComp : IEqualityComparer<IArtist>
        {
            bool IEqualityComparer<IArtist>.Equals(IArtist x, IArtist y)
            {
                return (x as ISessionPersistentObject).ID == (y as ISessionPersistentObject).ID;
            }

            int IEqualityComparer<IArtist>.GetHashCode(IArtist obj)
            {
                return (obj as ISessionPersistentObject).ID;
            }
        }

        public OrphanCleanerTestor()
        {
        }

        //override 

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

        protected override bool? OpenClean
        {
            get
            {
                return null;
            }
        }

        private void Excecute(IInternalMusicSession Impl, Func<ISession, bool> Ac)
        {
            using (ImportTransaction it = Impl.GetNewSessionContext())
            {
                using (IDBSession session = DBSession.CreateorGetCurrentSession(it))
                {
                    using (ITransaction trans = session.NHSession.BeginTransaction())
                    {
                        Ac(session.NHSession);
                        trans.Commit();
                    }
                }
            }
        }

        [Test]
        public void Test()
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));


                Excecute(msi, (it) =>
                    {
                        OrphanArtistDBCleaner oac = new OrphanArtistDBCleaner(it);
                        var res = oac.GetArtistsCount();
                        Assert.That(res, Is.EqualTo(0));
                        Assert.That(oac.GetArtists().Count, Is.EqualTo(0));
                        return true;
                    });

   

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

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);


                Console.WriteLine("Import Successful 5 Albums");

                Excecute(msi, (it) =>
                   {
                       //using (ImportTransaction it = msi.GetNewSessionContext())
                       //{
                       OrphanArtistDBCleaner oac = new OrphanArtistDBCleaner(it);
                       var res = oac.GetArtistsCount();
                       Assert.That(res, Is.EqualTo(0));
                       Assert.That(oac.GetArtists().Count, Is.EqualTo(0));
                       return true;
                   });

            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

                Excecute(msi, (it) =>
                  {
                      //using (ImportTransaction it = msi.GetNewSessionContext())
                      //{
                      OrphanArtistDBCleaner oac = new OrphanArtistDBCleaner(it);
                      var res = oac.GetArtistsCount();
                      Assert.That(res, Is.EqualTo(0));
                      Assert.That(oac.GetArtists().Count, Is.EqualTo(0));
                      return true;
                  }
               );

                IAlbum al = ms.AllAlbums[0];
                IList<IArtist> la = al.Artists.ToList();

                Assert.That(la.All(a => a.Albums.Count == 1));
                Assert.That(la.All(a => a.Albums[0] == al));

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));

                IMusicRemover imr = ms.GetMusicRemover();
                imr.AlbumtoRemove.Add(al);
                imr.IncludePhysicalRemove = true;
                imr.Comit(true);

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllAlbums.Contains(al), Is.False);

                Assert.That(la.All(a => a.Albums.Count == 0));

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(9));

                AssertAlbums(ms, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);

               // Excecute(msi, (it) =>
               ////using (ImportTransaction it = msi.GetNewSessionContext())
               //{
               //    OrphanArtistDBCleaner oac = new OrphanArtistDBCleaner(it);
               //    var res = oac.GetArtistsCount();
               //    Assert.That(res, Is.EqualTo(la.Count));
               //    var res2 = oac.GetArtists();
               //    Assert.That(res2.Count, Is.EqualTo(la.Count));
               //    Assert.That(res2.SequenceCompareWithoutOrder(la, _AC), Is.True);
               //    return true;
               //}
               //       );
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(9));
            }


            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                //normal load only of albums
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(9));
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                IInternalMusicSession msi = ms as IInternalMusicSession;

                using (var t = msi.GetNewSessionContext())
                {

                    using (IDBSession session = DBSession.CreateorGetCurrentSession(t))
                    {
                        GenericIntDAO<Artist> AD = new GenericIntDAO<Artist>(session.NHSession);
                        var res = AD.LoadAll();
                        //12 artist dans la base de donnee
                        Assert.That(res.Count, Is.EqualTo(9));
                    }
                }
            }



            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Excecute(msi, (it) =>
               //{
               //using (var tr = msi.GetNewSessionContext())
               {
                   OrphanArtistDBCleaner oac = new OrphanArtistDBCleaner(it);
                   oac.Clean();
                   return true;
               }
               );
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                IInternalMusicSession msi = ms as IInternalMusicSession;

                using (var t = msi.GetNewSessionContext())
                {

                    using (IDBSession session = DBSession.CreateorGetCurrentSession(t))
                    {
                        GenericIntDAO<Artist> AD = new GenericIntDAO<Artist>(session.NHSession);
                        var res = AD.LoadAll();
                        //9 artists dans la base de donnee
                        Assert.That(res.Count, Is.EqualTo(9));
                    }
                }
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(9));

                AssertAlbums(ms, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);

                IAlbum a1 =ms.AllAlbums.Where(al => al.Name=="Live - Instal Glasgow - 2006").FirstOrDefault();
                Assert.That(a1, Is.Not.Null);

                IAlbum a2 = ms.AllAlbums.Where(al => al.Name == "Behind The Night").FirstOrDefault();
                Assert.That(a2, Is.Not.Null);

                IArtist a1_tobr = ms.AllArtists.Where(ar => ar.Name == "Kuwayama").FirstOrDefault();
                Assert.That(a1_tobr, Is.Not.Null);
                Assert.That(a1.Artists.Contains(a1_tobr), Is.True);

                IArtist a2_survivor = ms.AllArtists.Where(ar => ar.Name == "Urs Leimgruber").FirstOrDefault();
                Assert.That(a2_survivor, Is.Not.Null);
                Assert.That(a2.Artists.Contains(a2_survivor), Is.True);
                Assert.That(a1.Artists.Contains(a2_survivor), Is.False);


                using (AlbumInfoEditor aie = new AlbumInfoEditor((a1 as Album).RawTracks.ToList(), ms))
                {
                    aie.Author = "Urs Leimgruber & Kijima";
                    aie.CommitChanges(true);
                }

                a1 = ms.AllAlbums.Where(al => al.Name == "Live - Instal Glasgow - 2006").FirstOrDefault();
                Assert.That(a1.Artists.Contains(a2_survivor), Is.True);
                Assert.That(a1.Artists.Contains(a1_tobr), Is.False);
                Assert.That(ms.AllArtists.Count, Is.EqualTo(8));
                Assert.That(ms.AllArtists.Contains(a1_tobr), Is.False);
                AssertAlbums(ms, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);
           



                IMusicRemover imr = ms.GetMusicRemover();
                imr.AlbumtoRemove.Add(a2);
                imr.IncludePhysicalRemove = true;
                imr.Comit(true);

                AssertAlbums(ms, OldAlbums[9], AlbumDescriptorCompareMode.AlbumandTrackMD);
                Assert.That(ms.AllArtists.Count, Is.EqualTo(4));
                Assert.That(ms.AllArtists.Contains(a2_survivor), Is.True);


    






            }
        }
    }
}
