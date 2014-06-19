//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using NUnit.Framework;

//using MusicCollectionTest.TestObjects;
//using MusicCollection.ToolBox;
//using MusicCollection.ToolBox.Collection;
//using MusicCollection.Infra;

//namespace MusicCollectionTest.ToolBox
//{
//    [TestFixture]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("ToolBox")]
//    class LifeCycleCollectionTester
//    {

//        private LifeCycleCollection<string, MyObject> _Collection;
//        private LifeCycleCollection<string, MyObject> _CleverCollection;
//        private LifeCycleCollection<string, MyObject> _TCollection;
//        private LifeCycleCollection<string, MyDummyObject> _DummyCollection;
//        private List<string> _CleverExpected;
//        private List<string> _TExpected;
//        private List<MyObject> _Elements;
//        private List<string> _Keys;


//        private MyObject _Un;
//        private MyObject _Deux;
//        private MyObject _Trois;
//        private MyObject _Quatre;
//        private MyObject _Cinq;
//        private MyObject _Six;

//        private MyDummyObject _DUn;
//        private MyDummyObject _DDeux;
//        private MyDummyObject _DTrois;
//        private MyDummyObject _DQuatre;
//        private MyDummyObject _DCinq;
//        private MyDummyObject _DSix;

//        private const string _1n="a";
//        private const string  _2n="b";
//        private const string _3n="c";
//        private const string _4n="d";
//        private const string _5n="e";
//        private const string _6n = "f";

//        //[SetUpFixture]
//        //public void GlobalSetUp()
//        //{
//        //}


//        [SetUp]
//        public void SetUp()
//        {
//            _Collection = new LifeCycleCollection<string, MyObject>(o=>o.Name);
//            _Un = new MyObject(_1n, 1);
//            _Deux = new MyObject(_2n, 2);
//            _Trois = new MyObject(_3n, 3);
//            _Quatre = new MyObject(_4n, 4);
//            _Cinq = new MyObject(_5n, 5);
//            _Six = new MyObject(_6n, 6);

//            _Collection.Register(_Un);
//            _Collection.Register(_Deux);
//            _Collection.Register(_Trois);
//            _Collection.Register(_Quatre);
//            _Collection.Register(_Cinq);
//            _Collection.Register(_Six);

//            _CleverCollection = new LifeCycleCollection<string, MyObject>(o => string.Format("{0} {1} {2}", o.Name,o.Value,_Un.Name));
//            _CleverCollection.Register(_Un);
//            _CleverCollection.Register(_Deux);
//            _CleverCollection.Register(_Trois);
//            _CleverCollection.Register(_Quatre);
//            _CleverCollection.Register(_Cinq);
//            _CleverCollection.Register(_Six);

//            _CleverExpected = new List<string>();
//            _CleverExpected.Add("a 1 a");
//            _CleverExpected.Add("b 2 a");
//            _CleverExpected.Add("c 3 a");
//            _CleverExpected.Add("d 4 a");
//            _CleverExpected.Add("e 5 a");
//            _CleverExpected.Add("f 6 a");

//            _TCollection = new LifeCycleCollection<string, MyObject>(o => o.Name, n => string.Format("< {0} >",n));
//            _TCollection.Register(_Un);
//            _TCollection.Register(_Deux);
//            _TCollection.Register(_Trois);
//            _TCollection.Register(_Quatre);
//            _TCollection.Register(_Cinq);
//            _TCollection.Register(_Six);

//            _TExpected = new List<string>();
//            _TExpected.Add("< a >");
//            _TExpected.Add("< b >");
//            _TExpected.Add("< c >");
//            _TExpected.Add("< d >");
//            _TExpected.Add("< e >");
//            _TExpected.Add("< f >");

            

//            _DummyCollection = new LifeCycleCollection<string, MyDummyObject>(o => o.Name);
//            _DUn = new MyDummyObject(_1n, 1);
//            _DDeux = new MyDummyObject(_2n, 0);
//            _DTrois = new MyDummyObject(_3n, 3);
//            _DQuatre = new MyDummyObject(_4n, 4);
//            _DCinq = new MyDummyObject(_5n, 5);
//            _DSix = new MyDummyObject(_6n, 8);

//            _DummyCollection.Register(_DUn);
//            _DummyCollection.Register(_DDeux);
//            _DummyCollection.Register(_DTrois);
//            _DummyCollection.Register(_DQuatre);
//            _DummyCollection.Register(_DCinq);
//            _DummyCollection.Register(_DSix);

//            _Elements = new List<MyObject>();

//            _Elements.Add(_Un); _Elements.Add(_Deux); _Elements.Add(_Trois); _Elements.Add(_Quatre); _Elements.Add(_Cinq); _Elements.Add(_Six);

