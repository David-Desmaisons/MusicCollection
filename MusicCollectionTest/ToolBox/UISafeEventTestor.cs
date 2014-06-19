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

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class UISafeEventTestor
    {
        private UISafeEvent<EventArgs> _target;

        [SetUp]
        public void SU()
        {
            _target = new UISafeEvent<EventArgs>(this);
            _ARE = new AutoResetEvent(false);
            _Fired = null;
            _EventThread = null;
        }

        [TearDown]
        public void TD()
        {
            _ARE.Dispose();
            _ARE = null;
            _Fired = null;
            _EventThread = null;
        }

        [Test]
        public void Zero_Event_Test()
        {
            _target.IsLoaded.Should().BeFalse();
            Action fire = () => _target.Fire(EventArgs.Empty, true);
            fire.ShouldNotThrow();
            _target.MonitorEvents();
            _target.IsLoaded.Should().BeTrue();
            _target.ShouldNotRaise("Event");
        }

        [Test]
        public void Simple_Event_Test()
        {
            _target.MonitorEvents();
            _target.Fire(EventArgs.Empty, true);
            _target.ShouldRaise("Event").WithSender(this).WithArgs<EventArgs>(p => p == EventArgs.Empty);
        }

        private AutoResetEvent _ARE;
        private EventArgs _Fired;
        private Thread _EventThread;


        [Test]
        public void Asynchrone_Event_Test()
        {
            _target.Event += _Evt_Event;

            Thread nt = new Thread(SendAsynchroneEvent);
            nt.Start();
            _ARE.WaitOne();
            _EventThread.Should().NotBe(Thread.CurrentThread);
            _Fired.Should().NotBeNull();
            _Fired.Should().Be(EventArgs.Empty);
            _target.Event -= _Evt_Event;
        }

        private void SendAsynchroneEvent()
        {
            _target.Fire(EventArgs.Empty, false);
        }

        private void _Evt_Event(object sender, EventArgs e)
        {
            _Fired = e;
            _EventThread = Thread.CurrentThread;
            _ARE.Set();
        }
    }


   

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class UISafeEventTestorDispatcher : WPFThreadingHelper, IDispatcher
    {
        private EventArgs _Fired;
        private Thread _CallingThread;
        private UISafeEvent<EventArgs> _Target;

        [SetUp]
        public void Setup()
        {
            base.BaseSetUp();
            _Target = new UISafeEvent<EventArgs>(this);
            _Fired = null;
            _CallingThread = null;
         
        }

        [TearDown]
        public void TD()
        {
            BaseTearDown();
            _Fired = null;
            _CallingThread = null;
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
            _Fired.Should().Be(EventArgs.Empty);
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
            _Fired.Should().Be(EventArgs.Empty);
            _CallingThread.Should().Be(UIThread);

            _Target.Event -= _Evt_Event2;

        }

        private void SendAsynchroneEvent()
        {
            _Target.Fire(EventArgs.Empty, false);
        }

        private void SendSynchroneEvent()
        {
            _Target.Fire(EventArgs.Empty, true);
        }

        private void _Evt_Event2(object sender, EventArgs e)
        {
            _Fired = e;
            _CallingThread = Thread.CurrentThread;
        }
    }
}
