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

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class CoupleTest
    {
        private List<MyObject> _Obj;
        private HashSet<SimpleCouple<MyObject>> _HS;

        [SetUp]
        public void SetUp()
        {
            _Obj = new List<MyObject>();
            _HS = new HashSet<SimpleCouple<MyObject>>();
            for (int i = 0; i < 30; i++)
            {
                _Obj.Add(new MyObject(string.Format("Name{0}", i), i));
            }
        }


        [Test]
        public void Test0()
        {
            SimpleCouple<MyObject> c = new SimpleCouple<MyObject>(_Obj[1], _Obj[0]);
            c.Item1.Should().Be(_Obj[1]);
            c.Item2.Should().Be(_Obj[0]);
            _HS.Add(c).Should().BeTrue();

            SimpleCouple<MyObject> c1 = new SimpleCouple<MyObject>(_Obj[1], _Obj[0]);
            c1.Item1.Should().Be(_Obj[1]);
            c1.Item2.Should().Be(_Obj[0]);
            _HS.Add(c1).Should().BeFalse();

            c1.Equals(c).Should().BeTrue();
            //c1.CompareTo(c).Should().Be(0);

            SimpleCouple<MyObject> c2 = new SimpleCouple<MyObject>(_Obj[0], _Obj[1]);
            c2.Item1.Should().Be(_Obj[0]);
            c2.Item2.Should().Be(_Obj[1]);

            _HS.Add(c2).Should().BeFalse();

            c2.Equals(c).Should().BeTrue();
            //c2.CompareTo(c).Should().Be(0);

            _HS.Count.Should().Be(1);

            SimpleCouple<MyObject> c3 = new SimpleCouple<MyObject>(_Obj[0], _Obj[0]);
            c3.Item1.Should().Be(_Obj[0]);
            c3.Item2.Should().Be(_Obj[0]);

            _HS.Add(c3).Should().BeTrue();

            c3.Equals(c).Should().BeFalse();
            //c3.CompareTo(c).Should().NotBe(0);
            _HS.Count.Should().Be(2);

            Action ino = () => c3.ToString();
            ino.ShouldNotThrow();

            c3.Equals(null).Should().BeFalse();
        }


    }
}
