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
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class DynamicAverageTestor : LiveObservableHelperResult<MyObject, Nullable<double>>
    {
         [SetUp]
        public void SetUp()
        {

            _Collection = new ObservableCollection<MyObject>();
            this._Computed = _Collection.LiveAverage(o => o.Value);
            this._Evaluator = (e) => e.Any() ? e.Average(o => o.Value) : new Nullable<double>();
            this._LD = _Computed as IInvariant;

            base.Init();
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
            }

            this.AssertNonEvent();
            Assert.That(this.ComputedValue, Is.EqualTo(null));

            MyObject toto = new MyObject("ToolBox", 20);

            using (Transaction())
            {
                _Collection.Add(toto);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(20));
            AssertChangeEvent(null, 20);

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

            Assert.That(this.ComputedValue, Is.EqualTo(10));
            AssertChangeEvent(5, 10);

            MyObject toto2 = new MyObject("ToolBox", 10);

            using (Transaction())
            {
                _Collection.Add(toto2);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(10));
            this.AssertNonEvent();



            using (Transaction())
            {
                _Collection.Remove(toto);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(12.5));
            AssertChangeEvent(10, 12.5);

            using (Transaction())
            {
                toto1.Value = 10;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(10));
            AssertChangeEvent(12.5,10);

            MyObject toto3 = new MyObject("O", 30);

            using (Transaction())
            {
                _Collection[1] = toto3;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(20));
            AssertChangeEvent(10, 20);

            using (Transaction())
            {
                _Collection[1] = toto1;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(10));
            AssertChangeEvent(20, 10);

            using (Transaction())
            {
                _Collection.Clear();
            }

            Assert.That(this.ComputedValue, Is.Null);
            AssertChangeEvent(10, null);
        }
    }
}
