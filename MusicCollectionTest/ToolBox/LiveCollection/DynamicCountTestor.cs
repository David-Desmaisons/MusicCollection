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
    class DynamicCountTestor : LiveObservableHelperResult<MyObject,int>
    {

        [SetUp]
        public void SetUp()
        {
            
            _Collection = new ObservableCollection<MyObject>();
            this._Computed = _Collection.LiveCount();
            this._Evaluator = (e) => e.Count();
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
            Assert.That(this.ComputedValue,Is.EqualTo(0));

            MyObject toto = new MyObject("ToolBox", 20);

            using (Transaction())
            {
                _Collection.Add(toto);
            }

             Assert.That(this.ComputedValue,Is.EqualTo(1));
             AssertChangeEvent(0, 1);

             using (Transaction())
             {
                 _Collection.Add(toto);
             }

             Assert.That(this.ComputedValue, Is.EqualTo(2));
             AssertChangeEvent(1, 2);

             MyObject titi = new MyObject("ToolBoxt", 10);

             using (Transaction())
             {
                 _Collection[0] = titi;
             }

             Assert.That(this.ComputedValue, Is.EqualTo(2));
             this.AssertNonEvent();

             using (Transaction())
             {
                 _Collection.Insert(0,titi);
             }

             Assert.That(this.ComputedValue, Is.EqualTo(3));
             AssertChangeEvent(2, 3);

             using (Transaction())
             {
                 _Collection.RemoveAt(1);
             }

             Assert.That(this.ComputedValue, Is.EqualTo(2));
             AssertChangeEvent(3, 2);

             using (Transaction())
             {
                 _Collection.Clear();
             }

             Assert.That(this.ComputedValue, Is.EqualTo(0));
             AssertChangeEvent(2, 0);

            

        }
    }
}
