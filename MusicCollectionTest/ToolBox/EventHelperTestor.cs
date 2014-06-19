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

namespace MusicCollectionTest.ToolBox
{
    class EventHelperTestor : WPFThreadingHelper , IDispatcher
    {
        [SetUp]
        public void SetUp()
        {
            BaseSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            BaseTearDown();
        }

        [Test]
        public void DummyShouldBeNull()
        {          
            Action action =() => Console.Write("Message");
            Delegate del = action;
            del.GetDispatcher().Should().BeNull();
        }

        [Test]
        public void IDispatcherShouldBeOK()
        {
            Action action = () => this.Action();
            Delegate del = action;
            del.GetDispatcher().Should().NotBeNull();
            del.GetDispatcher().Should().Be(MainWindow.Dispatcher);
        }

        [Test]
        public void WPFElementShouldBeOK()
        {
            Delegate del = Delegate.CreateDelegate(typeof(Action), MainWindow, typeof(Window).GetMethod("Show"));
            del.Should().NotBeNull();
            del.GetDispatcher().Should().NotBeNull();
            del.GetDispatcher().Should().Be(MainWindow.Dispatcher);
        }

        private void Action()
        {
        }

        public Dispatcher GetDispatcher()
        {
            return MainWindow.Dispatcher;
        }
    }
}
