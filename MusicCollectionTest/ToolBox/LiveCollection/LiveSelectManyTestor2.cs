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
    public class LiveSelectManyTestor2
    {
        private class Tester : IDisposable
        {
            private LiveSelectManyTestor2 _Father;
            public Tester(LiveSelectManyTestor2 lsft)
            {
                _Father = lsft;
            }

            public void Dispose()
            {
                _Father.Check();
            }
        }

        private IDisposable GetTester()
        {
            return new Tester(this);
        }

        private ObservableCollection<MyObject> _Collection;
        private IExtendedObservableCollection<MyObject> _Target;
        private IEnumerable<MyObject> _Expected;

        public void Check()
        {
            _Target.ShoulBeACoherentList();
            _Target.ShoulBeACoherentNonGenericList();
            _Target.Should().Equal(_Expected);
        }

        [SetUp]
        public void SU()
        {
            _Collection = new ObservableCollection<MyObject>();
            _Target = _Collection.LiveSelectMany(s => s.MyFriends);
            _Expected = _Collection.SelectMany(s => s.MyFriends);
        }

        [TearDown]
        public void TD()
        {
            _Target.Dispose();
        }

        [Test]
        public void Test_LiveSelectManyTestor2()
        {
            using (GetTester())
            {
            }

            IList<MyObject> objs = EnumerableFactory.Create<MyObject>(20, i => new MyObject(i.ToString(), i + 1)).ToList();


            using (GetTester())
            {
                _Collection.Add(objs[0]);
            }

            _Target.Should().BeEmpty();

            using (GetTester())
            {
                _Collection.Add(objs[1]);
            }

            _Target.Should().BeEmpty();

            for (int i = 2; i < 20; i++)
            {
                using (GetTester())
                {
                    _Collection.Add(objs[i]);
                }
            }

            using (GetTester())
            {
                _Collection.Move(0, 10);
            }

            _Target.Should().BeEmpty();

            _Target.MonitorEvents();

            for (int i = 0; i < 20; i++)
            {
                using (GetTester())
                {
                    objs[i].MyFriends.Add(objs[i]);
                }

                _Target.Count.Should().Be(i + 1);
                _Target.ShouldRaise("CollectionChanged").WithSender(_Target).WithArgs<NotifyCollectionChangedEventArgs>(a => a.Action == NotifyCollectionChangedAction.Add);
            }


            for (int i = 0; i < 20; i++)
            {
                using (GetTester())
                {
                    MyObject res = null;
                    if (i == 0)
                    {
                        res = objs[19];
                    }
                    else
                    {
                        res = objs[i - 1];
                    }
                    objs[i].MyFriends.Add(res);
                }

                _Target.Count.Should().Be(i + 21);
                _Target.ShouldRaise("CollectionChanged").WithSender(_Target).WithArgs<NotifyCollectionChangedEventArgs>(a => a.Action == NotifyCollectionChangedAction.Add);
            }

            ObservableCollection<MyObject> newmid = new ObservableCollection<MyObject>(objs);

            using (GetTester())
            {
                objs[4].MyFriends = newmid;
            }

            _Target.Count.Should().Be(58);

            using (GetTester())
            {
                objs[4].MyFriends.Move(0, 5);
            }


        }


    }
}
