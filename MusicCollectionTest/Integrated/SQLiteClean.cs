using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated
{

    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    class SQLiteClean : IntegratedBase
    {
        [TearDown]
        public void TD()
        {
            base.CleanDirectories();
        }

        [SetUp]
        public void SetUp()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;


            Init();
        }

        private long _SizeBefore = 7412736;
        private long _SizeAfter = 7112704;
        private int _AlbumNumber = 2860;
        private int _ArtistNumber = 2261;
        private TimeSpan _MaxTimeToOpenBefore = TimeSpan.FromSeconds(20);
        private TimeSpan _MaxTimeToOpenAfter = TimeSpan.FromSeconds(15);

        [Test]
        public void Test()
        {
           
            

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                FileInfo fi = new FileInfo(Path.Combine(Root, "MusicCollection.db"));

                Assert.That(fi.Exists);               
                Assert.That(fi.Length, Is.EqualTo(_SizeBefore));

                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                var t = TimeTracer.TimeTrack("Load before vacuum");
                using (t)
                {
                    IMusicImporter imi = ms.GetDBImporter();
                    Assert.That(imi, Is.Not.Null);
                    imi.Load();
                }


                TimeSpan ts = (TimeSpan)t.EllapsedTimeSpent;
                Assert.That(ts, Is.LessThan(_MaxTimeToOpenBefore));


                Assert.That(ms.AllAlbums.Count, Is.EqualTo(_AlbumNumber));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(_ArtistNumber));

                Console.WriteLine("Import Successful {0} Albums",_AlbumNumber);

                SQLExecute sqe = new SQLExecute("VACUUM;", msi);
                sqe.Execute();

                fi.Refresh();
                Assert.That(fi.Exists);
                Assert.That(fi.Length, Is.EqualTo(_SizeAfter));

                Console.WriteLine("Diminution %: {0:0.00}",((_SizeBefore-_SizeAfter)*100/_SizeBefore));


                //using (ImportTransaction it = msi.GetNewSessionContext())
                //{
                //    using (IDBSession session = DBSession.CreateorGetCurrentSession(it))
                //    {
                //        //using (ITransaction trans = session.NHSession.BeginTransaction())
                //        //{
                //            RawSQLExecute sql = new RawSQLExecute("VACUUM;", session.NHSession);
                //            sql.Execute();
                //        //}
                //    }
                //}




            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                FileInfo fi = new FileInfo(Path.Combine(Root, "MusicCollection.db"));

                Assert.That(fi.Exists);
                Assert.That(fi.Length, Is.EqualTo(_SizeAfter));

                MusicSessionImpl msi = ms as MusicSessionImpl;

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                  var t = TimeTracer.TimeTrack("Load after vacuum");
                  using (t)
                  {
                      IMusicImporter imi = ms.GetDBImporter();
                      Assert.That(imi, Is.Not.Null);
                      imi.Load();
                  }


                TimeSpan ts = (TimeSpan)t.EllapsedTimeSpent;
                Assert.That(ts, Is.LessThan(_MaxTimeToOpenAfter));


                Assert.That(ms.AllAlbums.Count, Is.EqualTo(_AlbumNumber));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(_ArtistNumber));
            }

        }



    }
}
