using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.LambdaExpressions;
using System.Collections.Specialized;
using System.Reflection;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class ExpressionVisitorFunctionTest
    {
        [Test]
        public void Test_1()
        {
            IVisitIObjectAttribute iviv = Substitute.For<IVisitIObjectAttribute>();

            Expression<Func<int>> myexpression = () => 35; 
            ExpressionVisitorFunction<int> evf = new ExpressionVisitorFunction<int>(myexpression);
            Func<IVisitIObjectAttribute, int> func = evf.Transformed.Compile();

            int res = func(iviv);

            res.Should().Be(35);

            iviv.DidNotReceive().Visit(Arg.Any<INotifyCollectionChanged>());
            iviv.DidNotReceive().Visit(Arg.Any<IObjectAttribute>(), Arg.Any<PropertyInfo>(), Arg.Any<bool>());
        }

        [Test]
        public void Test_2()
        {
            IVisitIObjectAttribute iviv = Substitute.For<IVisitIObjectAttribute>();
            MyObject mo = new MyObject("Simple", 65);

            Expression<Func<int>> myexpression = () => mo.Value;
            ExpressionVisitorFunction<int> evf = new ExpressionVisitorFunction<int>(myexpression);
            Func<IVisitIObjectAttribute, int> func = evf.Transformed.Compile();

            int res = func(iviv);
            res.Should().Be(65);

            iviv.DidNotReceive().Visit(Arg.Any<INotifyCollectionChanged>());
            iviv.Received().Visit(mo, Arg.Any<PropertyInfo>(), false);
        }

        [Test]
        public void SimpleFunction_Test_1()
        {
            MyObject mo = new MyObject("Simple", 98);
            SimpleFunction<int> sf = new SimpleFunction<int>(() => mo.Value);

            sf.MonitorEvents();
            sf.Value.Should().Be(98);
            sf.ShouldNotRaisePropertyChangeFor(o=>o.Value);

            sf.Dispose();
        }

        [Test]
        public void SimpleFunction_Test_2()
        {
            MyObject mo = new MyObject("Simple", 98);
            SimpleFunction<int> sf = new SimpleFunction<int>(() => mo.Value);

            sf.MonitorEvents();
            sf.Value.Should().Be(98);
            sf.ShouldNotRaisePropertyChangeFor(o => o.Value);

            mo.Name="Complexe";
            sf.ShouldNotRaisePropertyChangeFor(o => o.Value);
            sf.Value.Should().Be(98);


            mo.Value = 120;
            sf.Value.Should().Be(120);
            sf.ShouldRaisePropertyChangeFor(o => o.Value);


            sf.Dispose();

        }

        [Test]
        public void SimpleFunction_Test_3()
        {
            MyObject mo = new MyObject("Simple", 98);
            MyObject mi = new MyObject("Simple2", 100);
            mo.Friend = mi;


            SimpleFunction<int> sf = new SimpleFunction<int>(() => mo.Friend.Value);

            sf.MonitorEvents();
            sf.Value.Should().Be(100);
            sf.ShouldNotRaisePropertyChangeFor(o => o.Value);

            mo.Name = "Complexe";
            sf.ShouldNotRaisePropertyChangeFor(o => o.Value);
            sf.Value.Should().Be(100);


            mo.Value = 120;
            sf.Value.Should().Be(100);
            sf.ShouldNotRaisePropertyChangeFor(o => o.Value);

            mi.Value = 130;
            sf.Value.Should().Be(130);
            sf.ShouldRaisePropertyChangeFor(o => o.Value);

            MyObject nf = new MyObject("nff", -255);
            mo.Friend = nf;
            sf.Value.Should().Be(-255);
            sf.ShouldRaisePropertyChangeFor(o => o.Value);

            sf.Dispose();

        }

        [Test]
        public void CompileToObservable_Test_1()
        {
            MyObject mo = new MyObject("Simple", 98);
        
            Expression<Func<string>> ef = () => mo.Name;
            IFunction<string> target = ef.CompileToObservable();

            target.Should().NotBeNull();
            target.Value.Should().Be("Simple");
        }

        [Test]
        public void CompileToObservable_Test_Collection()
        {
            MyObject mo = new MyObject("Simple", 98);
            MyObject mi = new MyObject("Simple2", 100);

            Expression<Func<int>> ef = () => mo.MyFriends.Count();
            IFunction<int> target = ef.CompileToObservable();

            target.Should().NotBeNull();
            target.Value.Should().Be(0);

            target.MonitorEvents();
            mo.MyFriends.Add(mi);
            target.Value.Should().Be(1);
            target.ShouldRaisePropertyChangeFor(s => s.Value);

            target.Dispose();
        }

        [Test]
        public void CompileToObservable_Test_Collection_Complex()
        {
            MyObject mo = new MyObject("Simple", 98);
            MyObject mi = new MyObject("Simple2", 100);
            mo.Friend = mi;

            Expression<Func<int>> ef = () => mo.Friend.MyFriends.Count();
            IFunction<int> target = ef.CompileToObservable();

            target.Should().NotBeNull();
            target.Value.Should().Be(0);

            target.MonitorEvents();
            mi.MyFriends.Add(mi);
            target.Value.Should().Be(1);
            target.ShouldRaisePropertyChangeFor(s => s.Value);

            mi.MyFriends.Add(mo);
            target.Value.Should().Be(2);
            target.ShouldRaisePropertyChangeFor(s => s.Value);

            mo.Friend = mo;
            target.Value.Should().Be(0);
            target.ShouldRaisePropertyChangeFor(s => s.Value);

            mo.MyFriends.Add(mo);
            target.Value.Should().Be(1);
            target.ShouldRaisePropertyChangeFor(s => s.Value);

            target.Dispose();
        }

        [Test]
        public void CompileToObservable_Test_2()
        {
            Expression<Func<string>> ef = null;
            IFunction<string> target = null;

            Action willfail = () => target = ef.CompileToObservable();

            willfail.ShouldThrow<Exception>();
        }


    }
}
