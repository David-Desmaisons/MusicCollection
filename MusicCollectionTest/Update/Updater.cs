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
 
    [Ignore("Sample of how upadte id3 flags from meta data")]
    [TestFixture]
    [NUnit.Framework.Category("Update")]
    public class Updater
    {
        private IInternalMusicSession _Session;
        private IImportContext _IMT;

        [SetUp]
        public void Setup()
        {
            _Session = MusicSession.GetSession(null ) as IInternalMusicSession;

            IMusicImporter imi = ((IMusicSession)_Session).GetDBImporter();
            imi.Load();


            _IMT = _Session.GetNewSessionContext();
        }

        private bool NeedUpdateCovers(IAlbum al)
        {
            string tr = al.Tracks[0].Path;

            using (TagLib.File TLF = TagLib.File.Create(tr))
            {
                if (TLF == null)
                {
                    Console.WriteLine("ERROR - Problem for status {0}", al);
                    return false;
                }


                return ((TLF.Tag.Pictures == null) || (TLF.Tag.Pictures.Length == 0));
            }
        }

        [Test]
        public void Test()
        {
            var alls = _Session.AllAlbums.Where(a => a.DateAdded > new DateTime(2013, 4, 23)).ToList();
            foreach (IAlbum al in alls)
            {
                Console.WriteLine("Working on {0}", al);
                bool res = this.NeedUpdateCovers(al);
                if (res == false)
                {
                    Console.WriteLine("Album {0} already contains covers", al);
                    continue;
                }

                Console.WriteLine("Album {0} needs cover update", al);
                Album aal = al as Album;
                aal.SaveTracks(_IMT);
                Console.WriteLine("Album {0} updated", al);

            }

        }

        [TearDown]
        public void TearDown()
        {
            _IMT.Dispose();
            _Session.Dispose();
        }
    }
}
