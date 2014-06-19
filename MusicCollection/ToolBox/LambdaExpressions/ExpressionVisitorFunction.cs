using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class ExpressionVisitorFunction<TDes> : ExpressionWatcherInterceptorBuilderNoneGeneric
    {
         internal Expression<Func<IVisitIObjectAttribute, TDes>> Transformed
        {
            get { return this.BasicTransformed as Expression<Func<IVisitIObjectAttribute, TDes>>; }
        }


        internal ExpressionVisitorFunction(Expression<Func<TDes>> Original)
            : base(Original,Expression.Parameter(typeof(IVisitIObjectAttribute)))
        {
        }

        private bool _First = true;
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (_First)
            {
                _First = false;
                Expression<Func<TDes>> trans = base.VisitLambda(node) as Expression<Func<TDes>>;
                return Expression.Lambda<Func<IVisitIObjectAttribute, TDes>>(trans.Body, new ParameterExpression[] { this._Visitor });
            }

            return base.VisitLambda(node);
        }
    }
}
