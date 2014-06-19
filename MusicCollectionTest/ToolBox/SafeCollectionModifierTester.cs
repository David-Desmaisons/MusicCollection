using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using NUnit.Framework;

using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Implementation;

using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class SafeCollectionModifierTester
    {
        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _Quatre;
        private MyObject _Cinq;

        private List<MyObject> _DecoratedOriginalElements;
        private List<MyObject> _OriginalElements;
        private List<IObject> _ListForComparison;

        private SafeCollectionModifierConverter<MyObject, IObject> _SCMC = null;
        private IIsolatedMofiableList<IObject> _IML;
        private ObservableCollection<IObject> _ChangedList;

        private EventListener<EventArgs> _EL;

        private void SingleElementChangedListener(object sender, EventColllectionChangedArgs<IObject> e)
        {
            if (e.What == NotifyCollectionChangedAction.Add)
                e.Who.Value = 110;

            if (e.What== NotifyCollectionChangedAction.Remove)
                e.Who.Value = -110;
        }

        [SetUp]
        public void SetUp()
        {
            _Un = new MyObject("a", 0);
            _Deux = new MyObject("b", 1);
            _Trois = new MyObject("c", 2);
            _Quatre = new MyObject("d", 3);
            _Cinq = new MyObject("e", 4);

            _DecoratedOriginalElements = new List<MyObject>();
            _DecoratedOriginalElements.Add(_Un); _DecoratedOriginalElements.Add(_Deux); _DecoratedOriginalElements.Add(_Trois); _DecoratedOriginalElements.Add(_Quatre); _DecoratedOriginalElements.Add(_Cinq);

            _ListForComparison = new List<IObject>();
            _ListForComparison.AddRange(_DecoratedOriginalElements);

            _OriginalElements = new List<MyObject>();
            _OriginalElements.AddRange(_DecoratedOriginalElements);

            _SCMC = SafeCollectionModifierConverter<MyObject, IObject>.GetSafeCollectionModifierConverterDerived<MyObject, IObject>(_DecoratedOriginalElements);
            _IML = _SCMC;
            _ChangedList = _IML.MofifiableCollection;

            _EL = new EventListener<EventArgs>();

            _SCMC.OnCommit += _EL.SingleElementChangedListener;

        }

        [Test]
        public void BasicTest()
        {
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_DecoratedOriginalElements.OrderBy(o => o.Name)), Is.True);
            _ChangedList.Remove(_Deux);
            _ListForComparison.Remove(_Deux);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            _IML.CommitChanges();
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_OriginalElements.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.False);
        }

        [Test]
        public void BasicTest2()
        {
            {
                Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_DecoratedOriginalElements.OrderBy(o => o.Name)), Is.True);
                _ChangedList.Remove(_Deux);
                _ListForComparison.Remove(_Deux);
                Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
                Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);
            }

            Assert.That(_EL.EventCount, Is.EqualTo(0));

        }

        [Test]
        public void BasicTestAdd()
        {
            Assert.That(_EL.EventCount, Is.EqualTo(0));

            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_DecoratedOriginalElements.OrderBy(o => o.Name)), Is.True);
            MyObject toto = new MyObject("z", 26);
            _ChangedList.Add(toto);
            _ListForComparison.Add(toto);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            _ChangedList.Remove(_Trois);
            _ListForComparison.Remove(_Trois);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            MyObject toto2 = new MyObject("q", 16);
            _ChangedList.Add(toto2);
            _ListForComparison.Add(toto2);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            Assert.That(_EL.EventCount, Is.EqualTo(0));
            _IML.CommitChanges();
            Assert.That(_EL.EventCount, Is.EqualTo(1));

            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);

            Assert.That(_EL.EventCount, Is.EqualTo(1));
        }

        [Test]
        public void BasicTestWithListener()
        {

            _IML.OnBeforeChangedCommited += SingleElementChangedListener;

            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_DecoratedOriginalElements.OrderBy(o => o.Name)), Is.True);
            MyObject toto = new MyObject("z", 26);
            _ChangedList.Add(toto);
            _ListForComparison.Add(toto);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            _ChangedList.Remove(_Trois);
            _ListForComparison.Remove(_Trois);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            MyObject toto2 = new MyObject("q", 16);
            _ChangedList.Add(toto2);
            _ListForComparison.Add(toto2);
            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_OriginalElements.OrderBy(o => o.Name)), Is.True);

            _IML.CommitChanges();
            Assert.That(_EL.EventCount, Is.EqualTo(1));

            Assert.That(_ChangedList.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);
            Assert.That(_DecoratedOriginalElements.OrderBy(o => o.Name).SequenceEqual(_ListForComparison.OrderBy(o => o.Name)), Is.True);

            Assert.That(_Trois.Value, Is.EqualTo(-110));
            Assert.That(toto.Value, Is.EqualTo(110));
            Assert.That(toto2.Value, Is.EqualTo(110));
        }

        [Test]
        public void BasicTestWithReplace()
        {

            MyObject nov = new MyObject("nov",12);
            _ChangedList[1] = nov;
            _ListForComparison[1] = nov;
            _DecoratedOriginalElements.Should().Equal(_OriginalElements);

            _ChangedList[2] = nov;
            _ListForComparison[2] = nov;
            _DecoratedOriginalElements.Should().Equal(_OriginalElements);

            MyObject nov2 = new MyObject("nov2", 122);
            _ChangedList[0] = nov2;
            _ListForComparison[0] = nov2;
            _DecoratedOriginalElements.Should().Equal(_OriginalElements);

            _IML.CommitChanges();
            _DecoratedOriginalElements.Should().Equal(_ListForComparison);

        }

        //
    }
}
