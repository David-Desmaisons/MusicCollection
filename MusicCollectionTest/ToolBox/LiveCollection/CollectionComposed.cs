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
    class CollectionComposed : LiveObservableHelper<MyObject, MyObject>, IInvariant
    {
        bool IInvariant.Invariant
        {
            get
            {
                return _Treated.SequenceEqual(_Collection.Where(o => o.MyFriends.Any()));
            }
        }
 
        [SetUp]
        public void SetUp()
        {
            
            _Collection = new ObservableCollection<MyObject>();

            var ld = _Collection.LiveWhere((o) => o.MyFriends.LiveAny((m) => true).Value);

            _Treated = ld;
            _LD = this;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        [Test]
        public void Test()
        {
            MyObject Reference = new MyObject("f", 4);

            using (Transaction())
            {
            }

            using (Transaction())
            {
                _Collection.Add(Reference);
            }

            MyObject toto = new MyObject("t", 10);
            using (Transaction())
            {
                _Collection.Add(toto);
            }

            MyObject toto2 = new MyObject("d", 3);
            using (Transaction())
            {
                _Collection.Add(toto2);
            }

            MyObject totoe = new MyObject("b", 1);
            using (Transaction())
            {
                _Collection.Insert(0, totoe);
            }

            using (Transaction())
            {
                toto.MyFriends.Add(toto2);
            }

            using (Transaction())
            {
                toto2.MyFriends.Add(totoe);
            }

            using (Transaction())
            {
                Reference.MyFriends.Add(totoe);
            }

            using (Transaction())
            {
                toto2.MyFriends.Add(Reference);
            }

            using (Transaction())
            {
                toto2.MyFriends.Clear();
            }
        }
    }
}
