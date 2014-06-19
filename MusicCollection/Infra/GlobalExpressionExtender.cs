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

            //LambdaInspector<Tor, TDes> li = new LambdaInspector<Tor, TDes>(expression);
            //return li.ObjectAttributes;

            return CompleteDynamicFunction<Tor, TDes>.GetCompleteDynamicFunction(expression);
        }

        public static ISimpleFunction<Tor, TDes> CompileToObservableFunction<Tor, TDes>(this Expression<Func<Tor, TDes>> expression) where Tor : class //,IObjectAttribute
        {
            return expression.CompileToObservable<Tor, TDes>();
        }

        //public static IFunction<Tor, TDes> FromExpression<Tor, TDes>( Expression<Func<Tor, TDes>> expression) where Tor : class
        //{
        //    if (expression == null)
        //        throw new ArgumentNullException();

        //    LambdaInspector<Tor, TDes> li = new LambdaInspector<Tor, TDes>(expression);
        //    return li.ObjectAttributes;
        //}

     

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

        //internal static Expression<Func<TSource, IList<TDest>>> TreatLiveCollectionOutput<TSource, TCollection, TDest>
        //  (this Expression<Func<TSource, IList<TCollection>>> @this, Expression<Func<TSource, TCollection, TDest>> Mixer) where TCollection:class
        //{
        //    var CollectionParameter = Expression.Parameter(typeof(TCollection));
   
        //    Func<TSource, Expression<Func<TCollection, TDest>>> querymaker = (s) =>
        //         Expression.Lambda<Func<TCollection, TDest>>(
        //        Expression.Invoke(Mixer, Expression.Constant(s, typeof(TSource)), CollectionParameter), CollectionParameter

        //        );

        //    Expression<Func<IList<TCollection>, TSource, IList<TDest>>> TransformCollection = (c, s) => c.LiveSelect(querymaker(s));

        //    Expression<Func<TSource, IList<TDest>>> final =
        //       Expression.Lambda<Func<TSource, IList<TDest>>>

        //       (Expression.Invoke(TransformCollection,
        //                Expression.Invoke(@this, @this.Parameters[0]), @this.Parameters[0]), @this.Parameters[0]);


        //    return final;

        //}



    }
}
