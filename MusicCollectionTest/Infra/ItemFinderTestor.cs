using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;
using System.Collections.ObjectModel;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Infra")]
    [NUnit.Framework.Category("Unitary")]
    public class ItemFinderTestor
    {
        private ObservableCollection<MyObject> _Collection = new ObservableCollection<MyObject>();
        private ItemFinder<MyObject> _Finder ;

        [SetUp]
        public void SU()
        {
        }

        private void SetUp()
        { 
            _Collection.Add(new MyObject("b", 60));
            _Collection.Add(new MyObject("ToRTo", 1));
            _Collection.Add(new MyObject("aaaaaa", 2));
            _Collection.Add(new MyObject("baBA", 3));
            _Collection.Add(new MyObject("Anthony Braxton", 4));
            _Collection.Add(new MyObject("Steve Lacy", 5));
            _Collection.Add(new MyObject("lac", 6));
           
        }

        [Test]
        public void Test()
        {
            SetUp();
            _Finder = new ItemFinder<MyObject>(_Collection, o => o.Name);

            IEnumerable<MyObject> res = _Finder.Search("A");
            res.Should().BeNull();

            res = _Finder.Search("B");
            res.Should().BeNull();

            res = _Finder.Search("rt");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o=>o.Name).Should().Contain("ToRTo");


            MyObject unres = _Finder.FindExactMatches("B").FirstOrDefault();
            unres.Should().NotBeNull();
            unres.Name.Should().Be("b");

            res = _Finder.Search("ab");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o => o.Name).Should().Contain("baBA");

            res = _Finder.Search("brax");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o => o.Name).Should().Contain("Anthony Braxton");

            res = _Finder.Search("lac");
            res.Should().NotBeNull();
            res.Count().Should().Be(2);
            res.Select(o => o.Name).Should().Contain(new string[]{"Steve Lacy","lac"});

            _Collection[0].Name = "la";
            res = _Finder.Search("la");
            res.Should().NotBeNull();
            res.Count().Should().Be(3);
            res.Select(o => o.Name).Should().Contain(new string[] { "la", "Steve Lacy", "lac" });

            unres = _Finder.FindExactMatches("B").FirstOrDefault();
            unres.Should().BeNull();

            res = _Finder.Search("aa");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o => o.Name).Should().Contain(new string[] { "aaaaaa" });

            _Collection[2].Name = "aa";
            res = _Finder.Search("aa");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o => o.Name).Should().Contain(new string[] { "aa" });

            _Collection[2].Name = "BAA";
            res = _Finder.Search("aa");
            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Select(o => o.Name).Should().Contain(new string[] { "BAA" });

            _Collection.RemoveAt(2);
            res = _Finder.Search("aa");
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            MyObject mo = new MyObject("lac", 20);
            _Collection.Add(mo);
            res = _Finder.FindExactMatches("lac");
            res.Should().NotBeNull();
            res.Count().Should().Be(2);
            res.Should().Contain(mo);
            


        }
    }
}
