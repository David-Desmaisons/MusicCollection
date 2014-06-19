using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Infra;
using MusicCollectionTest.TestObjects;
using System.Collections.ObjectModel;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class ListLiveQueryExtensionTest
    {
        private ObservableCollection<MyObject> _Los;

        [SetUp]
        public void SU()
        {
            _Los = new ObservableCollection<MyObject>();
        }

        [Test]
        public void LiveSelect_Test()
        {
            IExtendedObservableCollection<string> target = _Los.LiveSelect<MyObject,string>("");
            target.Should().BeNull();

            target = _Los.LiveSelect<MyObject, string>("Name");
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            _Los.Add(new MyObject("aa", 1));
            target.Should().Equal(new string[]{"aa"});
        }


        [Test]
        public void LiveOrderBy_Test()
        {
            IExtendedObservableCollection<MyObject> target = _Los.LiveOrderBy<MyObject, int>("Value");
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            MyObject aa = new MyObject("aa", 10);
            _Los.Add(aa);
            target.Should().Equal(aa);

            MyObject bb = new MyObject("bb", 1);
            _Los.Add(bb);
            target.Should().Equal(bb,aa);

            MyObject cc = new MyObject("cc", 5);
            _Los.Add(cc);
            target.Should().Equal(bb,cc,aa);
  
  
        }

        [Test]
        public void SelectLive_Test()
        {
             IExtendedObservableCollection<string> target = _Los.SelectLive<MyObject,string>(o=>o.Name);
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            _Los.Add(new MyObject("aa", 1));
            target.Should().Equal(new string[]{"aa"});

            _Los.Add(new MyObject("bb", 1));
            target.Should().Equal(new string[] { "aa","bb" });
  
        }

        [Test]
        public void LiveReadOnly_Test()
        {
            IExtendedObservableCollection<MyObject> target = _Los.LiveReadOnly();
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            MyObject aa = new MyObject("aa", 10);
            _Los.Add(aa);
            target.Should().Equal(aa);

            MyObject bb = new MyObject("bb", 1);
            _Los.Add(bb);
            target.Should().Equal(aa,bb);
        }

        [Test]
        public void LiveReadOnly_Test_2()
        {
            IExtendedObservableCollection<IObject> target = _Los.LiveReadOnly<MyObject,IObject>();
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            MyObject aa = new MyObject("aa", 10);
            _Los.Add(aa);
            target.Should().Equal(aa);

            MyObject bb = new MyObject("bb", 1);
            _Los.Add(bb);
            target.Should().Equal(aa, bb);
        }


        [Test]
        public void LiveToLookUp_Test()
        {
            IObservableLookup<string, MyObject> target = _Los.LiveToLookUp(o => o.Name);
            target.Should().NotBeNull();
            target.Should().BeEmpty();

            MyObject aa = new MyObject("aa", 10);
            _Los.Add(aa);

            ILookup<string, MyObject> t = target;
            t.Count.Should().Be(1);
            t.First().Key.Should().Be("aa");
            t.First().Should().Equal(aa);

        }
    }
}
