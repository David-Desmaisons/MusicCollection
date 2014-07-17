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
using MusicCollection.Utilies;
using MusicCollection.Utilies.Edition;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;


namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    public class FileStatus : IntegratedBase
    {
        public FileStatus()
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

        private IList<Track> MoveIn(MusicSessionImpl msi, int packect, bool CheckUpdateState=true)
        {
            IList<Track> res = MoveFilePacketCheckState(msi, packect, this.DirectoryIn, TempDirectoryOut, true);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.Available), Is.True);

            if (CheckUpdateState)
            {
                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.UpdatedState == ObjectState.FileNotAvailable), Is.True);
                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.FileNotAvailable), Is.True);
            }

            return res;
        }

        private void MoveOut(MusicSessionImpl msi, int packect)
        {
            MoveFilePacketCheckState(msi, packect,TempDirectoryOut, this.DirectoryIn, false);
        }

        private IList<Track> MoveFilePacketCheckState(IInternalMusicSession msi, int packect, string iDirectoryIn, string iDirectoryOut, bool FindOriginalFiles)
        {
  
            //packect
            var HalfFiles = new DirectoryInfo(iDirectoryIn).GetFiles().Where(fi => fi.Extension == FileServices.MP3).Take(packect).ToList();
            Assert.That(HalfFiles.Count, Is.EqualTo(packect));

            //string myout = this.TempDirectoryOut;

            //MusicSessionImpl msi = ms as MusicSessionImpl;

            IList<Track> res = null;

            if (FindOriginalFiles)
            {

                var InitialTrack = HalfFiles.SelectMany(fi => msi.Tracks.Find(new FakeTrackDescriptor(fi.FullName))).ToList();
                Assert.That(InitialTrack.Count, Is.EqualTo(packect));
                Assert.That(InitialTrack.All(mt => mt.Precision == MatchPrecision.Exact), Is.True);
                res = InitialTrack.Select(mt => mt.FindItem).ToList();
                Assert.That(res.Count, Is.EqualTo(packect));

                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.Available), Is.True);
                
            }

            HalfFiles.Apply(fi => fi.MoveTo(Path.Combine(iDirectoryOut, fi.Name)));

            return res;
        }

        [Test]
        public void Test()
        {
            Console.WriteLine("FileStatus");

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

                
                AssertAlbums(ms, Albums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

                IObjectState al = ms.AllAlbums[0] as IObjectStateCycle;
                Assert.That(al.State, Is.EqualTo(ObjectState.Available));
                Assert.That(al.UpdatedState, Is.EqualTo(ObjectState.Available));

                MusicSessionImpl msi = ms as MusicSessionImpl;
                IList<Track> res = MoveIn(msi, 5);
                Assert.That(res, Is.Not.Null);

                Assert.That(al.State, Is.EqualTo(ObjectState.Available));
                Assert.That(al.UpdatedState, Is.EqualTo(ObjectState.Available));

                IList<Track> res2 = MoveIn(msi, 5);
                Assert.That(res2, Is.Not.Null);

                Assert.That(al.State, Is.EqualTo(ObjectState.FileNotAvailable));
                Assert.That(al.UpdatedState, Is.EqualTo(ObjectState.FileNotAvailable));


                MoveOut(msi, 5);

                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.FileNotAvailable), Is.True);
                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.UpdatedState == ObjectState.Available), Is.True);
                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.Available), Is.True);

                Assert.That(al.State, Is.EqualTo(ObjectState.Available));
                Assert.That(al.UpdatedState, Is.EqualTo(ObjectState.Available));

                IList<Track> res3 = MoveIn(msi, 5, false);
                Assert.That(res.SequenceEqual(res3), Is.True);

                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.Available), Is.True);
                Assert.That(al.State, Is.EqualTo(ObjectState.Available));
                Assert.That(al.UpdatedState, Is.EqualTo(ObjectState.FileNotAvailable));
                Assert.That(res.Cast<IInternalTrack>().All(tr => tr.InternalState == ObjectState.FileNotAvailable), Is.True);
                Assert.That(al.State, Is.EqualTo(ObjectState.FileNotAvailable));

            }
        }
    }
}
