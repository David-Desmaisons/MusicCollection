using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit;
using FluentAssertions;
using NSubstitute;

using MusicCollection.Infra;
using MusicCollectionTest.TestObjects;
using MusicCollectionTest.ToolBox;
using System.Threading;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    class DictionaryExtensionTest
    {
        private Dictionary<string, MyObject> _Dic;
        private Dictionary<string, int> _Dic2;

        [SetUp]
        public void SetUp()
        {
            _Dic = new Dictionary<string, MyObject>();
            _Dic2 = new Dictionary<string, int>();
        }

        [Test]
        public void Test_DictionaryExtension_Basic()
        {
            var res = _Dic.FindOrCreate("toto", t => new MyObject(t,23));

            res.Should().NotBeNull();
            res.CollectionStatus.Should().Be(CollectionStatus.Create);

            MyObject rres =  res.Item;
            rres.Should().NotBeNull();
            rres.Name.Should().Be("toto");
            rres.Value.Should().Be(23);

            _Dic.Count.Should().Be(1);

            var res2 = _Dic.FindOrCreate("toto", t => new MyObject(t, 45));


            res2.CollectionStatus.Should().Be(CollectionStatus.Find);
            MyObject rre2s = res2.Item;
            rre2s.Should().NotBeNull();
            rre2s.Name.Should().Be("toto");
            rre2s.Value.Should().Be(23);

        }

        [Test]
        public void Test_DictionaryExtension_FindOrCreate_ThreadSafe()
        {
            var res = _Dic.FindOrCreate_ThreadSafe("toto", t => new MyObject(t, 23));

            res.Should().NotBeNull();
            res.CollectionStatus.Should().Be(CollectionStatus.Create);

            MyObject rres = res.Item;
            rres.Should().NotBeNull();
            rres.Name.Should().Be("toto");
            rres.Value.Should().Be(23);

            _Dic.Count.Should().Be(1);

            var res2 = _Dic.FindOrCreate_ThreadSafe("toto", t => new MyObject(t, 45));


            res2.CollectionStatus.Should().Be(CollectionStatus.Find);
            MyObject rre2s = res2.Item;
            rre2s.Should().NotBeNull();
            rre2s.Name.Should().Be("toto");
            rre2s.Value.Should().Be(23);
        }

        [Test]
        public void Test_DictionaryExtension_FindOrCreate_ThreadSafe_ThreadTest()
        {
            var tr = Enumerable.Range(0,20).Select(i=> new Thread(Start)).ToList();

            tr.Apply(trh => trh.Start());

            Thread.Sleep(2000);

            _Dic.Count.Should().Be(1);
            _Dic["toto"].Value.Should().Be(23);
            _Dic["toto"].Name.Should().Be("toto");
        }

        private void Start()
        {
            _Dic.FindOrCreate_ThreadSafe("toto", t => { Thread.Sleep(1000); return new MyObject(t, 23); });
        }

        [Test]
        public void Test_DictionaryExtension_Add_Or_Update()
        {
            int res = _Dic2.AddOrUpdate("toto", 1,(n,c)=>++c);
            res.Should().Be(1);

            res = _Dic2.AddOrUpdate("toto", 1, (n, c) => ++c);
            res.Should().Be(2);

            res = _Dic2.AddOrUpdate("toto", 1, (n, c) => ++c);
            res.Should().Be(3);

            res = _Dic2.AddOrUpdate("totoee", 1, (n, c) => ++c);
            res.Should().Be(1);

            _Dic2["toto"].Should().Be(3);
            _Dic2["totoee"].Should().Be(1);

        }

        [Test]
        public void Test_DictionaryExtension_Import()
        {
            _Dic2.Add("a", 1);
            _Dic2.Add("b", 2);
            _Dic2.Add("c", 3);
            _Dic2.Add("d", 4);

            Dictionary<string, int> nv = new Dictionary<string, int>();
            nv.Import(_Dic2);
            nv.ShouldBeExtaclyTheSame(_Dic2);

        }
    }
}
