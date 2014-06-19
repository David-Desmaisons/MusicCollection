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
using MusicCollection.Utilies;
using MusicCollection.Nhibernate.Session;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using System.Diagnostics;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    public class SessionClosingTest : IntegratedBase
    {

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
            IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder);


            MusicSessionImpl msi = ms as MusicSessionImpl;

            ms.AllAlbums.Count.Should().Be(0);
            ms.AllGenres.Count.Should().Be(0);
            ms.AllArtists.Count.Should().Be(0);

            IDisposable dip = msi.GetSessionLock();
            dip.Should().NotBeNull();

            ms.Dispose();

            Action ac = () => dip.Dispose();
            ac.ShouldNotThrow<Exception>();

        }

        [Test]
        public void Test2()
        {
            IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder);


            MusicSessionImpl msi = ms as MusicSessionImpl;

            ms.AllAlbums.Count.Should().Be(0);
            ms.AllGenres.Count.Should().Be(0);
            ms.AllArtists.Count.Should().Be(0);

            Action ac = () => EmulateBrokenTransaction(msi);
            ac.ShouldNotThrow<Exception>();

        }

        private void EmulateBrokenTransaction(IInternalMusicSession msi)
        {
            using (IDisposable l = msi.GetSessionLock())
            {
                try
                {
                    msi.Dispose();
                    int i = msi.AllAlbums.Count;
                }
                catch
                {
                    Trace.WriteLine("Assertion Raised");
                }
            }
        }

    }
}
