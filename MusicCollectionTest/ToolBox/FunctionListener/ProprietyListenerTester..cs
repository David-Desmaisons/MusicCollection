using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

using NUnit;
using NUnit.Framework;
using NSubstitute;
using FluentAssertions;

using MusicCollectionTest.TestObjects;


using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.FunctionListener;

namespace MusicCollectionTest.ToolBox.FunctionListener
{
    internal class MyNewProprietyListener : ProprietyListener
    {
        public MyNewProprietyListener(string p, string d)
        {
            _Password = p;
            _Directory = d;
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { Set(ref _Password, value); }
        }

        private string _Directory;
        public string Directory
        {
            get { return _Directory; }
            set { Set(ref _Directory, value); }
        }

        public string PD
        {
            get
            {
                return this.RegisterDinamic(() => string.Format("{0}-{1}", Password, Directory));
            }
        }
    }

    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class ProprietyListenerTester
    {
        private MyNewProprietyListener _MyNewProprietyListener;
        [SetUp]
        public void SU()
        {
            _MyNewProprietyListener = new MyNewProprietyListener("p", "d");
        }

        [TearDown]
        public void TearDown()
        {
            _MyNewProprietyListener.Dispose();
        }

        [Test]
        public void Test_First()
        {
            _MyNewProprietyListener.MonitorEvents();
            _MyNewProprietyListener.PD.Should().Be("p-d");
            _MyNewProprietyListener.ShouldNotRaisePropertyChangeFor(p => p.PD);
        }

        [Test]
        public void Test_Second()
        {
            _MyNewProprietyListener.MonitorEvents();
            _MyNewProprietyListener.PD.Should().Be("p-d");
            _MyNewProprietyListener.ShouldNotRaisePropertyChangeFor(p => p.PD);

            _MyNewProprietyListener.Password = "papapa";
            _MyNewProprietyListener.ShouldRaisePropertyChangeFor(p => p.PD);
            _MyNewProprietyListener.PD.Should().Be("papapa-d");
        }

        [Test]
        public void Test_4()
        {
            _MyNewProprietyListener.MonitorEvents();
            _MyNewProprietyListener.PD.Should().Be("p-d");
            _MyNewProprietyListener.ShouldNotRaisePropertyChangeFor(p => p.PD);

            _MyNewProprietyListener.Directory = "dadadada";
            _MyNewProprietyListener.ShouldRaisePropertyChangeFor(p => p.PD);
            _MyNewProprietyListener.PD.Should().Be("p-dadadada");
        }

        [Test]
        public void Test_5()
        {
            _MyNewProprietyListener.ObjectChanged += _MyNewProprietyListener_ObjectChanged;
            _MyNewProprietyListener.PD.Should().Be("p-d");

            _MyNewProprietyListener.Directory = "dadadada";
            _MyNewProprietyListener.PD.Should().Be("p-dadadada");
            _MyNewProprietyListener.ObjectChanged -= _MyNewProprietyListener_ObjectChanged;

            _MyNewProprietyListener.Directory = "y";
            _MyNewProprietyListener.PD.Should().Be("p-y");
        }

        void _MyNewProprietyListener_ObjectChanged(object sender, ObjectModifiedArgs e)
        {
        }

        [Test]
        public void Test_6()
        {
            _MyNewProprietyListener.PD.Should().Be("p-d");
            _MyNewProprietyListener.Directory = "dadadada";
            _MyNewProprietyListener.PD.Should().Be("p-dadadada");

            _MyNewProprietyListener.MonitorEvents();

            _MyNewProprietyListener.Directory = "y";
            _MyNewProprietyListener.ShouldRaisePropertyChangeFor(p => p.PD);
            _MyNewProprietyListener.PD.Should().Be("p-y");
        }

        [Test]
        public void Test_7()
        {
            _MyNewProprietyListener.ObjectChanged += _MyNewProprietyListener_ObjectChanged;
            _MyNewProprietyListener.Directory.Should().Be("d");
            _MyNewProprietyListener.ObjectChanged -= _MyNewProprietyListener_ObjectChanged;

            _MyNewProprietyListener.MonitorEvents();
            _MyNewProprietyListener.PD.Should().Be("p-d");
            _MyNewProprietyListener.ShouldNotRaisePropertyChangeFor(p => p.PD);

            _MyNewProprietyListener.Directory = "dadadada";
            _MyNewProprietyListener.ShouldRaisePropertyChangeFor(p => p.PD);
            _MyNewProprietyListener.PD.Should().Be("p-dadadada");
        }
    }
}

