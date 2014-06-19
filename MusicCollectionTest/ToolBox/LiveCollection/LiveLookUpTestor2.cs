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
    class LiveLookUpTestor2: LiveObservableHelper<MyObject, IObservableGrouping<int, string>>
    {
         [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<MyObject>();

            var ld = LiveToLookUpDouble<int,MyObject,string>.BuildFromKeyElementSelectors(_Collection,o=>o.Value,o=>o.Name);
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
            using (Transaction())
            {
            }

            MyObject Un = new MyObject("Rey", 1);
            using (Transaction())
            {
                _Collection.Add(Un);
            }

            MyObject deux = new MyObject("deux", 2);
            using (Transaction())
            {
                _Collection.Add(deux);
            }

            MyObject zero = new MyObject("zero", 0);
            using (Transaction())
            {
                _Collection.Add(zero);
            }

            MyObject zerozero = new MyObject("zerozero", 0);
            using (Transaction())
            {
                _Collection.Add(zero);
            }

            using (Transaction())
            {
                _Collection.Add(zerozero);
            }

            using (Transaction())
            {
                zerozero.Value=6;
            }

            using (Transaction())
            {
                _Collection.Remove(deux);
            }

            using (Transaction())
            {
                zerozero.Name = "zeroooo";
            }

            using (Transaction())
            {
                _Collection.Remove(zero);
            }

            using (Transaction())
            {
                _Collection.Remove(zero);
            }
        }
    }  
}
