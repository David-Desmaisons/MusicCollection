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

    class ExpressionWatcherInterceptorBuilder<Tor, TDes> : ExpressionWatcherInterceptorBuilderNoneGeneric
    {


        internal Expression<Func<Tor, IVisitIObjectAttribute, TDes>> Transformed
        {
            get { return this.BasicTransformed as Expression<Func<Tor, IVisitIObjectAttribute, TDes>>; }
        }


        internal ExpressionWatcherInterceptorBuilder(Expression<Func<Tor, TDes>> Original)
            : base(Original,Expression.Parameter(typeof(IVisitIObjectAttribute)))
        {
        }

        private bool _First = true;
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (_First)
            {
                _First = false;
                Expression<Func<Tor, TDes>> trans = base.VisitLambda(node) as Expression<Func<Tor, TDes>>;
                return Expression.Lambda<Func<Tor, IVisitIObjectAttribute, TDes>>(trans.Body, new ParameterExpression[2] { trans.Parameters[0], this._Visitor });
            }

            return base.VisitLambda(node);
        }
    }

}
