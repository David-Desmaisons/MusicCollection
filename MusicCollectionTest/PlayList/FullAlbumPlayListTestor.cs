using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using NUnit.Framework;
using System.Linq.Expressions;
using MusicCollectionTest.TestObjects;
using System.Collections.ObjectModel;

using FluentAssertions;
using MusicCollection.PlayList;
using NSubstitute;
using MusicCollection.Fundation;
using MusicCollection.Implementation;

namespace MusicCollectionTest.PlayList
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("PlayList")]
    public class FullAlbumPlayListTestor
    {
        public FullAlbumPlayListTestor()
        {
        }

        [Test]
        public void Test_Basic()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            target.PlayListname.Should().Be("Test");
            target.ReadOnlyTracks.Should().BeEmpty();
            target.Albums.Should().BeEmpty();
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();
            target.AutoReplay.Should().BeFalse();

            target.MonitorEvents();
            target.PlayListname = "toto";
            target.PlayListname.Should().Be("toto");
            target.ShouldRaisePropertyChangeFor(x=>x.PlayListname);

            target.AutoReplay = true;
            target.AutoReplay.Should().BeTrue();
            target.ShouldRaisePropertyChangeFor(x => x.AutoReplay);

            target.Init();
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();

            target.Transition();
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();

            target.Dispose();
        }

        [Test]
        public void Test_Add_Album()
         {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum();

            target.AddAlbum(al1);

            target.ReadOnlyTracks.Should().BeEmpty();
            target.Albums.Should().Equal(al1);

            target.ReadOnlyTracks.MonitorEvents();

            IInternalTrack track1 = SubstiteBuilder.ForTrack(al1);
           

            target.Albums.Should().Equal(al1);

            target.ReadOnlyTracks.ShouldRaiseCollectionEvent(eac => eac.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add);
            target.ReadOnlyTracks.Should().Equal(track1);

            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();
          
            target.Init();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(track1);


            target.Transition();
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();

            target.Init();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(track1);

            target.AutoReplay = true;

            target.Transition();
            target.CurrentAlbumItem.Should().Be(al1);
            target.CurrentTrack.Should().Be(track1);

            target.Dispose();
        }

        [Test]
        public void Test_Set_TrackNotList()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);

            target.AddAlbum(al1);

            target.ReadOnlyTracks.Should().HaveCount(5);
            target.Albums.Should().Equal(al1);

            var source = al1.Tracks[0];
            target.AddAlbum(al1);

            target.CurrentAlbumItem = al1;
            target.CurrentTrack.ShouldBeSameAs(source);
            target.CurrentAlbumItem.Should().Be(al1);

            ITrack itr = SubstiteBuilder.ForTrack(SubstiteBuilder.ForAlbum());
            target.CurrentTrack = itr;
            target.CurrentTrack.ShouldBeSameAs(source);
            target.CurrentAlbumItem.Should().Be(al1);

             IAlbum al2 = SubstiteBuilder.ForAlbum(2);

             target.CurrentAlbumItem = al2;
             target.CurrentTrack.ShouldBeSameAs(source);
             target.CurrentAlbumItem.Should().Be(al1);
        }

        [Test]
        public void Test_Add_Album_NoBasic()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);

            target.AddAlbum(al1);

            target.ReadOnlyTracks.Should().HaveCount(5);
            target.Albums.Should().Equal(al1);

            target.Init();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(al1.Tracks[0]);


            target.Transition();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(al1.Tracks[1]);

            target.CurrentTrack = al1.Tracks[4];
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(al1.Tracks[4]);

            target.AutoReplay = true;

            target.Transition();
            target.CurrentAlbumItem.Should().Be(al1);
            target.CurrentTrack.Should().Be(al1.Tracks[0]);

            al1.Tracks.Remove(al1.Tracks[0]);
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();

            var source = al1.Tracks[3];

            target.CurrentTrack = al1.Tracks[3];
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(source);

            al1.Tracks.Remove(al1.Tracks[0]);
            target.CurrentAlbumItem.Should().Be(al1);
            target.CurrentTrack.Should().Be(source);

            target.Dispose();
        }

        [Test]
        public void Test_Event()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);
            target.AddAlbum(al1);

            target.MonitorEvents();
            target.Init();
            target.CurrentAlbumItem.Should().NotBeNull();
            target.ShouldRaise("SelectionChanged").WithSender(target);
        }

        [Test]
        public void Test_Event2()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);
            target.AddAlbum(al1); 
            target.Init();

            target.MonitorEvents();
            target.Transition();
            target.CurrentAlbumItem.Should().NotBeNull();
            target.ShouldRaise("SelectionChanged").WithSender(target);
        }

        [Test]
        public void Test_Add_Album_Remove()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);

            target.AddAlbum(al1);

            target.ReadOnlyTracks.Should().HaveCount(5);
            target.Albums.Should().Equal(al1);

            target.Init();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(al1.Tracks[0]);

            target.RemoveAlbum(al1);
            target.CurrentAlbumItem.Should().BeNull();
            target.CurrentTrack.Should().BeNull();

            target.ReadOnlyTracks.Should().BeEmpty();
            target.Albums.Should().BeEmpty();
        }

        [Test]
        public void Test_Add_Album_Remove_2()
        {
            FullAlbumPlayList target = new FullAlbumPlayList("Test");
            IAlbum al1 = SubstiteBuilder.ForAlbum(5);

            target.AddAlbum(al1);
            target.ReadOnlyTracks.Should().HaveCount(5);
            target.Albums.Should().Equal(al1);

            target.Init();
            target.CurrentAlbumItem.ShouldBeSameAs(al1);
            target.CurrentTrack.ShouldBeSameAs(al1.Tracks[0]);

            IAlbum al2 = SubstiteBuilder.ForAlbum(5);

            target.AddAlbum(al2);
            target.ReadOnlyTracks.Should().HaveCount(10);
            target.Albums.Should().Equal(al1, al2);

            target.CurrentAlbumItem = al2;
            target.CurrentAlbumItem.Should().Be(al2);
            target.CurrentTrack.Should().Be(al2.Tracks[0]);

            target.RemoveAlbum(al1);
            target.CurrentAlbumItem.Should().Be(al2);
            target.CurrentTrack.Should().Be(al2.Tracks[0]);

            target.ReadOnlyTracks.Should().HaveCount(5);
            target.Albums.Should().Equal(al2);
        }
    }
}
