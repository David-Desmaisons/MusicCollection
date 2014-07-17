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

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class BasicTest: IntegratedBase
    {
        public BasicTest()
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


        

        [Test]
        public void Test()
        { 
            string oout = Path.Combine(_SK.OutPath,"Custo test");
            string pmcc = Path.Combine(oout, "Albums.mcc");


            //Test Genre factory
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                IMusicGenreFactory gf = ms.GetGenreFactory();
                gf.Should().NotBeNull();

                IGenre g = gf.CreateDummy();
                g.Should().NotBeNull();
                g.Name.Should().BeEmpty();

                IGenre al = gf.Create("Rock");
                al.Name.Should().Be("Rock");
                al.Father.Should().BeNull();
                al.FullName.Should().Be("Rock");

                ms.AllGenres.Count.Should().Be(1);

                IGenre al2 = gf.Create(@"Monde/Auvergne");
                al2.Name.Should().Be("Auvergne");
                al2.Father.Should().NotBeNull();
                al2.Father.Name.Should().Be("Monde");
                al2.FullName.Should().Be(@"Monde/Auvergne");

                ms.AllGenres.Count.Should().Be(3);


                IGenre al3 = gf.Create("Country");
                al3.Name.Should().Be("Country");
                al3.Father.Should().BeNull();
                al3.FullName.Should().Be("Country");

                ms.AllGenres.Count.Should().Be(4);

                IGenre al4 = gf.Create(@"Country/Rap");
                al4.Name.Should().Be("Rap");
                al4.Father.Should().Be(al3);
                al4.Father.Name.Should().Be("Country");
                al4.FullName.Should().Be(@"Country/Rap");

                ms.AllGenres.Count.Should().Be(5);

               IGenre all4 = al3.AddSubGenre("Rap");
                all4.Should().Be(al4);


                IGenre al5 = al3.AddSubGenre("Rap/jjjj");
                al5.Should().BeNull();

                IGenre al6 = al3.AddSubGenre("Jazz");
                al6.Should().NotBeNull();
                al6.Name.Should().Be("Jazz");


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

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(5));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(12));
  
                AssertAlbums(ms, Albums[0], AlbumDescriptorCompareMode.AlbumMD);
   
                Console.WriteLine("Import Successful 5 Albums");


                IMusicExporterFactory imef = ms.GetExporterFactory();
                Assert.That(imef, Is.Not.Null);
                IMusicExporter ime = imef.FromType(MusicExportType.Custo);
                Assert.That(ime, Is.Not.Null);
                IMusicCompleteFileExporter imfe = ime as IMusicCompleteFileExporter;
                Assert.That(imfe, Is.Not.Null);
                imfe.AlbumToExport = ms.AllAlbums;
               
                Directory.CreateDirectory(oout);
                imfe.FileDirectory = oout;
                imfe.Export(true);

                Assert.That(File.Exists(pmcc), Is.True);

                DirectoryInfo fi = new DirectoryInfo(DirectoryIn);
                fi.Empty(true);

                Assert.That(fi.GetFiles().Length, Is.EqualTo(0));

                IMusicRemover imr = ms.GetMusicRemover();
                Assert.That(imr, Is.Not.Null);
                imr.AlbumtoRemove.AddCollection(ms.AllAlbums);
                imr.IncludePhysicalRemove = true;
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

                _SK.Settings.RarFileManagement.RarZipFileAfterSuccessfullExtract = CompleteFileBehaviour.Delete;

                IMusicImporterBuilder imib = ms.GetImporterBuilder(MusicImportType.Custo);
                Assert.That(imib, Is.Not.Null);
                ICustoFilesImporterBuilder icf = imib as ICustoFilesImporterBuilder;
                Assert.That(icf, Is.Not.Null);
                
                icf.Files = new string[] { pmcc };
                icf.DefaultAlbumMaturity = AlbumMaturity.Discover;
                Assert.That(icf.IsValid, Is.True);
                IMusicImporter imit = icf.BuildImporter();
                Assert.That(imit, Is.Not.Null);
                imit.Load();

                AssertAlbums(ms, Albums[0], AlbumDescriptorCompareMode.AlbumMD);
                Assert.That(File.Exists(pmcc), Is.False);
            }

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                AssertAlbums(ms, Albums[0], AlbumDescriptorCompareMode.AlbumMD);

                //TestAlbums(ms.AllAlbums);
            }

        }
    }
}
