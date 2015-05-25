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
        private int _Count = 0;


        internal ROCollection(int Reservable)
            : base(Reservable)
        {
            _CollectionChanged = new CollectionUISafeEvent(this,()=> this.PropertyHasChangedUIOnly("Count"));
        }

        internal ROCollection()
            : base()
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => this.PropertyHasChangedUIOnly("Count"));
        }

        internal ROCollection(IEnumerable<T> copyfrom)
            : base(copyfrom)
        {
            _CollectionChanged = new CollectionUISafeEvent(this, () => this.PropertyHasChangedUIOnly("Count"));
            _Count = this.Count;
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
        { 
            add { _CollectionChanged.Event += value; } 
            remove { _CollectionChanged.Event -= value; } 
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

            PropertyHasChanged("Count", _Count,Count);
            _Count = Count;
        }

        protected override void OnCollectionChanged(IEnumerable<NotifyCollectionChangedEventArgs> e)
        {
            _CollectionChanged.CollectionChanged(e);

            PropertyHasChanged("Count", _Count, Count);
            _Count = Count;
        }

        protected IDisposable GetEventFactorizable()
        {
            return _CollectionChanged.GetEventFactorizable();
        }

        #endregion

    }
}
