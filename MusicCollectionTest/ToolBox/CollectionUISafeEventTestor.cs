using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Security.Permissions;
using System.Collections.Specialized;

using NSubstitute;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class CollectionUISafeEventTestor
    {
        private CollectionUISafeEvent _target;
        private bool _ActionCalled = false;

        [SetUp]
        public void SU()
        {
            _target = new CollectionUISafeEvent(this, () => ActionCall());
            _ARE = new AutoResetEvent(false);
            _Fired = null;
            _EventThread = null;
            _ActionCalled = false;
        }

        private void ActionCall()
        {
            _ActionCalled = true;
        }

        [TearDown]
        public void TD()
        {
            _ARE.Dispose();
            _ARE = null;
            _Fired = null;
            _EventThread = null;
            _ActionCalled = false;
        }

        [Test]
        public void Zero_Event_Test()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            Action fire = () => _target.CollectionChanged(ncea);
            fire.ShouldNotThrow();
            _target.MonitorEvents();
            _target.ShouldNotRaise("Event");
            _ActionCalled.Should().BeTrue();
        }

        [Test]
        public void Simple_Event_Test()
        {
            _ActionCalled.Should().BeFalse();
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            _target.MonitorEvents();
            _target.CollectionChanged(ncea);
            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p == ncea);
            _ActionCalled.Should().BeTrue();
        }

        private AutoResetEvent _ARE;
        private NotifyCollectionChangedEventArgs _Fired;
        private Thread _EventThread;


        [Test]
        public void Asynchrone_Event_Test()
        {
            _ActionCalled.Should().BeFalse();
            _target.Event += _Evt_Event;

            Thread nt = new Thread(SendAsynchroneEvent);
            nt.Start();
            _ARE.WaitOne();
            _EventThread.Should().NotBe(Thread.CurrentThread);
            _Fired.Should().NotBeNull();
            _Fired.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            _target.Event -= _Evt_Event;
            _ActionCalled.Should().BeTrue();
        }

        private void SendAsynchroneEvent()
        {
            _target.CollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), false);
        }

        private void _Evt_Event(object sender, NotifyCollectionChangedEventArgs e)
        {
            _Fired = e;
            _EventThread = Thread.CurrentThread;
            _ARE.Set();
        }



        [Test]
        public void GetEventFactorizable_Test_Ultra_Simple()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), 5);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
            }

            _target.ShouldNotRaise("Event");
        }

        [Test]
        public void GetEventFactorizable_Test_Pretty_Simple()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), 5);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged((NotifyCollectionChangedEventArgs)null);
            }

            _target.ShouldNotRaise("Event");
        }

        [Test]
        public void GetEventFactorizable_Test_Simple()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,new object(),5);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p == ncea);
        }

        [Test]
        public void GetEventFactorizable_Test_ResetComputed()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), 5);
            NotifyCollectionChangedEventArgs ncea2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), 9);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();

                _target.CollectionChanged(ncea2);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p.Action == NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void GetEventFactorizable_Test_ResetComputed_2()
        {
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new object(), 5);
            NotifyCollectionChangedEventArgs ncea2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), 9);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();

                _target.CollectionChanged(ncea2);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p.Action == NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void GetEventFactorizable_Test_MoveComputed()
        {
            object un = new object();
            object deux = new object();
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, un, 5);
            NotifyCollectionChangedEventArgs ncea2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, un, 9);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();

                _target.CollectionChanged(ncea2);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p.Action == NotifyCollectionChangedAction.Move && p.NewItems[0] == un && p.OldItems[0] == un && p.OldStartingIndex==5 && p.NewStartingIndex==9  );
        }

        [Test]
        public void GetEventFactorizable_Test_ReplaceComputed()
        {
            object un = new object();
            object deux = new object();
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, un, 5);
            NotifyCollectionChangedEventArgs ncea2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, deux, 5);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();

                _target.CollectionChanged(ncea2);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldRaise("Event").WithSender(this).WithArgs<NotifyCollectionChangedEventArgs>(p => p.Action == NotifyCollectionChangedAction.Replace && p.NewItems[0] == deux && p.OldItems[0] == un && p.OldStartingIndex == 5 && p.NewStartingIndex == 5);
        }

        [Test]
        public void GetEventFactorizable_Test_NothingComputed()
        {
            object un = new object();
            object deux = new object();
            NotifyCollectionChangedEventArgs ncea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, un, 5);
            NotifyCollectionChangedEventArgs ncea2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, un, 5);
            _target.MonitorEvents();

            using (_target.GetEventFactorizable())
            {
                _target.CollectionChanged(ncea);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();

                _target.CollectionChanged(ncea2);
                _target.ShouldNotRaise("Event");
                _ActionCalled.Should().BeFalse();
            }

            _target.ShouldNotRaise("Event");
        }
    }


   

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class CollectionUISafeEventTestorDispatcher : WPFThreadingHelper, IDispatcher
    {
        private NotifyCollectionChangedEventArgs _Fired;
        private Thread _CallingThread;
        private CollectionUISafeEvent _Target;
        //private bool _ActionCalled;

        [SetUp]
        public void Setup()
        {
            base.BaseSetUp();
            _Target = new CollectionUISafeEvent(this, () => ActionCall());
            _Fired = null;
            _CallingThread = null;
            //_ActionCalled = false;
         
        }


        private void ActionCall()
        {
            //_ActionCalled = true;
        }

        [TearDown]
        public void TD()
        {
            BaseTearDown();
            _Fired = null;
            _CallingThread = null;
            //_ActionCalled = true;
        }
  

        public Dispatcher GetDispatcher()
        {
            return MainWindow.Dispatcher;
        }

        [Test]
        public void Synchrone_Event_Test_OtherThread()
        {
            Thread.CurrentThread.Should().NotBe(UIThread);
            _Target.Event +=  _Evt_Event2;

            Thread nt = new Thread(SendSynchroneEvent);
            nt.Start();
            nt.Join();

            _Fired.Should().NotBeNull();
            _Fired.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            _CallingThread.Should().Be(UIThread);

            _Target.Event -= _Evt_Event2;

        }


        [Test]
        public void AsSynchrone_Event_Test_OtherThread()
        {
            Thread.CurrentThread.Should().NotBe(UIThread);
            _Target.Event += _Evt_Event2;

            Thread nt = new Thread(SendAsynchroneEvent);
            nt.Start();
            nt.Join();
            Thread.Sleep(500);

            _Fired.Should().NotBeNull();
            _Fired.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            _CallingThread.Should().Be(UIThread);

            _Target.Event -= _Evt_Event2;

        }

        private void SendAsynchroneEvent()
        {
            _Target.Fire(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), false);
        }

        private void SendSynchroneEvent()
        {
            _Target.Fire(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), true);
        }

        private void _Evt_Event2(object sender, NotifyCollectionChangedEventArgs e)
        {
            _Fired = e;
            _CallingThread = Thread.CurrentThread;
        }
    }
}
