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
using MusicCollection.Nhibernate.Session;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;
using MusicCollection.Implementation.Session;

namespace MusicCollectionTest.Update
{
    [Ignore("Sample of how upadte album art from file id3 tags")]
    [TestFixture]
    [NUnit.Framework.Category("Update")]
    public class Clean_Updater_Cleaner_Album_Cover_Art
    {
        private IInternalMusicSession _Session;
        //private IImportContext _IMT;

        [SetUp]
        public void Setup()
        {
            _Session = MusicSession.GetSession(null) as IInternalMusicSession;

            IMusicImporter imi = ((IMusicSession)_Session).GetDBImporter();
            imi.Load();

            //_IMT = _Session.GetNewSessionContext();
        }

        private bool NeedUpdateCovers(IAlbum al)
        {
            return ((al.Images.Count == 0) || (!al.Images.All(a => ((a != null) && (a.IsBroken == false)))));
        }


        private List<string> ImagesFromDirectory(string iDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(iDirectory);
            if (!d.Exists)
                return new List<string>();

            return FileServices.ImagesFiles.SelectMany(f => d.GetFiles("*"+f))
                .Select(fi => fi.FullName).ToList();
        }

        private bool AddImagesFromFile(IModifiableAlbum imoda)
        {
            try
            {
                string MainDirectory = imoda.MainDirectory;
                List<string> myfiles = ImagesFromDirectory(MainDirectory);

                if (myfiles.Count == 0)
                {
                    string otherdir = Path.Combine(MainDirectory, "cover");
                    myfiles = ImagesFromDirectory(otherdir);

                    if (myfiles.Count == 0)
                    {
                        string newotherdir = Path.Combine(MainDirectory, "covers");
                        myfiles = ImagesFromDirectory(newotherdir);
                    }
                }

                if (myfiles.Count == 0)
                {
                    return false;
                }

                int i = 0;
                foreach (string file in myfiles)
                {
                    imoda.AddAlbumPicture(file, i++);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem {0}", e);
                return false;
            }

        }

        [Test]
        public void Test()
        {
            int i = 0;
            var alls = _Session.AllAlbums.Reverse().ToList();
            foreach (IAlbum al in alls)
            {
                Console.WriteLine(@"Album {0}/{1}",++i,alls.Count);
                Console.WriteLine("Working on {0}", al);
                bool res = this.NeedUpdateCovers(al);
                if (res == false)
                {
                    Console.WriteLine("Album {0} already contains covers", al);

                    if (al.CoverImage == null)
                    {
                        using (var IMT = _Session.GetNewSessionContext())
                        {
                            using (IMusicTransaction IMut = IMT.CreateTransaction())
                            {
                                Album alr = al as Album;
                                IMut.ImportContext.AddForUpdate(alr);
                                Console.WriteLine("Need to regenerate cover image for {0}", al);                               
                                alr.RegenerateCoverArt();
                                IMut.ImportContext.Commit();                             
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Everything is OK for {0}", al);
                    }

                    continue;
                }

                Console.WriteLine("Album {0} needs cover update", al);
                //Album aal = al as Album;

                try
                {

                    IModifiableAlbum almodi = al.GetModifiableAlbum(true);

                    if (!AddImagesFromFile(almodi))
                    {
                        almodi.ReinitImages();
                    }

                    almodi.Commit(true);
                }
                catch(Exception e)
                {
                     Console.WriteLine("Problem 2 :{0}", e);
                }

                if (al.Images.Count > 0)
                {
                    Console.WriteLine("Album {0} updated", al);
                }
                else
                {
                    Console.WriteLine("No art founf for Album {0} updated", al);
                }
            }

        }

        [TearDown]
        public void TearDown()
        {
            //_IMT.Dispose();
            _Session.Dispose();
        }
    }
}
