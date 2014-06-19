using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;


using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class ReadOnlyInterfacedObservableCollectionTestor : ObservableHelper
    {
        private ObservableCollection<int> _Collection = null;
        //private ObservableCollection<int> _RO = null;

        [SetUp]
        public void SetUp()
        {
            _Collection = new ObservableCollection<int>();
            InitializeAndRegisterCollection(_Collection);
            //_RO = _Collection;
        }

        [TearDown]
        public void SetDown()
        {
            _Collection.Clear();
        }

        protected override void DisplayInformation()
        {
            Console.WriteLine("Collection: {0}", string.Join(",", _Collection));
            Console.WriteLine();
        }

        [Test]
        public void Test_1()
        {
            using (Transaction())
            {
                _Collection.Add(1);
            }
            this.AssertAddEvent(1, 0);


            //using (Transaction())
            //{
            //    _RO.Add(0);
            //}
            //this.AssertNonEvent();


            //using (Transaction())
            //{
            //    _RO.Remove(0);
            //}
            //this.AssertNonEvent();


            using (Transaction())
            {
                _Collection.Add(2);
            }
            this.AssertAddEvent(2, 1);

            using (Transaction())
            {
                _Collection.Insert(1, 3);
            }
            this.AssertAddEvent(3, 1);

            using (Transaction())
            {
                _Collection.Remove(3);
            }
            this.AssertRemoveEvent(3, 1);

            //using (Transaction())
            //{
            //    _RO.Remove(3);
            //}
            //this.AssertNonEvent();

            //using (Transaction())
            //{
            //    _RO.AddCollection(_Collection);
            //}
            //this.AssertNonEvent();


            //using (Transaction())
            //{
            //    _RO.Clear();
            //}
            //this.AssertNonEvent();

            //using (Transaction())
            //{
            //    _RO.Insert(0, 0);
            //}
            //this.AssertNonEvent();

            //using (Transaction())
            //{
            //    _RO[0] = 99;
            //}
            //this.AssertNonEvent();

            //using (Transaction())
            //{
            //    _RO.RemoveAt(0);
            //}
            //this.AssertNonEvent();

        }


    }
}
