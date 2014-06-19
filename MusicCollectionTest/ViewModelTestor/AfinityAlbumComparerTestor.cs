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
using MusicCollection.Infra.Collection;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollectionWPF.ViewModel.Element;
using NSubstitute;
using MusicCollectionTest.TestObjects;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionTest.ViewModelTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    public class AfinityAlbumComparerTestor
    {
        private class AlbumCollectionFake : ObservableCollection<IAlbum>, IFullObservableCollection<IAlbum>
        {
            public void Dispose()
            {
            }
        }

        private IMusicSession _IMusicSession;
        private AlbumCollectionFake _MyAlbums;
        private IUpdatableComparer<IAlbum> _Target;

        [SetUp]
        public void SetUp()
        {
            _MyAlbums = new AlbumCollectionFake();
            _IMusicSession = Substitute.For<IMusicSession>();
            _IMusicSession.AllAlbums.Returns(_MyAlbums);

        }

        [TearDown]
        public void TD()
        {
            if (_Target != null)
            {
                _Target.Dispose();
                _Target = null;
            }
        }

        //private IAlbum CreateFake(string iName = null, string iArtist = null, string iGenre = null)
        //{
        //    return Substitute.For<IAlbum>();
        //}

        private void BuildFull()
        {
            _MyAlbums.AddCollection(Enumerable.Range(0, 1000).Select(i => SubstiteBuilder.ForAlbum(iName: string.Format("i"))));
            //_Target = new AfinityAlbumComparer(_IMusicSession, _IMusicSession.AllAlbums[10]);
            _Target = new AlbumDistanceComparerFactory(_IMusicSession).GetComparer(_IMusicSession.AllAlbums[10]);
        }

        [Test]
        public void FullTest()
        {
            BuildFull();
            _MyAlbums.MinBy(_Target).Should().Be(_IMusicSession.AllAlbums[10]);

        }
    }
}