//            _Keys=new List<string>();
//            _Keys.AddRange(from el in _Elements orderby el.Name select el.Name);
  
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void SingleKey()
//        {
//            MyObject gus = new MyObject(_4n, 90);
//            _Collection.Register(gus);
//        }

//        [Test]
//        public void BasicTest()
//        {
//             //TValue Find(TKey key);
//              //TValue Find(TValue key);

//            Assert.That(_Collection.Count, Is.EqualTo(6));

//            foreach(MyObject o in _Elements)
//                Assert.That(_Collection.Find(o), Is.EqualTo(o));

//            Assert.That(_Collection.Find(_1n), Is.EqualTo(_Un));
//            Assert.That(_Collection.Find(_2n), Is.EqualTo(_Deux)); 
//            Assert.That(_Collection.Find(_3n), Is.EqualTo(_Trois)); 
//            Assert.That(_Collection.Find(_4n), Is.EqualTo(_Quatre));
//            Assert.That(_Collection.Find(_5n), Is.EqualTo(_Cinq));
//            Assert.That(_Collection.Find(_6n), Is.EqualTo(_Six));

//            //     IEnumerable<TValue> Values { get; }
//            //IEnumerable<TKey> Keys { get; }

//            Assert.That(_Collection.Keys.OrderBy(o=>o).SequenceEqual(_Keys),Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o=>o.Name)), Is.True);

//           Assert.That(_Collection.Find( "aasss"),Is.Null);
//           MyObject gus = new MyObject("aasssssss", 90);
//           Assert.That(_Collection.Find(gus),Is.Null);          
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void DuplicateKeys()
//        {
//            _Un.Name="b";
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void InsertRemoved()
//        {
//            MyObject gus = new MyObject("aasss", 90);
//            IObjectStateCycle a = gus;
//            a.SetInternalState( ObjectState.Removed,null);
//            _Collection.Register(gus);
//        }

//        [Test]
//        public void TransformTest()
//        {
//            Assert.That(_TCollection.Keys.OrderBy(o => o).SequenceEqual(_TExpected.OrderBy(o => o)), Is.True);
//            Assert.That(_TCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            MyObject res = _TCollection.Find("a");
//            Assert.That(res, Is.EqualTo(_Un));
//            res = _TCollection.Find("z");
//            Assert.That(res, Is.Null);

//            _Un.Name = "z";

//            res = _TCollection.Find("a"); 
//            Assert.That(res, Is.Null);      
//            res = _TCollection.Find("z");
//           Assert.That(res, Is.EqualTo(_Un));
//        }

//        [Test]
//        public void CleverTest()
//        {
//            Assert.That(_CleverCollection.Keys.OrderBy(o => o).SequenceEqual(_CleverExpected.OrderBy(o => o)), Is.True);
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            MyObject res = _CleverCollection.Find("a 1 a");
//            Assert.That(res, Is.EqualTo(_Un));

//            _Un.Name = "Geg'";

//            res = _CleverCollection.Find("a 1 a");
//            Assert.That(res, Is.Null);
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _CleverCollection.Find("Geg' 1 Geg'");
//            Assert.That(res, Is.EqualTo(_Un));
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _CleverCollection.Find("b 2 Geg'");
//            Assert.That(res, Is.EqualTo(_Deux));

//            res = _CleverCollection.Find("d 4 Geg'");
//            Assert.That(res, Is.EqualTo(_Quatre));

//            _Deux.Name = "2";
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _CleverCollection.Find("b 2 Geg'");
//            Assert.That(res, Is.Null);
            
//            res = _CleverCollection.Find("2 2 Geg'");
//            Assert.That(res, Is.EqualTo(_Deux));

//            _Deux.Name = "b";
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _CleverCollection.Find("b 2 Geg'");
//            Assert.That(res, Is.EqualTo(_Deux));


//            res = _CleverCollection.Find("2 2 Geg'");
//            Assert.That(res, Is.Null);

//            _Un.Name="a";
//            Assert.That(_CleverCollection.Keys.OrderBy(o => o).SequenceEqual(_CleverExpected.OrderBy(o => o)), Is.True);
//            Assert.That(_CleverCollection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _CleverCollection.Find("a 1 a");
//            Assert.That(res, Is.EqualTo(_Un));


//            //_CleverCollection = new LifeCycleCollection<string, MyObject>(o => string.Format("{0} {1} {2}", o.Name,o.Value,_Un.Name));
 
//        }

//            //   
//            //_CleverCollection.Register(_Un);
//            //_CleverCollection.Register(_Deux);
//            //_CleverCollection.Register(_Trois);
//            //_CleverCollection.Register(_Quatre);
//            //_CleverCollection.Register(_Cinq);
//            //_CleverCollection.Register(_Six);

//        [Test]
//        public void Register()
//        {
//            MyObject gus = new MyObject("aasss", 90);
//             _Collection.Register(gus);


