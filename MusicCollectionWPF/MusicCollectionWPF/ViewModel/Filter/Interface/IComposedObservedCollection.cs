using System;
using System.Collections;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using System.Windows.Input;
using MusicCollectionWPF.ViewModelHelper;
using System.Collections.Generic;


namespace MusicCollectionWPF.ViewModel
{
    public interface IComposedObservedCollection : IDisposable
    {
        IList Collection { get; }
    }


    public interface IComposedCompleteObservedCollection
    {
        object Current { get; }

        ICommand Next { get; }

        ICommand Precedent { get; }

        IList Collection { get; }

        bool IsTransition { get; }
    }

    public class ComposedObservedCollection<TKey, TColl> : ViewModelBase
        where TKey : class
        where TColl : class
    {
        public TKey Key { get; private set; }
        private CollectionWithDetailVM<TColl> _CollectionVM;
        public ComposedObservedCollection(TKey iRoot, IList<TColl> iCollection)
        {
            Key = iRoot;
            _CollectionVM = new CollectionWithDetailVM<TColl>(iCollection) { Circular = true };
        }

        public override void Dispose()
        {
            base.Dispose();
            _CollectionVM.Dispose();
        }

        public IList<TColl> Collection
        {
            get { return _CollectionVM.Collection; }
        }

        public bool CanNavigate
        {
            get { return Get<ComposedObservedCollection<TKey, TColl>, bool>(() => (t) => t._CollectionVM.CanNavigate); }
        }

        public TColl Current
        {
            get { return Get<ComposedObservedCollection<TKey, TColl>, TColl>(() => (t) => t._CollectionVM.Current); }
        }

        public bool IsTransition
        {
            get { return Get<ComposedObservedCollection<TKey, TColl>, bool>(() => (t) => t._CollectionVM.IsInTransition); }
        }

        public ICommand Next { get { return _CollectionVM.Next; } }

        public ICommand Precedent { get { return _CollectionVM.Previous; } }
    }

    public class ComposedObservedCollectionAdapter<T> : IComposedObservedCollection
    {
        private IExtendedObservableCollection<T> _Coll;
        public ComposedObservedCollectionAdapter(IExtendedObservableCollection<T> coll)
        {
            _Coll = coll;
        }

        public void Dispose()
        {
            _Coll.Dispose();
        }

        public IList Collection
        {
            get { return _Coll; }
        }
    }
}
