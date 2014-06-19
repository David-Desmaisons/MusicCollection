//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using NUnit.Framework;

//using MusicCollectionTest.TestObjects;

//using MusicCollection.ToolBox;
//using MusicCollection.Infra;
//using MusicCollection.ToolBox.Collection;
//using MusicCollection.ToolBox.Collection.Observable;

//namespace MusicCollectionTest.ToolBox
//{
//    [TestFixture]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("ToolBox")]
//    class LifeCycleObservableCollectionTester : ObservableHelper
//    {


//        private LifeCycleObservableCollection<IObjectStateCycle> _MyCollection;
//        private IObjectStateCycle _Un;
//        private IObjectStateCycle _Deux;
//        private IObjectStateCycle _Trois;
//        private IObjectStateCycle _Quatre;
//        private IObjectStateCycle _Cinq;
//        private IObjectStateCycle _Six;

//        [SetUp]
//        public void SetUp()
//        {
//            _MyCollection = new LifeCycleObservableCollection<IObjectStateCycle>();
//            _Un = new MyObject("aa", 1);
//            _Deux = new MyObject("a", 0);
//            _Trois = new MyObject("ab", 3);
//            _Quatre = new MyObject("b", 4);
//            _Cinq = new MyObject("bb", 5);
//            _Six = new MyObject("ba", 8);
//            _MyCollection.Add(_Un);
//            _MyCollection.Add(_Deux);
//            _MyCollection.Add(_Trois);
//            _MyCollection.Add(_Quatre);
//            _MyCollection.Add(_Cinq);
//            _MyCollection.Add(_Six);
//            InitializeAndRegisterCollection(_MyCollection);
//        }


//        [Test]
//        public void BasicTest()
//        {
//            DisplayInformation();
//            Assert.That(_MyCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyCollection[0], Is.EqualTo(_Un));
//            Assert.That(_MyCollection[1], Is.EqualTo(_Deux));
//            Assert.That(_MyCollection[2], Is.EqualTo(_Trois));
//            Assert.That(_MyCollection[3], Is.EqualTo(_Quatre));
//            Assert.That(_MyCollection[4], Is.EqualTo(_Cinq));
//            Assert.That(_MyCollection[5], Is.EqualTo(_Six));

//            MyObject newo=new MyObject("wba", 8);

//            using(Transaction())
//            {
//                _MyCollection.Add(newo);
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(7));
//            AssertAddEvent(newo, 6);

//            using (Transaction())
//            {
//                _MyCollection.Remove(_Trois);
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(6));
//            AssertRemoveEvent(_Trois, 2);

//            using (Transaction())
//            {
//                _Trois.SetInternalState(ObjectState.Removed,null);
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                ObjectState os = (_Deux as MyObject).Break();
//                 Assert.That(os, Is.EqualTo(ObjectState.FileNotAvailable));
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();


//            using (Transaction())
//            {
//                _Deux.SetInternalState( ObjectState.Removed,null);
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(5));
//            AssertRemoveEvent(_Deux, 1);


//            using (Transaction())
//            {
//                _MyCollection.Move(0, 4);
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(5));
//            AssertMoveEvent(0,4);

//            using (Transaction())
//            {
//                _MyCollection.Clear();
//            }

//            Assert.That(_MyCollection.Count, Is.EqualTo(0));
//            AssertResetEvent();

//        }

//        protected override void DisplayInformation()
//        {
//            Console.WriteLine("Original Collection: {0}", string.Join(",", _MyCollection));
//            Console.WriteLine();
//        }

//    }
//}
