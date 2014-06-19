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
    public static class DicExtender
    {
        public static void ShouldBeCoherent<TK, TV>(this IDictionary<TK, TV> @this)
        {
            int Count = @this.Count;
            var l1 = @this.ToList();
            var l2 = @this.Keys.ToList();
            var l3 = @this.Values.ToList();

            Count.Should().Be(l1.Count);
            Count.Should().Be(l2.Count);
            Count.Should().Be(l3.Count);
            l1.Select(kvp => kvp.Key).ShouldHaveSameElements(l2);
            l1.Select(kvp => kvp.Value).ShouldHaveSameElements(l3);

            List<TV> t = new List<TV>();
            foreach (TK k in @this.Keys)
            {
                TV v = @this[k];
                v.Should().NotBeNull();
                TV v2 = default(TV);
                @this.TryGetValue(k, out v2).Should().BeTrue();
                v.Should().Be(v2);
                l3.Should().Contain(v);
                t.Add(v);
                l1.Should().Contain(new KeyValuePair<TK, TV>(k, v));
            }

            foreach (KeyValuePair<TK, TV> kvp in @this)
            {
                TV v = @this[kvp.Key];
                v.Should().Be(kvp.Value);

                TV v2 = default(TV);
                @this.TryGetValue(kvp.Key, out v2).Should().BeTrue();
                v.Should().Be(v2);
                l3.Should().Contain(v);
                l2.Should().Contain(kvp.Key);
                @this.Contains(kvp).Should().BeTrue();
            }
        }


        public static void ShouldBeExtaclyTheSame<TK, TV>(this IDictionary<TK, TV> @this, IDictionary<TK, TV> target)
        {

            @this.Should().Equal(target);
            @this.AsEnumerable().ShouldHaveSameElements(target);
            @this.AsEnumerable().Count().Should().Be(target.Count);

        }

        public static void ShouldBehaveTheSame<TK, TV>(this IDictionary<TK, TV> @this, IDictionary<TK, TV> target, Action<IDictionary<TK, TV>> Ac)
        {
            @this.ShouldBeCoherent();
            @this.ShouldBeExtaclyTheSame(target);
            Ac(@this);
            Ac(target);
            @this.ShouldBeExtaclyTheSame(target);
            @this.ShouldBeCoherent();
        }
    }   

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class DictionaryTester
    {
        private List<MyObject> _Obj;

        [SetUp]
        public void SetUp()
        {
            _Obj = new List<MyObject>();
            for (int i = 0; i < 30; i++)
            {
                _Obj.Add(new MyObject(string.Format("Name{0}", i), i));
            }
        }

        private void SingleKey(IDictionary<string, MyObject> @this)
        {
            @this.Should().BeEmpty();
            IDictionary<string, MyObject> target = new Dictionary<string, MyObject>();

            @this.ShouldBehaveTheSame(target, _ => { });
            @this.ShouldBehaveTheSame(target, d => d.Add("toto", _Obj[0]));
            @this.ShouldBehaveTheSame(target, d => d.Add("toto0", _Obj[0]));
            @this.ShouldBehaveTheSame(target, d => d.Remove("toto0"));
            @this.ShouldBehaveTheSame(target, d => d.Remove("toto"));
            @this.Should().BeEmpty();

            foreach (MyObject o in _Obj)
            {
                @this.ShouldBehaveTheSame(target, d => d.Add(o.Name, o));
                @this.Should().ContainKey(o.Name);
                @this.Should().ContainValue(o);
            }

            @this.Count.Should().Be(30);

            MyObject dummy = null;
            @this.TryGetValue("ikolo", out dummy).Should().BeFalse();
            dummy.Should().BeNull();

            @this.TryGetValue("Name0", out dummy).Should().BeTrue();
            dummy.Should().NotBeNull();
            dummy.Name.Should().Be("Name0");

            foreach (MyObject o in _Obj)
            {
                @this.ShouldBehaveTheSame(target, d => d.Remove(o.Name).Should().BeTrue());
                @this.Should().NotContainKey(o.Name);
                @this.Should().NotContainValue(o);
            }

            @this.Count.Should().Be(0);

            for (int k = 0; k < 30; k++)
            {
                MyObject o = _Obj[k];
                if (k % 2 == 0)
                    @this.ShouldBehaveTheSame(target, d => d.Add(o.Name, o));
            }

            @this.Count.Should().Be(15);

            for (int k = 0; k < 30; k++)
            {
                MyObject o = _Obj[k];
                MyObject res = null;
                if (k % 2 == 0)
                {
                    @this.Should().ContainKey(o.Name);
                    @this.Should().ContainValue(o);
                    @this.TryGetValue(o.Name, out res).Should().BeTrue();
                    res.Should().Be(o);
                    @this[o.Name].Should().Be(res);
                }
                else
                {
                    @this.Should().NotContainKey(o.Name);
                    @this.Should().NotContainValue(o);
                    @this.TryGetValue(o.Name, out res).Should().BeFalse();
                    @this.Remove(o.Name).Should().BeFalse();
                }

            }

            @this.ShouldBehaveTheSame(target, d => d.Clear());
            @this.Count.Should().Be(0);
            @this.Should().BeEmpty();

            @this.ShouldBehaveTheSame(target, d => d.Add("0", _Obj[1]));
            @this.ShouldBehaveTheSame(target, d => d["0"]=_Obj[0]);
            @this["0"].Should().Be(_Obj[0]);
            Action Prob = ()=>@this["1"] = _Obj[0];
            Prob.ShouldThrow<Exception>();

            MyObject dd = null;
            Action Prob2 = () => dd = @this["1"];
            Prob2.ShouldThrow<Exception>();

            @this.ShouldBehaveTheSame(target, d => d.Add(new KeyValuePair<string, MyObject>("DK", _Obj[0])));
            Action Failed = () => @this.Add("DK", _Obj[0]);
            Failed.ShouldThrow<Exception>();

            var myonl = @this.FirstOrDefault();
            @this.Should().Contain(myonl);
            @this.Should().Contain(new KeyValuePair<string, MyObject>("DK", _Obj[0]));
            @this.Should().NotContain(new KeyValuePair<string, MyObject>("NJ", _Obj[0]));
            @this.Should().NotContain(new KeyValuePair<string, MyObject>("DK", _Obj[1]));

            @this.Contains(new KeyValuePair<string, MyObject>("DK", _Obj[0])).Should().BeTrue();
            @this.Contains(new KeyValuePair<string, MyObject>("NJ", _Obj[0])).Should().BeFalse();
            @this.Contains(new KeyValuePair<string, MyObject>("DK", _Obj[1])).Should().BeFalse();

            @this.Remove(new KeyValuePair<string, MyObject>("NJ", _Obj[0])).Should().BeFalse();
            @this.Remove(new KeyValuePair<string, MyObject>("DK", _Obj[1])).Should().BeFalse();


            @this.Remove(new KeyValuePair<string, MyObject>("NJ", _Obj[0])).Should().BeFalse();
            @this.Remove(new KeyValuePair<string, MyObject>("DK", _Obj[1])).Should().BeFalse();

            @this.ShouldBehaveTheSame(target, d => d.Remove(new KeyValuePair<string, MyObject>("DK", _Obj[0])).Should().BeTrue());

        }

        [Test]
        public void Test_PolyMorphDictionary()
        {
            SingleKey(new PolyMorphDictionary<string, MyObject>());
        }

        [Test]
        public void Test_PolyMorphSimpleDictionary()
        {
            SingleKey(new PolyMorphSimpleDictionary<string, MyObject>());
        }

        [Test]
        public void Test_PolyMorphSimpleDictionary_Empty_Test()
        {
            PolyMorphSimpleDictionary<string, MyObject> target = new PolyMorphSimpleDictionary<string, MyObject>();
            target.Count.Should().Be(0);
            target.Contains(new KeyValuePair<string,MyObject>("a",null)).Should().BeFalse();
            MyObject res = null;
            target.TryGetValue("r", out res).Should().BeFalse();
            Action l = () => res = target[""];
            l.ShouldThrow<Exception>();

            l = () => target[""]= null;
            l.ShouldThrow<Exception>();
        }

          [Test]
        public void Test_ListDictionary()
        {
            SingleKey(new ListDictionary<string, MyObject>());
        }

         [Test]
        public void Test_WeakDictionary()
        {
            SingleKey(new WeakDictionary<string, MyObject>());
        }


        
    }
}
