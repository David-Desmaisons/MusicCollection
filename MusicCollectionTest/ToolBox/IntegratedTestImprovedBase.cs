using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using FluentAssertions;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated.Tools
{
    public class IntegratedTestImprovedBase : IntegratedBase
    {

        protected internal IntegratedTestImprovedBase()
        {
        }

        protected void baseTestFixtureSetUp()
        {
            Init();
        }

        protected void baseTestFixtureTearDown()
        {
            base.CleanDirectories();
        }

        protected IMusicSession _IMusicSession;

        protected void baseSetUp()
        {
            _IMusicSession = MusicSessionImpl.GetSession(_SK.Builder);

            _IMusicSession.AllAlbums.Should().BeEmpty();
            _IMusicSession.AllGenres.Should().BeEmpty();
            _IMusicSession.AllArtists.Should().BeEmpty();

            IMusicImporter imi = _IMusicSession.GetDBImporter();
            _IMusicSession.Should().NotBeNull();
            imi.Load();

            Console.WriteLine("Importing Music Folder");

            PostOpen(_IMusicSession);
        }

        protected virtual void PostOpen(IMusicSession ims)
        {
        }

        protected void baseTearDown(bool NeedResetDB=false)
        {
            _IMusicSession.Dispose();
            if (NeedResetDB)
                base.BigClean();
        }
    }
}
