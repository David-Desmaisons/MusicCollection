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
using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Fundation;
using System.Collections;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class LiveWhereTestor : LiveObservableHelper<MyObject, MyObject>
    {
        private MyObject _Reference;

        [SetUp]
        public void SetUp()
        {
            _Reference = new MyObject("Ref", 0);
            _Collection = new ObservableCollection<MyObject>();

            var ld = new LiveWhere<MyObject>(_Collection, o => o.Value % 2 == _Reference.Value);
            _Treated = ld;
            _LD = ld;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void UpdateCountAndEvent()
        {
            Assert.That(_Treated.SequenceEqual(_Collection.Where(o => o.Value % 2 == _Reference.Value)));
            base.UpdateCountAndEvent();
        }


        [Test]
        public void Test_1()
        {
            List<MyObject> Expected = new List<MyObject>();
            AssertCollectionIs(_Treated, Expected);

            Assert.That(_Treated.Count, Is.EqualTo(0));

            MyObject first = new MyObject("David", 0);
            using (Transaction())
            {
                _Collection.Add(first);
            }

            this.AssertAddEvent(first, 0);
            Expected.Add(first);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.Add(first);
            }

            this.AssertAddEvent(first, 1);
            Expected.Add(first);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.Add(first);
            }
            this.AssertAddEvent(first, 2);
            Expected.Add(first);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.RemoveAt(1);
            }

            this.AssertRemoveEvent(first, 1);
            Expected.Remove(first);
            AssertCollectionIs(_Treated, Expected);


            MyObject deux = new MyObject("David", 1);
            using (Transaction())
            {
                _Collection.Insert(0, deux);
            }

            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                deux.Value = 8;
            }

            this.AssertAddEvent(deux, 0);
            Expected.Insert(0, deux);
            AssertCollectionIs(_Treated, Expected);



            using (Transaction())
            {
                first.Value = 17;
            }

            this.AssertEvent(NotifyCollectionChangedAction.Remove, 2);
            Expected.Clear();
            Expected.Add(deux);
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
                first.Value = 16;
            }

            this.AssertEvent(NotifyCollectionChangedAction.Add, 2);
            Expected.Add(first); Expected.Add(first);
            AssertCollectionIs(_Treated, Expected);


            MyObject trois = new MyObject("David", 1);

            using (Transaction())
            {
                _Collection.Insert(2, trois);
            }

            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                this._Reference.Value = 1;
            }

            this.AssertEvent(_Collection.Count);
            Expected.Clear();
            Expected.Add(trois);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                this._Reference.Value = 0;
            }

            this.AssertEvent(_Collection.Count);

        }

        [Test]
        public void TestGroupChange()
        {
            ObservableCollection<MyObject> Collection = new ObservableCollection<MyObject>();
            MyObject ob = new MyObject("a", 1);
            for (int i = 0; i < 30; i++)
            {
                Collection.Add(ob);
            }

            MyObject hh = new MyObject("hh", 1);

            ICompleteFunction<MyObject, bool> icf = CompleteDynamicFunction<MyObject, bool>.GetCompleteDynamicFunction(o => o.Value == hh.Value);
            icf.FactorizeEvent = true;
            LiveWhere<MyObject> ld = new LiveWhere<MyObject>(Collection, icf);

            ld.Should().Equal(Collection);

            ld.MonitorEvents();
            hh.Value = 0;
            ld.ShouldRaise("CollectionChanged").WithSender(ld).WithArgs<NotifyCollectionChangedEventArgs>(ar => ar.Action == NotifyCollectionChangedAction.Reset);
            ld.Should().BeEmpty();

            hh.Value = 3;
            ld.Should().BeEmpty();

            hh.Value = 1;
            ld.Should().Equal(Collection);
        }

        [Test]
        public void TestGroupChange_Individual()
        {
            ObservableCollection<MyObject> Collection = new ObservableCollection<MyObject>();
            MyObject ob = new MyObject("a", 1);
            for (int i = 0; i < 30; i++)
            {
                Collection.Add(ob);
            }

            MyObject hh = new MyObject("hh", 1);

            ICompleteFunction<MyObject, bool> icf = CompleteDynamicFunction<MyObject, bool>.GetCompleteDynamicFunction(o => o.Value == hh.Value);
            LiveWhere<MyObject> ld = new LiveWhere<MyObject>(Collection, icf);
            icf.FactorizeEvent = false;
   
            ld.Should().Equal(Collection);

            ld.MonitorEvents();
            hh.Value = 0;
            ld.ShouldRaise("CollectionChanged").WithSender(ld).WithArgs<NotifyCollectionChangedEventArgs>(ar => ar.Action == NotifyCollectionChangedAction.Remove);
            ld.Should().BeEmpty();

            hh.Value = 3;
            ld.Should().BeEmpty();

            hh.Value = 1;
            ld.Should().Equal(Collection);
        }

        [Test]
        public void NoChanges()
        {
            ObservableCollection<MyObject> Collection = new ObservableCollection<MyObject>();
            MyObject om = new MyObject("a", 2);
            Collection.Add(om);
            var lw = Collection.LiveWhere(o => o.Name.Length == 2);


            lw.ShouldBeReadOnlyCollection_Generic();
            lw.ShouldBeReadOnlyCollection();

            //Action add = () => lw.Add(new MyObject("b", 2));
            //add.ShouldThrow<AccessViolationException>();

            //Action rem = () => lw.Remove(om);
            //rem.ShouldThrow<AccessViolationException>();


            //Action clr = () => ((IList<MyObject>)lw).Clear();
            //clr.ShouldThrow<AccessViolationException>();

            //Action ins = () => lw.Insert(0, om);
            //ins.ShouldThrow<AccessViolationException>();

            
        }

      
    }
}
