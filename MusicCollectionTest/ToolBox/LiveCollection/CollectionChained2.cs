using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class CollectionChained2 : LiveObservableHelper<MyObject, MyObject>, IInvariant
    {
        private MyObject Reference;
        [SetUp]
        public void SetUp()
        {
            Reference = new MyObject("f", 4);
            _Collection = new ObservableCollection<MyObject>();
            _Collection.Add(Reference);

            var ld = _Collection.LiveOrderBy(o => o.Name).LiveThenBy(o => o.Value);

            _Treated = ld;
            _LD = this;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void DisplayInformation()
        {
            Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine("Treated Collection: {0}", string.Join(",", _Treated));
            Console.WriteLine();
        }

        [Test]
        public void Test()
        {
            using (Transaction())
            {
            }

            MyObject toto = new MyObject("t", 10);
            using (Transaction())
            {
                _Collection.Add(toto);
            }


            toto = new MyObject("d",3);
            using (Transaction())
            {
                _Collection.Add(toto);
            }

            MyObject totoe = new MyObject("b", 1);
            using (Transaction())
            {
                _Collection.Insert(0, totoe);
            }

            MyObject totoee = new MyObject("f", 0);
            using (Transaction())
            {
                _Collection.Add(totoee);
            }

            MyObject todee = new MyObject("f", 100);
            using (Transaction())
            {
                _Collection.Add(todee);
            }

            MyObject torrrdee = new MyObject("f",10);
            using (Transaction())
            {
                _Collection.Add(torrrdee);
            }

            using (Transaction())
            {
                torrrdee.Name = "a";
            }

            using (Transaction())
            {
                torrrdee.Value = -1;
            }

            using (Transaction())
            {
                totoee.Value = -1;
            }

        }

        bool IInvariant.Invariant
        {
            get
            {
                return _Treated.SequenceEqual(_Collection.OrderBy(o => o.Name).ThenBy(o => o.Value));
            }
        }
    }
}
