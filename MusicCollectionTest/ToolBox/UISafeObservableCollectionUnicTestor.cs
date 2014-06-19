using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;
using System.Collections.Specialized;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class UISafeObservableCollectionUnicTestor
    {
        private UISafeObservableCollectionUnic<MyObject> _Target;
        private IList<MyObject> _Objs;

        [SetUp]
        public void SetUp()
        {
            _Target = new UISafeObservableCollectionUnic<MyObject>();
            _Objs = EnumerableFactory.CreateList(10, i => new MyObject(i.ToString(), i));
        }


        [Test] 
        public void TestSimpleAdd()
        {      
            _Target.MonitorEvents(); 
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs[0].SingleItemCollection());
            _Target.ShouldRaise("CollectionChanged").WithSender(_Target)
                .WithArgs<NotifyCollectionChangedEventArgs>(n => n.Action== NotifyCollectionChangedAction.Add);

        }

        [Test]
        public void TestSimpleAdd_2()
        {
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs[0].SingleItemCollection());

            _Target.MonitorEvents();
            _Target.Add(_Objs[1]);
            _Target.Should().Equal(new MyObject[]{_Objs[0],_Objs[1]});
            _Target.ShouldRaise("CollectionChanged").WithSender(_Target)
                .WithArgs<NotifyCollectionChangedEventArgs>(n => n.Action == NotifyCollectionChangedAction.Add);
        }

        [Test]
        public void TestSimpleAdd_NoDuplicate()
        {
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs[0].SingleItemCollection());

            _Target.MonitorEvents();
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(new MyObject[] { _Objs[0]});
            _Target.ShouldNotRaise("CollectionChanged");
        }

        [Test]
        public void TestSimpleRemove()
        {
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs[0].SingleItemCollection());

            _Target.MonitorEvents();

            _Target.Remove(_Objs[0]);
            _Target.ShouldRaise("CollectionChanged").WithSender(_Target)
                .WithArgs<NotifyCollectionChangedEventArgs>(n => n.Action == NotifyCollectionChangedAction.Remove);

        }

        [Test]
        public void TestSimple_Constructor()
        {
            _Target = new UISafeObservableCollectionUnic<MyObject>(_Objs);
            _Target.Should().Equal(_Objs);

            _Target.MonitorEvents();
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs);
            _Target.ShouldNotRaise("CollectionChanged");

            _Target[0] = _Objs[5];
            _Target.Should().Equal(_Objs);
            _Target[0].Should().Be(_Objs[0]);
            _Target.ShouldNotRaise("CollectionChanged");

        }

        [Test]
        public void TestSimple_Constructor_2()
        {
            IEnumerable<MyObject> el = new MyObject[] { _Objs[0], _Objs[1], _Objs[2], _Objs[1] };
            _Target = new UISafeObservableCollectionUnic<MyObject>(el);
            _Target.Should().Equal(el.Distinct());
        }

        [Test]
        public void TestSimple_Insert_OK()
        {
            _Target.Add(_Objs[0]);
            _Target.Should().Equal(_Objs[0].SingleItemCollection());

            _Target.MonitorEvents();
            _Target[0]=_Objs[5];
            _Target.Should().Equal(new MyObject[] { _Objs[5] });
            _Target.ShouldRaise("CollectionChanged").WithSender(_Target)
                .WithArgs<NotifyCollectionChangedEventArgs>(n => n.Action == NotifyCollectionChangedAction.Replace);

        }
    }
}
