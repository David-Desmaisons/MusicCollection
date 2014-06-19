using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;

using FluentAssertions;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class DummySingleObservableCollectionTestor
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void SetDown()
        {
        }

        [Test]
        public void TestBasic()
        {
            DummySingleObservableCollection<int> dscc = new DummySingleObservableCollection<int>(23);
            dscc.ShoulBeACoherentList();
            dscc.ShouldBeReadOnlyCollection_Generic();
            dscc.Should().Equal(23);
        }

        [Test]
        public void TestBasic_2()
        {
            int vt = 23;
            var dscc = vt.SingleObservableCollection();
            dscc.ShoulBeACoherentList();
            dscc.ShouldBeReadOnlyCollection_Generic();
            dscc.Should().Equal(23);
        }

        [Test]
        public void Test_Exceptions()
        {
            DummySingleObservableCollection<int> dscc = new DummySingleObservableCollection<int>(23);
            dscc.ShoulBeACoherentList();

            dscc.MonitorEvents();

            int res = 0;
            Action ac = () => res = dscc[1];
            ac.ShouldThrow<ArgumentOutOfRangeException>();

            dscc.ShouldNotRaise("CollectionChanged");
            dscc.ShouldNotRaise("PropertyChanged");
        }

        [Test]
        public void Test_Exceptions_GetEnumerator()
        {
            DummySingleObservableCollection<int> dscc = new DummySingleObservableCollection<int>(23);
            dscc.ShoulBeACoherentList();

            IEnumerator<int> ie = dscc.GetEnumerator();
            ie.MoveNext().Should().BeTrue();
            ie.Current.Should().Be(23);
            ie.MoveNext().Should().BeFalse();

            int res = 0;
            Action ac = () => res = ie.Current;
            ac.ShouldThrow<InvalidOperationException>();

            ie.Reset();
            ie.MoveNext().Should().BeTrue();
            ie.Current.Should().Be(23);
        }

        
    }
}
