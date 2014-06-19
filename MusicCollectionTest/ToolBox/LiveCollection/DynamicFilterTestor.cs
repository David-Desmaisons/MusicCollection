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
    class DynamicFilterTestor : LiveObservableHelper<MyObject, MyObject>
    {
        private DynamicFilter<MyObject> _DF;

        [SetUp]
        public void SetUp()
        {
            _DF = new DynamicFilter<MyObject>((o)=>(o.Value%2)==1);

            _Collection = new ObservableCollection<MyObject>();

            var ld = _Collection.LiveWhere(_DF);
            _DF.FactorizeEvent = false;
            _Treated = ld;
            _LD = ld as IInvariant;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        [Test]
        public void TestFilterOnly()
        {
            MyObject king = new MyObject("ToolBox", 20);

            Assert.That(_DF.Evaluate(king), Is.False);
            _DF.Register(king);
            Assert.That(_DF.Evaluate(king), Is.False);

            SmartEventListener sme = new SmartEventListener();
            _DF.ElementChanged += sme.Listener;
            //_DF.AllChanged += sme.Listener;
            sme.SetExpectation(new ObjectAttributeChangedArgs<bool>(king, null, false, true));

            king.Value = 1;
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.IsOk, Is.True);

            sme.NoExpectation();
            king.Value = 3;
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.IsWaiting, Is.True);

            sme.NoExpectation();
            _DF.FilterExpression = (o) => o.Name.StartsWith("Too");
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.LastReceived, Is.Null);
            //Assert.That(sme.LastReceived, Is.Not.Null);
            //Assert.That(sme.LastReceived is GroupedValueChangedArgs<MyObject, bool>, Is.True);
            //GroupedValueChangedArgs<MyObject, bool> ev = sme.LastReceived as GroupedValueChangedArgs<MyObject, bool>;
            //ObjectAttributeChangedArgs<bool> expected = ev.GetChangesFor(king);
            //Assert.That(expected, Is.Null);


            sme.NoExpectation();
            king.Value = 0;
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.IsWaiting, Is.True);

            sme.SetExpectation(new ObjectAttributeChangedArgs<bool>(king, null, true, false));
            king.Name = "gg";
            Assert.That(_DF.Evaluate(king), Is.False);
            Assert.That(sme.IsOk, Is.True);

            sme.NoExpectation();
            _DF.FilterExpression = (o) => true;
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.LastReceived, Is.Not.Null);
            //Assert.That(sme.LastReceived is GroupedValueChangedArgs<MyObject, bool>, Is.True);
            //ev = sme.LastReceived as GroupedValueChangedArgs<MyObject, bool>;
            //expected = ev.GetChangesFor(king);
            ObjectAttributeChangedArgs<bool> expected = sme.LastReceived as ObjectAttributeChangedArgs<bool>;
            Assert.That(expected, Is.EqualTo(new ObjectAttributeChangedArgs<bool>(king, null, false, true)));

            Assert.That(king.IsObserved, Is.False);

            sme.NoExpectation();
            _DF.FilterExpression = (o) => o.Value != 0;
            Assert.That(_DF.Evaluate(king), Is.False);
            Assert.That(sme.LastReceived, Is.Not.Null);
            //Assert.That(sme.LastReceived is GroupedValueChangedArgs<MyObject, bool>, Is.True);
            //ev = sme.LastReceived as GroupedValueChangedArgs<MyObject, bool>;
            //expected = ev.GetChangesFor(king);
            expected = sme.LastReceived as ObjectAttributeChangedArgs<bool>;
            Assert.That(expected, Is.EqualTo(new ObjectAttributeChangedArgs<bool>(king, null, true, false)));

            Assert.That(king.IsObserved, Is.True);

            sme.SetExpectation(new ObjectAttributeChangedArgs<bool>(king, null, false, true));
            king.Value = 1;
            Assert.That(_DF.Evaluate(king), Is.True);
            Assert.That(sme.IsOk, Is.True);
        }


        
    }
}
