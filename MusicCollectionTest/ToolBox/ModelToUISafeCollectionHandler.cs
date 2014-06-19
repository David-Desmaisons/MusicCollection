using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

using NUnit.Framework;

using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Implementation;
using MusicCollection.Infra;

using MusicCollectionTest.TestObjects;
using System.Collections;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class ModelToUISafeCollectionHandler
    {
        private ModelToUISafeCollectionHandler<MyObject, IObject> _MTCH;

        private IImprovedList<MyObject> _ModelCollection;
        private IObservableCollection<IObject> _ReadOnlyUICollection;
        private IObservableCollection<IObject> _ModifiableUICollection;


        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _Quatre;
        private MyObject _Cinq;

        private List<MyObject> _Elements;
        private List<MyObject> _CompElements;

        private EventListener<NotifyCollectionChangedEventArgs> _Model;
        private EventListener<NotifyCollectionChangedEventArgs> _UI;
        private EventListener<NotifyCollectionChangedEventArgs> _UIR;

        private NotifyCollectionChangedEventArgs CheckEvent(NotifyCollectionChangedAction action)
        {
            Assert.That(_Model.EventCount, Is.EqualTo(1));
            Assert.That(_Model.EventCount, Is.EqualTo(1));
            Assert.That(_Model.EventCount, Is.EqualTo(1));

            NotifyCollectionChangedEventArgs m = _Model.GetDeplieEvent();
            NotifyCollectionChangedEventArgs u = _UI.GetDeplieEvent();
            NotifyCollectionChangedEventArgs ur = _UIR.GetDeplieEvent();

            Assert.That(m.Action, Is.EqualTo(action));
            Assert.That(u.Action, Is.EqualTo(action));
            Assert.That(ur.Action, Is.EqualTo(action));

            Assert.That(m.OldItems, Is.EqualTo(u.OldItems));
            Assert.That(m.OldItems, Is.EqualTo(ur.OldItems));

            Assert.That(m.NewItems, Is.EqualTo(u.NewItems));
            Assert.That(m.NewItems, Is.EqualTo(ur.NewItems));

            Assert.That(m.NewStartingIndex, Is.EqualTo(u.NewStartingIndex));
            Assert.That(m.NewStartingIndex, Is.EqualTo(ur.NewStartingIndex));

            Assert.That(m.OldStartingIndex, Is.EqualTo(u.OldStartingIndex));
            Assert.That(m.OldStartingIndex, Is.EqualTo(ur.OldStartingIndex));

            return ur;
        }

        private void CheckAddEvent(MyObject added,int Index)
        {
            NotifyCollectionChangedEventArgs add = CheckEvent(NotifyCollectionChangedAction.Add);
            Assert.That(add.OldItems, Is.Null);
            Assert.That(add.NewItems, Is.Not.Null);
            Assert.That(add.NewItems.Count, Is.EqualTo(1));
            Assert.That(add.NewItems[0], Is.EqualTo(added));
            Assert.That(add.NewStartingIndex, Is.EqualTo(Index));
        }

        private void CheckRemoveEvent(MyObject added, int Index)
        {
            NotifyCollectionChangedEventArgs add = CheckEvent(NotifyCollectionChangedAction.Remove);
            Assert.That(add.NewItems, Is.Null);
            Assert.That(add.OldItems, Is.Not.Null);
            Assert.That(add.OldItems.Count, Is.EqualTo(1));
            Assert.That(add.OldItems[0], Is.EqualTo(added));
            Assert.That(add.OldStartingIndex, Is.EqualTo(Index));
        }

        private void CheckReplace(MyObject old, MyObject nnwe,int Index)
        {
            NotifyCollectionChangedEventArgs add = CheckEvent(NotifyCollectionChangedAction.Replace);
            Assert.That(add.NewItems, Is.Not.Null);
            Assert.That(add.OldItems, Is.Not.Null);
            Assert.That(add.OldItems.Count, Is.EqualTo(1));
            Assert.That(add.OldItems[0], Is.EqualTo(old));
            Assert.That(add.OldStartingIndex, Is.EqualTo(Index));
            Assert.That(add.NewItems.Count, Is.EqualTo(1));
            Assert.That(add.NewItems[0], Is.EqualTo(nnwe));
            Assert.That(add.NewStartingIndex, Is.EqualTo(Index));
        }

        private void CheckMove(MyObject moved,int oldindex, int newindex)
        {
            NotifyCollectionChangedEventArgs add = CheckEvent(NotifyCollectionChangedAction.Move);
            add.NewItems.Should().NotBeNull();
            add.NewItems.Count.Should().Be(1);
            add.NewItems[0].Should().Be(moved);
            add.NewStartingIndex.Should().Be(newindex);
            add.OldStartingIndex.Should().Be(oldindex);
        }

        private void CheckCollection<T>(IList<MyObject> ToBeCompared, Func<IObject, T> Comparer)
        {
            Assert.That(_ModelCollection.OrderBy(Comparer).SequenceEqual(ToBeCompared.OrderBy(Comparer)), Is.True);
            Assert.That(_ModelCollection.Count, Is.EqualTo(ToBeCompared.Count));
            Assert.That(_ReadOnlyUICollection.OrderBy(Comparer).SequenceEqual(ToBeCompared.OrderBy(Comparer)), Is.True);
            Assert.That(_ReadOnlyUICollection.Count, Is.EqualTo(ToBeCompared.Count));
            Assert.That(_ModifiableUICollection.OrderBy(Comparer).SequenceEqual(ToBeCompared.OrderBy(Comparer)), Is.True);
            Assert.That(_ModifiableUICollection.Count, Is.EqualTo(ToBeCompared.Count));

            foreach (MyObject o in ToBeCompared) CheckContains(o);
        }

        private void CheckContains(MyObject value)
        {
            Assert.That(_ModelCollection.Contains(value), Is.True);
            Assert.That(_ReadOnlyUICollection.Contains(value), Is.True);
            Assert.That(_ModifiableUICollection.Contains(value), Is.True);
        }

        private void CheckIndex(int Index, MyObject value)
        {
            Assert.That(_ModelCollection[Index], Is.EqualTo(value));
            Assert.That(_ReadOnlyUICollection[Index], Is.EqualTo(value));
            Assert.That(_ModifiableUICollection[Index], Is.EqualTo(value));
        }

        private void SCheckCollection(IList<MyObject> ToBeCompared)
        {
            CheckCollection(ToBeCompared, o => o.Value);
        }


        [SetUp]
        public void SetUp()
        {
            _Un = new MyObject("a", 0);
            _Deux = new MyObject("b", 1);
            _Trois = new MyObject("c", 2);
            _Quatre = new MyObject("d", 3);
            _Cinq = new MyObject("e", 4);

            _Elements = new List<MyObject>();
            _Elements.Add(_Un); _Elements.Add(_Deux); _Elements.Add(_Trois); _Elements.Add(_Quatre); _Elements.Add(_Cinq);

            _CompElements = new List<MyObject>();
            _CompElements.AddRange(_Elements);

            _MTCH = new ModelToUISafeCollectionHandler<MyObject, IObject>(_Elements);

            _ModelCollection = _MTCH.ModelCollection;
            _ReadOnlyUICollection = _MTCH.ReadOnlyUICollection;
            _ModifiableUICollection = _MTCH.ModifiableUICollection;

            _Model = new EventListener<NotifyCollectionChangedEventArgs>();
            _UI = new EventListener<NotifyCollectionChangedEventArgs>();
            _UIR = new EventListener<NotifyCollectionChangedEventArgs>();

            _ModelCollection.CollectionChanged += _Model.SingleElementChangedListener;
            _ReadOnlyUICollection.CollectionChanged += _UIR.SingleElementChangedListener;
            _ModifiableUICollection.CollectionChanged += _UI.SingleElementChangedListener;
        }

        [Test]
        public void BasicTest()
        {
            Assert.That(_Elements.OrderBy(o => o.Name).SequenceEqual(_CompElements.OrderBy(o => o.Name)), Is.True);
            Assert.That(_ModelCollection.OrderBy(o => o.Name).SequenceEqual(_CompElements.OrderBy(o => o.Name)), Is.True);
            Assert.That(_ReadOnlyUICollection.OrderBy(o => o.Name).SequenceEqual(_CompElements.OrderBy(o => o.Name)), Is.True);
            Assert.That(_ModifiableUICollection.OrderBy(o => o.Name).SequenceEqual(_CompElements.OrderBy(o => o.Name)), Is.True);
            SCheckCollection(_CompElements);

            CheckIndex(0, _CompElements[0]);
            CheckIndex(1, _CompElements[1]);
            CheckIndex(2, _CompElements[2]);
            CheckIndex(3, _CompElements[3]);
            CheckIndex(4, _CompElements[4]);

            _ModifiableUICollection.ShoulBeACoherentList();
            (_ModifiableUICollection as IList).ShoulBeACoherentNonGenericList();

            _ModelCollection.ShoulBeACoherentList();
            //(_ModelCollection as IList).ShoulBeACoherentNonGenericList();

            _ReadOnlyUICollection.ShoulBeACoherentList();
            (_ReadOnlyUICollection as IList).ShoulBeACoherentNonGenericList();
        }

        [Test]
        public void CheckSort()
        {
            _ModelCollection.Sort(false, (a, b) => a.Value - b.Value);

            CheckIndex(4, _CompElements[0]);
            CheckIndex(3, _CompElements[1]);
            CheckIndex(2, _CompElements[2]);
            CheckIndex(1, _CompElements[3]);
            CheckIndex(0, _CompElements[4]);
        }

        [Test]
        public void BasicTestModifiableUI()
        {
            MyObject f = new MyObject("z", 24);
            IObject myf = f;
            _ModifiableUICollection.Add(myf);
            CheckAddEvent(f,5);
            _CompElements.Add(f);
            SCheckCollection(_CompElements);

            _ModifiableUICollection.Remove(_Un);
            CheckRemoveEvent(_Un,0);
            _CompElements.Remove(_Un);
            SCheckCollection(_CompElements);

            _ModelCollection.Insert(0, _Un);
            CheckAddEvent(_Un,0);
            _CompElements.Add(_Un);
            SCheckCollection(_CompElements);
            CheckIndex(0, _Un);

            _ModelCollection.RemoveAt(2);
            CheckRemoveEvent(_Trois,2);
            _CompElements.Remove(_Trois);
            SCheckCollection(_CompElements);
            CheckIndex(2, _Quatre);

            _ModifiableUICollection.Clear();
            CheckEvent(NotifyCollectionChangedAction.Reset);
            SCheckCollection(Enumerable.Empty<MyObject>().ToList());
        }

        [Test]
        public void BasicTestModel()
        {
            _ModelCollection.IsReadOnly.Should().BeFalse();

            MyObject f = new MyObject("z", 24);
            _ModelCollection.Add(f);
            CheckAddEvent(f,5);
            _CompElements.Add(f);
            SCheckCollection(_CompElements);

            _ModelCollection.Remove(_Un);
            CheckRemoveEvent(_Un,0);
            _CompElements.Remove(_Un);
            SCheckCollection(_CompElements);

            _ModelCollection.Insert(0,_Un);
            CheckAddEvent(_Un,0);
            _CompElements.Add(_Un);
            SCheckCollection(_CompElements);
            CheckIndex(0, _Un);

            _ModelCollection.RemoveAt(2);
            CheckRemoveEvent(_Trois,2);
            _CompElements.Remove(_Trois);
            SCheckCollection(_CompElements);
            CheckIndex(2, _Quatre);

             MyObject fd = new MyObject("z", 24);
             _ModifiableUICollection[1] = fd;
             CheckReplace(_Deux, fd, 1);

             _ModifiableUICollection.ShoulBeACoherentList();
             (_ModifiableUICollection as IList).ShoulBeACoherentNonGenericList();
             _ModelCollection.ShoulBeACoherentList();

             MyObject fd2 = new MyObject("z2", 242);
             _ModelCollection[1] = fd2;
             CheckReplace(fd, fd2, 1);

             _ModifiableUICollection.ShoulBeACoherentList();
             (_ModifiableUICollection as IList).ShoulBeACoherentNonGenericList();
             _ModelCollection.ShoulBeACoherentList();

             MyObject c0 = _ModelCollection[0];
             _ModelCollection.Move(0, 1);
             CheckMove(c0, 0, 1);

             _ModifiableUICollection.ShoulBeACoherentList();
             (_ModifiableUICollection as IList).ShoulBeACoherentNonGenericList();
             _ModelCollection.ShoulBeACoherentList();
            
            _ModelCollection.Clear();
            CheckEvent(NotifyCollectionChangedAction.Reset);
            SCheckCollection(Enumerable.Empty<MyObject>().ToList());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadOnlyDefense1()
        {
            Assert.That(_ReadOnlyUICollection.IsReadOnly, Is.True);
            _ReadOnlyUICollection.Add(new MyObject(string.Empty, 1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadOnlyDefense2()
        {
            Assert.That(_ReadOnlyUICollection.IsReadOnly, Is.True);
            _ReadOnlyUICollection.Remove(_Un);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadOnlyDefense3()
        {
            Assert.That(_ReadOnlyUICollection.IsReadOnly, Is.True);
            _ReadOnlyUICollection.Clear();
        }

        [Test]
        public void ReadOnlyDefense4()
        {
            _ReadOnlyUICollection.ShouldBeReadOnlyCollection_Generic();
            (_ReadOnlyUICollection as IList).ShouldBeReadOnlyCollection();
        }
    }
}
