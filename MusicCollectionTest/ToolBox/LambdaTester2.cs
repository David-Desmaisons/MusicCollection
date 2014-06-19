using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Specialized;

using NUnit.Framework;

using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{
    class ConstantExpressionFinder<Tor, TDes> : ExpressionVisitor where Tor : class
    {
        private Expression<Func<Tor, TDes>> _Expr;
        internal ConstantExpressionFinder(Expression<Func<Tor, TDes>> expr)
        {
            _Expr = expr;
            Visit(_Expr);
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;

            if (!node.IsParameterDependant())
            {
                _C.Add(MakeAction(node));
                return node;
            }


            return base.Visit(node);
        }

        static private Expression<Func<object>> MakeAction(Expression expr)
        {
            Expression inter = Expression.Convert(expr, typeof(object));
            Expression<Func<object>> res = Expression.Lambda<Func<object>>(inter, new ParameterExpression[0]);
            return res;
        }


        private List<Expression<Func<object>>> _C = new List<Expression<Func<object>>>();
        internal IList<Expression<Func<object>>> Constants
        {
            get { return _C; }
        }
    }

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class LambdaTester2 : NotifyCompleteAdapterNoCache
    {
        private int _TestFactor = 1; //10 for perfo
        private LambdaInspectorTestor<bool> _LH1;
        private LambdaInspectorTestor<int> _LH2;
        private MyObject _Un;
        private MyObject _Deux;
        private MyObject _Trois;
        private MyObject _o1;
        private MyObject _o2;

        internal MyObject Un
        {
            get { return _Un; }
            set
            {
                if (_Un == value)
                    return;

                var old = _Un;
                _Un = value;
                PropertyHasChanged("Un", old, _Un);
            }
        }

        internal MyObject Deux
        {
            get { return _Deux; }
            set
            {
                if (_Deux == value)
                    return;

                var old = _Deux;
                _Deux = value;
                PropertyHasChanged("Deux", old, _Deux);
            }
        }



        [SetUp]
        public void setUP()
        {
            MyObject mo = new MyObject("e", 4);
            _Trois = new MyObject("eg", 2);
            Un = mo;
            Deux = mo;

            _o2 = new MyObject("e", 4);
            _o1 = new MyObject("e", 40);


        }

        [TearDown]
        public void TearDown()
        {
            if (_LH1 != null)
                _LH1.Dispose();

            if (_LH2 != null)
                _LH2.Dispose();
        }

        private class DummyVisitor : IVisitIObjectAttribute
        {
            public void Visit(IObjectAttribute io, PropertyInfo myProperty,bool iisp)
            {
            }

            public void Visit(INotifyCollectionChanged icc)
            {
            }
        }


        private class Visitor : IVisitIObjectAttribute
        {
            private List<Tuple<IObjectAttribute, PropertyInfo>> _HS = new List<Tuple<IObjectAttribute, PropertyInfo>>();
            private List<INotifyCollectionChanged> _HSc = new List<INotifyCollectionChanged>();

            public IList<Tuple<IObjectAttribute, PropertyInfo>> ObjectAtributeList
            {
                get { return _HS; }
            }

            public IList<INotifyCollectionChanged> CollectionList
            {
                get { return _HSc; }
            }

            public void Visit(IObjectAttribute io, PropertyInfo myProperty, bool iisp)
            {
                _HS.Add(new Tuple<IObjectAttribute, PropertyInfo>(io, myProperty));
            }


            public void Visit(INotifyCollectionChanged icc)
            {
                _HSc.Add(icc);
            }
        }

        [Test]
        public void test_ConstantExpressionFinder()
        {
            Expression<Func<MyObject, int>> fun = o => o.Value;
            ConstantExpressionFinder<MyObject, int> vs = new ConstantExpressionFinder<MyObject, int>(fun);
            Assert.That(vs.Constants.Count, Is.EqualTo(0));
            Expression<Func<object>> ex = () => 1;


            fun = o => 1;
            vs = new ConstantExpressionFinder<MyObject, int>(fun);
            Assert.That(vs.Constants.Count, Is.EqualTo(1));
            Assert.That(vs.Constants[0].IsTheSame(ex));
            var roto = vs.Constants[0].Compile();

            fun = o => 1 + o.Value;

            vs = new ConstantExpressionFinder<MyObject, int>(fun);
            Assert.That(vs.Constants.Count, Is.EqualTo(1));
            Assert.That(vs.Constants[0].IsTheSame(ex));
            roto = vs.Constants[0].Compile();


            fun = o => o.MyFriends.Where(f => f.Value == _Trois.Value).Count() + 1;
            vs = new ConstantExpressionFinder<MyObject, int>(fun);
            Assert.That(vs.Constants.Count, Is.EqualTo(2));
            Assert.That(vs.Constants.Contains(ex, ExpressionComparer.Comparer));
            roto = vs.Constants[0].Compile();

            Expression<Func<object>> exp2 = () => _Trois.Value;
            Assert.That(vs.Constants.Contains(exp2, ExpressionComparer.Comparer));
            roto = vs.Constants[1].Compile();

            fun = o => _Trois.Value + 5;
            Expression<Func<object>> exp3 = () => _Trois.Value + 5;
            vs = new ConstantExpressionFinder<MyObject, int>(fun);
            Assert.That(vs.Constants.Count, Is.EqualTo(1));
            Assert.That(vs.Constants.Contains(exp3, ExpressionComparer.Comparer));
            roto = vs.Constants[0].Compile();



        }

        [Test]
        public void test0()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.Value;
            ExpressionWatcherInterceptorBuilder<MyObject, int> lr = new ExpressionWatcherInterceptorBuilder<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            int res1 = func(_o1, vs);
            Assert.That(res1, Is.EqualTo(_o1.Value));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(1));
            Assert.That(vs.ObjectAtributeList.First().Item1, Is.EqualTo(_o1));

            res1 = func(_o2, vs);
            Assert.That(res1, Is.EqualTo(_o2.Value));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(2));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_o2), Is.True);

            res1 = func(_Trois, vs);
            Assert.That(res1, Is.EqualTo(_Trois.Value));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(3));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_Trois), Is.True);

            Assert.That(vs.CollectionList.Count, Is.EqualTo(0));



        }


        [Test]
        public void test00()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.MyFriends.Where(t => t.Value < 200).Count();
            ExpressionWatcherInterceptorBuilder<MyObject, int> lr = new ExpressionWatcherInterceptorBuilder<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            _o1.MyFriends.Add(_o2);
            int res1 = func(_o1, vs);
            Assert.That(res1, Is.EqualTo(_o1.MyFriends.Where(t => t.Value < 200).Count()));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(2));
            Assert.That(vs.ObjectAtributeList.First().Item1, Is.EqualTo(_o1));
            Assert.That(vs.ObjectAtributeList.Skip(1).First().Item1, Is.EqualTo(_o2));

            Assert.That(vs.CollectionList.Count, Is.EqualTo(1));
            Assert.That(vs.CollectionList[0], Is.EqualTo(_o1.MyFriends));


            res1 = func(_o2, vs);
            Assert.That(res1, Is.EqualTo(_o2.MyFriends.Where(t => t.Value < 200).Count()));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(3));
            Assert.That(vs.ObjectAtributeList.First().Item1, Is.EqualTo(_o1));
            Assert.That(vs.ObjectAtributeList.Skip(1).First().Item1, Is.EqualTo(_o2));
            Assert.That(vs.ObjectAtributeList.Skip(2).First().Item1, Is.EqualTo(_o2));


            Assert.That(vs.CollectionList.Count, Is.EqualTo(2));
            Assert.That(vs.CollectionList[1], Is.EqualTo(_o2.MyFriends));

        }


        [Test]
        public void test_Complete()
        {

            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.Value + Un.Value;
            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(1));

            Expression<Action<IVisitIObjectAttribute>> res2 = lr.BuildFromConstant();
            var t = res2.Compile();
            t(vs);
            Assert.That(vs.ObjectAtributeList.Count, Is.EqualTo(2));
            Assert.That(vs.ObjectAtributeList.Select(h => h.Item1).Contains(Un));
            Assert.That(vs.ObjectAtributeList.Select(h => h.Item1).Contains(this));


        }

        [Test]
        public void test_Complete_More()
        {

            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.Value + Un.Value + Deux.Value;
            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(2));

            Expression<Action<IVisitIObjectAttribute>> res2 = lr.BuildFromConstant();
            var t = res2.Compile();
            t(vs);
            Assert.That(vs.ObjectAtributeList.Count, Is.EqualTo(4));
            Assert.That(vs.ObjectAtributeList.Select(h => h.Item1).Contains(Un));
            Assert.That(vs.ObjectAtributeList.Select(h => h.Item1).Contains(this));
            Assert.That(vs.ObjectAtributeList.Select(h => h.Item1).Contains(Deux));

        }

        [Test]
        public void test_Complete_Less()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.Value + 1;
            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(1));

            Expression<Action<IVisitIObjectAttribute>> res2 = lr.BuildFromConstant();
            Assert.That(res2, Is.Null);

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();

            int res1 = func(_o1, vs);
            Assert.That(res1, Is.EqualTo(_o1.Value + 1));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(1));
            Assert.That(vs.ObjectAtributeList.First().Item1, Is.EqualTo(_o1));

            res1 = func(_o2, vs);
            Assert.That(res1, Is.EqualTo(_o2.Value + 1));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(2));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_o2), Is.True);

            res1 = func(_Trois, vs);
            Assert.That(res1, Is.EqualTo(_Trois.Value + 1));
            Assert.That(vs.ObjectAtributeList.Count(), Is.EqualTo(3));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_Trois), Is.True);

            Assert.That(vs.CollectionList.Count, Is.EqualTo(0));
        }


        [Test]
        public void test_Complete_Less_OneParameter()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.Value * 2 + o.Value;
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(1));

            Expression nulle = lr.BuildFromConstant();
            Assert.That(nulle, Is.Null);

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            int result = func(_o1, vs);
            Assert.That(result, Is.EqualTo(expected(_o1)));

            Assert.That(vs.ObjectAtributeList.Count, Is.EqualTo(1));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_o1));


        }

        [Test]
        public void test_Complete_Less_OneParameter_Collection()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.MyFriends.Count() * 2 + o.MyFriends.Where(f => f.Value == 0).Count();
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(2));

            Expression nulle = lr.BuildFromConstant();
            Assert.That(nulle, Is.Null);

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            int result = func(_o1, vs);
            Assert.That(result, Is.EqualTo(expected(_o1)));

            Assert.That(vs.ObjectAtributeList.Count, Is.EqualTo(1));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(_o1));
            Assert.That(vs.ObjectAtributeList[0].Item2.Name, Is.EqualTo("MyFriends"));

            Assert.That(vs.CollectionList.Count, Is.EqualTo(1));
            Assert.That(vs.CollectionList.Contains(_o1.MyFriends));

        }

        [Test]
        public void test_Complete_Less_()
        {
            Visitor vs = new Visitor();

            Expression<Func<MyObject, int>> fun = o => o.MyFriends.Select(f => f.Value + Un.Value).Sum() + 1 + Un.Value;
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(3));

            Expression<Action<IVisitIObjectAttribute>> res2 = lr.BuildFromConstant();
            Assert.That(res2, Is.Not.Null);
            Action<IVisitIObjectAttribute> fu = res2.Compile();
            fu(vs);
            Assert.That(vs.ObjectAtributeList.Count, Is.EqualTo(2));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(this));
            Assert.That(vs.ObjectAtributeList.Select(t => t.Item1).Contains(Un));

            Visitor vs2 = new Visitor();
            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            int result = func(_o2, vs2);
            Assert.That(result, Is.EqualTo(expected(_o2)));
            Assert.That(vs2.ObjectAtributeList.Count, Is.EqualTo(1));
            Assert.That(vs2.ObjectAtributeList.Select(t => t.Item1).Contains(_o2));

            Assert.That(vs2.CollectionList.Count, Is.EqualTo(1));
            Assert.That(vs2.CollectionList.Contains(_o2.MyFriends));

            result = func(_o1, null);
            Assert.That(result, Is.EqualTo(expected(_o1)));



        }



        [Test]
        public void test_Complete_Less_Visitor_Testor1()
        {
            //IListenedElementCollectionBuilder<MyObject, int> vs = ListenedElementCollection<MyObject, int>.GetBuilder();
            IListenedElementCollectionBuilder  vs = ListenedElementCollection.GetBuilder();

            Expression<Func<MyObject, int>> fun = o => o.MyFriends.Count() + 1 + o.Value;
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(1));
            Func<MyObject, IVisitIObjectAttribute, int> RealFunc = res.Compile();
            RealFunc(_o1, vs);

            //ListenedElementCollection<MyObject, int> le = vs.GetCollection();
            ListenedElementCollection le = vs.GetCollection();
            Assert.That(le, Is.Not.Null);
            Assert.That(le.ObjectAttributePropertiesListened.Count(), Is.EqualTo(1));
            ListenedElement lel = le.ObjectAttributePropertiesListened.First();

            Assert.That(lel.ListenedObject, Is.EqualTo(_o1));
            Assert.That(lel.Properties.Count(), Is.EqualTo(2));
            Assert.That(lel.Properties.Select(p => p.Name).Contains("MyFriends"));
            Assert.That(lel.Properties.Select(p => p.Name).Contains("Value"));

            Assert.That(le.CollectionListened.Count(), Is.EqualTo(1));
        }


        [Test]
        public void test_Complete_Less_Visitor_Testor2()
        {

            //IListenedElementCollectionBuilder<MyObject, int> vs = ListenedElementCollection<MyObject, int>.GetBuilder();
            IListenedElementCollectionBuilder vs = ListenedElementCollection.GetBuilder();

            Expression<Func<MyObject, int>> fun = o => o.Value;
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(0));
            Func<MyObject, IVisitIObjectAttribute, int> RealFunc = res.Compile();

            GC.Collect();
            GC.WaitForFullGCComplete();

            using (TimeTracer.TimeTrack("Raw Call"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = expected(_o1);
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            using (TimeTracer.TimeTrack("With Introspection"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = RealFunc(_o1, vs);
                    //ListenedElementCollection<MyObject, int> le = vs.GetCollection();
                    ListenedElementCollection le = vs.GetCollection();
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            using (TimeTracer.TimeTrack("With null Introspection"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = RealFunc(_o1, null);
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }


            DummyVisitor dv = new DummyVisitor();

            using (TimeTracer.TimeTrack("With Dummy Introspection"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = RealFunc(_o1, dv);
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            Visitor vsi = new Visitor();

            using (TimeTracer.TimeTrack("With Introspection with simple listner"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = RealFunc(_o1, vsi);
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }

            using (TimeTracer.TimeTrack("With Introspection only one object created"))
            {
                for (int i = 0; i < 1000000 * _TestFactor; i++)
                {
                    int resuu = RealFunc(_o1, vs);
                }
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
        }





        [Test]
        public void test_Complete_Less_Visitor()
        {
            //IListenedElementCollectionBuilder<MyObject, int> vs = ListenedElementCollection<MyObject, int>.GetBuilder();
            IListenedElementCollectionBuilder vs = ListenedElementCollection.GetBuilder();

            Expression<Func<MyObject, int>> fun = o => o.MyFriends.Select(f => f.Value + Un.Value).Sum() + 1 + Un.Value;
            Func<MyObject, int> expected = fun.Compile();

            ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int> lr = new ExpressionWatcherInterceptorBuilderParameterOnly<MyObject, int>(fun);

            Expression<Func<MyObject, IVisitIObjectAttribute, int>> res = lr.Transformed;
            Assert.That(res, Is.Not.Null);
            Assert.That(lr.Constants.Count, Is.EqualTo(3));
            Func<MyObject, IVisitIObjectAttribute, int> RealFunc = res.Compile();
            RealFunc(_o1, vs);

            //ListenedElementCollection<MyObject, int> le = vs.GetCollection();
            ListenedElementCollection le = vs.GetCollection();
            Assert.That(le, Is.Not.Null);


            Expression<Action<IVisitIObjectAttribute>> res2 = lr.BuildFromConstant();
            Assert.That(res2, Is.Not.Null);
            Action<IVisitIObjectAttribute> fu = res2.Compile();
            fu(vs);


            //quelque assert ici

            Func<MyObject, IVisitIObjectAttribute, int> func = res.Compile();
            int result = func(_o1, null);
            Assert.That(result, Is.EqualTo(expected(_o1)));
            le = vs.GetCollection();
            Assert.That(le, Is.Not.Null);

            //quelque assert ici


        }

        [Test]
        public void test30()
        {
            _LH1 = new LambdaInspectorTestor<bool>(o => (o.MyFriends.Any(f => f.Value == 0)), _Trois);

            Assert.That(_LH1.CachedValue, Is.EqualTo(false));
            Assert.That(_LH1.Check, Is.True);
        }

        [Test]
        public void test31()
        {
            _LH1 = new LambdaInspectorTestor<bool>(o => (o.Value == Un.Value) || (o.Value == Deux.Value), _Trois);

            Assert.That(_LH1.CachedValue, Is.EqualTo(false));
            Assert.That(_LH1.Check, Is.True);

            _Trois.Value = 4;

            Assert.That(_LH1.CachedValue, Is.EqualTo(true));
            Assert.That(_LH1.Check, Is.True);

            Un.Value = 2;

            Assert.That(_LH1.CachedValue, Is.EqualTo(false));
            Assert.That(_LH1.Check, Is.True);


            MyObject mj = new MyObject("e", 40);
            Deux = mj;

            Assert.That(_LH1.CachedValue, Is.EqualTo(false));
            Assert.That(_LH1.Check, Is.True);

            Un.Value = 4;
            Assert.That(_LH1.CachedValue, Is.EqualTo(true));
            Assert.That(_LH1.Check, Is.True);

            _Trois.Value = 202;
            Assert.That(_LH1.CachedValue, Is.EqualTo(false));
            Assert.That(_LH1.Check, Is.True);

            Deux.Value = 202;
            Assert.That(_LH1.CachedValue, Is.EqualTo(true));
            Assert.That(_LH1.Check, Is.True);

        }

        [Test]
        public void test_Collection()
        {
            _LH2 = new LambdaInspectorTestor<int>(o => o.MyFriends.Where(f=>f.Value<20).Sum(p=>p.Value), _Trois);

            Assert.That(_LH2.CachedValue, Is.EqualTo(0));
            Assert.That(_LH2.Check, Is.True);

            _Trois.Value = 4;

            Assert.That(_LH2.CachedValue, Is.EqualTo(0));
            Assert.That(_LH2.Check, Is.True);

            _o1.Value = 10;
            _Trois.MyFriends.Add(_o1);

            Assert.That(_LH2.CachedValue, Is.EqualTo(10));
            Assert.That(_LH2.Check, Is.True);

            _o1.Value = 5;

            Assert.That(_LH2.CachedValue, Is.EqualTo(5));
            Assert.That(_LH2.Check, Is.True);


            _o1.Value = 50;

            Assert.That(_LH2.CachedValue, Is.EqualTo(0));
            Assert.That(_LH2.Check, Is.True);

            _o2.Value = 19;
            _Trois.MyFriends.Add(_o2);

            Assert.That(_LH2.CachedValue, Is.EqualTo(19));
            Assert.That(_LH2.Check, Is.True);

            _o1.Value = 19;

            Assert.That(_LH2.CachedValue, Is.EqualTo(38));
            Assert.That(_LH2.Check, Is.True);

            _Trois.MyFriends.Remove(_o1);

            Assert.That(_LH2.CachedValue, Is.EqualTo(19));
            Assert.That(_LH2.Check, Is.True);

            Assert.That(_o1.IsObserved, Is.False);

            MyObject kk = new MyObject("Und",10);
            _Trois.MyFriends[0] = kk;
            Assert.That(_LH2.CachedValue, Is.EqualTo(10));
            Assert.That(_LH2.Check, Is.True);

            Assert.That(_o2.IsObserved, Is.False);
            Assert.That(kk.IsObserved, Is.True);

            _Trois.MyFriends.Clear();
            Assert.That(_LH2.CachedValue, Is.EqualTo(0));
            Assert.That(_LH2.Check, Is.True);

            Assert.That(kk.IsObserved, Is.False);

        }

        [Test]
        public void Test_constant()
        {

            ICompleteFunction<MyObject, string> myconstfunction = CompleteDynamicFunction<MyObject, string>
                .GetCompleteDynamicFunction(o => "Toto");

            myconstfunction.Should().NotBeNull();
            myconstfunction.IsDynamic.Should().BeFalse();
            myconstfunction.IsParameterDynamic.Should().BeFalse();
            myconstfunction.IsConstantDynamic.Should().BeFalse();

            var func = myconstfunction.EvaluateAndRegister;
            func.Should().NotBeNull();
            func(null).Should().Be("Toto");

            MyObject to = new MyObject("lolol", 23);
            myconstfunction.Register(to).Should().BeTrue();

            myconstfunction.Listened.Should().Equal(to.SingleItemCollection());

            myconstfunction.Register(to).Should().BeFalse();
            myconstfunction.Listened.Should().Equal(to.SingleItemCollection());

            myconstfunction.ListenedCount.Should().Be(1);
            var lcv = myconstfunction.ListenedandCachedValue;
            lcv.Count().Should().Be(1);
            var ue = lcv.FirstOrDefault();
            ue.Should().NotBeNull();
            ue.Item1.Should().Be(to);
            ue.Item2.Should().Be("Toto");


            myconstfunction.UnRegister(to).Should().BeFalse();
            myconstfunction.Listened.Should().Equal(to.SingleItemCollection());

            myconstfunction.UnRegister(to).Should().BeTrue();
            myconstfunction.Listened.Should().BeEmpty();


            ConstantFunctionAdaptor<MyObject, string> cfa = myconstfunction as ConstantFunctionAdaptor<MyObject, string>;
            cfa.Should().NotBeNull();
        }


    }
}
