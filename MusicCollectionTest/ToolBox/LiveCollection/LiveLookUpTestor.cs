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
using System.Collections;


namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class LiveLookUpTestor : LiveObservableHelper<MyObject, IObservableGrouping<int, MyObject>>
    {
        private LiveToLookUpSimple<int, MyObject> _Original;
        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();


            _Original = new LiveToLookUpSimple<int,MyObject>(_Collection, o => o.Value);
            var ld = _Original;
            _Treated = ld;
            _LD = ld;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }


        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
            }

            MyObject Un = new MyObject("Rey", 1);
            using (Transaction())
            {
                _Collection.Add(Un);
            }

            MyObject deux = new MyObject("deux", 2);
            using (Transaction())
            {
                _Collection.Add(deux);
            }

            MyObject zero = new MyObject("zero", 0);
            using (Transaction())
            {
                _Collection.Add(zero);
            }

            MyObject zerozero = new MyObject("zerozero", 0);
            using (Transaction())
            {
                _Collection.Add(zero);
            }

            _Treated.ShoulBeACoherentList();
            (_Treated as IList).ShoulBeACoherentNonGenericList();

            using (Transaction())
            {
                _Collection.Add(zerozero);
            }

            _Treated.ShoulBeACoherentList();
            (_Treated as IList).ShoulBeACoherentNonGenericList();

            _Original.Contains(0).Should().BeTrue();
            _Original.Contains(6).Should().BeFalse();
            _Original.Contains(200).Should().BeFalse();

            var zerolist = _Original.GetObservableFromKey(0);
            zerolist.Should().NotBeNull();
            zerolist.Collection.Should().Contain(zerozero).And.Contain(zero);
            zerolist.Collection.Count.Should().Be(3);
            zerolist.Collection.MonitorEvents();

            using (Transaction())
            {
                zerozero.Value=6;
            }

            _Treated.ShoulBeACoherentList();
            (_Treated as IList).ShoulBeACoherentNonGenericList();

            zerolist.Collection.ShouldRaise("CollectionChanged").WithSender(zerolist.Collection).WithArgs<NotifyCollectionChangedEventArgs>(ar => ar.Action == NotifyCollectionChangedAction.Remove);
            zerolist.Collection.Count.Should().Be(2);
            zerolist.Collection.Should().Contain(zero);


            _Original.Contains(6).Should().BeTrue();

            using (Transaction())
            {
                _Collection.Remove(deux);
            }

            _Treated.ShoulBeACoherentList();
            (_Treated as IList).ShoulBeACoherentNonGenericList();
        }
    }  
}
