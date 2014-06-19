using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable
{
    public class WrappedObservableCollection<T> : ObservableCollection<T>, IObservableCollection<T>, IObjectAttribute
    {
        public event EventHandler<ObjectModifiedArgs> ObjectChanged;

        public WrappedObservableCollection(IEnumerable<T> icoll):base (icoll)
        {
            _OldCount = Count;
        }

        public WrappedObservableCollection(): base()
        {
            _OldCount = 0;
        }

        private int _OldCount = 0;

        protected void HandleOnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            EventHandler<ObjectModifiedArgs> eventoc = ObjectChanged;

            if (eventoc == null)
            {
                _OldCount = Count;
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    return;

                case NotifyCollectionChangedAction.Reset:
                    if (_OldCount != Count)
                    {
                        eventoc(this, new ObjectAttributeChangedArgs<int>(this, "Count", _OldCount, Count));
                        _OldCount = Count;
                    }
                    return;
            }

            int countremoved = (e.NewItems == null) ? 0 : e.NewItems.Count;
            int countadded = (e.OldItems == null) ? 0 : e.OldItems.Count;

            int delta = countadded - countremoved;
            if (delta != 0)
            {
                eventoc(this, new ObjectAttributeChangedArgs<int>(this, "Count", Count - delta, Count));
            }

            _OldCount = Count;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            HandleOnCollectionChanged(e);
        }

    }
}
