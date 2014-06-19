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
    class LiveDistinctTestor : LiveObservableHelper<int,int>
    {
 
        //private ObservableCollection<int> _Collection = null;
        //private IObservableCollection<int> _Treated = null;
        //private IInvariant _LD;

        private bool _TestOrder = false;

        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<int>();

             var ld = new LiveDistinct<int>(_Collection);
             _Treated = ld;
             _LD = ld;
            InitializeAndRegisterCollection(_Treated);
            _TestOrder = true;
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        //protected override void DisplayInformation()
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));          
        //    Console.WriteLine("Distinct Collection: {0}", string.Join(",", _Treated));
        //    Console.WriteLine();
        //}


        //protected override void UpdateCountAndEvent()
        //{
        //    base.UpdateCountAndEvent();
        //    Assert.That(_LD.Invariant, Is.True);
        //}

        

        [Test]
        public void Test_1()
        {
            List<int> Expected= new List<int>();
            AssertCollectionIs(_Treated,Expected);

            Assert.That(_Treated.Count,Is.EqualTo(0));

            using(Transaction())
            {
                _Collection.Add(1);
            }

            this.AssertAddEvent(1, 0);
            Expected.Add(1);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.Add(1);
            }

            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
                _Collection.Add(1);
            }

            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.Add(2);
            }

            Expected.Add(2);
            this.AssertAddEvent(2, 1);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                _Collection.Insert(2,3);
            }
            Expected.Insert(1,3);
            this.AssertAddEvent(3,1);
            AssertCollectionIs(_Treated, Expected);
            //1,1,3,1,2

            using (Transaction())
            {
                _Collection.RemoveAt(0);
            }
            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);
            //1,3,1,2    ->  1,3,2


            using (Transaction())
            {
                _Collection.RemoveAt(0);
            }


            if (_TestOrder)
                this.AssertMoveEvent(0, 1);
            else
                this.AssertNonEvent();

            Expected.Clear(); Expected.Add(3); Expected.Add(1); Expected.Add(2);


            AssertCollectionIs(_Treated, Expected, _TestOrder);
            //3,1,2      ->  3,1,2

            using (Transaction())
            {
                _Collection.Insert(0,1);
            }

            if (_TestOrder)
                this.AssertMoveEvent(1, 0);
            else
                this.AssertNonEvent();
            Expected.Clear(); Expected.Add(1); Expected.Add(3); Expected.Add(2);
            AssertCollectionIs(_Treated, Expected, _TestOrder);
            //1,3,1,2      ->  1,3,2


            using (Transaction())
            {
                _Collection.Insert(0, 1);
            }
            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);
            //1,1,3,1,2      ->  1,3,2

            using (Transaction())
            {
                _Collection.Remove(3);
            }
            this.AssertRemoveEvent(3, 1);
            Expected.Remove(3);
            AssertCollectionIs(_Treated, Expected);
            //1,1,1,2      ->  1,2

            using (Transaction())
            {
                _Collection.Add(2);
            }
            this.AssertNonEvent();
            AssertCollectionIs(_Treated, Expected);
            //1,1,1,2,2     ->  1,2


            using (Transaction())
            {
                _Collection.Clear();
            }
            this.AssertResetEvent();
            AssertCollectionIs(_Treated, Enumerable.Empty<int>());

                
        }
    }
}
