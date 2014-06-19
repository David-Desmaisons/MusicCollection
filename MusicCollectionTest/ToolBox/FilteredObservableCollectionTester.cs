//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;

//using NUnit;
//using NUnit.Framework;

//using MusicCollectionTest.TestObjects;

//using MusicCollection.ToolBox;
//using MusicCollection.ToolBox.Collection;
//using MusicCollection.ToolBox.Collection.Observable;
//using MusicCollection.Infra;


//namespace MusicCollectionTest.ToolBox
//{
//    [TestFixture]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("ToolBox")]

//    internal class FilteredObservableCollectionTester : ObservableHelper
//    {
       

//        protected override void DisplayInformation()
//        {
//            Console.WriteLine("Original Collection: {0}", string.Join(",", _MyCollection));
//            Console.WriteLine("Filter Collection: {0}", string.Join(",", _MyfilteredCollection));
//            Console.WriteLine();
//        }

      


//        private ObservableCollection<MyObject> _MyCollection;
//        private MyObject _Un;
//        private MyObject _Deux;
//        private MyObject _Trois;
//        private MyObject _Quatre;
//        private MyObject _Cinq;
//        private MyObject _Six;
 
//        FilteredTransformReadOnlyObservableCollection<MyObject, string, int> _MyfilteredCollection = null;
  

//        [SetUp]
//        public void SetUp()
//        {
//            _MyCollection = new ObservableCollection<MyObject>();
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

//             _MyfilteredCollection = new FilteredTransformReadOnlyObservableCollection<MyObject, string, int>(_MyCollection, li => true, li => li.Name, li => li.Value);

//            InitializeAndRegisterCollection(_MyfilteredCollection);

//        }

//        [TearDown]
//        public void SetDown()
//        {


//        }



//        [Test]
//        public void BasicTest()
//        {
//            DisplayInformation();

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Deux.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Trois.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Quatre.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Cinq.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Six.Name), Is.True);

//            using (Transaction())
//            {
//                 _MyfilteredCollection.Filter += n => false;
//            }

           
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(0));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyfilteredCollection.Filter += n => false;
//            }

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(0));
//            AssertNonEvent();

//        }

       

      


//        [Test]
//        public void EventHandlingRemovedItem()
//        {
//            DisplayInformation();

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);

//            using (Transaction())
//            {
//                _MyCollection.Remove(_Un);
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertResetEvent();


//            using (Transaction())
//            {
//                _MyCollection.Add(_Un);
//            }


//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();


//            using (Transaction())
//            {
//                _MyCollection.Add(_Un);
//            }


//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();


//            MyObject jok = new MyObject("zz", 10);

//            using (Transaction())
//            {
//                _MyCollection.Add(jok);
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(7));
//            AssertResetEvent();

//        }

//        [Test]
//        public void EventHandlingPropertyChanged()
//        {
//            DisplayInformation();

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True); 
            
//            string originalun = _Un.Name;

//            using (Transaction())
//            {
//                _Un.Name = "cc";
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(originalun), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();


//            originalun = _Un.Name;
//            using (Transaction())
//            {
//                _Un.Name = _Deux.Name;
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(originalun), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertResetEvent();



//            MyObject jok = new MyObject("zz", 10);

//            using (Transaction())
//            {
//                _MyCollection.Add(jok);
//            }
           

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();

//            string originalsix = jok.Name;
  
//            using (Transaction())
//            {
//                jok.Name = _Six.Name;
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(originalsix), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertResetEvent();

//            originalsix = jok.Name;
//            using (Transaction())
//            {
//                jok.Name = "44";
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(originalsix), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyCollection.Remove(jok);
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                jok.Name = "dd";
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertNonEvent();
         
//        }

//        [Test]
//        public void EventHandlingRemovedPropertyChanged()
//        {
//            DisplayInformation();

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);

//            MyObject jok = new MyObject("zz", 10);
//            using (Transaction())
//            {
//                _MyCollection.Add(jok);
//            }
            
//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(7));

//            AssertResetEvent();

//            string old = jok.Name;
//            using (Transaction())
//            {
//                jok.Name = _Trois.Name;
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(old), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyCollection.Remove(_Trois);
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Deux.Name), Is.True);
       
//            using (Transaction())
//            {
//                _MyCollection.Remove(jok);
//            }


//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                jok.Name = _Six.Name;
//                _MyCollection.Add(jok);
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                jok.Name = _Un.Name;
//                _MyCollection.Add(jok);
//            }

//            Assert.That(_MyfilteredCollection.Contains(jok.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                _MyCollection.Remove(jok);
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(5));
//            AssertNonEvent();

//        }

//        [Test]
//        public void FilterChanged()
//        {
//            DisplayInformation();

//            using (Transaction())
//            {
//                _MyfilteredCollection.Filter = n => n.Name.Contains("a");
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Contains(_Quatre.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Contains(_Cinq.Name), Is.False);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(4));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyfilteredCollection.Filter = n => ( n.Name.Contains("a") && (n.Name.Contains("b")));
//            }

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(2));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyCollection.Remove(_Un);
//            }

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(2));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                _MyCollection.Remove(_Six);
//            }

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(1));

//            AssertResetEvent();

//        }

//        [Test]
//        public void ComplexChanged()
//        {
//            DisplayInformation();

//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);

//            using (Transaction())
//            {
//                _MyCollection.Add(_Un);
//                Assert.That(_MyCollection.Count, Is.EqualTo(7));
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                _Un.Name = "66";
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();
 
//            using (Transaction())
//            {
//                _MyCollection.Remove(_Un);
//                Assert.That(_MyCollection.Count, Is.EqualTo(6));
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();

//            using (Transaction())
//            {
//                _Un.Name = "zz";
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();

//            using (Transaction())
//            {
//                _MyCollection.Add(_Un);
//                Assert.That(_MyCollection.Count, Is.EqualTo(7));
//            }


//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertNonEvent();

   
//            using (Transaction())
//            {
//                _Un.Name = "77";
//            }

//            Assert.That(_MyfilteredCollection.Contains(_Un.Name), Is.True);
//            Assert.That(_MyfilteredCollection.Count, Is.EqualTo(6));
//            AssertResetEvent();

//        }
//    }
//}
