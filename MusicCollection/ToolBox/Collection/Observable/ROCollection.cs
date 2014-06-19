using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;
using System.Diagnostics;

namespace MusicCollection.ToolBox.Collection.Observable
{
    [DebuggerDisplay("Count = {Count}")]
    internal class ROCollection<T> : CollectionBase<T>, IExtendedObservableCollection<T>
    {
        private CollectionUISafeEvent _CollectionChanged;


        internal ROCollection(int Reservable)
            : base(Reservable)
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => PropertyHasChanged(string.Empty));
        }

        internal ROCollection()
            : base()
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => PropertyHasChanged(string.Empty));
        }

        internal ROCollection(IEnumerable<T> copyfrom)
            : base(copyfrom)
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => PropertyHasChanged(string.Empty));
        }

        public virtual void Dispose()
        {
        }

        public virtual IDisposable GetFactorizableEvent()
        {
            return null;
        }

        #region Event

        public event NotifyCollectionChangedEventHandler CollectionChanged
        { add { _CollectionChanged.Event+=value; } remove { _CollectionChanged.Event-=value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyHasChanged(string pn)
        {
            PropertyChangedEventHandler c = PropertyChanged;
            if (c != null)
                c(this, new PropertyChangedEventArgs(pn));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _CollectionChanged.CollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                    return;
                case NotifyCollectionChangedAction.Replace:
                    return;
            }

            PropertyHasChanged("Count");
        }

        protected IDisposable GetEventFactorizable()
        {
            return _CollectionChanged.GetEventFactorizable();
        }

        #endregion

    }
}
