using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using NUnit.Framework;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{
    internal class ExpressionTransformer : ExpressionVisitor
    {
        private List<Expression> _Possibilities;
        private Expression _Out;


        private Lazy<HashSet<Expression>> _DirectEmbeded = new Lazy<HashSet<Expression>>(() => new HashSet<Expression>(ExpressionComparer.Comparer));


        private bool _SharpMode;

        private class PrivateVistor : ExpressionVisitor
        {
            private ExpressionTransformer _Father;
            private Expression _In;
            private Expression _Out;
            private string _ParameterName;

            internal bool Success
            {
                get;
                private set;

            }


            internal PrivateVistor(ExpressionTransformer fa, Expression In, Expression iOut)
            {
                _In = In;
                _Father = fa;
                Success = false;
                _ParameterName = _Father.ParameterName;
                _Out = iOut;
            }

            private ParameterExpression _Para;
            private ParameterExpression Parameter
            {
                get
                {
                    if (_Para == null)
                    {
                        _Para = (_ParameterName == null) ? Expression.Parameter(_In.Type) : Expression.Parameter(_In.Type, _ParameterName);
                    }

                    return _Para;
                }

            }

            internal LambdaExpression GetResult()
            {
                Expression intres = Visit(_Out);

                if (Success == false)
                    return null;



                return (_OriginalParameter == null) ? Expression.Lambda(intres, new ParameterExpression[] { Parameter }) : Expression.Lambda(intres, new ParameterExpression[] { Parameter, _OriginalParameter });
            }

            private ParameterExpression _OriginalParameter;
            protected override Expression VisitParameter(ParameterExpression node)
            {
                _OriginalParameter = Expression.Parameter(node.Type, "p");

                return _OriginalParameter;
            }

            public override Expression Visit(Expression node)
            {
                if (_Father.Compare(node, _In))
                {
                    Success = true;
                    return Parameter;
                }

                return base.Visit(node);
            }
        }

        private bool Compare(Expression comp, Expression Comp2)
        {
            if (_SharpMode)
                return comp == Comp2;

            return comp.IsTheSame(Comp2);
        }

        internal string ParameterName
        {
            get;
            private set;
        }

        internal ExpressionTransformer(Expression Out, Expression iIN, bool iSharpMode, string pn = null)
        {
            //_In = iIN;
            _SharpMode = iSharpMode;
            ParameterName = pn;
            _Out = Out;
            this._Possibilities = new List<Expression>();
            _Possibilities.Add(iIN);
            Visit(Out);
        }

        internal ExpressionTransformer(Expression Out, IEnumerable<Expression> iIN, bool iSharpMode, string pn = null)
        {
            _SharpMode = iSharpMode;
            _Out = Out;
            ParameterName = pn;
            this._Possibilities = iIN.ToList();
            Visit(Out);
        }

        public override Expression Visit(Expression node)
        {
            Expression ok = this._Possibilities.FirstOrDefault(p => Compare(p, node));

            if (ok != null)
            {
                //ExpressionDecorator ed = new ExpressionDecorator(ok);
                _DirectEmbeded.Value.Add(ok);

                return node;
            }

            return base.Visit(node);
        }

        internal IEnumerable<Expression> Success
        {
            get
            {
                return this._DirectEmbeded.Value;
            }
        }



        internal LambdaExpression GetTransformer(Expression In)
        {
            PrivateVistor pv = new PrivateVistor(this, In, _Out);
            return pv.GetResult();
        }
    }


    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class LambdaExtensionTestor
    {
        private Expression _IsName;
        private Expression _IsName2;
        private Expression _Name;
        private Expression _Name2;
        private Expression _My;
        private Expression _My2;
        private Expression _Un;
        private Expression _Un2;


        private Expression SetExpression<T>(Expression<Func<MyObject, T>> Ent, int t=0)
        {
            if (t==0)
                return Ent.Body;

            if (t == 1)
                return Ent;

            return null;
        }


        //[SetUp]
        //public void SU()
        //{
        //    _IsName = SetExpression(o => o.ID);
        //    _IsName2 = SetExpression(o => o.ID);
        //    _Name = SetExpression(o => o.Name);
        //    _Name2 = SetExpression(o => o.Name);
        //    _My = SetExpression(o => this);
        //    _My2 = SetExpression(o => this);
        //    _Un = SetExpression(o=>2);
        //    _Un2 = SetExpression(o => 2);

        //}


        private void AssertEq(Expression Un, Expression Deux)
        {
            Assert.That(Un.IsTheSame(Deux));
            Assert.That(Deux.IsTheSame(Un));
            Assert.That(Un.GetRelevantHashCode(), Is.EqualTo(Deux.GetRelevantHashCode()));
        }


        private void AssertExpressionIn(Expression Out, Expression In)
        {
            //Assert.That(In.IsEmbeddedIn(Out));
            //Assert.That(Out.IsEmbeddedIn(In),Is.False);

            //ExpressionDependency ed = new ExpressionDependency(Out, In, false);
            //Assert.That(ed.Success);

            //ed = new ExpressionDependency(In, Out, false);
            //Assert.That(ed.Success,Is.False);

            //Assert.That(In.IsEmbeddedIn(Out));

        }

        private void AssertDiff(Expression Un, Expression Deux)
        {
            Assert.That(Un.IsTheSame(Deux), Is.False);
            Assert.That(Deux.IsTheSame(Un), Is.False);
        }

        private void TestExpression()
        {
            Assert.That(_IsName, Is.Not.EqualTo(_IsName2));
            AssertEq(_IsName, _IsName2);

            Assert.That(_Name, Is.Not.EqualTo(_Name2));
            AssertEq(_Name, _Name2);

            Assert.That(_My, Is.Not.EqualTo(_My2));
            AssertEq(_My, _My2);

            Assert.That(_Un, Is.Not.EqualTo(_Un2));
            AssertEq(_Un, _Un2);

            AssertDiff(_IsName, _Name);
            AssertDiff(_IsName, _Name2);
            AssertDiff(_IsName2, _Name);
            AssertDiff(_IsName2, _Name2);
            AssertDiff(_IsName, _My);
            AssertDiff(_IsName2, _My2);
            AssertDiff(_IsName, _My2);
            AssertDiff(_IsName2, _My);
        }

        [Test]
        public void test1()
        {
            _IsName = SetExpression(o => o.ID);
            _IsName2 = SetExpression(o => o.ID);
            _Name = SetExpression(o => o.Name);
            _Name2 = SetExpression(o => o.Name);
            _My = SetExpression(o => this);
            _My2 = SetExpression(o => this);
            _Un = SetExpression(o => 2);
            _Un2 = SetExpression(o => 2);


            TestExpression();

        }

        [Test]
        public void test2()
        {
            _IsName = SetExpression(o => o.ID,1);
            _IsName2 = SetExpression(o => o.ID,1);
            _Name = SetExpression(o => o.Name,1);
            _Name2 = SetExpression(o => o.Name,1);
            _My = SetExpression(o => this,1);
            _My2 = SetExpression(o => this,1);
            _Un = SetExpression(o => 2,1);
            _Un2 = SetExpression(o => 2,1);


            TestExpression();

        }

        private LambdaExpression TestExpression2()
        {
            ExpressionTransformer et = new ExpressionTransformer(_IsName,_IsName2, false, "o");
            Assert.That(et.Success.Count(), Is.EqualTo(1));
            Assert.That(et.Success.Contains(_IsName2));

            //ExpressionDependency ed = new ExpressionDependency(_IsName, _IsName2,false);
            //Assert.That(ed.Success);

            LambdaExpression res = et.GetTransformer(_IsName2);
            this.AssertEq(res, _Name);
            return res;
        }

        [Test]
        public void test0()
        {
            _IsName = SetExpression(o => o.Friend.ID, 0);
            _IsName2 = SetExpression(o => o.Friend, 0);
            _Name = SetExpression(o => o.ID, 1);

            TestExpression2();
        }

        [Test]
        public void test01()
        {
            _IsName = SetExpression(o => o.Friend.ToString().Length%2, 0);
            _IsName2 = SetExpression(o => o.Friend, 0);
            _Name = SetExpression(o => o.ToString().Length%2, 1);

            TestExpression2();
        }

        [Test]
        public void test02()
        {
            _IsName = SetExpression(o => o.Friend.Friend.Name.Length % 2, 0);
            _IsName2 = SetExpression(o => o.Friend.Friend, 0);
            _Name = SetExpression(o => o.Name.Length % 2, 1);

            TestExpression2();
        }

        [Test]
        public void test03()
        {
            _IsName = SetExpression(o => o.Friend.Friend.Name.Length % 2, 0);
            _IsName2 = SetExpression(o => o.Friend.Friend.Name.Length, 0);

            Expression<Func<int, int>> name = o => o % 2;
            _Name = name;



            LambdaExpression res = TestExpression2();



            Delegate res2 = res.Compile();
            object resf = res2.DynamicInvoke(new object[1] { 2 });

            Assert.That(resf, Is.EqualTo(0));

            resf = res2.DynamicInvoke(new object[1] { 1 });

            Assert.That(resf, Is.EqualTo(1));


        }

        [Test]
        public void test04()
        {
            _IsName = SetExpression(o => o.Friend.Name.Length + o.Friend.ID, 0);
            _IsName2 = SetExpression(o => o.Friend, 0);

            _Name = SetExpression(o => o.Name.Length + o.ID, 1);

            TestExpression2();
        }

        [Test]
        public void test05()
        {
            _IsName = SetExpression(o => o.Friend.Friend.Name.Length % 2, 0);
            _IsName2 = SetExpression(o => o.Friend.Friend, 0);
            this._Un = SetExpression(o => o.Friend, 0);

            _Name = SetExpression(o => o.Name.Length % 2, 1);
            _Name2 = SetExpression(o => o.Friend.Name.Length % 2, 1);

            AssertExpressionIn(_IsName, _IsName2);

            //ExpressionDependency ed = new ExpressionDependency(_IsName, _IsName2, false);
            //Assert.That(ed.Success);

            AssertExpressionIn(_IsName, _Un);

            //ed = new ExpressionDependency(_IsName, _Un, false);
            //Assert.That(ed.Success);

            ExpressionTransformer et = new ExpressionTransformer(_IsName ,new List<Expression> { _IsName2,_Un}, false, "o");
            Assert.That(et.Success.Count(), Is.EqualTo(1));
            Assert.That(et.Success.Contains(_IsName2));

            Expression res1 = et.GetTransformer(_IsName2);
            this.AssertEq(res1, _Name);
            res1 = et.GetTransformer(_Un);
            this.AssertEq(res1, _Name2);


            et = new ExpressionTransformer(_IsName, new List<Expression> { _Un }, false, "o");
            Assert.That(et.Success.Count(), Is.EqualTo(1));
            Assert.That(et.Success.Contains(_Un));

            res1 = et.GetTransformer(_Un);
            this.AssertEq(res1, _Name2);


        }


        [Test]
        public void test06()
        {
            _IsName = SetExpression(o => o.Friend.Friend.Name.Length + o.Friend.ID, 0);
            _IsName2 = SetExpression(o => o.Friend.Friend, 0);
            this._Un = SetExpression(o => o.Friend, 0);

            _Name = SetExpression(o => o.Friend.Name.Length + o.ID, 1);

            AssertExpressionIn(_IsName, _IsName2);
            //ExpressionDependency ed = new ExpressionDependency(_IsName, _IsName2, false);
            //Assert.That(ed.Success);

            AssertExpressionIn(_IsName, _Un);
            //ed = new ExpressionDependency(_IsName, _Un, false);
            //Assert.That(ed.Success);
 
            ExpressionTransformer et = new ExpressionTransformer(_IsName, new List<Expression> { _IsName2, _Un }, false, "o");
            Assert.That(et.Success.Count(), Is.EqualTo(2));
            Assert.That(et.Success.Contains(_IsName2));
            Assert.That(et.Success.Contains(_Un));

            LambdaExpression res1 = et.GetTransformer(_Un);
            this.AssertEq(res1, _Name);


            res1 = et.GetTransformer(_IsName2);
            //this.AssertEq(res1, _Name2);

        }
    }
}
