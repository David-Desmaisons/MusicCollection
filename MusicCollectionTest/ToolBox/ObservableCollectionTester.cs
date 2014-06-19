//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using  System.Collections.Specialized;
//using System.ComponentModel;
//using System.Collections.ObjectModel;

//using NUnit;
//using NUnit.Framework;

//using MusicCollection.ToolBox;
//using MusicCollectionTest.TestObjects;
//using MusicCollection.ToolBox.Web;
//using MusicCollection.ToolBox.Collection.Observable;
//using MusicCollection.Infra;


//namespace MusicCollectionTest.ToolBox
//{
//    [TestFixture]
//    [NUnit.Framework.Category("Unitary")]
//    [NUnit.Framework.Category("ToolBox")]
//    public class ObservableCollectionTester
//    {

//        private class ListTester:IDisposable
//        {
//            private ObservableCollection<int> _Model;
//            private IList<int> _Copy;


//            internal int CallBackCount
//            {
//                get;
//                private set;
//            }

//            internal ObservableCollection<int> Model
//            {
//                get { return _Model; }
//            }

//            internal bool CheckEquality
//            {
//                get { return _Copy.SequenceEqual(_Model); }
//            }

//            internal ListTester()
//            {
//                _Model = new ObservableCollection<int>();
//                _Copy = new List<int>();
//                CallBackCount = 0;
//                _Model.CollectionChanged+=new NotifyCollectionChangedEventHandler(_Model_CollectionChanged);
//            }

//            private void _Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs nce)
//            {
//                CallBackCount++;
//                _Copy.ApplyChanges(nce);
//            }

//            public void Dispose()
//            {
//                _Model.CollectionChanged -= new NotifyCollectionChangedEventHandler(_Model_CollectionChanged);
//            }
//        }

//        private ListTester _LT;
//        private ListObservaleTester _LTT;

//        [SetUp]
//        public void SetUp()
//        {
//            _LT = new ListTester();
//            _LTT = new ListObservaleTester();
//        }

//        [TearDown]
//        public void SetDown()
//        {
//            _LT.Dispose();
//            _LT = null;

//            _LTT.Dispose();
//            _LTT = null;
//        }

//        private void Check(int Count)
//        {
//            Assert.That(_LT.CallBackCount, Is.EqualTo(Count));
//            Assert.That(_LT.CheckEquality, Is.True);
//        }

//        [Test]
//        public void BasicTestApplyChangesExtension()
//        {
//            Check(0);

//            _LT.Model.Add(1);
//            Check(1);

//            _LT.Model.Add(2);
//            Check(2);

//            _LT.Model.Add(100);
//            Check(3);

//            _LT.Model.Remove(2);
//            Check(4);

//            _LT.Model.RemoveAt(1);
//            Check(5);

//            _LT.Model.Add(100);
//            Check(6);

//            _LT.Model.Add(1000);
//            Check(7);

//            _LT.Model[_LT.Model.IndexOf(1000)]=3;
//            Check(8);

//            _LT.Model.Insert(2,25);
//            Check(9);

//            _LT.Model.Move(0, _LT.Model.Count - 1);
//            Check(10);

//            _LT.Model.Move(1, 2);
//            Check(11);

//        }


//        private class ListObservaleTester : IDisposable
//        {
//            private ObservableCollection<int> _Model;
//            private IFullObservableCollection<string> _Copy;


//            internal int CallBackCount
//            {
//                get;
//                private set;
//            }

//            internal ObservableCollection<int> Model
//            {
//                get { return _Model; }
//            }

//            internal bool CheckEquality
//            {
//                get { return _Copy.Select(i=>int.Parse(i)).SequenceEqual(_Model); }
//            }

//            internal ListObservaleTester()
//            {
//                _Model = new ObservableCollection<int>();
//                _Copy = new TransformObservableCollection<string, int>(_Model, i => i.ToString());
//             }

//            public void Dispose()
//            {
//                _Copy.Dispose();
//            }
//        }

//        [Test]
//        public void BasicTestTransformObservableCollection()
//        {
//            _LTT.Model.Add(1);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(10);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(10);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(100);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Clear();
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(1);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(10);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(2);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Add(3);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Move(0, 2);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model[2] = 45;
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.Remove(45);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.RemoveAt(0);
//            Assert.That(_LTT.CheckEquality, Is.True);

//            _LTT.Model.RemoveAt(1);
//            Assert.That(_LTT.CheckEquality, Is.True);



//        }

//    }
//}
