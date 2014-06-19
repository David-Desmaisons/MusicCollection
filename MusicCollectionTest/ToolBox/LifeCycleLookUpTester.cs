//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using NUnit.Framework;

//using MusicCollection.ToolBox;
//using MusicCollection.ToolBox.Collection;
//using MusicCollection.Implementation;
//using MusicCollection.Infra;
//using MusicCollectionTest.TestObjects;

//namespace MusicCollectionTest.ToolBox
//{
//    [TestFixture]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("ToolBox")]
//    internal class LifeCycleLookUpTester
//    {


//        private LifeCycleLookUp<string, MyObject> _LookUp;
//        private LifeCycleLookUp<int, MyObject> _LookUp2;
//        private LifeCycleLookUp<string, MyObject> _LookUp3;
//        private LifeCycleLookUp<int, MyObject> _LookUp4;

//        private MyObject _Un;
//        private MyObject _Deux;
//        private MyObject _Trois;
//        private MyObject _Quatre;
//        private MyObject _Cinq;
//        private List<string> _Keys;
//        private List<int> _IntKeys;
//        private List<MyObject> _Elements;

//        [SetUp]
//        public void SetUp()
//        {
//            _LookUp = new LifeCycleLookUp<string, MyObject>(o => o.Name,10);
//            _LookUp2 = new LifeCycleLookUp<int, MyObject>(o => o.Value,10);
//            _LookUp3 = new LifeCycleLookUp<string, MyObject>(o => string.Format("{0} {1}", o.Name, o.Value),10);
//            _LookUp4 = new LifeCycleLookUp<int, MyObject>(o => o.Name.Length,10);

//            _Un = new MyObject("a", 0);
//            _Deux = new MyObject("b", 1);
//            _Trois = new MyObject("c", 2);
//            _Quatre = new MyObject("d", 3);
//            _Cinq = new MyObject("e", 4);

//            _LookUp.Add(_Un);
//            _LookUp.Add(_Deux);
//            _LookUp.Add(_Trois);
//            _LookUp.Add(_Quatre);
//            _LookUp.Add(_Cinq);

//            _LookUp2.Add(_Un);
//            _LookUp2.Add(_Deux);
//            _LookUp2.Add(_Trois);
//            _LookUp2.Add(_Quatre);
//            _LookUp2.Add(_Cinq);

//            _LookUp3.Add(_Un);
//            _LookUp3.Add(_Deux);
//            _LookUp3.Add(_Trois);
//            _LookUp3.Add(_Quatre);
//            _LookUp3.Add(_Cinq);

//            _LookUp4.Add(_Un);
//            _LookUp4.Add(_Deux);
//            _LookUp4.Add(_Trois);
//            _LookUp4.Add(_Quatre);
//            _LookUp4.Add(_Cinq);

//            _Elements = new List<MyObject>();
//            _Elements.Add(_Un); _Elements.Add(_Deux); _Elements.Add(_Trois); _Elements.Add(_Quatre); _Elements.Add(_Cinq);


//            _Keys = new List<string>();
//            _Keys.AddRange(from el in _Elements orderby el.Name select el.Name);

//            _IntKeys = new List<int>();
//            _IntKeys.AddRange(from el in _Elements orderby el.Value select el.Value);

//        }

//        private void TestLookUpListRes<T, TK, TV>(LookUpImpl<TK, T> entry, TK Entry, List<T> res, Func<T, TV> keySelector) where T : class
//        {
//            var myobj = entry[Entry].ToList();
//            Assert.That(myobj.Count, Is.EqualTo(res.Count));
//            Assert.That(myobj.OrderBy(keySelector).SequenceEqual(res.OrderBy(keySelector)), Is.True);
//        }

//        private void TestLookUpMonoRes<T, TK>(LookUpImpl<TK, T> entry, TK Entry, T res) where T : class
//        {
//            var myobj = entry[Entry].ToList();
//            Assert.That(myobj.Count, Is.EqualTo(1));
//            Assert.That(myobj[0], Is.EqualTo(res));
//        }

