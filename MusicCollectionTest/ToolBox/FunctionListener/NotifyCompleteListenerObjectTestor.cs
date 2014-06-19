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
using MusicCollection.ToolBox.FunctionListener;


namespace MusicCollectionTest.ToolBox.FunctionListener
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class NotifyCompleteListenerObjectTestor
    {

        private class MyDummyListener : NotifyCompleteListenerObject
        {
            //private static IResultListenerFactory<MyDummyListener, string> _NE = GlobalFunctionListener.Register<MyDummyListener, string>(dl => string.Format("{0}-{1}", dl.Object.Name, dl.Object.Value), "MyFunnyName");
            internal MyDummyListener(MyObject o)
            {
                _Object = o;
            }

            public override string ToString()
            {
                return string.Format("Object:{0}",this.Object);
            }

            private MyObject _Object;

            public MyObject Object
            {
                get{return _Object;}
                set
                {
                    //if (object.ReferenceEquals(_Object,value))
                    //    return;
                    //var old = _Object;
                    //_Object = value;
                    //AttributeChanged("Object",old,value);
                    this.Set(ref _Object, value);
                }
            }

            public string MyFunnyName
            {
                get {return this.Get<MyDummyListener, string>(()=>dl => string.Format("{0}-{1}", dl.Object.Name, dl.Object.Value));}
                //get {return this.GetValue(_NE);}
            }
        }


        private class MyDummyListener0 : NotifyCompleteListenerObject
        {
            internal MyDummyListener0(MyObject o)
            {
                _Object = o;
            }

            public override string ToString()
            {
                return string.Format("Object:{0}", this.Object);
            }

            private MyObject _Object;

            public MyObject Object
            {
                get { return _Object; }
                set { this.Set(ref _Object, value); }
            }

            public string MyFunnyName
            {
                get { return Get<MyDummyListener0,string>(()=>(dl) => string.Format("{0}-{1}", dl.Object.Name, dl.Object.Value)); }
            }
        }

        private class MyDummyListener2 : MyDummyListener
        {
            internal MyDummyListener2(MyObject mo)
                : base(mo)
            {
            }
        }

        private class MyDummyListener3 : MyDummyListener2
        {
            internal MyDummyListener3(MyObject mo)
                : base(mo)
            {
            }

            //private static IResultListenerFactory<MyDummyListener3, int> _NEI = GlobalFunctionListener.Register<MyDummyListener3, int>(dl => dl.Object.Value * 2, "MyFunnyInt");

            public int MyFunnyInt
            {
                get { return Get<MyDummyListener3, int>(() => dl => dl.Object.Value * 2); }
                //get { return this.GetValue(_NEI); }
            }
        }

        private class MyDummyListener4 : MyDummyListener3
        {
            internal MyDummyListener4(MyObject mo):base(mo)
            {
            }
        }

        private List<MyObject> _LOs = new List<MyObject>();
        private EventListener<ObjectModifiedArgs> _EL=null;
        private EventListener<ObjectModifiedArgs> _EL0 = null;
        private EventListener<ObjectModifiedArgs> _EL4 = null;
        private MyDummyListener _DL = null;
        private MyDummyListener0 _DL0 = null;
        private MyDummyListener4 _DL4 = null;
       // private ObjectModifiedArgs _Expected = null;

        [SetUp]
        public void SetUp()
        {         
            _LOs.Add(new MyObject("Deus", 100));
            _LOs.Add(new MyObject("Evil", 10));
            _LOs.Add(new MyObject("Homen", 1));
            _LOs.Add(new MyObject("Chien", 25));

            _EL = new EventListener<ObjectModifiedArgs>();
            _EL0 = new EventListener<ObjectModifiedArgs>();
            _EL4 = new EventListener<ObjectModifiedArgs>();
            _DL = new MyDummyListener(_LOs[0]);
            _DL0 = new MyDummyListener0(_LOs[0]);
            _DL4 = new MyDummyListener4(_LOs[0]);

            _DL.ObjectChanged += _EL.SingleElementChangedListener;
            _DL0.ObjectChanged += _EL0.SingleElementChangedListener;
            _DL4.ObjectChanged += _EL4.SingleElementChangedListener;
        }

        [TearDown]
        public void SetDown()
        {
            _DL.Dispose();
            _DL0.Dispose();
            _DL4.Dispose();

            _LOs.Clear();
        }

        private void AssertEvent(ObjectModifiedArgs oma, EventListener<ObjectModifiedArgs> el, int evn)
        { 
           // int evc = el.EventCount;
            
            Assert.IsNotNull(oma);
            Assert.AreEqual(evn,el.EventCount);

            ObjectModifiedArgs ev = null;

            if (evn == 1)
            {
                ev = el.GetDeplieEvent();
            }
            else
            {
                var po = el.GetDeplieEvents().Where(o => o.AttributeName == oma.AttributeName).ToList();
                Assert.AreEqual(po.Count, 1);
                ev = po.First();
            }            
                
            Assert.IsNotNull(ev);
           
            Assert.AreEqual(ev.AttributeName, oma.AttributeName);
            Assert.AreEqual(ev.OldAttributeValue, oma.OldAttributeValue);
            Assert.AreEqual(ev.NewAttributeValue, oma.NewAttributeValue);
        }

        private ObjectModifiedArgs GetExpectations(object old, object newo, string MFN)
        {
            return new ObjectModifiedArgs(_DL, MFN,old,newo);
        }

        private void AssertEventsChanges(MyDummyListener MO, EventListener<ObjectModifiedArgs> EL, object old, object newo, string AN, int evn)
        {
            Assert.AreEqual(MO.MyFunnyName, newo);
            AssertEvent(GetExpectations(old, newo, AN), EL, evn);
        }

        private void AssertEventsChanges(MyDummyListener0 MO, EventListener<ObjectModifiedArgs> EL, object old, object newo, string AN, int evn)
        {
            Assert.AreEqual(MO.MyFunnyName, newo);
            AssertEvent(GetExpectations(old, newo, AN), EL, evn);
        }

        private void AssertEventChanges(string old, string newo, int evn = 1)
        {
            AssertEventsChanges(_DL,_EL,old,newo,"MyFunnyName",evn);
        }

        private void AssertEventChanges0(string old, string newo, int evn = 1)
        {
            AssertEventsChanges(_DL0, _EL0, old, newo, "MyFunnyName", evn);
        }

        private void AssertEventChanges2(string old, string newo, int evn = 1)
        {
            AssertEventsChanges(_DL4, _EL4,old, newo, "MyFunnyName", evn);
        }

        private void AssertEventChanges3(int old, int newo, int evn=1)
        {
            Assert.AreEqual(_DL4.MyFunnyInt, newo);
            AssertEvent(GetExpectations(old, newo, "MyFunnyInt"), _EL4, evn);
        }

        //private void AssertEventChanges4(string old, string newo, Nullable<int> evn = 1)
        //{
        //    Assert.AreEqual(_DL4.MyFunnyName, newo);
        //    AssertEvent(GetExpectations(old, newo, "MyFunnyName"), _EL4, evn);
        //}

        //private void AssertEventChanges5(int old, int newo, Nullable<int> evn = 1)
        //{
        //    Assert.AreEqual(_DL4.MyFunnyInt, newo);
        //    AssertEvent(GetExpectations(old, newo, "MyFunnyInt"), _EL4, evn);
        //}

        [Test]
        public void Test_Basic()
        {
            Assert.IsNull(_EL.GetDeplieEvent());
            Assert.AreEqual(_DL.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL.GetDeplieEvent());

            _LOs[0].Value = 100;
            Assert.AreEqual(_DL.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL.GetDeplieEvent());

            _LOs[0].Value = 1000;
            AssertEventChanges("Deus-100", "Deus-1000");

            _LOs[0].Name = "toto";
            AssertEventChanges("Deus-1000", "toto-1000");

            _DL.Object = _LOs[1];
            AssertEventChanges("toto-1000", "Evil-10",2);
        }

        [Test]
        public void Test_Basic_NewAPI()
        {
            Assert.IsNull(_EL0.GetDeplieEvent());
            Assert.AreEqual(_DL0.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL0.GetDeplieEvent());

            _LOs[0].Value = 100;
            Assert.AreEqual(_DL0.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL0.GetDeplieEvent());

            _LOs[0].Value = 1000;
            AssertEventChanges0("Deus-100", "Deus-1000");

            _LOs[0].Name = "toto";
            AssertEventChanges0("Deus-1000", "toto-1000");

            _DL0.Object = _LOs[1];
            AssertEventChanges0("toto-1000", "Evil-10", 2);
        }

        [Test]
        public void Test_Complexe_Derivation()
        {
            Assert.IsNull(_EL4.GetDeplieEvent());
            Assert.AreEqual(_DL4.MyFunnyInt, 200);
            Assert.AreEqual(_DL4.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 100;
            Assert.AreEqual(_DL4.MyFunnyInt, 200);
            Assert.IsNull(_EL.GetDeplieEvent());

            _LOs[0].Value = 1000;
            AssertEventChanges3(200, 2000, 2);

            _LOs[0].Value = 15;
            AssertEventChanges3(2000, 30, 2);

            _LOs[0].Name = "toto";
            _EL4.Clear();
            Assert.AreEqual(_DL4.MyFunnyInt, 30);
           // AssertEventChanges5("Deus-1000", "toto-1000");

            _DL4.Object = _LOs[1];
            AssertEventChanges3(30, 20, 3);
        }

        [Test]
        public void Test_Basic_Derivation()
        {
            Assert.IsNull(_EL4.GetDeplieEvent());
            Assert.AreEqual(_DL4.MyFunnyName, "Deus-100");

            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 100;
            Assert.AreEqual(_DL4.MyFunnyName, "Deus-100");
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 1000;
            AssertEventChanges2("Deus-100", "Deus-1000");

            _LOs[0].Name = "toto";
            AssertEventChanges2("Deus-1000", "toto-1000");

            _DL4.Object = _LOs[1];
            AssertEventChanges2("toto-1000", "Evil-10",2);
        }

        [Test]
        public void Test_Complexe_Derivation_Without_Listener()
        {
            Assert.IsNull(_EL4.GetDeplieEvent());
            Assert.AreEqual(_DL4.MyFunnyInt, 200);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _DL4.ObjectChanged -= _EL4.SingleElementChangedListener;

            _LOs[0].Value = 1;
            Assert.AreEqual(_DL4.MyFunnyInt, 2);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 1000;
            Assert.AreEqual(_DL4.MyFunnyInt, 2000);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 15;
            Assert.AreEqual(_DL4.MyFunnyInt, 30);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Name = "toto";
            Assert.AreEqual(_DL4.MyFunnyInt, 30);
            Assert.IsNull(_EL4.GetDeplieEvent());
 
            _DL4.Object = _LOs[1];
            Assert.AreEqual(_DL4.MyFunnyInt, 20);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value=100;
            _DL4.Object = _LOs[0];
            Assert.AreEqual(_DL4.MyFunnyInt, 200);
            Assert.IsNull(_EL4.GetDeplieEvent());

            //Add listener here
            _DL4.ObjectChanged += _EL4.SingleElementChangedListener;

            Assert.AreEqual(_DL4.MyFunnyInt, 200);
            Assert.IsNull(_EL4.GetDeplieEvent());

            _LOs[0].Value = 1000;
            AssertEventChanges3(200, 2000, 1);

            _LOs[0].Value = 15;
            AssertEventChanges3(2000, 30, 1);

            _DL4.Object = _LOs[1];
            AssertEventChanges3(30, 20, 2);
        }

    }
}
