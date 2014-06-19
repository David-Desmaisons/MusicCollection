using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace MusicCollection.Infra
{
    public interface ILiveResult<T> : INotifyPropertyChanged, IObjectAttribute, IDisposable
    {
        T Value
        {
            get;
        }
    }

    public interface IObservableEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
    }

    public interface IFullObservableCollection<T> : ICompleteObservableCollection<T>, IDisposable
    {
    }

    public interface ICompleteObservableCollection<T> : IObservableCollection<T>, IList
    {
        new int Count { get; }

        new T this[int index] { get; }
    }

    public interface IObservableLookup<TKey,T> : ILookup<TKey, T>, IExtendedObservableCollection<IObservableGrouping<TKey, T>>
    {
        IObservableGrouping<TKey, T> GetObservableFromKey(TKey key);
    }

    public interface ICollectionAcessor<T>
    {
        IExtendedObservableCollection<T> Collection
        {
            get;
        }
    }

    public interface IObservableGrouping<TKey, T> : IGrouping<TKey, T>, ICollectionAcessor<T>
    {
        
    }

    public interface IExtendedObservableCollection<T> : IList, IObservableCollection<T>, IDisposable
    {
        IDisposable GetFactorizableEvent();

        new T this [int index]
        {
            get;
            set;
        }

        new int Count
        {
            get;
        }
    }

    public interface IExtendedOrderedObservableCollection<T> : IExtendedObservableCollection<T>
    {
        IExtendedOrderedObservableCollection<T> CreateOrderedEnumerable<TKey>(Expression<Func<T, TKey>> keySelector) where TKey : IComparable<TKey>;
    }

    public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {       
    }

    public interface IRefreshableObservableCollection<T> : IObservableCollection<T>
    {
        void Refresh();
    }

    public interface IFilterObservableCollection<T, Traw> : IRefreshableObservableCollection<T>, IList
    {
        Func<Traw, bool> Filter
        {
            get;
            set;
        }
    }

    public interface IFilterObservableCollection<T> : IRefreshableObservableCollection<T>
    {
        Func<T, bool> Filter
        {
            get;
            set;
        }
    }
}
