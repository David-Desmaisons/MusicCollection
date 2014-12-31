using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

using MusicCollection.ToolBox.Event;
using MusicCollection.Infra;

//DEM

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal interface ICollectionListener<TSource>
    {
        void AddItem(TSource newItem, int index, Nullable<bool> First);

        bool RemoveItem(TSource oldItem, int index, Nullable<bool> Last);

        IDisposable GetFactorizable();

        void OnClear();
    }

    internal class RegistorCollectionChanged<TSource>
    {
        private ICollectionListener<TSource> _Listener;
        private INotifyCollectionChanged _SourceListened;

        protected RegistorCollectionChanged(IEnumerable<TSource> Source, ICollectionListener<TSource> iListener)
        {
            _Listener = iListener;
            _SourceListened = Source as INotifyCollectionChanged;
            if (_SourceListened != null)
                _SourceListened.CollectionChanged += OnCollectionChanged;
        }

        internal virtual void Register(IList<TSource> Source)
        {
        }

        static internal RegistorCollectionChanged<TSource> GetListener(IList<TSource> enumerable, ICollectionListener<TSource> iListener)
        {
            RegistorCollectionChanged<TSource> res = new RegistorCollectionChanged<TSource>(enumerable, iListener);
            res.Register(enumerable);
            return res;
        }

        protected virtual Nullable<bool> SubscribeToItem(TSource item)
        {
            return null;
        }

        protected virtual Nullable<bool> UnsubscribeFromItem(TSource item)
        {
            return null;
        }

        protected virtual Nullable<bool> NeedToRemove(TSource item)
        {
            return null;
        }
     
        protected virtual void UnListenAll()
        {
        }

        private IDisposable GetFactorizable()
        {
            return _Listener.GetFactorizable();
        }

        private void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            _Listener.AddItem(newItem, index, first);
        }

        private bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            return _Listener.RemoveItem(oldItem, index, last);
        }

        private void OnClear()
        {
            _Listener.OnClear();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (TSource toDelete in e.OldItems)
                    {
                        Nullable<bool> IsHere = this.NeedToRemove(toDelete);
                        RemoveItem(toDelete, e.OldStartingIndex, IsHere);
                        UnsubscribeFromItem(toDelete);
                    }
                    break;


                case NotifyCollectionChangedAction.Add:
                    int index = e.NewStartingIndex;
                    foreach (TSource toAdd in e.NewItems)
                    {
                        Nullable<bool> res = SubscribeToItem(toAdd);
                        AddItem(toAdd, index++, res);
                    }
                    break;


                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:

                    using (GetFactorizable())
                    {
                        TSource oldItem = (TSource)e.OldItems[0];
                        bool needUnregister = !object.ReferenceEquals(e.OldItems[0], e.NewItems[0]);

                        Nullable<bool> res = false;
                        if (needUnregister)
                        {
                            res = UnsubscribeFromItem(oldItem);
                        }

                        if (RemoveItem(oldItem, e.OldStartingIndex, res))
                        {
                            TSource newItem = (TSource)e.NewItems[0];
                            if (needUnregister)
                            {
                                res = SubscribeToItem(newItem);
                            }
                            AddItem(newItem, e.NewStartingIndex, res);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    UnListenAll();
                    Register(_SourceListened as IList < TSource >);
                    OnClear();                   
                    break;
            }
        }

        public virtual void Dispose()
        {
            if (_SourceListened != null)
            {
                _SourceListened.CollectionChanged -= OnCollectionChanged;
                _SourceListened = null;
            }
        }

    }
}
