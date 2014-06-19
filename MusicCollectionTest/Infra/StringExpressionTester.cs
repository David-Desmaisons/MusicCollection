using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;

using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.Infra;
using MusicCollection.ToolBox.StringExpression;


namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Infra")]
    [NUnit.Framework.Category("Unitary")]
    class StringExpressionTester
    {

        //[Test]
        //public void Test0()
        //{

        //    StringTokenizer uu = new StringTokenizer("((toto=1)and(ID=3))and Name=55 or ((toto=6) or (Name=false))");
        //    Console.WriteLine(string.Join("\n", uu.GetTokens().ToList()));

        //    uu = new StringTokenizer("((toto=1)and(ID=3))and Name=55");
        //    Console.WriteLine(string.Join("\n", uu.GetTokens().ToList()));
        //    Console.WriteLine();

        //    uu = new StringTokenizer(@"((toto=1)and(ID=3))and Name=55 and name=""thhtht bbbb tbb"" and(ID=3)");
        //    Console.WriteLine(string.Join("\n", uu.GetTokens().ToList()));
        //    Console.WriteLine();


        //    uu = new StringTokenizer(@"((toto!=1)and(ID=3))and Name=55 and name=""thhtht bbbb tbb"" and(ID!=3)");
        //    Console.WriteLine(string.Join("\n", uu.GetTokens().ToList()));
        //    Console.WriteLine();

        //    uu = new StringTokenizer(@"((toto!=1)and(ID=3))and Name=55 and name=""thhtht, 77& )  ( bbbb tbb"" and(ID!=3)");
        //    Console.WriteLine(string.Join("\n", uu.GetTokens().ToList()));
        //    Console.WriteLine();
        //}

        private void psct(Expression<Func<MyObject, bool>> expr)
        {
            Console.WriteLine(expr);
        }


        private void Tester(string iexpression, Func<MyObject, bool> ifunc)
        {
            ExpressionFilterBuilder<MyObject> trtr = new ExpressionFilterBuilder<MyObject>(iexpression);
            Console.WriteLine(trtr);

            if (ifunc != null)
                Assert.That(trtr.ExpressionResult, Is.Not.Null);
            else
            {
                Assert.That(trtr.ExpressionResult, Is.Null);
                return;
            }

            Func<MyObject, bool> target = trtr.ExpressionResult.Compile();

            foreach (var o in _Os)
            {
                Assert.That(target(o), Is.EqualTo(ifunc(o)));
            }

            Expression<Func<MyObject, bool>> myexp = trtr;
            Func<MyObject, bool> target2 = trtr.ExpressionResult.Compile();

            this.psct(trtr);

            foreach (var o in _Os)
            {
                Assert.That(target(o), Is.EqualTo(target2(o)));
            }

        }

        private List<MyObject> _Os;

        [SetUp]
        public void SU()
        {
            _Os = new List<MyObject>()
            {
                new MyObject("12", 45),
                new MyObject("1rrr2", 45),
                new MyObject("toto", 20),
                new MyObject("toti", 10),
                new MyObject("toti4", 1),
                new MyObject("toti4", 100),
            };
        }


        [Test]
        public void ExpressionFilterBuilder()
        {
            Tester("(ID>3)", (o) => (o.ID > 3));

            Tester("(ID>=3)", (o) => (o.ID >= 3));

            Tester("((Name=gygy)or(ID=34))", (o) => ((o.Name == "gygy") || (o.ID == 34)));

            Tester("Name=and", (o) => o.Name == "and");


            Tester("(ID<2)", (o) => (o.ID < 2));

            Tester("(ID<=2)", (o) => (o.ID <= 2));

            Tester("Name=12", (o) => (o.Name == "12"));

            Tester("((Name=12)or(ID=1))", (o) => ((o.Name == "12") || (o.ID == 1)));

            Tester("((Name=12)or(Value=42))", (o) => ((o.Name == "12") || (o.Value == 42)));

            Tester(@"((Name!=1)and(ID=3))and Name=55 and Name=""thhtht, 77& )  ( bbbb tbb"" and(ID!=3)", (o) => (
                ((o.Name != "1")
                && (o.ID == 3))
                && (o.Name == "55")
                && (o.Name == @"thhtht, 77& )  ( bbbb tbb")
                && (o.ID != 3))
                );

            Tester("((ID=3)or((Value=42)and(Name=1rrr2)))", (o) => ((o.ID == 3) || ((o.Value == 42) && o.Name == "1rrr2")));


            Tester("Name=", null);
            Tester("(Name=", null);
            Tester(")Name=", null);

            Tester(@"Name=""""", (o) => o.Name == string.Empty);

            Expression<Func<MyObject, bool>> t = new ExpressionFilterBuilder<MyObject>("(ID>3)and(Name=toti4)");

            Func<MyObject, bool> func = t.Compile();

            Assert.That(func(_Os[5]), Is.True);
            Assert.That(func(_Os[0]), Is.False);

        }


        private void Tester<T>(string iexpression, Func<MyObject, T> ifunc)
        {
            ExpressionMemberBuilder<MyObject, T> trtr = new ExpressionMemberBuilder<MyObject, T>(iexpression);
            Console.WriteLine(trtr);

            if (ifunc != null)
                Assert.That(trtr.ExpressionResult, Is.Not.Null);
            else
            {
                Assert.That(trtr.ExpressionResult, Is.Null);
                return;
            }

            Func<MyObject, T> target = trtr.ExpressionResult.Compile();

            foreach (var o in _Os)
            {
                Assert.That(target(o), Is.EqualTo(ifunc(o)));
            }

        }

        [Test]
        public void ExpressionMemberFilter()
        {

            Tester<string>("Name", (o) => o.Name);

            Tester<string>("Friends", null);

            Tester<int>("ID", (o) => o.ID);

            Tester<int>("13", null);


            ExpressionFilterBuilder<MyObject> emd = new ExpressionFilterBuilder<MyObject>("Value=0");
            emd.BuildingException.Should().BeNull();
            emd.ExpressionResult.Should().NotBeNull();

            Func<MyObject, bool> func = ((Expression<Func<MyObject, bool>>)(emd)).Compile();

            foreach (var o in _Os)
            {
                func(o).Should().Be(o.Value == 0);
            }

            emd = new ExpressionFilterBuilder<MyObject>("Value=0");
            emd.BuildingException.Should().BeNull();
            emd.ExpressionResult.Should().NotBeNull();

            Func<MyObject, bool> func2 = ((Expression<Func<MyObject, bool>>)(emd)).Compile();

            foreach (var o in _Os)
            {
                func2(o).Should().Be(o.Value == 0);
            }

            emd = new ExpressionFilterBuilder<MyObject>("Values=0");
            emd.BuildingException.Should().NotBeNull();
            emd.ExpressionResult.Should().BeNull();


        }

        [Test]
        public void TestExpressionFilterBuilderCache()
        {
            ExpressionMemberBuilder<MyObject, int> emd = new ExpressionMemberBuilder<MyObject, int>("Value");
            emd.BuildingException.Should().BeNull();
            emd.ExpressionResult.Should().NotBeNull();

            Func<MyObject, int> func = emd.ExpressionResult.Compile();

            foreach (var o in _Os)
            {
                func(o).Should().Be(o.Value);
            }

            emd = new ExpressionMemberBuilder<MyObject, int>("Value");
            emd.BuildingException.Should().BeNull();
            emd.ExpressionResult.Should().NotBeNull();

            Func<MyObject, int> func2 = emd.ExpressionResult.Compile();

            foreach (var o in _Os)
            {
                func2(o).Should().Be(o.Value);
            }
        }
    }
}
