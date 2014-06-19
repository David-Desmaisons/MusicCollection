using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

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
    public class ListExtensionTestor
    {
        private ObservableCollection<MyObject> _Collection;
        private ObservableHelper _OH;
        private IList<MyObject> _objects;
        private IList<MyObject> _Target;

        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();
            _OH = new ObservableHelper();
            _OH.InitializeAndRegisterCollection(_Collection);
            _objects = EnumerableFactory.CreateList(10, i => new MyObject(i.ToString(), i));
            _Target = new List<MyObject>();

        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        private void ApplyChangesAndTest(NotifyCollectionChangedEventArgs received, NotifyCollectionChangedAction Expected)
        {
            received.Should().NotBeNull();
            received.Action.Should().Be(Expected);

            int c = _Target.Count;
            
            Nullable<int> changes = _Target.ApplyChanges(received);
            changes.Should().HaveValue();
            _Target.Should().Equal(_Collection);
            changes.Should().Be(_Target.Count - c); 
        }

        [Test]
        public void Test_Trivial()
        {
            List<int> ni = null;
            ni.ApplyChanges(null).Should().NotHaveValue();

            ni = new List<int>();
            ni.ApplyChanges(null).Should().NotHaveValue();
        }

        [Test]
        public void Test_1()
        {
            using (_OH.Transaction())
            {
                _Collection.Add(_objects[0]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);

            using (_OH.Transaction())
            {
                _Collection.Add(_objects[1]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);


            using (_OH.Transaction())
            {
                _Collection.Insert(0,_objects[5]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);

            using (_OH.Transaction())
            {
                _Collection.Insert(1, _objects[6]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);

            using (_OH.Transaction())
            {
                _Collection[2] = _objects[8];
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Replace);


            using (_OH.Transaction())
            {
                _Collection.RemoveAt(0);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Remove);

            using (_OH.Transaction())
            {
                _Collection.Remove(_objects[8]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Remove);

            using (_OH.Transaction())
            {
                _Collection.Add(_objects[6]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);

            using (_OH.Transaction())
            {
                _Collection.Add(_objects[7]);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Add);

            using (_OH.Transaction())
            {
                _Collection.Move(0, _Collection.Count - 1);
            }

            ApplyChangesAndTest(_OH.Event, NotifyCollectionChangedAction.Move);

            using (_OH.Transaction())
            {
                _Collection.Clear();
            }

            _OH.Event.Should().NotBeNull();
            _OH.Event.Action.Should().Be(NotifyCollectionChangedAction.Reset);

            _Target.ApplyChanges(_OH.Event).Should().NotHaveValue();

        }
    }
}
