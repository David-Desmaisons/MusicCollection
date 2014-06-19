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
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

namespace MusicCollectionTest.ToolBox.LiveCollection
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class LiveSelectManyTestor : LiveObservableHelper<MyObject, MyObject>
    {

        private MyObject _Obj;
        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();


            _Obj = new MyObject("Deus", 100);

            _Collection.Add(_Obj);
            _Collection.Add(_Obj);

            var ld = new LiveSelectMany<MyObject, MyObject>(_Collection, o => o.MyFriends);
             _Treated = ld;
             _LD = ld;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }



        [Test]
        public void Test_1()
        {
            List<MyObject> Expected = new List<MyObject>();
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
            }
            AssertCollectionIs(_Treated, Expected);

            using (Transaction())
            {
                _Obj.MyFriends.Add(_Obj);
            }
            Expected.Add(_Obj); Expected.Add(_Obj);
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(2);


            MyObject toto = new MyObject("Deuds", 10);
            using (Transaction())
            {
                _Obj.MyFriends.Add(toto);
            }
            Expected.Insert(1, toto); Expected.Add(toto);
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(2);

            using (Transaction())
            {
                _Obj.MyFriends[0] = toto;
            }
            Expected.Remove(_Obj); Expected.Add(toto); Expected.Remove(_Obj); Expected.Add(toto);
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(2);

            using (Transaction())
            {
                _Obj.MyFriends.RemoveAt(1);
            }
            Expected.Remove(toto);  Expected.Remove(toto); 
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(2);

            toto.MyFriends.Add(_Obj);

            using (Transaction())
            {
                _Collection.Add(toto);
            }
            Expected.Add(_Obj);
            AssertCollectionIs(_Treated, Expected);
            this.AssertAddEvent(_Obj, 2);

            using (Transaction())
            {
                _Collection.Remove(_Obj);
            }
            Expected.Remove(toto);
            AssertCollectionIs(_Treated, Expected);
            this.AssertRemoveEvent(toto, 0);


            MyObject fifi = new MyObject("fifi", 10);
            using (Transaction())
            {
                toto.MyFriends.Add(fifi);
            }
            Expected.Add(fifi);
            AssertCollectionIs(_Treated, Expected);
            this.AssertAddEvent(fifi,2);

            
            using (Transaction())
            {
                _Obj.MyFriends.Insert(0,fifi);
            }
            Expected.Insert(0,fifi);
            AssertCollectionIs(_Treated, Expected);
            this.AssertAddEvent(fifi, 0);

            using (Transaction())
            {
                _Collection.Remove(_Obj);
            }
            Expected.RemoveAt(0); Expected.RemoveAt(0);
            AssertCollectionIs(_Treated, Expected);
            this.AssertEvent(2);



        }
    }
}