//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Find("aasss"), Is.EqualTo(gus));
//            _Elements.Add(gus);
//            _Keys.Add(gus.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            gus = new MyObject("aahhhsss", 90);
//            _Collection.Register(gus);


//            Assert.That(_Collection.Count, Is.EqualTo(8));
//            Assert.That(_Collection.Find("aahhhsss"), Is.EqualTo(gus));
//            _Elements.Add(gus);
//            _Keys.Add(gus.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void RegisterBasicError()
//        {
//            MyObject gus = new MyObject("aahhhsss", 90);
//            _Collection.Register(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Find("aahhhsss"), Is.EqualTo(gus));
    
//            _Collection.Register(gus);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void RegisterError()
//        {
//            MyObject gus = new MyObject("aahhhsss", 90);
//            _Collection.Register(gus);


//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Find("aahhhsss"), Is.EqualTo(gus));
//            _Keys.Add(gus.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);

//            gus.Name = "aahhrrrshsss";
//            _Collection.Register(gus);
//        }

//        [Test]
//        public void UpdateCapacity()
//        {
//            MyObject gus = new MyObject("aahhhsss", 90);
//            _Collection.Register(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Find("aahhhsss"), Is.EqualTo(gus));
//            _Keys.Add(gus.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);

//            gus.Name = "qq";
//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Find("qq"), Is.EqualTo(gus));
//            _Keys.Add(gus.Name);
//            _Keys.Remove("aahhhsss");
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//        }

//        [Test]
//        public void UpdateCapacityFailedInDummy()
//        {
//            MyDummyObject gus = new MyDummyObject("aahhhsss", 90);
//            _DummyCollection.Register(gus);

//            Assert.That(_DummyCollection.Count, Is.EqualTo(7));
//            Assert.That(_DummyCollection.Find("aahhhsss"), Is.EqualTo(gus));
//            _Keys.Add(gus.Name);
//            Assert.That(_DummyCollection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);

//            gus.Name = "qq";
//            Assert.That(_DummyCollection.Count, Is.EqualTo(7));
//            Assert.That(_DummyCollection.Find("qq"), Is.Null);
//        }

//        //[Test]
//        //[ExpectedException(typeof(ArgumentException))]
//        //public void DummyRegisterError()
//        //{
//        //    Assert.That(_DummyCollection.Count, Is.EqualTo(6));
//        //    Assert.That(_DummyCollection.Find(_1n), Is.EqualTo(_DUn));
//        //    Assert.That(_DummyCollection.Find(_2n), Is.EqualTo(_DDeux));
//        //    Assert.That(_DummyCollection.Find(_3n), Is.EqualTo(_DTrois));
//        //    Assert.That(_DummyCollection.Find(_4n), Is.EqualTo(_DQuatre));
//        //    Assert.That(_DummyCollection.Find(_5n), Is.EqualTo(_DCinq));
//        //    Assert.That(_DummyCollection.Find(_6n), Is.EqualTo(_DSix));

//        //    MyDummyObject gus = new MyDummyObject("aahhhsss", 90);
//        //    _DummyCollection.Register(gus);


//        //    Assert.That(_DummyCollection.Count, Is.EqualTo(7));
//        //    Assert.That(_DummyCollection.Find("aahhhsss"), Is.EqualTo(gus));
//        //    _Keys.Add(gus.Name);
//        //    Assert.That(_DummyCollection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);

//        //    gus.Name = "aahhshsss";
//        //    _DummyCollection.Register(gus);   
//        //}

//        [Test]
//        public void Remove()
//        {
//            //bool Remove(TKey key);
//            //bool Remove(TValue key);


//            bool res = _Collection.Remove(_Un.Name);
//            Assert.That(res, Is.True);
//            Assert.That(_Collection.Count, Is.EqualTo(5));
//            Assert.That(_Collection.Find(_Un), Is.Null);
//            Assert.That(_Collection.Find(_Un.Name), Is.Null);
//            _Elements.Remove(_Un);
//            _Keys.Remove(_Un.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _Collection.Remove(_Deux);
//            Assert.That(res, Is.True);
//            Assert.That(_Collection.Count, Is.EqualTo(4));
//            Assert.That(_Collection.Find(_Deux), Is.Null);
//            Assert.That(_Collection.Find(_Deux.Name), Is.Null);
//            _Elements.Remove(_Deux);
//            _Keys.Remove(_Deux.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            (_Deux as IObjectStateCycle).SetInternalState ( ObjectState.Removed,null);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);

//            res = _Collection.Remove(_Deux);
//            Assert.That(res, Is.False);
   
//        }

//        [Test]
//        public void FindOrCreate()
//        {
//            //TValue FindOrCreate(TKey key,Func<TKey,TValue> Constructor);
       
//            string name="aahhshsss";
//            MyObject nouveau = null;

