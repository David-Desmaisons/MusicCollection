using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Set;

using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class SimpleSetTestor
    {
        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;

        [SetUp]
        public void SetUp()
        {
            _Un = new MyObject("1", 1);
            _Deux = new MyObject("2", 2);
            _Trois = new MyObject("3", 3);
        }

        [TearDown]
        public void SetDown()
        {
        }

        private void BasicTestss(ISimpleSet<MyObject> tested)
        {
            tested.Count.Should().Be(0);
            tested.Count().Should().Be(0);
            tested.Should().BeEmpty();

            bool res = tested.Add(_Un);
            res.Should().BeTrue();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Un);
            tested.Should().NotContain(_Deux);

            res = tested.Contains(_Un);
            res.Should().BeTrue();

            res = tested.Contains(_Deux);
            res.Should().BeFalse();

            res = tested.Remove(_Deux);
            res.Should().BeFalse();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Un);
            tested.Should().NotContain(_Deux);

            res = tested.Remove(_Un);
            res.Should().BeTrue();

            tested.Count.Should().Be(0);
            tested.Count().Should().Be(0);
            tested.Should().NotContain(_Un);
            tested.Should().NotContain(_Deux);
            tested.Should().BeEmpty();


            res = tested.Add(_Deux);
            res.Should().BeTrue();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Deux);
            tested.Should().NotContain(_Un);

            res = tested.Contains(_Deux);
            res.Should().BeTrue();

            res = tested.Contains(_Un);
            res.Should().BeFalse();

            res = tested.Remove(_Un);
            res.Should().BeFalse();

            res = tested.Add(_Deux);
            res.Should().BeFalse();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Deux);
            tested.Should().NotContain(_Un);

        }


        private void MoreComplexTestss(ISimpleSet<MyObject> ls)
        {
            ls.Count.Should().Be(1);
            ls.Should().Contain(_Un);

            bool res = ls.Add(_Un);
            res.Should().BeFalse();

            ls.Count.Should().Be(1);
            ls.Should().Contain(_Un);

            res = ls.Contains(_Un);
            res.Should().BeTrue();

            res = ls.Contains(_Deux);
            res.Should().BeFalse();

            res = ls.Add(_Deux);
            res.Should().BeTrue();

            ls.Should().Contain(new MyObject[] { _Un, _Deux });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            res = ls.Remove(_Un);
            res.Should().BeTrue();

            ls.Should().Contain(_Deux);
            ls.Count.Should().Be(1);
            ls.Count().Should().Be(1);
            ls.Contains(_Deux).Should().BeTrue();
            ls.Contains(_Un).Should().BeFalse();

            ls.Add(_Un).Should().BeTrue();
            ls.Add(_Trois).Should().BeTrue();
            ls.Should().Contain(new MyObject[] { _Un, _Deux, _Trois });
            ls.Count.Should().Be(3);
            ls.Count().Should().Be(3);

            ls.Remove(_Un).Should().BeTrue();
            ls.Should().Contain(new MyObject[] { _Deux, _Trois });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            ls.Add(_Deux).Should().BeFalse();
            ls.Should().Contain(new MyObject[] { _Deux, _Trois });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            ls.Add(_Un).Should().BeTrue();
            ls.Should().Contain(new MyObject[] { _Un, _Deux, _Trois });
            ls.Count.Should().Be(3);
            ls.Count().Should().Be(3);
        }



        private void BasicTestletter(ILetterSimpleSet<MyObject> tested)
        {
            tested.Count.Should().Be(0);
            tested.Count().Should().Be(0);
            tested.Should().BeEmpty();



            bool res;
            tested.Add(_Un, out res);
            res.Should().BeTrue();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Un);
            tested.Should().NotContain(_Deux);

            res = tested.Contains(_Un);
            res.Should().BeTrue();

            res = tested.Contains(_Deux);
            res.Should().BeFalse();

            tested.Remove(_Deux, out res);
            res.Should().BeFalse();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Un);
            tested.Should().NotContain(_Deux);

            tested.Remove(_Un, out res);
            res.Should().BeTrue();

            tested.Count.Should().Be(0);
            tested.Count().Should().Be(0);
            tested.Should().NotContain(_Un);
            tested.Should().NotContain(_Deux);
            tested.Should().BeEmpty();

            tested.Add(_Deux, out res);
            res.Should().BeTrue();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Deux);
            tested.Should().NotContain(_Un);

            res = tested.Contains(_Deux);
            res.Should().BeTrue();

            res = tested.Contains(_Un);
            res.Should().BeFalse();

            tested.Remove(_Un, out res);
            res.Should().BeFalse();

            tested.Add(_Deux, out res);
            res.Should().BeFalse();

            tested.Count.Should().Be(1);
            tested.Count().Should().Be(1);
            tested.Should().Contain(_Deux);
            tested.Should().NotContain(_Un);

        }


        private void MoreComplexTestletter(ILetterSimpleSet<MyObject> ls)
        {
            ls.Count.Should().Be(1);
            ls.Should().Contain(_Un);

            bool res;
            ls.Add(_Un, out res);
            res.Should().BeFalse();

            ls.Count.Should().Be(1);
            ls.Should().Contain(_Un);

            res = ls.Contains(_Un);
            res.Should().BeTrue();

            res = ls.Contains(_Deux);
            res.Should().BeFalse();

            ls.Add(_Deux, out res);
            res.Should().BeTrue();

            ls.Should().Contain(new MyObject[] { _Un, _Deux });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            ls.Remove(_Un, out res);
            res.Should().BeTrue();

            ls.Should().Contain(_Deux);
            ls.Count.Should().Be(1);
            ls.Count().Should().Be(1);
            ls.Contains(_Deux).Should().BeTrue();
            ls.Contains(_Un).Should().BeFalse();

            ls.Add(_Un, out res);
            res.Should().BeTrue();

            ls.Add(_Trois, out res);
            res.Should().BeTrue();

            ls.Should().Contain(new MyObject[] { _Un, _Deux, _Trois });
            ls.Count.Should().Be(3);
            ls.Count().Should().Be(3);

            ls.Remove(_Un, out res);
            res.Should().BeTrue();
            ls.Should().Contain(new MyObject[] { _Deux, _Trois });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            ls.Add(_Deux, out res);
            res.Should().BeFalse();
            ls.Should().Contain(new MyObject[] { _Deux, _Trois });
            ls.Count.Should().Be(2);
            ls.Count().Should().Be(2);

            ls.Add(_Un, out res);
            res.Should().BeTrue();
            ls.Should().Contain(new MyObject[] { _Un, _Deux, _Trois });
            ls.Count.Should().Be(3);
            ls.Count().Should().Be(3);
        }

        [Test]
        public void SingleTest()
        {
            SingleSet<MyObject> ss = new SingleSet<MyObject>();
            BasicTestletter(ss);
        }

        [Test]
        public void SingleTest2()
        {
            SingleSet<MyObject> ss = new SingleSet<MyObject>(_Un);
            ss.Count.Should().Be(1);
            ss.Should().Contain(_Un);

            bool res;
            ILetterSimpleSet<MyObject> resss = ss.Add(_Deux, out res);
            resss.Should().NotBeNull();
            resss.GetType().Should().Be(typeof(ListSet<MyObject>));
            resss.Count.Should().Be(2);
            resss.Should().Contain(new MyObject[] { _Un, _Deux });
        }


        [Test]
        public void ListTest()
        {
            ListSet<MyObject> ls = new ListSet<MyObject>(10);
            BasicTestletter(ls);
        }

        [Test]
        public void ListTest2()
        {
            ListSet<MyObject> ls = new ListSet<MyObject>(3, _Un);
            MoreComplexTestletter(ls);

            MyObject quatre = new MyObject("4", 4);

            bool res;
            ILetterSimpleSet<MyObject> resss = ls.Add(quatre, out res);
            resss.Should().NotBeNull();
            resss.GetType().Should().Be(typeof(SimpleHashSet<MyObject>));
            resss.Count.Should().Be(4);
            resss.Should().Contain(new MyObject[] { _Un, _Deux, _Trois, quatre });
        }

        [Test]
        public void ListTest3()
        {
            MyObject[] amy = new MyObject[] { new MyObject("a", 1), new MyObject("a", 1), new MyObject("b", 1), new MyObject("cc", 3) };

            ListSet<MyObject> ls = new ListSet<MyObject>(6, amy);

            ls.ShouldHaveSameElements(amy);

            ls.Count.Should().Be(4);

            bool res = false;
            ILetterSimpleSet<MyObject> resl = ls.Add(amy[0], out res);
            res.Should().BeFalse();
            ((object)resl).Should().Be(ls);

            resl = ls.Add(amy[1], out res);
            res.Should().BeFalse();
            ((object)resl).Should().Be(ls);

            resl = ls.Add(amy[2], out res);
            res.Should().BeFalse();
            ((object)resl).Should().Be(ls);

            resl = ls.Add(new MyObject("ll", 8), out res);
            res.Should().BeTrue();
            ((object)resl).Should().Be(ls);

            resl = ls.Add(new MyObject("lly", 8), out res);
            res.Should().BeTrue();
            ((object)resl).Should().Be(ls);

            resl = ls.Add(new MyObject("llj", 8), out res);
            res.Should().BeTrue();
            ((object)resl).Should().NotBe(ls);



        }

        [Test]
        public void HashTest()
        {
            SimpleHashSet<MyObject> ls = new SimpleHashSet<MyObject>();
            BasicTestletter(ls);
        }

        [Test]
        public void HashTest2()
        {
            SimpleHashSet<MyObject> ls = new SimpleHashSet<MyObject>();
            ls.Add(_Un);
            MoreComplexTestletter(ls);
        }

        [Test]
        public void PolyMorphSetTest()
        {
            PolyMorphSet<MyObject> ls = new PolyMorphSet<MyObject>();
            ls.Add(_Un);
            MoreComplexTestss(ls);
        }

        [Test]
        public void PolyMorphSetTest_Bis()
        {
            PolyMorphSet<MyObject> ls = new PolyMorphSet<MyObject>(_Un);
            MoreComplexTestss(ls);
        }

        [Test]
        public void PolyMorphSetTest2()
        {
            PolyMorphSet<MyObject> ls = new PolyMorphSet<MyObject>();
            BasicTestss(ls);
        }


        [Test]
        public void PolyMorphSetTest3()
        {
            List<MyObject> Li = new List<MyObject>();

            for (int i = 0; i < 20; i++)
            {
                Li.Add(new MyObject(i.ToString(), i));
            }

            PolyMorphSet<MyObject> ls = new PolyMorphSet<MyObject>(Li);
            ls.Should().Contain(Li);
            ls.Count.Should().Be(20);
            ls.Count().Should().Be(20);

            foreach (MyObject ob in Li)
            {
                ls.Contains(ob).Should().BeTrue();
            }
        }

        private void TestSet<T>(IEnumerable<T> Init) where T : class
        {
            ILetterSimpleSet<T> sls = LetterSimpleSetFactory<T>.GetDefault(Init);
            sls.Should().NotBeNull();
            ISet<T> set = new HashSet<T>(Init);
            sls.Count.Should().Be(set.Count);
            sls.ShouldHaveSameElements(set);
        }

        [Test]
        public void PolyMorphFactory_Empty()
         {
             TestSet(Enumerable.Empty<MyObject>());
         }

        [Test]
        public void PolyMorphFactory_One()
        {
            TestSet(new MyObject("f", 1).SingleItemCollection());
        }

        [Test]
        public void PolyMorphFactory_LessThanMax()
        {
            TestSet(Enumerable.Range(0,5).Select(i=>new MyObject(i.ToString(),i)).ToList());
        }

        [Test]
        public void PolyMorphFactory_MoreThanMax()
        {
            TestSet(Enumerable.Range(0, 15).Select(i => new MyObject(i.ToString(), i)).ToList());
        }

        [Test]
        public void PolyMorphFactory_LessThanMaxWithDuplicate()
        {
            var ini = Enumerable.Range(0, 4).Select(i => new MyObject(i.ToString(), i)).ToList();
            TestSet(ini.Concat(ini));
        }

        [Test]
        public void PolyMorphFactory_NullException()
        {
            ILetterSimpleSet<object> sls;
            Action ac = () => sls = LetterSimpleSetFactory<object>.GetDefault(null);
            ac.ShouldThrow<ArgumentException>();
        }
    }
}
