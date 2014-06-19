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
using System.Threading;

namespace MusicCollectionTest.ViewModelHelper
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ViewModelHelper")]
    public class RelayCommandTest
    {
        [Test]
        public void TestBasic()
        {
            Action act = Substitute.For<Action>();
            ICommand target = RelayCommand.Instanciate(act);

            target.CanExecute(null).Should().BeTrue();
            target.CanExecute(new object()).Should().BeTrue();

            target.Execute(new object());

            act.Received()();
        }

        [Test]
        public void TestBasic_2()
        {
            Action<object> act = Substitute.For<Action<object>>();
            ICommand target = RelayCommand.Instanciate(act);

            target.CanExecute(null).Should().BeTrue();
            target.CanExecute(new object()).Should().BeTrue();
            object ob = new object();
            target.Execute(ob);

            act.Received()(ob);
        }

        [Test]
        public void TestBasic_3_0()
        {
            MyObject mo = new MyObject("dummy", 29);
            Action<MyObject> act = Substitute.For<Action<MyObject>>();
            ICommand target = RelayCommand.Instanciate(act);

            target.CanExecute(null).Should().BeTrue();
            target.CanExecute(mo).Should().BeTrue();
            target.Execute(mo);

            act.Received()(mo);
        }

        [Test]
        public void TestBasic_3_Typed_Dynamic()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            Action<MyObject> act = Substitute.For<Action<MyObject>>();
            IDynamicCommand target = RelayCommand.Instanciate(act,()=>triger.Value==1);

            target.CanExecute(null).Should().BeTrue();
            target.CanExecute(mo).Should().BeTrue();
            target.Execute(mo);

            act.Received()(mo);

            target.MonitorEvents();
            triger.Value = 2;
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            target.CanExecute(null).Should().BeFalse();
            target.CanExecute(mo).Should().BeFalse();

            target.Dispose();

        }


        [Test]
        public void TestBasic_3_Typed_Dynamic_2()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            Action act = Substitute.For<Action>();
            IDynamicCommand target = RelayCommand.Instanciate(act, () => triger.Value == 1);

            target.CanExecute(null).Should().BeTrue();
            target.CanExecute(mo).Should().BeTrue();
            target.Execute(mo);

            act.Received()();

            target.MonitorEvents();
            triger.Value = 2;
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            target.CanExecute(null).Should().BeFalse();
            target.CanExecute(mo).Should().BeFalse();

            target.Dispose();

        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_100_P()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);

            Action<MyObject> act = Substitute.For<Action<MyObject>>();

            IDynamicCommand target = RelayCommand.Instanciate(act, (t) => triger.Value == t.Value);

            target.CanExecute(mo).Should().BeFalse();
  
            target.MonitorEvents();

            triger.Value = 29;
            target.ShouldRaise("CanExecuteChanged").WithSender(target);
            target.CanExecute(mo).Should().BeTrue();

            target.Execute(mo);
            act.Received()(mo);

            target.Dispose();
        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_100_P_2()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);

            Action<MyObject> act = Substitute.For<Action<MyObject>>();

            IDynamicCommand target = RelayCommand.Instanciate(act, (t) => triger.Value == t.Value);

            target.CanExecute(mo).Should().BeFalse();

            target.MonitorEvents();

            mo.Value = 1;
            target.ShouldRaise("CanExecuteChanged").WithSender(target);
            target.CanExecute(mo).Should().BeTrue();

            target.Execute(mo);
            act.Received()(mo);

            target.Dispose();
        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_100_P_3()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject mo2 = new MyObject("dummy", 1);
            
            MyObject triger = new MyObject("dummy2", 1);

            Action<MyObject> act = Substitute.For<Action<MyObject>>();

            IDynamicCommand target = RelayCommand.Instanciate(act, (t) => triger.Value == t.Value);

            target.CanExecute(mo).Should().BeFalse();

            target.MonitorEvents();

            target.CanExecute(mo2).Should().BeTrue();

            mo.Value = 1;
            
            target.ShouldNotRaise("CanExecuteChanged");
           
            target.Dispose();
        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_100_P_4()
        {
            MyObject mo = new MyObject("dummy", 29);

            Action<MyObject> act = Substitute.For<Action<MyObject>>();

            IDynamicCommand target = RelayCommand.Instanciate(act, (t) => (( t==null) || (t.Value == 1)));

            target.CanExecute(null).Should().BeTrue();

            target.CanExecute(mo).Should().BeFalse();

            target.MonitorEvents();

            mo.Value = 1;
            target.ShouldRaise("CanExecuteChanged");
            target.CanExecute(mo).Should().BeTrue();

            target.Dispose();
        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_Async_2()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            int res = 300;

            Action act = () => { Thread.Sleep(4000); res = 0; };

            ICommand target = RelayCommand.InstanciateAsync(act);

            target.CanExecute(mo).Should().BeTrue();

            target.MonitorEvents();

            target.Execute(mo);

            Thread.Sleep(1000);

            target.CanExecute(mo).Should().BeFalse();
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            Thread.Sleep(6000);

            res.Should().Be(0);
            target.CanExecute(mo).Should().BeTrue();

        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_Async_2_Second_API_Call()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            int res = 300;

            Func<Task>  act = () => Task.Run(()=> { Thread.Sleep(4000); res = 0; });

            ICommand target = RelayCommand.InstanciateAsync(act);

            target.CanExecute(mo).Should().BeTrue();

            target.MonitorEvents();

            target.Execute(mo);

            Thread.Sleep(1000);

            target.CanExecute(mo).Should().BeFalse();
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            Thread.Sleep(6000);

            res.Should().Be(0);
            target.CanExecute(mo).Should().BeTrue();

        }




        [Test]
        public void TestBasic_3_Typed_Dynamic_Async_FromTask()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            int res = 300;

            Func<object, Task> act = _ => Task.Factory.StartNew(() => { Thread.Sleep(4000); res = 0; }) ;

            ICommand target = RelayCommand.InstanciateAsync(act);

            target.CanExecute(mo).Should().BeTrue();

            target.MonitorEvents();

            target.Execute(mo);

            Thread.Sleep(1000);

            target.CanExecute(mo).Should().BeFalse();
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            Thread.Sleep(6000);

            res.Should().Be(0);
            target.CanExecute(mo).Should().BeTrue();

        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_Async_FromAction()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            int res = 300;

            Action<object> act = _ => { Thread.Sleep(4000); res = 0; };

            ICommand target = RelayCommand.InstanciateAsync(act);

            target.CanExecute(mo).Should().BeTrue();

            target.MonitorEvents();

            target.Execute(mo);

            Thread.Sleep(1000);

            target.CanExecute(mo).Should().BeFalse();
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            Thread.Sleep(6000);

            res.Should().Be(0);
            target.CanExecute(mo).Should().BeTrue();

        }

        [Test]
        public void TestBasic_3_Typed_Dynamic_Async_FromTask_And_Condition()
        {
            MyObject mo = new MyObject("dummy", 29);
            MyObject triger = new MyObject("dummy2", 1);
            int res = 300;

            Func<MyObject, Task> act = _ => Task.Factory.StartNew(() => { Thread.Sleep(4000); res = 0; });

            ICommand target = RelayCommand.InstanciateAsync(act,t=>t.Value==triger.Value);

            target.CanExecute(mo).Should().BeFalse();

            target.MonitorEvents();

            target.Execute(mo);

            Thread.Sleep(1000);

            mo.Value = 1;
            target.CanExecute(mo).Should().BeFalse();
            target.ShouldRaise("CanExecuteChanged").WithSender(target);

            Thread.Sleep(6000);

            res.Should().Be(0);
            target.CanExecute(mo).Should().BeTrue();

        }

    }
}
