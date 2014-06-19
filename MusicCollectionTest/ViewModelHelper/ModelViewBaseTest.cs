using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollectionWPF.ViewModelHelper;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;
using MusicCollectionTest.TestObjects;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionTest.ViewModelHelper
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ViewModel")]
    class ModelViewBaseTest
    {

        public class MyDisposable : IDisposable
        {
            public MyDisposable()
            {
                IsDisposed = false;
            }

            public bool IsDisposed
            {
                get;
                private set;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public class MyModelViewBase : ViewModelBase
        {
            public T RegisterTest<T>(T idependency) where T : IDisposable
            {
                return Register<T>(idependency);
            }

            public MyModelViewBase MyFather
            {
                set { Father = value; }
            }

        }

        [Test]
        public void TestDisposableRegister()
        {
            MyModelViewBase mdr = new MyModelViewBase();

            MyDisposable md = new MyDisposable();

            MyDisposable registered = mdr.RegisterTest(md);

            registered.Should().Be(md);

            mdr.Dispose();

            md.IsDisposed.Should().BeTrue();
            mdr.CanClose().Should().BeTrue();

        }

        [Test]
        public void TestFather_Dummy()
        {
            MyModelViewBase mdr = new MyModelViewBase();
            mdr.Window.Should().BeNull();
        }

        [Test]
        public void TestFather_Dummy_2()
        {
            MyModelViewBase mdr = new MyModelViewBase();
            IWindow win = Substitute.For<IWindow>();
            mdr.Window = win;
            mdr.Window.Should().Be(win);
        }

        [Test]
        public void TestFather_Dummy_Father_Null()
        {
            MyModelViewBase mdr = new MyModelViewBase();
            MyModelViewBase mdrfater = new MyModelViewBase();
            mdr.MyFather = mdrfater;
            mdr.Window.Should().BeNull();    
        }

        [Test]
        public void TestFather_Dummy_Father_Not_Null()
        {
            
            MyModelViewBase mdrfater = new MyModelViewBase();
            IWindow win = Substitute.For<IWindow>(); 
            mdrfater.Window = win;

            MyModelViewBase mdr = new MyModelViewBase();     
            mdr.MyFather = mdrfater;

            mdr.Window.Should().Be(win);
        }
    }
}