//            MyObject res = _Collection.FindOrCreate(name, n => { nouveau = new MyObject(n, 40); return nouveau; });

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            _Elements.Add(nouveau);
//            _Keys.Add(nouveau.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(res, Is.EqualTo(nouveau));

//            nouveau = null;
//            res = _Collection.FindOrCreate(_5n, n => { nouveau = new MyObject(n, 40); return nouveau; });
//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(res, Is.EqualTo(_Cinq));
//            Assert.That(nouveau, Is.Null);


//        }

//        public void FindOrCreateValue()
//        {
//            //Tuple<TValue, bool> FindOrCreateValue(TKey key, Func<TKey, TValue> Constructor);

//            string name = "aahhshsss";
//            MyObject nouveau = null;

//            Tuple<MyObject, bool> res = _Collection.FindOrCreateValue(name, n => { nouveau = new MyObject(n, 40); return nouveau; });

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            _Elements.Add(nouveau);
//            _Keys.Add(nouveau.Name);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(res.Item1, Is.EqualTo(nouveau));
//            Assert.That(res.Item2, Is.False);

//            nouveau = null;
//            res = _Collection.FindOrCreateValue(_5n, n => { nouveau = new MyObject(n, 40); return nouveau; });
//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys.OrderBy(o => o)), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
//            Assert.That(res.Item1, Is.EqualTo(_Cinq));
//            Assert.That(res.Item2, Is.True);
//            Assert.That(nouveau, Is.Null);


//        }

        

//        [Test]
//        public void LifeCycle()
//        {
//            ObjectState os = _Deux.Break();
//            Assert.That(os, Is.EqualTo(ObjectState.FileNotAvailable));

//            //(_Deux as IObjectStateCycle).State = ObjectState.FileNotAvailable;

//            Assert.That(_Collection.Count, Is.EqualTo(6));
//            Assert.That(_Collection.Find(_Deux), Is.EqualTo(_Deux));
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);


//            (_Deux as IObjectStateCycle).SetInternalState ( ObjectState.Removed,null);

//            Assert.That(_Collection.Count, Is.EqualTo(5));
//            Assert.That(_Collection.Find(_Deux), Is.Null);
//            _Elements.Remove(_Deux);
//            _Keys.Remove(_2n);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);


//            bool res =_Collection.Remove(_Deux);
//            Assert.That(res, Is.False);
//            Assert.That(_Collection.Count, Is.EqualTo(5));
//            Assert.That(_Collection.Find(_Deux), Is.Null);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);


//            res = _Collection.Remove(_Trois);
//            Assert.That(res, Is.True);
//            Assert.That(_Collection.Count, Is.EqualTo(4));
//            Assert.That(_Collection.Find(_Trois), Is.Null);
//            _Elements.Remove(_Trois);
//            _Keys.Remove(_3n);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);


//            (_Trois as IObjectStateCycle).SetInternalState(ObjectState.Removed,null);
//            Assert.That(_Collection.Count, Is.EqualTo(4));
//            Assert.That(_Collection.Find(_Trois), Is.Null);
//            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
//            Assert.That(_Collection.Values.OrderBy(o => o.Name).SequenceEqual(_Elements.OrderBy(o => o.Name)), Is.True);
   

//        }

//        [Test]
//        public void findorregister1()
//        {
//            //TValue FindOrRegister(TValue value);

//            MyObject gus = new MyObject(_4n, 90);
//            MyObject gus2 = _Collection.FindOrRegister(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(6));
//            Assert.That(gus2, Is.EqualTo(_Quatre));
//            Assert.That(_Collection.Find(_4n), Is.EqualTo(_Quatre));
//            Assert.That(_Collection.Find(gus), Is.EqualTo(_Quatre));

//            gus = new MyObject("zz", 9);
//            gus2 = _Collection.FindOrRegister(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(gus2, Is.EqualTo(gus));
//        }

//        [Test]
//        public void findorregister2()
//        {
//            //TValue FindOrRegisterValue(TValue value);

//            MyObject gus = new MyObject(_4n, 90);
//            Tuple<MyObject,bool> gus2 = _Collection.FindOrRegisterValue(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(6));
//            Assert.That(gus2.Item1, Is.EqualTo(_Quatre));
//            Assert.That(gus2.Item2, Is.True);
//            Assert.That(_Collection.Find(_4n), Is.EqualTo(_Quatre));
//            Assert.That(_Collection.Find(gus), Is.EqualTo(_Quatre));

//            gus = new MyObject("zz", 9);
//            gus2 = _Collection.FindOrRegisterValue(gus);

//            Assert.That(_Collection.Count, Is.EqualTo(7));
//            Assert.That(gus2.Item1, Is.EqualTo(gus));
//            Assert.That(gus2.Item2, Is.False);
//        }
//    }
//}
