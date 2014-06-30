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
using MusicCollection.FileConverter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;
using SevenZip;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    internal class UnrarTest : IntegratedBase
    {
        public UnrarTest()
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
            SevenZipExtractor.SetLibraryPath(Path.GetFullPath(Path.Combine(@".\7z.dll")));

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

                //World_Saxophone_Quartet-Steppin_With

                _SK.Settings.RarFileManagement.RarZipFileAfterSuccessfullExtract = CompleteFileBehaviour.Delete;
                string rarpath = Path.Combine(DirectoryIn, "a.rar");
                Assert.That(File.Exists(rarpath), Is.True);

                IFilesImporterBuilder imi2 = ms.GetImporterBuilder(MusicImportExportType.Compressed) as IFilesImporterBuilder;
                Assert.That(imi2, Is.Not.Null);
                imi2.Files = new string[] { rarpath };
                Assert.That(imi2.IsValid, Is.True);
                imi = imi2.BuildImporter();
                ImportExportErrorEventArgs error=null;
                imi.Error += (o, e) => error = e;
                imi.Load();
                if (error != null)
                {
                    Console.WriteLine(error);
                    if (error is NotEnougthSpace)
                    {
                        Assert.Ignore("Not Enougth Disk space.  Omitting.");
                    }
                    else
                    {
                        Assert.That(false);
                    }
                }

                Assert.That(File.Exists(rarpath), Is.False);
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(1));

                CueSheet cs = new CueSheet(Path.Combine(DirectoryIn, "a.cue"));

                AssertAlbum(ms.AllAlbums[0], cs, AlbumDescriptorCompareMode.AlbumandTrackMD);
                AssertAlbum(ms.AllAlbums[0], Albums[0][0], AlbumDescriptorCompareMode.AlbumandTrackMD);

            }
        }
    }
}
