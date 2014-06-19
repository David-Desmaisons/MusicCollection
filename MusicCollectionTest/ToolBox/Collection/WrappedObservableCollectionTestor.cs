using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.FunctionListener;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollectionTest.ToolBox.Collection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class WrappedObservableCollectionTestor
    {
        private WrappedObservableCollection<string> _Target;

        [SetUp]
        public void SetUp()
        {
            _Target = new WrappedObservableCollection<string>();
        }

        [TearDown]
        public void SetDown()
        {
        }

        [Test]
        public void Test()
        {
            _Target.MonitorEvents();
            _Target.Add("totot");
            _Target.ShouldRaise("ObjectChanged");
        }

        [Test]
        public void Test_2()
        { 
            _Target.Add("totot");
            _Target.MonitorEvents();
            _Target.Remove("totot");
            _Target.ShouldRaise("ObjectChanged");
        }

        [Test]
        public void Test_3()
        {
            _Target.Add("totot");
            _Target.MonitorEvents();
            _Target[0] = "titi";
            _Target.ShouldNotRaise("ObjectChanged");
        }

        [Test]
        public void Test_4()
        {
            _Target.MonitorEvents();
            _Target.Clear();
            _Target.ShouldNotRaise("ObjectChanged");
        }

        [Test]
        public void Test_5()
        {
            _Target.Add("totot");
            _Target.MonitorEvents();
            _Target.Clear();
            _Target.ShouldRaise("ObjectChanged"); 
       }

        [Test]
        public void Test_6()
        {
            _Target = new WrappedObservableCollection<string>( new string[] {"totot","titi"});
            _Target.MonitorEvents();
            _Target.Clear();
            _Target.ShouldRaise("ObjectChanged");
        }


    }
}
