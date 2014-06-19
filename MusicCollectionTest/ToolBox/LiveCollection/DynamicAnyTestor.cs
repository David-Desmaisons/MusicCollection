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
    class DynamicAnyTestor : LiveObservableHelperResult<MyObject, bool>
    {
         [SetUp]
        public void SetUp()
        {

            _Collection = new ObservableCollection<MyObject>();
            this._Computed = _Collection.LiveAny(o => o.Value%2==0);
            this._Evaluator = (e) => e.Any(o => o.Value % 2 == 0);
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
            Assert.That(this.ComputedValue, Is.EqualTo(false));

            MyObject toto = new MyObject("ToolBox", 1);

            using (Transaction())
            {
                _Collection.Add(toto);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(false));
            this.AssertNonEvent();

            using (Transaction())
            {
                toto.Value = 5;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(false));
            this.AssertNonEvent();

            using (Transaction())
            {
                toto.Value = 68;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(true));
            this.AssertChangeEvent(false, true);

            MyObject toto1 = new MyObject("ToolBox", 0);

            using (Transaction())
            {
                _Collection.Add(toto1);
            }

            Assert.That(this.ComputedValue, Is.EqualTo(true));
            this.AssertNonEvent();

            using (Transaction())
            {
                toto.Value = 25;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(true));
            this.AssertNonEvent();

            using (Transaction())
            {
                toto1.Value = 25;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(false));
            this.AssertChangeEvent(true, false);


            using (Transaction())
            {
                toto1.Value = 0;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(true));
            this.AssertChangeEvent(false, true);

            using (Transaction())
            {
                toto1.Value = 1;
            }

            using (Transaction())
            {
                _Collection.Remove(toto1);
            }

            using (Transaction())
            {
                toto1.Value = 0;
            }

            using (Transaction())
            {
                _Collection.Add(toto1);
            }

            using (Transaction())
            {
                _Collection.Add(toto1);
            }

            using (Transaction())
            {
                toto1.Value = 1;
            }

            Assert.That(this.ComputedValue, Is.EqualTo(false));
            this.AssertChangeEvent(true, false);
        }

        [Test]
        public void TestAllChanged()
        {
            ObservableCollection<MyObject> Listo = new ObservableCollection<MyObject>();
            Listo.AddCollection(Enumerable.Range(0,20).Select(i=>new MyObject(i.ToString(),i=0)));
            MyObject f = Listo.Last();
            f.Should().NotBeNull();
            ILiveResult<bool> all = Listo.LiveAny(a => a.Value == f.Value) ;
            all.Should().NotBeNull();
            all.Value.Should().BeTrue();

            all.MonitorEvents();
            f.Value = 20;
            all.Value.Should().BeTrue();
            all.ShouldNotRaisePropertyChangeFor(a => a.Value);

         }
    }
}
