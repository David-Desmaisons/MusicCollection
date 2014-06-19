using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class NotifySimpleAdapterTester : ModifiableObjectEventTester
    {

        private MyObject _Deux;
        private MyObject _Un;
        
        [SetUp]
        public void SetUp()
        {
            Init();
            _Deux = new MyObject("Name1",0);
            _Un = new MyObject("Name1",2);
            _Un.ObjectChanged += Listener.SingleElementChangedListener;
        }

        [Test]
        public void Test()
        {
            using (Transaction())
            {
                _Un.Name = "huhu";
            }

            Assert.That(_Un.Name, Is.EqualTo("huhu"));
            AssertEvent();
            AssertEvent("Name","Name1","huhu");

            using (Transaction())
            {
                _Un.Value = 20;
            }

            Assert.That(_Un.Value, Is.EqualTo(20));
            AssertEvent();
            AssertEvent("Value", 2, 20);

            using (Transaction())
            {
                _Un.Value = 20;
            }

            Assert.That(_Un.Value, Is.EqualTo(20));
            AssertNonEvent();
          
        }

        protected override void DisplayInformation()
        {
            Console.WriteLine();
            Console.WriteLine(_Un);
        }
    }
}
