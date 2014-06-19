using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Infra
{
    public static class EnumerableObservableExtension
    {
        //static public IFilterObservableCollection<T, T> Filter<T>(this IEnumerable<T> collection, Func<T, bool> iFilter) where T:class
        //{
        //    return new FilteredTransformReadOnlyObservableCollection<T, T, T>(collection, iFilter, (t)=>t);
        //}

        //static public IFilterObservableCollection<TTrans,T> Transform<T, TTrans>(this IList<T> collection, Expression<Func<T, TTrans>> iTrans) where T : class
        //{
        //    return new FilteredTransformReadOnlyObservableCollection<T, TTrans, TTrans>(collection, (o)=>true, iTrans);
        //}

        //static public IFullObservableCollection<TTrans> Transform<T, TTrans>(this IList<T> collection, Func<T, TTrans> iTrans)
        //{
        //    return new TransformObservableCollection<TTrans,T>(collection, iTrans);
        //}

        //static public IFilterObservableCollection<TTrans, T> FilterAndTransform<T, TTrans>(this IEnumerable<T> collection, Func<T, bool> iFilter, Expression<Func<T, TTrans>> iTrans) where T : class
        //{
        //    return new FilteredTransformReadOnlyObservableCollection<T, TTrans, TTrans>(collection, iFilter, iTrans, iTrans.Compile()); 
        //}

        //static public IFilterObservableCollection<T, T> FilterAndOrder<T, TKey>(this IEnumerable<T> collection, Func<T, bool> iFilter, Func<T, TKey> Keyselector) where T : class
        //{
        //    return new FilteredTransformReadOnlyObservableCollection<T, T, TKey>(collection, iFilter, t => t, Keyselector); 
        //}

        //static public IFilterObservableCollection<TTrans, T> FilterTransformAndOrder<T, TTrans, TKey>(this IEnumerable<T> collection, Func<T, bool> iFilter, Expression<Func<T, TTrans>> iTrans, Func<T, TKey> Keyselector) where T : class
        //{
        //    return new FilteredTransformReadOnlyObservableCollection<T, TTrans, TKey>(collection, iFilter,iTrans,Keyselector);
        //}
    }
}
