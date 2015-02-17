using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.Infra.Collection;
using System.Collections.Specialized;
using MusicCollectionWPF.ViewModel.Interface;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel.Element
{
    public class AfinityCollection<T> : NotifyCompleteListenerObject where T : class
    {
        private int _Count;
        private IList<T> _OriginalCollection;
        private Func<T, IUpdatableComparer<T>> _AfinityProvider;
        private IUpdatableComparer<T> _Comparer;
        private IExtendedObservableCollection<T> _ResultCollection;
        private ObservableCollection<T> _InternalList;


        public AfinityCollection(IList<T> iOriginalCollection,
                                    Func<T, IUpdatableComparer<T>> iAfinityProvider,
                                    int iCount)
        {
            _Count = iCount;
            _OriginalCollection = iOriginalCollection;      
            _AfinityProvider = iAfinityProvider;
            _InternalList = new ObservableCollection<T>();
            _ResultCollection = _InternalList.LiveReadOnly();
            _Comparer = null;
            _Reference = null;

            INotifyCollectionChanged incc = _OriginalCollection as INotifyCollectionChanged;
            if (incc!=null) incc.CollectionChanged += _OriginalCollection_CollectionChanged;
        }

        private void _OriginalCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_OriginalCollection.Count == 0)
            {
                Reference = null;
                return;
            }

            if ((e.Action == NotifyCollectionChangedAction.Remove) && (!Collection.Contains(e.OldItems[0])))
            {
                return;
            }

            if (_Reference == null)
            {
                Reference = _OriginalCollection[0];
                return;
            }

            if (!_OriginalCollection.Contains(_Reference))
            {
                T newReference = Collection.FirstOrDefault((el) => _OriginalCollection.Contains(el));
                Reference = newReference ?? _OriginalCollection[0];
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newcollection = new List<T>(Collection).AddCollection(e.NewItems.Cast<T>());
                SetCollection(newcollection.SortFirst(_Count, _Comparer,true));
                return;
            }

            SetCollection(_OriginalCollection.SortFirst(_Count, _Comparer,true));
        }

        public override void Dispose()
        {
            INotifyCollectionChanged incc = _OriginalCollection as INotifyCollectionChanged;
            if (incc != null) incc.CollectionChanged -= _OriginalCollection_CollectionChanged;
                
            _ResultCollection.Dispose();
            Comparer = null;

            base.Dispose();
        }

        public Task ComputeAsync(T newpivotrefence)
        {
            if (object.ReferenceEquals(_Reference, newpivotrefence))
            {
                return Task.FromResult<object>(null);
            }

            return Task.Run(() => Reference = newpivotrefence);
        }

        private IUpdatableComparer<T> Comparer
        {
            set
            {
                if (object.ReferenceEquals(_Comparer, value))
                    return;

                if (_Comparer != null)
                {
                    _Comparer.OnChanged -= _Comparer_OnChanged;
                    _Comparer.OnElementChanged -= _Comparer_OnElementChanged;
                    _Comparer.Dispose();
                }

                _Comparer = value;

                if (_Comparer != null)
                {
                    _Comparer.OnChanged += _Comparer_OnChanged;
                    _Comparer.OnElementChanged += _Comparer_OnElementChanged;
                }
            }
        }

        private void _Comparer_OnElementChanged(object sender, ElementChangedArgs<T> e)
        {
            T element = e.Element;

            if (!_InternalList.Contains(element))
            {
                return;
                //int Rank = _InternalList.BinarySearch(element, _Comparer);
                //if (Rank < 0) Rank = ~Rank;

                //if (Rank == _Count)
                //    return; 
                
                //_InternalList.Insert(Rank, element);
                //_InternalList.RemoveAt(_Count);
            }
            else
            {
                SetCollection(_OriginalCollection.SortFirst(_Count, _Comparer, true));
            }
        }

        private void _Comparer_OnChanged(object sender, EventArgs e)
        {
            SetCollection(_OriginalCollection.SortFirst(_Count, _Comparer,true));
        }

        public IExtendedObservableCollection<T> Collection
        {
            get { return _ResultCollection; }
        }

        private void SetCollection(IEnumerable<T> value)
        {
            if (_ResultCollection.Take(_Count).SequenceEqual(value.Take(_Count)))
                return;

            using (_ResultCollection.GetFactorizableEvent())
            {
                _InternalList.Clear();
                _InternalList.AddCollection(value);
            }
        }

        private T _Reference;
        public T Reference
        {
            get { return _Reference; }
            set
            {
                if (!this.Set(ref _Reference, value))
                    return;

                if (Reference != null)
                {
                    Comparer = _AfinityProvider(Reference);
                    SetCollection(_OriginalCollection.SortFirst(_Count, _Comparer,true));
                }
                else
                {
                    Comparer = null;
                    _InternalList.Clear();
                }
            }
        }

       

    }
}
