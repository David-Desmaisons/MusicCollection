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
    class LiveOrderByTestor : LiveObservableHelper<MyObject, MyObject>
    {

        private List<MyObject> Expected;
        
        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();
            Expected = new List<MyObject>(); ;

            var ld = new LiveOrderBy<MyObject,int>(_Collection,o=>o.Value);
             _Treated = ld;
             _LD = ld;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void DisplayInformation()
        {
            base.DisplayInformation();

            _Treated.Should().Equal(Expected);

            if (_Treated.Count == 0)
                return;

            MyObject[] s1 = new MyObject[_Treated.Count];
            Expected.CopyTo(s1, 0);

            MyObject[] s2 = new MyObject[_Treated.Count];
            _Treated.CopyTo(s2, 0);

            s1.Should().Equal(s2);
        }

        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
            }

            
            AssertCollectionIs(_Treated, Expected);

            Assert.That(_Treated.Count, Is.EqualTo(0));


            MyObject f = new MyObject("David", 0);
            using (Transaction())
            {
                _Collection.Add(f);
                Expected.Add(f);
            }

            this.AssertAddEvent(f, 0);
           
            AssertCollectionIs(_Treated, Expected);

            MyObject f2 = new MyObject("Rey", 1);
            using (Transaction())
            {
                _Collection.Add(f2);
                Expected.Add(f2);
            }

            this.AssertAddEvent(f2, 1);
          
            AssertCollectionIs(_Treated, Expected);


            f2 = new MyObject("Deus", 100);
            using (Transaction())
            {
                _Collection.Add(f2);
                Expected.Add(f2);
            }

            this.AssertAddEvent(f2, 2);
           
            AssertCollectionIs(_Treated, Expected);


            f2 = new MyObject("My", -100);
            using (Transaction())
            {
                _Collection.Add(f2); 
                Expected.Insert(0,f2);
            }

            this.AssertAddEvent(f2, 0);
           
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                f2.Value = 10000;
                Expected.RemoveAt(0);
                Expected.Insert(3, f2);
            }

            this.AssertMoveEvent( 0,3,f2);
            
            AssertCollectionIs(_Treated, Expected);

            f2 = new MyObject("My", 3);
            using (Transaction())
            {
                _Collection.Add(f2);
                Expected.Insert(2, f2);
            }
            this.AssertAddEvent(f2,2);
            
            AssertCollectionIs(_Treated, Expected);

            //f2 = new MyObject("My", 3);
            using (Transaction())
            {
                _Collection.Add(f2); 
                Expected.Insert(2, f2);
            }
            this.AssertAddEvent(f2, 2);
           
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
                f2.Value = 101;
                MyObject o = Expected[4];
                Expected.RemoveAt(4);
                Expected.Insert(2, o);
            }
            this.AssertEvent(NotifyCollectionChangedAction.Move, 2);
            //Expected.Insert(2, f2);
            //AssertCollectionIs(_Treated, Expected);
            using (Transaction())
            {
                f2.Value = 104;
            }
            this.AssertNonEvent();

            using (Transaction())
            {
                f2.Value = 101;
            }
            this.AssertNonEvent();

            Action add = () => _Treated[0] = new MyObject("", 29);
            add.ShouldThrow<Exception>();

            MyObject gg = new MyObject("Geg", -1);

            using (Transaction())
            {
                _Collection[0] = gg;
                Expected[0] = gg;
            }


            //int indexx = 0;
            var expected = _Collection.OrderBy(o => o.Value).ToList();
            foreach (MyObject mo in expected)
            {
                int index = expected.IndexOf(mo);
                _Treated.IndexOf(mo).Should().Be(index);

                _Treated.Contains(mo).Should().BeTrue();

                MyObject ol = _Treated[index++];
                ol.Should().Be(mo);

            }

            _Collection.AddCollection(Enumerable.Range(3, 20).Select(i => new MyObject("s", i))); 

            MyObject nn = new MyObject("mul",1);
            var orde = _Collection.LiveOrderBy(c => c.Value * nn.Value);
            var Expected2 = _Collection.OrderBy(c => c.Value * nn.Value);
            orde.Should().Equal(Expected2);
            var oldv = Expected2.ToList();

            nn.Value = -1;
            orde.Should().Equal(Expected2);
            Expected2.Should().NotEqual(oldv);

            orde.ShouldBeReadOnlyCollection();
            orde.ShouldBeReadOnlyCollection_Generic();

            orde.ShoulBeACoherentNonGenericList();
            orde.ShoulBeACoherentList();


        }

        [Test]
        public void TestSimpleOrdered()
        {
            ObservableCollection<MyObject> Coll= new ObservableCollection<MyObject>();
            IExtendedOrderedObservableCollection<MyObject> target = Coll.OrderByLive(s => s.Value);
            var to = EnumerableFactory.CreateList(20, i => new MyObject(i.ToString(), i));
            var Expected = Coll.OrderBy(s => s.Value);

            target.Should().BeEmpty();


            Coll.Add(to[5]);

            target.Should().Equal(Expected);

            Coll.Insert(0, to[0]);

            target.Should().Equal(Expected);


            var res =target.LiveThenBy(s => s.ID);
            (res as object).Should().Be(target);

        }
    }
}