//        private void TestLookUpNullRes<T, TK>(LookUpImpl<TK, T> entry, TK Entry) where T : class
//        {
//            var myobj = entry[Entry].ToList();
//            Assert.That(myobj.Count, Is.EqualTo(0));
//        }

//        [Test]
//        public void UpdateTest()
//        {
//            _Un.Name = "z";

//            TestLookUpNullRes(_LookUp, "a");
//            TestLookUpMonoRes(_LookUp, "b", _Deux);
//            TestLookUpMonoRes(_LookUp, "c", _Trois);
//            TestLookUpMonoRes(_LookUp, "d", _Quatre);
//            TestLookUpMonoRes(_LookUp, "e", _Cinq);
//            TestLookUpNullRes(_LookUp, "f");
//            TestLookUpMonoRes(_LookUp, "z", _Un);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);



//            _Deux.Name = "z";

//            TestLookUpNullRes(_LookUp, "a");
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpMonoRes(_LookUp, "c", _Trois);
//            TestLookUpMonoRes(_LookUp, "d", _Quatre);
//            TestLookUpMonoRes(_LookUp, "e", _Cinq);
//            TestLookUpNullRes(_LookUp, "f");

//            List<MyObject> res = new List<MyObject>();
//            res.Add(_Un); res.Add(_Deux);

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);


//            _Trois.Name = "z";

//            TestLookUpNullRes(_LookUp, "a");
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpMonoRes(_LookUp, "d", _Quatre);
//            TestLookUpMonoRes(_LookUp, "e", _Cinq);
//            TestLookUpNullRes(_LookUp, "f");

//            res.Add(_Trois);
//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

//            _Deux.Name = "d";

//            TestLookUpNullRes(_LookUp, "a");
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpMonoRes(_LookUp, "e", _Cinq);
//            TestLookUpNullRes(_LookUp, "f");

//            res.Remove(_Deux);
//            List<MyObject> res2 = new List<MyObject>();
//            res2.Add(_Quatre); res2.Add(_Deux);

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);


//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);


//            _Cinq.Name = "a";

//            TestLookUpMonoRes(_LookUp, "a", _Cinq);
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpNullRes(_LookUp, "e");
//            TestLookUpNullRes(_LookUp, "f");

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

//            (_Quatre as IObjectStateCycle).SetInternalState(ObjectState.Removed,null);

//            res2.Remove(_Quatre);
//            _Elements.Remove(_Quatre);

//            TestLookUpMonoRes(_LookUp, "a", _Cinq);
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpNullRes(_LookUp, "e");
//            TestLookUpNullRes(_LookUp, "f");

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

//            (_Un as IObjectStateCycle).SetInternalState( ObjectState.UnderEdit,null);

//            TestLookUpMonoRes(_LookUp, "a", _Cinq);
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpNullRes(_LookUp, "e");
//            TestLookUpNullRes(_LookUp, "f");

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

//            _LookUp.Remove(_Un);

//            res.Remove(_Un);
//            _Elements.Remove(_Un);

//            TestLookUpMonoRes(_LookUp, "a", _Cinq);
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpNullRes(_LookUp, "e");
//            TestLookUpNullRes(_LookUp, "f");

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

//            (_Un as IObjectStateCycle).SetInternalState( ObjectState.Removed,null);
//            TestLookUpMonoRes(_LookUp, "a", _Cinq);
//            TestLookUpNullRes(_LookUp, "b");
//            TestLookUpNullRes(_LookUp, "c");
//            TestLookUpNullRes(_LookUp, "e");
//            TestLookUpNullRes(_LookUp, "f");

//            TestLookUpListRes(_LookUp, "z", res, o => o.Value);
//            TestLookUpListRes(_LookUp, "d", res2, o => o.Value);

//            Assert.That((from o in _LookUp from el in o select el).OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
//            Assert.That(_LookUp.AllElements.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);




//        }
//    }
//}
