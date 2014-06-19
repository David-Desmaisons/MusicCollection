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
//using MusicCollection.ToolBox.Collection.Observable.LiveQuery.TrackingOrderCollection;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class CollectionChained : LiveObservableHelper<MyObject, string>, IInvariant
    {


        private MyObject Reference;
        [SetUp]
        public void SetUp()
        {
            Reference = new MyObject("Ref", 0);
            _Collection = new ObservableCollection<MyObject>();
            _Collection.Add(Reference);

            var ld = _Collection.LiveWhere(o => o.Name.Contains("R")).LiveOrderBy(o => o.Name).LiveSelect(o => o.Name).LiveDistinct();

            _Treated = ld;
            _LD = this;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void DisplayInformation()
        {
            Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine("Treated Collection: {0}", string.Join(",", _Treated));
            Console.WriteLine();
        }


        private void TeTe()
        {
        //    Indexer<MyObject> y = new Indexer<MyObject>(l1);
        //    Indexer<MyObject> z = new Indexer<MyObject>(l2);

        //    Assert.That(y.Invariant, Is.True);
        //    Assert.That(z.Invariant, Is.True);
        }




        private List<MyObject> l1;
        private List<MyObject> l2;


        [Test]
        public void infra_test()
        {

            l1 = new List<MyObject>();
            l2 = new List<MyObject>();
            TeTe();


            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.True);

            MyObject toto = new MyObject(",", 23);
            l1.Add(toto);

            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
            TeTe();


            MyObject toto2 = new MyObject(",", 20);
            l2.Add(toto2);

            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
             TeTe();

            l1.Add(toto2);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
             TeTe();

            l2.Add(toto);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.True);
             TeTe();

            l2.Add(toto);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
             TeTe();

            l1.Insert(0, toto);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.True);
             TeTe();

            l1[1] = toto2;
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
             TeTe();

            l2.Remove(toto);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.False);
             TeTe();

            l2.Add(toto2);
            Assert.That(l1.SequenceCompareWithoutOrder(l2), Is.True);
             TeTe();



        }


        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
            }

            using (Transaction())
            {
                Reference.Name = "a";
            }

            using (Transaction())
            {
                Reference.Name = "Rza";
            }

            MyObject toto = new MyObject("Ra", 0);
            using (Transaction())
            {
                _Collection.Add(toto);
            }


            toto = new MyObject("Rb", 0);
            using (Transaction())
            {
                _Collection.Add(toto);
            }

            MyObject totoe = new MyObject("b", 0);
            using (Transaction())
            {
                _Collection.Insert(0, totoe);
            }

            using (Transaction())
            {
                totoe.Name = "abR";
            }

            MyObject totoee = new MyObject("abR", 0);
            using (Transaction())
            {
                _Collection.Add(totoee);
            }


            using (Transaction())
            {
                _Collection.Add(totoee);
            }





        }


        bool IInvariant.Invariant
        {
            get { return _Treated.SequenceEqual(_Collection.Where(o => o.Name.Contains("R")).OrderBy(o => o.Name).Select(o => o.Name).Distinct()); }
        }
    }

}
