using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class CollectionWithDetailVM<T>: ViewModelBase where T:class
    {
        private IList<T> _Collection;
       
        public CollectionWithDetailVM(IList<T> iCollection)
        {
            _Collection = iCollection;

            var inc = Observable;
            if (inc!=null)
                inc.CollectionChanged += OnCollectionChanged;

            Init();

            _Previous = new Lazy<ICommand>( ()=> Register( RelayCommand.Instanciate(DoPreviewImage, () => CanPrevious)));
            _Next = new Lazy<ICommand>(() => Register(RelayCommand.Instanciate(DoNextImage, () => CanNext)));
        }

        public override void Dispose()
        {
            base.Dispose();
            var inc = Observable;
            if (inc != null)
                inc.CollectionChanged -= OnCollectionChanged;
        }

        public IList<T> Collection { get { return _Collection; } }


        public bool CanNavigate
        {
            get { return Get<CollectionWithDetailVM<T>, bool>(() => (t) => t._Collection.Count > 1); }
        }

         public bool CanPrevious
        {
            get { return Get<CollectionWithDetailVM<T>, bool>(() => (t) => ((t._Collection.Count > 1) && (t.Circular || t.Count > 0))); }
        }

        public bool CanNext
        {
            get { return Get<CollectionWithDetailVM<T>, bool>(() => (t) => ((t._Collection.Count > 1) && (t.Circular ||(t.Count != t._Collection.Count - 1))));  }
        }  

        private void Init()
        {
            SetCurrent( (_Collection.Count > 0) ? _Collection[0] : null);
            Count = (_Current == null) ? -1 : 0;
        }

        private int _Count = 0;
        private int Count
        {
            get { return _Count; }
            set { Set(ref _Count, value); }
        }

        private T _Current;
        public T Current
        {
            get { return _Current; }
            set { IsInTransition = true; Set(ref _Current, value); Count = _Collection.IndexOf(value); IsInTransition = false; }
        }

        private void SetCurrent(T value)
        {
            IsInTransition = true; 
            Set(ref _Current, value, "Current"); 
            IsInTransition = false;
        }

        private bool _IsInTransition = false;
        public bool IsInTransition
        {
            get { return _IsInTransition; }
            set { Set(ref _IsInTransition, value); }
        }

        private bool _Circular = false;
        public bool Circular
        {
            get { return _Circular; }
            set { Set(ref _Circular, value); }
        } 

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = (_Current != null) ? _Collection.IndexOf(_Current) : -1;
            if (index != -1)
            {
                Count = index;
                return;
            }

            Init();
        }

        private INotifyCollectionChanged Observable
        {
            get { return _Collection as INotifyCollectionChanged; }
        }

        private void SetCount(int iCount)
        {
            Count = iCount;
            SetCurrent( _Collection[Count]);
        }

        private void DoNextImage()
        {
            if (_Count < _Collection.Count - 1)
            {
                SetCount(Count+1);
            }
            else if ((Circular) && (_Collection.Count>0))
            {
                SetCount(0);
            }
        }

        private void DoPreviewImage()
        {
            if (_Count > 0)
            {
                SetCount(Count-1);
            }
            else if ((Circular) && (_Collection.Count > 0))
            {
                SetCount(_Collection.Count - 1);
            }
        }

        private Lazy<ICommand> _Previous;
        private Lazy<ICommand> _Next;

        public ICommand Previous { get { return _Previous.Value; } }

        public ICommand Next { get { return _Next.Value; } }
    }
}
