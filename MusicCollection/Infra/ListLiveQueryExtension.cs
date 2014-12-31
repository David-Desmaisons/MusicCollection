using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

using MusicCollection.ToolBox.StringExpression;
using MusicCollection.ToolBox.Collection.Observable.LiveQuery;

namespace MusicCollection.Infra
{
    public static class ListLiveQueryExtension
    {
        public static IExtendedObservableCollection<T> LiveDistinct<T>(this IList<T> origin)
        {
            return new LiveDistinct<T>(origin);
        }

        public static IExtendedObservableCollection<TDest> LiveSelect<TSource, TDest>(this IList<TSource> origin, Expression<Func<TSource, TDest>> Transformer) where TSource : class
        {
            return new LiveSelectCollection<TSource, TDest>(origin, Transformer);
        }


        public static IExtendedObservableCollection<TDest> LiveSelect<TSource, TDest>(this IList<TSource> origin, string stringExpression) where TSource : class
        {
            Expression<Func<TSource, TDest>> exp = new ExpressionMemberBuilder<TSource, TDest>(stringExpression);
            if (exp == null)
            {
                return null;
            }

            return new LiveSelectCollection<TSource, TDest>(origin, exp);
        }
              
        
        public static IExtendedObservableCollection<TDest> SelectLive<TSource, TDest>(this IList<TSource> origin, Func<TSource, TDest> Transformer) where TSource : class
        {
            return LiveSelectCollection<TSource, TDest>.FromFunction(origin, Transformer);
        }


        public static IExtendedObservableCollection<T> LiveWhere<T>(this IList<T> origin, Expression<Func<T, bool>> Filter) where T : class
        {
            return new LiveWhere<T>(origin, Filter);
        }


        public static IExtendedObservableCollection<T> LiveWhere<T>(this IList<T> origin, string stringExpression) where T : class
        {         
            Expression<Func<T, bool>> exp = new ExpressionFilterBuilder<T>(stringExpression);
            if (exp == null)
            {
                return null;
            }

            return new LiveWhere<T>(origin, exp);
        }

        public static IExtendedObservableCollection<T> LiveWhere<T>(this IList<T> origin, IFunction<T, bool> Filter) where T : class
        {
            return new LiveWhere<T>(origin, Filter);
        }

        public static IExtendedOrderedObservableCollection<T> LiveOrderBy<T, TKey>(this IList<T> origin, Expression<Func<T, TKey>> KeySelector)
            where T : class
            where TKey : IComparable<TKey>
        {
            return new LiveOrderBy<T, TKey>(origin, KeySelector);
        }

        public static IExtendedOrderedObservableCollection<T> LiveOrderBy<T, TKey>(this IList<T> origin, string isKeySelector)
            where T : class
            where TKey : IComparable<TKey>
        {
            Expression<Func<T, TKey>> KeySelector = new ExpressionMemberBuilder<T, TKey>(isKeySelector);
            return new LiveOrderBy<T, TKey>(origin, KeySelector);
        }

        public static IExtendedOrderedObservableCollection<T> OrderByLive<T, TKey>(this IList<T> origin, Func<T, TKey> KeySelector)
            where T : class
            where TKey : IComparable<TKey>
        {
            return new LiveOrderBy<T, TKey>(origin, KeySelector.CompileToConst());
        }

        public static IExtendedOrderedObservableCollection<T> LiveThenBy<T, TKey2>(this IExtendedOrderedObservableCollection<T> origin, Expression<Func<T, TKey2>> KeySelector)
            where T : class
            where TKey2 : IComparable<TKey2>
        {
            return origin.CreateOrderedEnumerable<TKey2>(KeySelector);
        }

        public static IExtendedObservableCollection<TDest> LiveSelectMany<TSource, TDest>(this IList<TSource> origin, Expression<Func<TSource, IList<TDest>>> Transform)
            where TSource : class
        {
            return new LiveSelectMany<TSource, TDest>(origin, Transform);
        }

        public static IExtendedObservableCollection<TDest> LiveSelectMany<TSource, TCollection, TDest>(this IList<TSource> origin,
            Expression<Func<TSource, IList<TCollection>>> Transform, Expression<Func<TSource, TCollection, TDest>> Conv)

          where TSource : class
          where TCollection : class
        {
            return new LiveSelectManyTuple<TSource, TCollection>(origin, Transform).LiveSelect(Conv.ConvertToTupleOperator());
        }

        public static IExtendedObservableCollection<Tuple<TSource, TDest>> LiveSelectManyTuple<TSource, TDest>(this IList<TSource> origin,
            Expression<Func<TSource, IList<TDest>>> Transform)

            where TSource : class
        {
            return new LiveSelectManyTuple<TSource, TDest>(origin, Transform);
        }

        public static IExtendedObservableCollection<TDest> LiveReadOnly<TSource, TDest>(this IList<TSource> origin)
            where TSource : class,TDest
        {
            return new ReadOnlyWraper<TSource, TDest>(origin);
        }

        public static IExtendedObservableCollection<TSource> LiveReadOnly<TSource>(this IList<TSource> origin) 
            where TSource : class
        {
            return new ReadOnlySimpleWraper<TSource>(origin);
        }

        public static ILiveResult<int> LiveCount<TSource>(this IList<TSource> origin)
           where TSource : class
        {
            return DynamicCountResult<TSource>.GetDynamicCountResult(origin);
        }

        public static ILiveResult<int> LiveSum<TSource>(this IList<TSource> origin, Expression<Func<TSource, int>> sum)
         where TSource : class
        {
            return DynamicSumResult<TSource>.GetDynamicAverage(origin,sum);
        }

        public static ILiveResult<Nullable<double>> LiveAverage<TSource>(this IList<TSource> origin, Expression<Func<TSource, int>> sum)
          where TSource : class
        {
            return DynamicAverage<TSource>.GetDynamicAverage(origin, sum);
        }

        public static ILiveResult<bool> LiveAny<TSource>(this IList<TSource> origin, Expression<Func<TSource, bool>> Filter)
        where TSource : class
        {
            return DynamicAny<TSource>.GetDynamicAny(origin, Filter);
        }

        public static IObservableLookup<TKey, T> LiveToLookUp<T, TKey>(this IList<T> origin, Expression<Func<T, TKey>> KeySelector)
            where T : class
        {
            return new LiveToLookUpSimple<TKey, T>(origin, KeySelector);
        }

        public static IObservableLookup<TKey, TElement> LiveToLookUp<TKey, T, TElement>(this IList<T> origin, Expression<Func<T, TKey>> KeySelector, Expression<Func<T, TElement>> ElementSelector)
            where T : class
        {
            return LiveToLookUpDouble<TKey, T, TElement>.BuildFromKeyElementSelectors(origin, KeySelector, ElementSelector);
        }

    }
}
