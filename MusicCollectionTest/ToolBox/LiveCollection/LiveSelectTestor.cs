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
    class LiveSelectTestor : LiveObservableHelper<MyObject,string>
    {
        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();

            var ld = new LiveSelectCollection<MyObject,string>(_Collection, o => string.Format("{0}-{1}",o.Name,o.Value));
            _Treated = ld;
            _LD = ld;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        [Test]
        public void Test()
        {
            List<string> Expected = new List<string>();
            AssertCollectionIs(_Treated, Expected);

            Assert.That(_Treated.Count, Is.EqualTo(0));

            MyObject f = new MyObject("David", 0);
            using (Transaction())
            {
                _Collection.Add(f);
            }

            Expected.Add("David-0");
            this.AssertAddEvent(Expected[0], 0);
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
                _Collection.Add(f);
            }

            Expected.Add("David-0");
            this.AssertAddEvent(Expected[0], 1);
            AssertCollectionIs(_Treated, Expected);

            MyObject f2 = new MyObject("toto", 2);
            using (Transaction())
            {
                _Collection.Add(f2);
            }

            Expected.Add("toto-2");
            this.AssertAddEvent(Expected[2], 2);
            AssertCollectionIs(_Treated, Expected);


            using (Transaction())
            {
                f.Name = "Didier";
            }

            Expected.Clear();
            Expected.Add("Didier-0"); Expected.Add("Didier-0"); Expected.Add("toto-2");
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(NotifyCollectionChangedAction.Replace, 2);


            using (Transaction())
            {
                f.Value = 2;
            }

            Expected.Clear();
            Expected.Add("Didier-2"); Expected.Add("Didier-2"); Expected.Add("toto-2");
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(NotifyCollectionChangedAction.Replace, 2);


            MyObject dedeagain = new MyObject("David", 4);

            using (Transaction())
            {
                _Collection[0] = dedeagain;
            }

            Expected.Clear();
            Expected.Add("David-4"); Expected.Add("Didier-2"); Expected.Add("toto-2");
            AssertCollectionIs(_Treated, Expected);
            this.AssertReplaceEvent(0, "Didier-2", "David-4");


            using (Transaction())
            {
               bool res = _Collection.Remove(f2);
               Assert.That(res, Is.True);
            }

            Expected.Remove("toto-2");
            AssertCollectionIs(_Treated, Expected);
            this.AssertRemoveEvent("toto-2",2);


 

            using (Transaction())
            {
                f2.Value = 45;
            }

             AssertCollectionIs(_Treated, Expected);
             this.AssertNonEvent();          
            
            
            MyObject mama = new MyObject("Martine", 40);


            using (Transaction())
            {
                _Collection.Insert(1, mama);
            }

            Expected.Insert(1, "Martine-40");
            AssertCollectionIs(_Treated, Expected);
            this.AssertAddEvent("Martine-40", 1);



            using (Transaction())
            {
                _Collection.Move(0, 2);
            }

            Expected.Clear();
            Expected.Add("Martine-40"); Expected.Add("Didier-2");Expected.Add("David-4"); 
            AssertCollectionIs(_Treated, Expected);
            this.AssertMoveEvent(0, 2, "David-4");


            using (Transaction())
            {
                dedeagain.Name = "Batman";
            }

            Expected.Clear();
            Expected.Add("Martine-40"); Expected.Add("Didier-2"); Expected.Add("Batman-4");
            AssertCollectionIs(_Treated, Expected);
            this.AssertReplaceEvent(2, "David-4", "Batman-4");

        }

    }
}
