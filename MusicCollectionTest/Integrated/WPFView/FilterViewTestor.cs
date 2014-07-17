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

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModel.Filter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated.WPFView
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder("BasicSessionTest")]
    public class FilterViewTestor : IntegratedBase
    {
        private IMusicSession _IMS;
        private FilterView _FV;
        private IExtendedObservableCollection<IAlbum> _Collection;

        [SetUp]
        public void TD()
        {
            base.Init();
            using (TimeTracer.TimeTrack("Create session"))
            {
                _IMS = MusicSessionImpl.GetSession(_SK.Builder);
            }
            _FV = new FilterView(_IMS);
            _Collection = _IMS.AllAlbums.LiveWhere(_FV.FilterAlbum);
        }

        [TearDown]
        public void SetUp()
        {
            _Collection.Dispose();
            _Collection = null;
            _IMS.Dispose();
            base.CleanDirectories();
        }

        private IEnumerable<IArtist> GetFromName(string n)
        {
            return _IMS.AllArtists.Where(a => a.Name.Normalized().Contains(n));
        }

        private IEnumerable<IAlbum> GetFromAllName(string n)
        {
            return _IMS.AllAlbums.Where(a => a.Name.Normalized().Contains(n));
        }

        [Test]
        public void Test()
        {
            Assert.That(_Collection.Count, Is.EqualTo(0));

            IMusicImporter imi = _IMS.GetDBImporter();
            Assert.That(imi, Is.Not.Null);
            imi.Load();
            Assert.That(_IMS.AllAlbums.Count, Is.EqualTo(0));
            Assert.That(_IMS.AllGenres.Count, Is.EqualTo(25));
            Assert.That(_IMS.AllArtists.Count, Is.EqualTo(0));

            Assert.That(_Collection.Count, Is.EqualTo(0));

            IDirectoryImporterBuilder imib = _IMS.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
            Assert.That(imib, Is.Not.Null);
            imib.Directory = DirectoryIn;
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

            Assert.That(imib.IsValid, Is.True);
            imi = imib.BuildImporter();
            Assert.That(imi, Is.Not.Null);
            imi.Load();

            AssertAlbums(_IMS, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);
            AssertAlbums(_Collection, OldAlbums[0], AlbumDescriptorCompareMode.AlbumandTrackMD);

            Assert.That(_Collection.Count, Is.EqualTo(5));


            var ar = this.GetFromName("kelley");
            Assert.That(ar.Count(), Is.EqualTo(1));
            IArtist art = ar.First();

            var al = art.Albums;
            Assert.That(al.Count(), Is.EqualTo(1));
            IAlbum all = al.First();

            ////_FV.FilteringEntity = FilterType.All;
            //_FV.SetFilterAll("kelley");

            //AssertAlbums(_Collection, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);
            //Assert.That(_Collection.Count, Is.EqualTo(1));
            //Assert.That(_Collection[0], Is.EqualTo(all));

            //_FV.SetFilterAll("kelleyki");
            //Assert.That(_Collection.Count, Is.EqualTo(0));

            //_FV.SetFilterAll( "kelley");
            //AssertAlbums(_Collection, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);

            //_FV.SetFilterAll("Instal Glasgow");
            //AssertAlbums(_Collection, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);


            //_FV.SetFilterAll("kelley");
            //AssertAlbums(_Collection, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);



            using (AlbumInfoEditor aie = new AlbumInfoEditor((all as Album).RawTracks.ToList(), this._IMS))
            {
                aie.Author = "toto";
                aie.Commit();
                //aie.CommitChanges(true);
            }

            //AssertAlbums(_IMS, OldAlbums[9], AlbumDescriptorCompareMode.AlbumandTrackMD);

            //Assert.That(_Collection.Count, Is.EqualTo(0));

            //_FV.SetFilterAll( "el");
            //AssertAlbums(_Collection, OldAlbums[10], AlbumDescriptorCompareMode.AlbumandTrackMD);

            var alls = GetFromAllName("el");
            Assert.That(alls.Count(), Is.EqualTo(1));
            IAlbum myal = alls.First();
            Assert.That(myal.Name, Is.EqualTo("Field Recordings Vol. 1 The Birthday"));

            using (AlbumInfoEditor aie = new AlbumInfoEditor((myal as Album).RawTracks.ToList(), this._IMS))
            {
                aie.AlbumName = "toto";
                aie.Commit();
                //aie.CommitChanges(true);
            }

            //AssertAlbums(_IMS, OldAlbums[14], AlbumDescriptorCompareMode.AlbumandTrackMD);
            //AssertAlbums(_Collection, OldAlbums[11], AlbumDescriptorCompareMode.AlbumandTrackMD);


            //_FV.SetFilterAll("toto");
            //AssertAlbums(_Collection, OldAlbums[12], AlbumDescriptorCompareMode.AlbumandTrackMD);


            ArtistFilter atf = new ArtistFilter(_IMS.AllArtists.Where(a => a.Name.ToLower() == "toto").First());
            //atf.Filter = _IMS.AllArtists.Where(a=>a.Name.ToLower()=="toto").First();


            _FV.FilteringObject = atf;

            AssertAlbums(_Collection, OldAlbums[12], AlbumDescriptorCompareMode.AlbumandTrackMD);

            _FV.FilteringObject = new ArtistFilter(_IMS.AllArtists.Where(a => a.Name.ToLower() == "kuwayama").First());

            //atf.Filter = _IMS.AllArtists.Where(a => a.Name.ToLower() == "kuwayama").First();
            AssertAlbums(_Collection, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);

            IAlbum myalb = _Collection[0];

            using (AlbumInfoEditor aie = new AlbumInfoEditor((myalb as Album).RawTracks.ToList(), this._IMS))
            {
                aie.Author = "Some Guy";
                //aie.CommitChanges(true);
                aie.Commit();
            }

            AssertAlbums(_IMS, OldAlbums[13], AlbumDescriptorCompareMode.AlbumandTrackMD);
            Assert.That(_Collection.Count, Is.EqualTo(0));

            _Collection.Dispose();

            _FV.Dispose();


            //_FV.FilteringEntity = FilterType.Artist;

            //AssertAlbums(_Collection, OldAlbums[12], AlbumDescriptorCompareMode.AlbumandTrackMD);

            //_FV.FilterValue = "Kuwayama";
            //AssertAlbums(_Collection, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);

            //IAlbum myalb = _Collection[0];

            //using (AlbumInfoEditor aie = new AlbumInfoEditor((myalb as Album).RawTracks.ToList(), this._IMS))
            //{
            //    aie.Author = "Some Guy";
            //    aie.CommitChanges(true);
            //}

            //AssertAlbums(_IMS, OldAlbums[13], AlbumDescriptorCompareMode.AlbumandTrackMD);
            //Assert.That(_Collection.Count, Is.EqualTo(0));

        }
    }
}
