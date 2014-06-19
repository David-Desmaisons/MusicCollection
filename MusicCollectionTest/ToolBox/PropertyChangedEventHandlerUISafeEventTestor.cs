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
    public class PropertyChangedEventHandlerUISafeEventTestor
    {
        private PropertyChangedEventHandlerUISafeEvent _Evt;

        [SetUp]
        public void SU()
        {
            _Evt = new PropertyChangedEventHandlerUISafeEvent(this);
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
            _Evt.IsObserved.Should().BeFalse();
            Action fire = () => _Evt.Fire("", true);
            fire.ShouldNotThrow();
            _Evt.MonitorEvents();
            _Evt.IsObserved.Should().BeTrue();
            _Evt.ShouldNotRaise("Event");
        }

        [Test]
        public void Simple_Event_Test()
        {
            _Evt.MonitorEvents();
            _Evt.Fire("Foo", true);
            _Evt.ShouldRaise("Event").WithSender(this).WithArgs<PropertyChangedEventArgs>(p => p.PropertyName == "Foo");
        }

        private AutoResetEvent _ARE;
        private PropertyChangedEventArgs _Fired;
        private Thread _EventThread;


        [Test]
        public void Asynchrone_Event_Test()
        {
            _Evt.Event += new PropertyChangedEventHandler(_Evt_Event);

            Thread nt = new Thread(SendAsynchroneEvent);
            nt.Start();
            _ARE.WaitOne();
            _EventThread.Should().NotBe(Thread.CurrentThread);
            _Fired.Should().NotBeNull();
            _Fired.PropertyName.Should().Be("Foo");
            _Evt.Event -= new PropertyChangedEventHandler(_Evt_Event);
        }

        private void SendAsynchroneEvent()
        {
            _Evt.Fire("Foo", false);
        }

        private void _Evt_Event(object sender, PropertyChangedEventArgs e)
        {
            _Fired = e;
            _EventThread = Thread.CurrentThread;
            _ARE.Set();
        }
    }


   

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class PropertyChangedEventHandlerUISafeEventTestorDispatcher : WPFThreadingHelper, IDispatcher
    {
        private PropertyChangedEventArgs _Fired;
        private Thread _CallingThread;
        private PropertyChangedEventHandlerUISafeEvent _Target;

        [SetUp]
        public void Setup()
        {
            base.BaseSetUp();
            _Target = new PropertyChangedEventHandlerUISafeEvent(this);
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
            _Target.Event += new PropertyChangedEventHandler(_Evt_Event2);

            Thread nt = new Thread(SendSynchroneEvent);
            nt.Start();
            nt.Join();

            _Fired.Should().NotBeNull();
            _Fired.PropertyName.Should().Be("Foo");
            _CallingThread.Should().Be(UIThread);

            _Target.Event -= new PropertyChangedEventHandler(_Evt_Event2);

        }


        [Test]
        public void AsSynchrone_Event_Test_OtherThread()
        {
            Thread.CurrentThread.Should().NotBe(UIThread);
            _Target.Event += new PropertyChangedEventHandler(_Evt_Event2);

            Thread nt = new Thread(SendAsynchroneEvent);
            nt.Start();
            nt.Join();
            Thread.Sleep(500);

            _Fired.Should().NotBeNull();
            _Fired.PropertyName.Should().Be("FooAss");
            _CallingThread.Should().Be(UIThread);

            _Target.Event -= new PropertyChangedEventHandler(_Evt_Event2);

        }

        private void SendAsynchroneEvent()
        {
            _Target.Fire("FooAss", false);
        }

        private void SendSynchroneEvent()
        {
            _Target.Fire("Foo", true);
        }

        private void _Evt_Event2(object sender, PropertyChangedEventArgs e)
        {
            _Fired = e;
            _CallingThread = Thread.CurrentThread;
        }
    }
}
