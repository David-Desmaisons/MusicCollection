using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollectionTest.TestObjects;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class DynamicSumTestor : LiveObservableHelperResult<MyObject, int>
    {
         [SetUp]
        public void SetUp()
        {

            _Collection = new ObservableCollection<MyObject>();
            this._Computed = _Collection.LiveSum(o => o.Value);
            this._Evaluator = (e) => e.Sum(o => o.Value);
            this._LD = _Computed as IInvariant;

            base.Init();
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
            _Computed.Dispose();
        }

        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
            }

            this.AssertNonEvent();
            Assert.That(this.ComputedValue, Is.EqualTo(0));

            MyObject toto = new MyObject("ToolBox", 20);

            using (Transaction())
            {
                _Collection.Add(toto);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(20));
            AssertChangeEvent(0, 20);

            using (Transaction())
            {
                toto.Value = 5;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(5));
            AssertChangeEvent(20, 5);


            MyObject toto1 = new MyObject("ToolBox", 15);

            using (Transaction())
            {
                _Collection.Add(toto1);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(20));
            AssertChangeEvent(5, 20);

            MyObject toto2 = new MyObject("ToolBox", 10);

            using (Transaction())
            {
                _Collection.Add(toto2);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(30));
            AssertChangeEvent(20, 30);

            using (Transaction())
            {
                _Collection.Remove(toto);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(25));
            AssertChangeEvent(30, 25);

            using (Transaction())
            {
                toto1.Value = 10;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(20));
            AssertChangeEvent(25, 20);


            MyObject toto3 = new MyObject("foo2", -10);
            using (Transaction())
            {
                _Collection[1] = toto3;
            }

            this.ComputedValue.Should().Be(0);
            AssertChangeEvent(20, 0);

            using (Transaction())
            {
                _Collection[1] = toto1;
            }

            this.ComputedValue.Should().Be(20);
            AssertChangeEvent(0, 20);



            using (Transaction())
            {
                _Collection.Clear();
            }

            //_Computed.MonitorEvents();

            //using(_Computed.get

            Assert.That(this.ComputedValue, Is.EqualTo(0));
            AssertChangeEvent(20, 0);
        }
    }
}
