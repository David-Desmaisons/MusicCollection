using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;

using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModel.Filter;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;
using MusicCollectionWPF.ViewModel.Element;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionTest.ViewModelTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    public class AfinityCollectionTestor
    {
        private class ObservableCollectionDecorated<T> : ObservableCollection<T>, IObservableCollection<T>
        {
        }

        private ObservableCollectionDecorated<MyObject> _List = new ObservableCollectionDecorated<MyObject>();
        private ObservableCollectionDecorated<MyObject> _ListEmpty = new ObservableCollectionDecorated<MyObject>();
        private Func<MyObject, IUpdatableComparer<MyObject>> _MyComparerFactory=null;

        [SetUp]
        public void SetUp()
        {         
            _List.AddCollection(Enumerable.Range(0, 1000).Select(i => new MyObject(string.Format("Name {0}", i), i)));
            _MyComparerFactory = (t) => new ElementCenteredComparer<MyObject>(_List, new UpdatableComparer_MyObject_Center(t));
        }

        [TearDown]
        public void TD()
        {
            _List.Clear();
        }

        //private class UpdatableComparer_MyObject_Center : IUpdatableComparer<MyObject>, IDisposable
        //{
        //    private MyObject _Center;
        //    public UpdatableComparer_MyObject_Center(MyObject iCenter)
        //    {
        //        _Center = iCenter;
        //        _Center.ObjectChanged += _Center_ObjectChanged;
        //    }

        //    private void _Center_ObjectChanged(object sender, ObjectModifiedArgs e)
        //    {
        //        if  ( (OnChanged != null) && (e.AttributeName=="Value"))
        //        {
        //            OnChanged(this, EventArgs.Empty);
        //        }
        //    }

        //    public event EventHandler OnChanged;
        //    private HashSet<MyObject> _Observeds = new HashSet<MyObject>();

        //    public int Compare(MyObject x, MyObject y)
        //    {
        //        int res = Math.Abs(x.Value - _Center.Value) - Math.Abs(y.Value - _Center.Value);
        //        RegisterIfNeeded(x);
        //        RegisterIfNeeded(y);
        //        return res;
        //    }

        //    private void RegisterIfNeeded(MyObject x)
        //    {
        //        if (object.ReferenceEquals(_Center, x))
        //            return;

        //        if (_Observeds.Add(x))
        //        {
        //            x.PropertyChanged += x_PropertyChanged;
        //        }
        //    }

        //    private void x_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //    {
        //        if (OnElementChanged != null)
        //            OnElementChanged(this, ElementChangedArgs<MyObject>.From(sender as MyObject));
        //    }

        //    public void Dispose()
        //    {
        //        _Center.ObjectChanged -= _Center_ObjectChanged;
        //        _Observeds.Apply(x=> x.PropertyChanged -= x_PropertyChanged);
        //        _Observeds.Clear();
        //    }


        //    public event EventHandler<ElementChangedArgs<MyObject>> OnElementChanged;
        //}

        private class UpdatableComparer_MyObject_Center : IDistanceEvaluator<MyObject>
        {
            private MyObject _Center;
            public UpdatableComparer_MyObject_Center(MyObject iCenter)
            {
                _Center = iCenter;
            }

            public int EvaluateDistance(MyObject x)
            {
                return x.Value - _Center.Value;
            }

            public MyObject Reference
            {
                get { return _Center; }
            }

            public void UpdateCacheData()
            {
            }
        }
     
       
        private class UpdatableComparer_MyObject: IUpdatableComparer<MyObject>
        {
            private IComparer<MyObject> _IUpdatableComparer;
            public UpdatableComparer_MyObject(IComparer<MyObject> iIUpdatableComparer)
            {
                _IUpdatableComparer = iIUpdatableComparer;
            }

            public event EventHandler OnChanged
            {
                add { }
                remove { }
            }

            public int Compare(MyObject x, MyObject y)
            {
 	            return _IUpdatableComparer.Compare(x,y);
            }

            public event EventHandler<ElementChangedArgs<MyObject>> OnElementChanged
            {
                add { }
                remove { }
            }

            public void Dispose()
            {
            }
        }

        [Test]
        public void EmpyShouldBeEmpty()
        {
            AfinityCollection<MyObject>  acm = new AfinityCollection<MyObject>(_ListEmpty, 
                t => new UpdatableComparer_MyObject( Comparer<MyObject>.Default) ,30);

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Dispose();
            
        }

        [Test]
        public void Normal_CompleteTest()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            first.Value = 100;

            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[1].Should().Be(_List[100]);

            acm.Dispose();

        }

        [Test]
        public void Normal_CompleteTest_2()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var mid = _List[50];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            acm.Reference = mid;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(mid);

            acm.Reference = mid;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(mid);

            acm.Dispose();

        }


        [Test]
        public void Normal_CompleteTest_3()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            first.Value = -1;

            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[1].Should().Be(_List[1]);

            acm.Dispose();

        }


        [Test]
        public void Normal_CompleteTest_Remove_In()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var second = _List[1];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            _List.RemoveAt(0);

            acm.Reference.Should().Be(second);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(second);
            acm.Collection.Should().NotContain(first);


            acm.Dispose();

        }

        [Test]
        public void Normal_CompleteTest_Remove_All()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 1);

            var first = _List[0];
            var second = _List[1];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(1);
            acm.Collection[0].Should().Be(first);

            _List.RemoveAt(0);

            acm.Reference.Should().Be(second);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(1);
            acm.Collection[0].Should().Be(second);
            acm.Collection.Should().NotContain(first);

            acm.Dispose();
        }

        [Test]
        public void Normal_CompleteTest_Remove_Out()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            _List.RemoveAt(80);

            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
          
            acm.Dispose();

        }

        [Test]
        public void Normal_CompleteTest_SwapElement()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var second = _List[1];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            _List.Move(2, 20);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            acm.Dispose();
        }


        [Test]
        public void Normal_CompleteTest_ClearSource()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var second = _List[1];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            _List.Clear();
            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Dispose();
        }

        [Test]
        public void Normal_CompleteTest_AsyncAPI()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0]; var second = _List[1];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            acm.ComputeAsync(first).IsCompleted.Should().BeTrue();

            acm.ComputeAsync(second).Wait();
            acm.Reference.Should().Be(second);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection[0].Should().Be(second);

            acm.Dispose();
        }

        [Test]
        public void Normal_CompleteTest_AddFirst()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_ListEmpty, _MyComparerFactory, 30);

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            var first = new MyObject("my object",90);


            _ListEmpty.Add(first);
            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().Equal(first);

            var second = new MyObject("my object2", 190);


            _ListEmpty.Add(second);
            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().Equal(first, second);


            acm.Dispose();
        }

        [Test]
        public void Normal_CompleteTest_ChangeElement()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0]; 
            var second = _List[50];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().NotContain(second);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            second.Value = 1;

            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().Contain(second);
            acm.Collection[0].Should().Be(first);
            acm.Collection.Skip(1).Take(2).Should().Contain(second);



            acm.Dispose();
        }

        [Test]
        public void Normal_CompleteTest_ChangeElement_NoImpact()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var second = _List[50];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().NotContain(second);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            var itwas = acm.Collection.ToList();

            second.Value = 1000;

            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().NotContain(second);
            acm.Collection.Should().Equal(itwas);

            acm.Dispose();
        }


        [Test]
        public void Normal_CompleteTest_ChangeElement_Middle()
        {
            AfinityCollection<MyObject> acm = new AfinityCollection<MyObject>(_List, _MyComparerFactory, 30);

            var first = _List[0];
            var second = _List[15];

            acm.Reference.Should().BeNull();
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().BeEmpty();

            acm.Reference = first;
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().Contain(second);
            acm.Collection[0].Should().Be(first);
            acm.Collection[29].Should().Be(_List[29]);

            var itwas = acm.Collection.ToList();

            second.Value = 1000;

            acm.Reference.Should().Be(first);
            acm.Collection.Should().NotBeNull();
            acm.Collection.Should().HaveCount(30);
            acm.Collection.Should().NotContain(second);
            acm.Collection.Should().NotEqual(itwas);

            acm.Dispose();
        }

    }
}
