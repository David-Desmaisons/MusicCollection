using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.ToolBox.LambdaExpressions;

namespace MusicCollection.Infra
{
    public static class GlobalExpressionExtender
    {
        public static IFunction<Tor, TDes> CompileToConst<Tor, TDes>(this Func<Tor, TDes> Func) where Tor : class //,IObjectAttribute
        {
            if (Func == null)
                throw new ArgumentNullException();

            return new ConstantFunctionAdaptor<Tor, TDes>(Func);
        }

        public static IFunction<TDes> CompileToObservable<TDes>(this Expression<Func<TDes>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();

            return new SimpleFunction<TDes>(expression);
        }

        public static IPropertyFunction<TDes> CompileToObservable<TDes>(this Expression<Func<TDes>> expression, string iPropertyName)
        {
            if (expression == null)
                throw new ArgumentNullException();

            return new SimplePropertyFunction<TDes>(expression, iPropertyName);
        }


        internal static ICompleteFunction<Tor, TDes> CompileToObservable<Tor, TDes>(this Expression<Func<Tor, TDes>> expression) where Tor : class //,IObjectAttribute
        {
            if (expression == null)
                throw new ArgumentNullException();

            return CompleteDynamicFunction<Tor, TDes>.GetCompleteDynamicFunction(expression);
        }

        public static ISimpleFunction<Tor, TDes> CompileToObservableFunction<Tor, TDes>(this Expression<Func<Tor, TDes>> expression) where Tor : class //,IObjectAttribute
        {
            return expression.CompileToObservable<Tor, TDes>();
        }

 
        internal static Expression<Func<TSource, TDest>> Merge<TSource, TInt1, TInt2, TDest>
                  (this Expression<Func<TSource, TInt1>> @this, Expression<Func<TSource, TInt2>> other,
                 Expression<Func<TInt1, TInt2, TDest>> Mixer)
        {
            var newother = Expression.Invoke(other, @this.Parameters.Cast<Expression>());
            var newthis = Expression.Invoke(@this, @this.Parameters.Cast<Expression>());

            var res = Expression.Invoke(Mixer, newthis, newother);

            return Expression.Lambda<Func<TSource, TDest>>(res, @this.Parameters);
        }

        internal static Expression<Func<Tuple<TInt1, TInt2>,TDest>> ConvertToTupleOperator<TInt1, TInt2, TDest>
              (this Expression<Func<TInt1, TInt2, TDest>> @this)
        {
            ParameterExpression pe = Expression.Parameter(typeof(Tuple<TInt1, TInt2>));

            Expression<Func<Tuple<TInt1, TInt2>, TInt1>> Getter1 = t => t.Item1;
            Expression<Func<Tuple<TInt1, TInt2>, TInt2>> Getter2 = t => t.Item2;

            Expression Getter1InAction = Expression.Invoke(Getter1, pe);
            Expression Getter2InAction = Expression.Invoke(Getter2, pe);

            Expression Intermediaire = Expression.Invoke(@this, Getter1InAction, Getter2InAction);

            return Expression.Lambda<Func<Tuple<TInt1, TInt2>, TDest>>(Intermediaire, pe);
        }


        internal static Expression<Func<TSource, Tuple<TDest1, TDest2>>> Merge<TSource, TDest1, TDest2>
               (this Expression<Func<TSource, TDest1>> @this, Expression<Func<TSource, TDest2>> other)
        {
            return @this.Merge(other, (un, deux) => new Tuple<TDest1, TDest2>(un, deux));
        }

    }
}
