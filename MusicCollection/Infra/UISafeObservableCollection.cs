using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.ToolBox.Event;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Infra
{
    public class UISafeObservableCollection<T> : WrappedObservableCollection<T>, ICompleteObservableCollection<T>
    {
        private CollectionUISafeEvent _CollectionChanged;
        private PropertyChangedEventHandlerUISafeEvent _PropertyChangedEventHandler;


        public UISafeObservableCollection(IEnumerable<T> ienum)
            : base(ienum)
        {
            InitCollectionChanged();
        }

        public UISafeObservableCollection()
            : base()
        {
            InitCollectionChanged();
        }

        private void InitCollectionChanged()
        {
            _PropertyChangedEventHandler = new PropertyChangedEventHandlerUISafeEvent(this);
            _CollectionChanged = new CollectionUISafeEvent(this, () => _PropertyChangedEventHandler.Fire("Count", true));
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged
        { add { _CollectionChanged.Event += value; } remove { _CollectionChanged.Event -= value; } }

        protected override event PropertyChangedEventHandler PropertyChanged
        { add { _PropertyChangedEventHandler.Event += value; } remove { _PropertyChangedEventHandler.Event -= value; } }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                _CollectionChanged.CollectionChanged(e);
            }

            HandleOnCollectionChanged(e);
        }

    }
}
