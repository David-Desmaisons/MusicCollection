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
    public class RollBackOnImportError : IntegratedBase
    {
        [TestFixtureTearDown]
        public void TD()
        {
            base.CleanDirectories();
            _msi.Dispose();
        }

        private string _AddDirectory;
        private string _MainDirectory;
        private MusicSessionImpl _msi;
        private IMusicSession _MS;

        [TestFixtureSetUp]
        public void SetUp()
        {
            base.Init();

            _AddDirectory = Path.Combine(this.DirectoryIn, "Greg Kelley And Co");
            Directory.CreateDirectory(_AddDirectory);

            string copysong = Path.Combine(_AddDirectory, "02 One To Grow On.mp3");
            string mysong = Path.Combine(this.DirectoryIn, "02 One To Grow On.mp3");

            FileInfo fi = new FileInfo(mysong);
            fi.MoveTo(copysong);

            _MainDirectory = Path.Combine(this.DirectoryIn, "Main");
            Directory.CreateDirectory(_MainDirectory);

            DirectoryInfo di = new DirectoryInfo(this.DirectoryIn);
            foreach (FileInfo fif in di.GetFiles())
            {
                if (Path.GetExtension(fif.Name) != ".xml")
                {
                    string Des = Path.Combine(_MainDirectory, fif.Name);
                    fif.MoveTo(Des);
                }
            }

            _MS = MusicSessionImpl.GetSession(_SK.Builder);
            _msi = _MS as MusicSessionImpl;

            IMusicImporter imi = _MS.GetDBImporter();
            Assert.That(imi, Is.Not.Null);
            imi.Load();

            Assert.That(_msi.AllAlbums.Count, Is.EqualTo(0)); 
            Assert.That(_msi.AllArtists.Count, Is.EqualTo(0));
            Assert.That(_msi.AllGenres.Count, Is.EqualTo(25));
           

        }

        [Test]
        public void TestRollBack()
        {
            IDirectoryImporterBuilder imib = _MS.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
            Assert.That(imib, Is.Not.Null);
            imib.Directory = this._MainDirectory;
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

            Assert.That(imib.IsValid, Is.True);
            IMusicImporter imi = imib.BuildImporter();
            Assert.That(imi, Is.Not.Null);
            imi.Load();

            AssertAlbums(_MS, OldAlbums[7], AlbumDescriptorCompareMode.AlbumandTrackMD);

            IAlbum ia = _MS.AllAlbums.Where(al => al.Name == "Live - Instal Glasgow - 2006").FirstOrDefault();
            Assert.That(ia, Is.Not.Null);

            IModifiableAlbum ima = ia.GetModifiableAlbum();
            ima.Artists.Clear();
            ima.Artists.Add(_MS.CreateArtist( "Greg Kelley"));
            ima.Commit();

            AssertAlbums(_MS, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);
            _MS.AllAlbums.ShouldBeCoherentWithAlbums(OldAlbums[8]);

            IArtist ar = _MS.AllArtists.Where(art => art.Name == "Greg Kelley").FirstOrDefault();
            Assert.That(ar, Is.Not.Null);

            Assert.That(ar.Albums.Count, Is.EqualTo(1));

            Assert.That(_MS.AllArtists.Count, Is.EqualTo(8));


            //Prep End
            DummyBufferType.IsOK = false;//needs to fails

            imib = _MS.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
            Assert.That(imib, Is.Not.Null);
            imib.Directory = this._AddDirectory;
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

            Assert.That(imib.IsValid, Is.True);
            imi = imib.BuildImporter();
            Assert.That(imi, Is.Not.Null);

            ImportExportErrorEventArgs error = null;

            imi.Error += (o, e) => error = e;
            imi.Load();

            Assert.That(error, Is.Not.Null);

            ar = _MS.AllArtists.Where(art => art.Name == "Tatsuya Nakatani").FirstOrDefault();
            Assert.That(ar, Is.Null);

            //Tatsuya Nakatani

            AssertAlbums(_MS, OldAlbums[8], AlbumDescriptorCompareMode.AlbumandTrackMD);
            Assert.That(_MS.AllArtists.Count, Is.EqualTo(8));

            Assert.That(error is UnknowError, Is.True);


            //import will be ok
            DummyBufferType.IsOK = true;


            imib = _MS.GetImporterBuilder(MusicImportType.Directory) as IDirectoryImporterBuilder;
            Assert.That(imib, Is.Not.Null);
            imib.Directory = this._AddDirectory;
            imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

            Assert.That(imib.IsValid, Is.True);
            imi = imib.BuildImporter();
            Assert.That(imi, Is.Not.Null);

            error = null;

            imi.Error += (o, e) => error = e;
            imi.Load();

            Assert.That(error, Is.Null);

            AssertAlbums(_MS, OldAlbums[9], AlbumDescriptorCompareMode.AlbumandTrackMD);
            Assert.That(_MS.AllArtists.Count, Is.EqualTo(10));

            ar = _MS.AllArtists.Where(art => art.Name == "Tatsuya Nakatani").FirstOrDefault();
            Assert.That(ar, Is.Not.Null);



                 


        }

        protected override SessionKeys GetKeys()
        {
            return new DummySessionKeys(DirectoryOut, OpenClean);
        }
    }
}
