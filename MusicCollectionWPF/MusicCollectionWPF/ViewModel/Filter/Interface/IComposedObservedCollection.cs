using System;
using System.Collections;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using System.Windows.Input;
using MusicCollectionWPF.ViewModelHelper;


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

    public class ComposedObservedCollection<T> : ViewModelBase
    {
        public T Key { get; private set; }
        public ComposedObservedCollection(T iRoot, IList iCollection)
        {
            Key = iRoot;
            _Collection = iCollection;
            Next = RelayCommand.Instanciate(() => Index += 1);
            Precedent = RelayCommand.Instanciate(() => Index -= 1);
            Update();
        }

 
        private IList _Collection;
        public IList Collection
        {
            get { return _Collection; }
        }

        private int _Index = 0;
        public int Index
        {
            set 
            { 
                var Count = Collection.Count; 
                if (Count == 0) 
                    return; 
                value = value % Collection.Count;
                if (value < 0) value += Collection.Count; 
                _Index = value; 
                Current = Collection[_Index]; 
            }
            get { return _Index; }
        }

        private void Update()
        {
            if (Collection.Count > 0) Index = 0;
        }

        private object _Current;
        public object Current 
        { 
            get { return _Current; }
            private set { IsTransition = true; Set(ref _Current, value); IsTransition = false; }
        }

        private bool _IsTransition=false;
        public bool IsTransition
        {
            get { return _IsTransition; }
            private set { Set(ref _IsTransition, value); }
        }

        public ICommand Next { get; private set; }

        public ICommand Precedent { get; private set; }

    }


    public class ComposedObservedCollectionAdapter<T> : IComposedObservedCollection
    {
        private IExtendedObservableCollection<T> _Coll;
        public ComposedObservedCollectionAdapter(IExtendedObservableCollection<T> coll)
        {
            _Coll = coll;
            //Collection = _Coll;
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
