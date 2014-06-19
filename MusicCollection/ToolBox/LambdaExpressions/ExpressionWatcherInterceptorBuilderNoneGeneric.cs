using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class ExpressionWatcherInterceptorBuilderNoneGeneric : ExpressionVisitor
    {
        internal ExpressionWatcherInterceptorBuilderNoneGeneric(Expression Original, ParameterExpression Visitor )
        {
            _Visitor = Visitor;
            BasicTransformed = Visit(Original);
        }

        private bool _Changed = false;

        protected readonly ParameterExpression _Visitor;

        private static readonly MethodInfo _ObjectAttributeExtractor;
        private static readonly MethodInfo _ObjectCollectionExtractor;
        private static readonly Type[] _Tuples;

        private HashSet<Expression> _Treated = new HashSet<Expression>(ExpressionComparer.Comparer);

        internal Expression BasicTransformed
        {
            get;
            private set;
        }

        internal bool Changed
        {
            get { return _Changed; }
        }

        static ExpressionWatcherInterceptorBuilderNoneGeneric()
        {
            _ObjectAttributeExtractor = typeof(ExpressionWatcherInterceptorBuilderNoneGeneric).GetMethod("ObjectAttributeExtractor", BindingFlags.NonPublic | BindingFlags.Static);
            _ObjectCollectionExtractor = typeof(ExpressionWatcherInterceptorBuilderNoneGeneric).GetMethod("ObjectCollectionExtractor", BindingFlags.NonPublic | BindingFlags.Static);
            _Tuples = new Type[] { typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>), typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>) };
        }

        private static bool IsTypeTuple(Type itype)
        {
            if (!itype.IsGenericType)
                return false;

            return _Tuples.Contains(itype.GetGenericTypeDefinition());
        }

        private static object ObjectAttributeExtractor(object MyRef, PropertyInfo myProperty, IVisitIObjectAttribute iioa, bool Isparameter)
        {
            IObjectAttribute io = MyRef as IObjectAttribute;
            if (io == null)
                return MyRef;

            if (iioa != null)
                iioa.Visit(io, myProperty, Isparameter);

            return MyRef;
        }

        private static object ObjectCollectionExtractor(object MyRef, IVisitIObjectAttribute iioa)
        {
            INotifyCollectionChanged io = MyRef as INotifyCollectionChanged;
            if (io == null)
                return MyRef;

            if (iioa != null)
                iioa.Visit(io);

            return MyRef;
        }

        private Expression InvestigatePotencialObservable(Expression canditate)
        {
            if (canditate == null)
                return null;

            bool IsEnumerable = canditate.Type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Any();
            if (IsEnumerable && canditate.Type != typeof(string))
            {
                if (_Treated.Contains(canditate))
                {
                    return canditate;
                }
                else
                {
                    Expression Called = Expression.Call(null, _ObjectCollectionExtractor, Visit(canditate), this._Visitor);
                    Expression newarg = Expression.Convert(Called, canditate.Type);
                    _Treated.Add(newarg);
                    _Changed = true;
                    return newarg;
                }
            }
            else
            {
                return Visit(canditate);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            List<Expression> newargs = new List<Expression>();
            foreach (Expression arg in node.Arguments)
            {
                //Expression newarg = null;
                //bool IsEnumerable = arg.Type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Any();
                //if (IsEnumerable && arg.Type!=typeof(string))
                //{
                //    if (_Treated.Contains(arg))
                //    {
                //        newarg = arg;
                //    }
                //    else
                //    {
                //        Expression Called = Expression.Call(null, _ObjectCollectionExtractor, Visit(arg), this._Visitor);
                //        newarg = Expression.Convert(Called, arg.Type);
                //        _Treated.Add(newarg);
                //        _Changed = true;
                //    }
                //}
                //else
                //{
                //    newarg = Visit(arg);
                //}
                //newargs.Add(newarg);

                newargs.Add(InvestigatePotencialObservable(arg));
            }

            //return Expression.Call(Visit(node.Object), node.Method, newargs);

            return Expression.Call(InvestigatePotencialObservable(node.Object), node.Method, newargs);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (_Treated.Contains(node))
                return node;

            if ( ((node.Member.MemberType == MemberTypes.Property) && (node.Expression != null))
                   && (node.Expression.Type.IsValueType == false) && (! IsTypeTuple(node.Expression.Type)) )
            {
  
                PropertyInfo pi = node.Member as PropertyInfo;
                Expression Called = Expression.Call(null, _ObjectAttributeExtractor, Visit(node.Expression), Expression.Constant(pi),
                    this._Visitor, Expression.Constant(node.Expression.NodeType == ExpressionType.Parameter));
                Expression inter = Expression.Convert(Called, pi.DeclaringType);
                Expression res = Expression.MakeMemberAccess(inter, node.Member);

                _Treated.Add(node);

                _Changed = true;

                return res;
            }


            return base.VisitMember(node);
        }
    }
}
