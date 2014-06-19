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
    class CollectionChained3 : LiveObservableHelper<MyObject, IObservableGrouping<MyObject, MyObject>>, IInvariant, IEqualityComparer<IGrouping<MyObject, MyObject>>
    {

        private MyObject Reference;
        private IList<Tuple<MyObject, MyObject>> _Collect;
        private IInvariant _Inv;

        [SetUp]
        public void SetUp()
        {
            Reference = new MyObject("Ref", 0);
            _Collection = new  ObservableCollection<MyObject>();
            _Collection.Add(Reference);

            _Collect = _Collection.LiveSelectMany((o) => o.MyFriends, (o, c) => new Tuple<MyObject, MyObject>(o, c));
            _Inv = _Collect as IInvariant;

            var ld = _Collect.LiveToLookUp((s) => s.Item2, (s) => s.Item1);

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
            _Exp = _Collection.SelectMany((o) => o.MyFriends, (o, c) => new Tuple<MyObject, MyObject>(o, c)).ToLookup((s) => s.Item2, (s) => s.Item1); 
            
            Console.WriteLine("Original Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine("Treated Collection: {0}", string.Join(",", _Treated));
            Console.WriteLine("Expected Collection: {0}", string.Join(",", _Exp));
            Console.WriteLine();
        }

        private IEnumerable< IGrouping<MyObject, MyObject>> _Exp;

        bool IInvariant.Invariant
        {
            get
            {
                if (_Inv.Invariant == false)
                    return false;

                if (!_Collect.SequenceEqual(_Collection.SelectMany((o) => o.MyFriends, (o, c) => new Tuple<MyObject, MyObject>(o, c))))
                    return false;

               // return true;


                return _Treated.Cast<IGrouping<MyObject, MyObject>>().OrderBy(g => g.Key.ID).SequenceEqual(_Collection.SelectMany((o) => o.MyFriends, (o, c) => new Tuple<MyObject, MyObject>(o, c)).ToLookup((s) => s.Item2, (s) => s.Item1).OrderBy(g => g.Key.ID), this); 
            }
        }

        //[Test]
        //public void Test_0()
        //{
        //    List<int> l = new List<int>();
        //    l.Add(1); l.Add(1); l.Add(1);

        //    List<int> l2 = new List<int>();
        //    l2.Add(1);

        //    l.GetIndex(l2, 2);


        //}

        [Test]
        public void Test_1()
        {
            MyObject toto = new MyObject("Ra", 0);
            using (Transaction())
            {
                _Collection.Add(toto);
            }


            toto = new MyObject("Rb", 0);
            using (Transaction())
            {
                _Collection.Add(toto);
            }

            MyObject totoe = new MyObject("b", 0);
            using (Transaction())
            {
                _Collection.Insert(0, totoe);
            }

            using (Transaction())
            {
                totoe.Name = "abR";
            }

            MyObject totoee = new MyObject("abR1", 0);
            using (Transaction())
            {
                _Collection.Add(totoee);
            }


            using (Transaction())
            {
                _Collection.Add(totoee);
            }

            MyObject sbom = new MyObject("sbom", 0);

            using (Transaction())
            {
                totoee.MyFriends.Add(sbom);
            }

            using (Transaction())
            {
                totoee.MyFriends.Add(sbom);
            }

            using (Transaction())
            {
                totoe.MyFriends.Add(sbom);
            }

            using (Transaction())
            {
                _Collection.Remove(totoe);
            }

            using (Transaction())
            {
                _Collection.Add(totoe);
            }

            using (Transaction())
            {
                totoe.MyFriends.Add(totoee);
            }

            using (Transaction())
            {
                _Collection.Remove(totoe);
            }

            using (Transaction())
            {
                _Collection.Insert(1,totoe);
            }

            using (Transaction())
            {
                totoe.MyFriends.Add(totoee);
            }

            MyObject oo = new MyObject("dddd", 0);
            using (Transaction())
            {
                totoe.MyFriends.Insert(1,oo);
            }

            using (Transaction())
            {
                totoe.MyFriends[totoe.MyFriends.Count - 1] = totoee;
            }


            using (Transaction())
            {
                totoe.MyFriends.Clear();
            }

            using (Transaction())
            {
                totoe.MyFriends.Add( totoee);
            }


            using (Transaction())
            {
                totoe.MyFriends.Remove(totoee);
            }

            MyObject ood = new MyObject("ddddhhhddd", 20);
            using (Transaction())
            {
                _Collection.Add(ood);
            }







        }

        public bool Equals(IGrouping<MyObject, MyObject> x, IGrouping<MyObject, MyObject> y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (!object.Equals(x.Key, y.Key))
                return false;

            return x.SequenceEqual(y);
        }

        public int GetHashCode(IGrouping<MyObject, MyObject> obj)
        {
            int res = obj.Key.GetHashCode();
            obj.Apply(e => res = res ^ e.GetHashCode());
            return res;
        }
    }
}
