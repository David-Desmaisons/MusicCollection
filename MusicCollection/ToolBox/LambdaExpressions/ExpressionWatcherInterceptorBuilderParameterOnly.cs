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
    class ExpressionWatcherInterceptorBuilderParameterOnly<Tor, TDes> : ExpressionWatcherInterceptorBuilder<Tor, TDes> where Tor : class
    {
        internal ExpressionWatcherInterceptorBuilderParameterOnly(Expression<Func<Tor, TDes>> Original)
            : base(Original)
        {
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;

            if (!node.IsParameterDependant())
            {
                _C.Add(node);
                return node;
            }

            return base.Visit(node);
        }

        internal Expression<Action<IVisitIObjectAttribute>>  BuildFromConstant()
        {
            if (_C.Count==0)
                return null;

            ParameterExpression pe = Expression.Parameter(typeof(IVisitIObjectAttribute));

            List<Expression<Action<IVisitIObjectAttribute>>> Res = new List<Expression<Action<IVisitIObjectAttribute>>>();
            HashSet<Expression> OriginalList = new HashSet<Expression>(_C,ExpressionComparer.Comparer);

            foreach (Expression c in OriginalList)
            {
                ExpressionWatcherInterceptorBuilderNoneGeneric ewnc = new ExpressionWatcherInterceptorBuilderNoneGeneric(c,pe);
                if (!ewnc.Changed)
                    continue;

                Res.Add(Expression.Lambda<Action<IVisitIObjectAttribute>>(ewnc.BasicTransformed, pe));
            }

            if (Res.Count==0)
                return null;

            if (Res.Count == 1)
            {
                return Res[0];
            }
            
            Expression almost = Expression.Block(Res.Select(e => Expression.Invoke(e, pe)));
            return Expression.Lambda<Action<IVisitIObjectAttribute>>(almost, pe);           
        }

        private List<Expression> _C = new List<Expression>();
        internal IList<Expression> Constants
        {
            get { return _C; }
        }
    }
}

