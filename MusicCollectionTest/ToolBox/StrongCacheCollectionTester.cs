using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Implementation;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class StrongCacheCollectionTester
    {
        private StrongCacheCollection<string, MyObject> _Collection;
        private StrongCacheCollection<string, MyObject> _Collection2;

        private const string _1n = "a";
        private const string _2n = "b";
        private const string _3n = "c";
        private const string _4n = "d";
        private const string _5n = "e";
        private const string _6n = "f";


        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _Quatre;
        private MyObject _Cinq;
        private MyObject _Six;

        private List<MyObject> _Elements;
        private List<string> _Keys;


        [SetUp]
        public void SetUp()
        {
            _Collection = new StrongCacheCollection<string, MyObject>(o => o.Name);
            _Un = new MyObject(_1n, 1);
            _Deux = new MyObject(_2n, 2);
            _Trois = new MyObject(_3n, 3);
            _Quatre = new MyObject(_4n, 4);
            _Cinq = new MyObject(_5n, 5);
            _Six = new MyObject(_6n, 6);

            _Collection.Register(_Un);
            _Collection.Register(_Deux);
            _Collection.Register(_Trois);
            _Collection.Register(_Quatre);
            _Collection.Register(_Cinq);
            _Collection.Register(_Six);

            _Elements = new List<MyObject>();
            _Elements.Add(_Un); _Elements.Add(_Deux); _Elements.Add(_Trois); _Elements.Add(_Quatre); _Elements.Add(_Cinq); _Elements.Add(_Six);

            _Keys = new List<string>();
            _Keys.AddRange(from el in _Elements orderby el.Name select el.Name);

            _Collection2 = new StrongCacheCollection<string, MyObject>(o => o.Name, s => s.ToUpper(), () => new Dictionary<string, MyObject>());
            _Elements.Apply(co => _Collection2.Register(co));
        }


        [Test]
        public void BasicTest()
        {
            Assert.That(_Collection.Find(_1n), Is.EqualTo(_Un));
            Assert.That(_Collection.Find(_2n), Is.EqualTo(_Deux));
            Assert.That(_Collection.Find(_3n), Is.EqualTo(_Trois));
            Assert.That(_Collection.Find(_4n), Is.EqualTo(_Quatre));
            Assert.That(_Collection.Find(_5n), Is.EqualTo(_Cinq));
            Assert.That(_Collection.Find(_6n), Is.EqualTo(_Six));

            Assert.That(_Collection.Keys.OrderBy(o => o).SequenceEqual(_Keys), Is.True);
            Assert.That(_Collection.Values.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);

            _Un.Name = string.Empty;
            Assert.That(_Collection.Values.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
            Assert.That(_Collection.Find(_1n), Is.Null);
            Assert.That(_Collection.Find(string.Empty), Is.EqualTo(_Un));

            _Collection.Remove(_Un);
            _Elements.Remove(_Un);
            _Keys.Remove(_1n);

            Assert.That(_Collection.Values.OrderBy(o => o.Value).SequenceEqual(_Elements.OrderBy(o => o.Value)), Is.True);
            Assert.That(_Collection.Find(_1n), Is.Null);
        }

        [Test]
        public void BasicTest_Part2()
        {
            _Collection2.Find(_1n).Should().Be(_Un);
            _Collection2.Find(_2n).Should().Be(_Deux);
            _Collection2.Find(_3n).Should().Be(_Trois);
            _Collection2.Find(_4n).Should().Be(_Quatre);
            _Collection2.Find(_5n).Should().Be(_Cinq);
            _Collection2.Find(_6n).Should().Be(_Six);

            _Collection2.Find("F").Should().Be(_Six);
            _Collection2.Find("f").Should().Be(_Six);

            Tuple<MyObject,bool> res = _Collection2.FindOrCreateValue("F", s => new MyObject(s,30));
            res.Item2.Should().BeTrue();
            res.Item1.Should().Be(_Six);

            MyObject m = null;
            string lol = "lololo";
            res = _Collection2.FindOrCreateValue(lol, s => { m = new MyObject(s, 10); return m; });
            m.Should().NotBeNull();
            m.Name.Should().Be(lol.ToUpper());
            res.Item2.Should().BeFalse();
            res.Item1.Should().Be(m);

        }
    }
}
