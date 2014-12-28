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
using NSubstitute;
using FluentAssertions;

using MusicCollection.Fundation;
using MusicCollection.MusicPlayer;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using System.Threading;

namespace MusicCollectionTest.MusicPlayerTest
{

    internal static class IReadOnlyPlayList_Extender
    {
        internal static void ChangeCurrent(this IReadOnlyPlayList @this, IInternalTrack iit)
        {
            @this.CurrentTrack.Returns(iit);
            @this.SelectionChanged += Raise.EventWith(new SelectionChangedargs());
        }

        internal static void ShouldRaiseTrackEvent(this MusicPlayer @this, ITrack tr, TrackPlayingEvent tpe)
        {
            @this.ShouldRaise("TrackEvent").WithSender(@this).WithArgs<MusicTrackEventArgs>(e => e.Track == tr && tpe == e.What);
        }


    }

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("MusicPlayer")]
    class MusicPlayerTestor
    {
        #region Helpers

        public MusicPlayer CreateMusicPlayer(out IInternalTrack it, out IInternalPlayer iip, out IReadOnlyPlayList irop)
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);
            iip.FileSource = null;

            irop = Substitute.For<IReadOnlyPlayList>();

            it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack.Returns(it);

            MusicPlayer res = new MusicPlayer(imf);
            res.PlayList = irop;

            return res;
        }

        #endregion

        #region property playlist and track

        [Test]
        public void MusicPlayer_Basic()
        {
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer mp = new MusicPlayer(imf);

            mp.Mode.Should().Be(PlayMode.Stopped);
            mp.MusicTrackSource.Should().BeNull();

            mp.Mode = PlayMode.Play;
            mp.MusicTrackSource.Should().BeNull();
            mp.Mode.Should().Be(PlayMode.Stopped);
            mp.AlbumPlayList.Should().BeNull();
        }

        [Test]
        public void MusicPlayer_Set_PlayList_CurrentTrack_Null()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer target = new MusicPlayer(imf);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            irop.CurrentTrack = null;

            target.MonitorEvents();           
            
            //act
            target.PlayList = irop;

            //assert
            target.PlayList.Should().Be(irop);
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.Mode.Should().Be( PlayMode.Stopped);

            target.Dispose();
        }

        [Test]
        public void MusicPlayer_Set_PlayList_Twice()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer target = new MusicPlayer(imf);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            irop.CurrentTrack = null;

            target.PlayList = irop;

            //act
            
            target.MonitorEvents();
            target.PlayList = irop;


            //assert
            target.PlayList.Should().Be(irop);
            target.ShouldNotRaisePropertyChangeFor(t => t.PlayList);
            target.Mode.Should().Be(PlayMode.Stopped);

            target.Dispose();
        }

        [Test]
        public void MusicPlayer_Set_PlayList_CurrentTrack_NotNull()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer target = new MusicPlayer(imf);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack = it;

            //act
            target.MonitorEvents();
            target.PlayList = irop;
            
            //assert
            target.PlayList.Should().Be(irop);
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            iip.FileSource.Should().Be("MyPath");
            iip.Received().Play();

            iip.Listener.Should().Be(target);

            target.Mode.Should().Be(PlayMode.Play);
            target.MusicTrackSource.Should().Be(it);

            target.Mode = PlayMode.Paused;
            target.Mode.Should().Be(PlayMode.Paused);

            iip.Received().Pause();

            //clean
            target.Dispose();
        }


        [Test]
        public void MusicPlayer_Set_PlayList_CurrentTrack_NotNull_ChangePlayList()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer target = new MusicPlayer(imf);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack = it;

            target.PlayList = irop;

            //act
            target.MonitorEvents();
            target.PlayList = null;

            //assert
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.PlayList.Should().BeNull();
            iip.Received().Stop();

            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(null);

            //clean
            target.Dispose();
        }

        #endregion

        #region property delegate to envelope

        [Test]
        public void MusicPlayer_Volume_Change()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);
            iip.Volume.Returns(0.5d);

            MusicPlayer target = new MusicPlayer(imf);
               
            //act-assert 1
            target.Volume.Should().Be(0.5d); 

            //act-assert 2
            target.MonitorEvents();
            target.Volume = 0.2;
            target.Volume.Should().Be(0.2d);
            iip.Volume.Should().Be(0.2d);
            //iip.Listener.OnVolumeChange();
            target.ShouldRaisePropertyChangeFor(t => t.Volume);

            //act-assert 3
            target.Volume = -1;
            target.Volume.Should().Be(0d);
            iip.Volume.Should().Be(0d);

            //act-assert 4
            target.Volume = 5;
            target.Volume.Should().Be(1d);
            iip.Volume.Should().Be(1d);

            //assert
            
            target.PlayList.Should().BeNull();

            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(null);

            //clean
            target.Dispose();
        }


        [Test]
        public void MusicPlayer_Position_Change()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);
            iip.Position.Returns(TimeSpan.FromSeconds(9));

            MusicPlayer target = new MusicPlayer(imf);

            //act-assert 1
            target.Position.Should().Be(TimeSpan.FromSeconds(9));

            //act-assert 2
            target.MonitorEvents();
            target.Position = TimeSpan.FromSeconds(90);
            target.Position.Should().Be(TimeSpan.FromSeconds(90));
            iip.Position.Should().Be(TimeSpan.FromSeconds(90));

            //clean
            target.Dispose();
        }

        #endregion

        #region Play

        [Test]
        public void MusicPlayer_Play_Track_Null()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            // act
            MusicPlayer target = new MusicPlayer(imf);
            target.Play();

            //assert
            target.MusicTrackSource.Should().BeNull();
            target.Mode.Should().Be(PlayMode.Stopped);

           
        }

        [Test]
        public void MusicPlayer_Play_List_Current_Null_Init_OnPLay()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack.Returns((IInternalTrack)null);

            irop.When(x => x.Init()).Do(x=> irop.CurrentTrack.Returns(it));

            // act
            MusicPlayer target = new MusicPlayer(imf);
            target.MonitorEvents();
            target.PlayList = irop;
            target.Play();

            //assert
            target.PlayList.Should().Be(irop);
            target.MusicTrackSource.Should().Be(it);          
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.ShouldRaisePropertyChangeFor(t => t.MusicTrackSource);
            iip.FileSource.Should().Be("MyPath");
            iip.Received().Play();
            irop.Received().Init();

            iip.Listener.Should().Be(target);

            target.Mode.Should().Be(PlayMode.Play);
        }

        [Test]
        public void MusicPlayer_Play_Play_Current()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack.Returns(it);
            irop.When(x => x.Init()).Do(x => irop.CurrentTrack.Returns(it));

            // act
            MusicPlayer target = new MusicPlayer(imf);
            target.MonitorEvents();
            target.PlayList = irop;
            target.Stop();
            target.Mode.Should().Be(PlayMode.Stopped);
            target.Play();

            //assert
            target.PlayList.Should().Be(irop);
            target.MusicTrackSource.Should().Be(it);
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.ShouldRaisePropertyChangeFor(t => t.MusicTrackSource);
            iip.FileSource.Should().Be("MyPath");
            iip.Received().Play();
            irop.DidNotReceive().Init();

            iip.Listener.Should().Be(target);

            target.Mode.Should().Be(PlayMode.Play);
        }

        [Test]
        public void MusicPlayer_Play_Play_Do_No_Change()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack.Returns(it);
            irop.When(x => x.Init()).Do(x => irop.CurrentTrack.Returns(it));

            // act
            MusicPlayer target = new MusicPlayer(imf);
           
            target.PlayList = irop; 
            
            target.Mode.Should().Be(PlayMode.Play);
            target.MonitorEvents();
            target.Mode = PlayMode.Play;

            //assert
            target.ShouldNotRaisePropertyChangeFor(t => t.Mode);
            target.Mode.Should().Be(PlayMode.Play);
        }

        [Test]
        public void MusicPlayer_Play_Do_NotPlay_Broken()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);
            iip.FileSource = null;

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            it.IsBroken.Returns(true);
            irop.CurrentTrack.Returns(it);


            // act
            MusicPlayer target = new MusicPlayer(imf);
            target.MonitorEvents();
            target.PlayList = irop;
          

            //assert
            target.PlayList.Should().Be(irop);
            target.MusicTrackSource.Should().Be(it);
            target.Mode.Should().Be(PlayMode.Stopped);
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.ShouldRaisePropertyChangeFor(t => t.MusicTrackSource);
            iip.FileSource.Should().Be("MyPath");
            iip.DidNotReceive().Play();
            irop.DidNotReceive().Init();
            iip.Listener.Should().Be(target);

            //act2 
            target.Pause();

            //assert2
            target.Mode.Should().Be(PlayMode.Stopped);
            iip.DidNotReceive().Pause();

            //act2 
            target.Stop();

            //assert2
            target.Mode.Should().Be(PlayMode.Stopped);

        }

        [Test]
        public void MusicPlayer_Play_Do_NotPlay_UnderEdit()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);
            iip.FileSource = null;

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            it.InternalState.Returns(ObjectState.UnderEdit);
            irop.CurrentTrack.Returns(it);


            // act
            MusicPlayer target = new MusicPlayer(imf);
            target.MonitorEvents();
            target.PlayList = irop;


            //assert
            target.PlayList.Should().Be(irop);
            target.MusicTrackSource.Should().Be(it);
            target.Mode.Should().Be(PlayMode.Stopped);
            target.ShouldRaisePropertyChangeFor(t => t.PlayList);
            target.ShouldRaisePropertyChangeFor(t => t.MusicTrackSource);
            iip.FileSource.Should().Be("MyPath");
            iip.DidNotReceive().Play();
            irop.DidNotReceive().Init();
            iip.Listener.Should().Be(target);

            //act2
            target.Pause();

            //assert2 
            target.Mode.Should().Be(PlayMode.Stopped);
            iip.DidNotReceive().Pause();

            //act3
            target.Stop();

            //assert3
            target.Mode.Should().Be(PlayMode.Stopped);
            iip.DidNotReceive().Play();


            //act4
            IInternalMusicPlayer iimp = target;
            it.UpdatedState.Returns(ObjectState.Available);
            it.InternalState.Returns(ObjectState.Available);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.UnderEdit, ObjectState.Available));

            target.Play();

            //assert4
            target.Mode.Should().Be(PlayMode.Play);

        }

        #endregion

        #region internalplayer events

        [Test]
        public void MusicPlayer_Event_BeginPlay()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            //act
            IInternalPlayerListener listener = target;
            target.MonitorEvents();
            listener.OnTrackLoadedPlay();

            
            //assert
            target.ShouldRaiseTrackEvent(it, TrackPlayingEvent.BeginPlay);
        }

        [Test]
        public void MusicPlayer_Event_Broken()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            //act
            target.Mode.Should().Be(PlayMode.Play);
            IInternalPlayerListener listener = target;
            
            listener.OnBroken();
           
            //assert
            target.Mode.Should().Be(PlayMode.Stopped);
            target.ShouldRaiseTrackEvent(it, TrackPlayingEvent.Broken);
        }

        [Test]
        public void MusicPlayer_Event_Playing()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            object mp = null;
            MusicTrackPlayingEventArgs mte = null;
            ManualResetEvent mre = new ManualResetEvent(false);

            target.TrackPlaying += (o, e) => { mp = o; mte = e;mre.Set(); };       

            //act
            target.Mode.Should().Be(PlayMode.Play);
            IInternalPlayerListener listener = target;
            listener.OnTrackPlayingEvent(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(4));

            mre.WaitOne();

            ////assert
            target.Mode.Should().Be(PlayMode.Play);
            mp.Should().Be(target); //1
            mte.MaxPosition.Should().Be(TimeSpan.FromMinutes(4));
            mte.Position.Should().Be(TimeSpan.FromSeconds(1));
            mte.Track.Should().Be(it); //3
            mte.What.Should().Be(TrackPlayingEvent.Playing); //2
        }

        [Test]
        public void MusicPlayer_Event_Track_End()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            irop.Transition().Returns(false);
            irop.When(t => t.Transition()).Do(_ => irop.CurrentTrack.Returns((ITrack)null));


            target.MonitorEvents();
            //act
            target.Mode.Should().Be(PlayMode.Play);
            IInternalPlayerListener listener = target;
            listener.OnTrackEnd();


            ////assert
            target.Mode.Should().Be(PlayMode.Stopped);
            target.ShouldRaisePropertyChangeFor(t => t.Mode);
            target.ShouldRaiseTrackEvent(it, TrackPlayingEvent.EndPlay);
        }

        [Test]
        public void MusicPlayer_Event_Track_End_PLaylistContinue()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            IInternalTrack it2 = Substitute.For<IInternalTrack>();

            irop.Transition().Returns(true);
            irop.When(t => t.Transition()).Do(_ => irop.ChangeCurrent(it2));


            target.MonitorEvents();
            //act
            target.Mode.Should().Be(PlayMode.Play);
            IInternalPlayerListener listener = target;
            listener.OnTrackEnd();


            ////assert
            target.Mode.Should().Be(PlayMode.Play);
            target.ShouldRaiseTrackEvent(it, TrackPlayingEvent.EndPlay);
            target.MusicTrackSource.Should().Be(it2);
        }

        #endregion

        #region TrackPlayingEvent events

        [Test]
        public void MusicPlayer_Event_Skipped()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            IInternalTrack it2 = Substitute.For<IInternalTrack>();

            iip.Position.Returns(TimeSpan.FromSeconds(3));
            target.Position.Should().Be(TimeSpan.FromSeconds(3));
         
            //act
            target.Mode.Should().Be(PlayMode.Play);
            target.MonitorEvents();

            irop.ChangeCurrent(it2);


            ////assert
            target.Mode.Should().Be(PlayMode.Play);
            target.ShouldRaiseTrackEvent(it, TrackPlayingEvent.Skipped);
            target.MusicTrackSource.Should().Be(it2);
        }

        [Test]
        public void MusicPlayer_Event_Skipped_Less_Than2sec()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

            IInternalTrack it2 = Substitute.For<IInternalTrack>();

            iip.Position.Returns(TimeSpan.FromSeconds(1));
            target.Position.Should().Be(TimeSpan.FromSeconds(1));

            //act
            target.Mode.Should().Be(PlayMode.Play);
            target.MonitorEvents();

            irop.ChangeCurrent(it2);


            ////assert
            target.Mode.Should().Be(PlayMode.Play);
            target.MusicTrackSource.Should().Be(it2);
        }




        #endregion

        #region Playlist events

        [Test]
        public void MusicPlayer_Set_PlayList_Change_Track()
        {
            //arrange
            IMusicFactory imf = Substitute.For<IMusicFactory>();
            IInternalPlayer iip = Substitute.For<IInternalPlayer>();
            imf.GetInternalPlayer().Returns(iip);

            MusicPlayer target = new MusicPlayer(imf);

            IReadOnlyPlayList irop = Substitute.For<IReadOnlyPlayList>();
            IInternalTrack it = Substitute.For<IInternalTrack>();
            it.Path.Returns("MyPath");
            irop.CurrentTrack = it;

            target.PlayList = irop;

            IInternalTrack it2 = Substitute.For<IInternalTrack>();
            it2.Path.Returns("MyPath2");

            target.MonitorEvents();

            //act

            irop.ChangeCurrent(it2);

            //assert
            iip.FileSource.Should().Be("MyPath2");
            iip.Received().Play();
            target.ShouldRaisePropertyChangeFor(t => t.MusicTrackSource);

            target.Mode.Should().Be(PlayMode.Play);
            target.MusicTrackSource.Should().Be(it2);

            //clean
            target.Dispose();
        }


        //[Test]
        //public void MusicPlayer_PlayListEvent_OnRemove_Should_BeNull()
        //{
        //    // arrange
        //    IInternalTrack it = null;
        //    IInternalPlayer iip = null;
        //    IReadOnlyPlayList irop = null;
        //    MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

        //    //act
        //    target.Mode.Should().Be(PlayMode.Play);
        //    target.MonitorEvents();
        //    it.InternalState.Returns(ObjectState.Removed);
        //    Action ac = () => irop.ChangeCurrent(null);


        //    ////assert
        //    ac.ShouldNotThrow<Exception>();
        //    //target.Mode.Should().Be(PlayMode.Stopped);
        //    //target.ShouldRaisePropertyChangeFor(t => t.Mode);
        //    //target.MusicTrackSource.Should().Be(null);
        //}

        //[Test]
        //public void MusicPlayer_PlayListEvent_OnRemove_Should_BeNull_Exception()
        //{
        //    // arrange
        //    IInternalTrack it = null;
        //    IInternalPlayer iip = null;
        //    IReadOnlyPlayList irop = null;
        //    MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);

        //    IInternalTrack it2 = Substitute.For<IInternalTrack>();

        //    //act
        //    target.Mode.Should().Be(PlayMode.Play);
        //    target.MonitorEvents();
        //    it.InternalState.Returns(ObjectState.Removed);
        //    Action ac = () => irop.ChangeCurrent(it2);


        //    ////assert
        //    ac.ShouldThrow<Exception>();
        //}


        [Test]
        public void MusicPlayer_PlayListEvent_Change_For_Null()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);


            //act
            target.Mode.Should().Be(PlayMode.Play);
            target.MonitorEvents();

            irop.ChangeCurrent(null);

            ////assert
            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(null);
        }

        [Test]
        public void MusicPlayer_PlayListEvent_Change_For_Broken()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);


            //act
            target.Mode.Should().Be(PlayMode.Play);
            target.MonitorEvents();

            IInternalTrack it2 = Substitute.For<IInternalTrack>();
            it2.UpdatedState.Returns(ObjectState.FileNotAvailable);

            irop.ChangeCurrent(it2);


            ////assert
            irop.Received().Transition();
        }

        #endregion

        #region IInternalTrack Listener


        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Playing()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);
            iip.Position.Returns(TimeSpan.FromSeconds(10));

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Position.Should().Be(TimeSpan.FromSeconds(10));

            //act
            target.MonitorEvents();

            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));
          

            ////assert
            target.Position.Should().Be(TimeSpan.FromSeconds(10));
            target.Mode.Should().Be(PlayMode.Paused);
            target.MusicTrackSource.Should().Be(it);
            target.ShouldRaisePropertyChangeFor(t => t.Mode);
            iip.Received().Pause();
            iip.Received().Stop();
            iip.Received().Close();

        }

        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Paused()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Pause();
            target.Mode.Should().Be(PlayMode.Paused);
            iip.ClearReceivedCalls();

            //act
            target.MonitorEvents();

            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));


            ////assert

            target.Mode.Should().Be(PlayMode.Paused);
            target.MusicTrackSource.Should().Be(it);
            iip.Received().Stop();
            iip.Received().Close();

        }


        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Stopped()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Mode = PlayMode.Stopped;
            target.Mode.Should().Be(PlayMode.Stopped);
            iip.ClearReceivedCalls();

            //act
            target.MonitorEvents();

            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));


            ////assert

            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(it);
            iip.Received().Close();

        }

        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Playing_ThenStop()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);
            iip.Position.Returns(TimeSpan.FromSeconds(10));

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Position.Should().Be(TimeSpan.FromSeconds(10));

            //act
            target.MonitorEvents();

            it.InternalState.Returns(ObjectState.UnderEdit);
            it.UpdatedState.Returns(ObjectState.UnderEdit);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));

            iip.ClearReceivedCalls();

            target.Stop();

            ////assert
            target.ShouldRaisePropertyChangeFor(t => t.Mode);
            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(it);
            iip.DidNotReceive().Close();
            iip.DidNotReceive().Stop();
            iip.DidNotReceive().Pause();

        }

        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Paused_ThenReleased()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Pause();
            target.Mode.Should().Be(PlayMode.Paused);
            iip.Position.Returns(TimeSpan.FromSeconds(20));
            iip.Volume.Returns(0.9);
            iip.ClearReceivedCalls();
            target.Position.Returns(TimeSpan.FromSeconds(20));

            //act
            target.MonitorEvents();
            it.InternalState.Returns(ObjectState.UnderEdit);
            it.UpdatedState.Returns(ObjectState.UnderEdit);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));

            iip.ClearReceivedCalls();
            it.InternalState.Returns(ObjectState.Available);
            it.UpdatedState.Returns(ObjectState.Available);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.UnderEdit, ObjectState.Available));


            ////assert
            target.Mode.Should().Be(PlayMode.Paused);
            target.MusicTrackSource.Should().Be(it);
            iip.DidNotReceive().Pause();
            target.Position.Should().Be(TimeSpan.FromSeconds(20));

            iip.ClearReceivedCalls();

            //act2 
            target.Play();
            iip.Listener.OnTrackLoadedPlay();


            ////assert 2
            target.Mode.Should().Be(PlayMode.Play);
            target.MusicTrackSource.Should().Be(it);
            iip.Received().Play();
            target.Position.Should().Be(TimeSpan.FromSeconds(20));
            target.Volume.Should().Be(0.9);

        }

        [Test]
        public void MusicPlayer_IInternalTrack_Listener_UnderEdit_Playing_Then_Playing()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);
            iip.Position.Returns(TimeSpan.FromSeconds(10));

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Position.Should().Be(TimeSpan.FromSeconds(10));
            iip.Volume.Returns(0.9);


            //act
            it.InternalState.Returns(ObjectState.UnderEdit);
            it.UpdatedState.Returns(ObjectState.UnderEdit);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.UnderEdit));

            iip.When(p => p.Play()).Do(_ => iip.Listener.OnTrackLoadedPlay());

            target.MonitorEvents();
            iip.ClearReceivedCalls();
            it.InternalState.Returns(ObjectState.Available);
            it.UpdatedState.Returns(ObjectState.Available);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.UnderEdit, ObjectState.Available));

            ////assert
            target.Position.Should().Be(TimeSpan.FromSeconds(10));
            target.Mode.Should().Be(PlayMode.Play);
            target.MusicTrackSource.Should().Be(it);
            target.ShouldRaisePropertyChangeFor(t => t.Mode);
            iip.Received().Play();
            target.Volume.Should().Be(0.9);
            //iip.ShouldNotRaise("TrackEvent");
        }

        [Test]
        public void MusicPlayer_IInternalTrack_Listener_FileNotAvailable()
        {
            // arrange
            IInternalTrack it = null;
            IInternalPlayer iip = null;
            IReadOnlyPlayList irop = null;
            MusicPlayer target = CreateMusicPlayer(out it, out iip, out irop);
            target.MonitorEvents();

            it.Received().AddPlayer(target);
            iip.Position.Returns(TimeSpan.FromSeconds(10));

            IInternalMusicPlayer iimp = target;
            target.Mode.Should().Be(PlayMode.Play);
            target.Position.Should().Be(TimeSpan.FromSeconds(10));
            iip.Volume.Returns(0.9);
            target.MonitorEvents(); 
            iip.ClearReceivedCalls();

            irop.Transition().Returns(false);
            irop.When(r => r.Transition()).Do(_=>irop.ChangeCurrent(null));

            //act
            it.InternalState.Returns(ObjectState.FileNotAvailable);
            it.UpdatedState.Returns(ObjectState.FileNotAvailable);
            iimp.OnLockEvent(it, new ObjectStateChangeArgs(it, ObjectState.Available, ObjectState.FileNotAvailable));
           
           
     
            ////assert
            target.Position.Should().Be(TimeSpan.FromSeconds(10));
            target.Mode.Should().Be(PlayMode.Stopped);
            target.MusicTrackSource.Should().Be(null);
            target.ShouldRaisePropertyChangeFor(t => t.Mode);
            iip.Received().Close();
        }
        


        #endregion

        [Test]
        public void Test()
        {
            InternalPlayerAdapter ipa = Substitute.For<InternalPlayerAdapter>(1);
        }
    }
}
