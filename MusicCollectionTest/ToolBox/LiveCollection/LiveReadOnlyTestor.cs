using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    public static class ListExtender
    {


        [TestFixture]
        [NUnit.Framework.Category("Unitary")]
        [NUnit.Framework.Category("ToolBox")]
        class LiveReadOnlyTestor : LiveObservableHelper<string, string>, IInvariant
        {
            private ReadOnlyWraper<string, string> _Original;

            [SetUp]
            public void SetUp()
            {
                _Collection = new ObservableCollection<string>();
                //var ld = _Collection.LiveWhere(o => o.Name.Contains("R")).LiveOrderBy(o => o.Name).LiveSelect(o => o.Name).LiveDistinct();

                _Original = new ReadOnlySimpleWraper<string>(_Collection);
                _Treated = _Original;
                _LD = this;
                InitializeAndRegisterCollection(_Treated);
            }

            protected override void DisplayInformation()
            {
                base.DisplayInformation();

                _Original.Should().Equal(_Collection);

                if (_Original.Count == 0)
                    return;

                string[] s1 = new string[_Original.Count];
                _Collection.CopyTo(s1, 0);

                string[] s2 = new string[_Original.Count];
                _Original.CopyTo(s2, 0);

                s1.Should().Equal(s2);
            }

            [TearDown]
            public void SetDown()
            {
                _Collection.Clear();
            }

            [Test]
            public void Test()
            {
                using (Transaction())
                {
                    _Collection.Add("R");
                    _Treated.Count.Should().Be(1);
                }
                AssertAddEvent("R", 0);

                using (Transaction())
                {
                    _Collection.Add("Ro");
                    _Treated.Count.Should().Be(2);
                }
                AssertAddEvent("Ro", 1);

                using (Transaction())
                {
                    _Collection.Add("Ror");
                    _Treated.Count.Should().Be(3);
                }
                AssertAddEvent("Ror", 2);

                using (Transaction())
                {
                    _Collection.Add("Rdor");
                    _Treated.Count.Should().Be(4);
                }
                AssertAddEvent("Rdor", 3);

                using (Transaction())
                {
                    _Collection.Remove("Ror");
                    _Treated.Count.Should().Be(3);
                }
                this.AssertRemoveEvent("Ror", 2);

                _Treated.MonitorEvents();

                using (Transaction())
                {
                    _Collection[0] = "hghgh";
                    _Treated.ShouldRaise("CollectionChanged").WithSender(_Treated).
                        WithArgs<NotifyCollectionChangedEventArgs>(args => args.Action == NotifyCollectionChangedAction.Replace);
                    _Treated.ShouldNotRaisePropertyChangeFor(x => x.Count);
                }

                using (Transaction())
                {
                    _Collection.Add("heheh");
                    _Treated.ShouldRaise("CollectionChanged").WithSender(_Treated).
                        WithArgs<NotifyCollectionChangedEventArgs>(args => args.Action == NotifyCollectionChangedAction.Add);
                    _Treated.ShouldRaisePropertyChangeFor(x => x.Count);
                }

                using (Transaction())
                {
                    _Collection.Remove("heheh");
                    _Treated.ShouldRaise("CollectionChanged").WithSender(_Treated).
                        WithArgs<NotifyCollectionChangedEventArgs>(args => args.Action == NotifyCollectionChangedAction.Remove);
                    _Treated.ShouldRaisePropertyChangeFor(x => x.Count);
                }


                Action ch = () => _Treated[0] = "uuu";
                ch.ShouldThrow<AccessViolationException>();

                Action ch2 = () => (_Treated as System.Collections.IList)[0] = "uuu";
                ch2.ShouldThrow<AccessViolationException>();

            }

            [Test]
            [ExpectedException(typeof(AccessViolationException))]
            public void RegisterError()
            {
                _Treated.Add("Ror");
            }

            [Test]
            public void DisconnectBehaviour()
            {

                _Collection.Add("w");
                _Collection.Add("we");
                _Collection.Add("aa");

                _Treated.Should().Equal(_Collection);
                int or = _Treated.Count;
                or.Should().Be(3);

                _Treated.MonitorEvents();

                _Original.Dispose();
                _Collection.Clear();
                _Collection.Should().BeEmpty();
                _Treated.Should().BeEmpty();
                _Treated.Count.Should().Be(0);

                _Treated.ShouldNotRaise("CollectionChanged");

            }

            public bool Invariant
            {
                get
                {
                    _Treated.ShoulBeACoherentList();
                    (_Treated as IList).ShoulBeACoherentNonGenericList();
                    return _Treated.SequenceEqual(_Collection);
                }
            }
        }
    }
}
