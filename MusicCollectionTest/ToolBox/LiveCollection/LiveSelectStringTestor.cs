using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

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
    class LiveSelectStringTestor : LiveObservableHelper<MyObject, MyObject>
    {
        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();

            var ld = _Collection.LiveWhere("(Value>=5)or(Name=toto)");
            _Treated = ld;
            _LD = ld as IInvariant;
            InitializeAndRegisterCollection(_Treated);
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void UpdateCountAndEvent()
        {
            Assert.That(_Treated.SequenceEqual(_Collection.Where(o => (o.Value >=5) || (o.Name== "toto") )));
            base.UpdateCountAndEvent();
        }

        [Test]
        public void Test()
        {
            MyObject f = new MyObject("David", 0);
            using (Transaction())
            {
                _Collection.Add(f);
            }

            f = new MyObject("David", 10);
            using (Transaction())
            {
                _Collection.Add(f);
            }


            f = new MyObject("toto", 0);
            using (Transaction())
            {
                _Collection.Add(f);
            }


            f = new MyObject("ggggg", 10);
            using (Transaction())
            {
                _Collection.Add(f);
            }


            f = new MyObject("David", 10);
            using (Transaction())
            {
                _Collection.Add(f);
            }


            f = new MyObject("toto", 10);
            using (Transaction())
            {
                _Collection.Add(f);
            }

           
            using (Transaction())
            {
                f.Name = "totdo";
            }

            using (Transaction())
            {
                f.Value = 3;
            }

            using (Transaction())
            {
                f.Value = 777;
            }

           var nulltarget =  _Collection.LiveWhere("mimi=0");
           nulltarget.Should().BeNull();
        }
    }


}
