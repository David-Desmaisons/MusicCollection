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
        //private WeakEventHandler _CollectionListener;
        private ICollectionListener<TSource> _Listener;
        //private IList<TSource> _Source;
        private INotifyCollectionChanged _SourceListened;
        //private Dictionary<TSource, int> _SourceCount = new Dictionary<TSource, int>();
        //private HashSet<TSource> _SourceCount = new HashSet<TSource>();

        protected RegistorCollectionChanged(IEnumerable<TSource> Source, ICollectionListener<TSource> iListener)
        {
            _Listener = iListener;
            //_Source = Source;
            _SourceListened = Source as INotifyCollectionChanged;
            if (_SourceListened != null)
                _SourceListened.CollectionChanged += OnCollectionChanged;

                   
            //Source.Apply(t => TrySubscribeToItem(t));

            //_CollectionListener = WeakEventHandler.Register(Listened, (o, evt) => o.CollectionChanged += evt.Convert<NotifyCollectionChangedEventHandler>(), (o, ev) => o.CollectionChanged -= ev.Convert<NotifyCollectionChangedEventHandler>(), this, (l, o, ev) => l.OnCollectionChanged(o, (NotifyCollectionChangedEventArgs)ev));           
        }

        internal virtual void Register(IList<TSource> Source)
        {
            //Source.Apply(t => SubscribeToItem(t));
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

    
        //private bool TrySubscribeToItem(TSource item)
        //{
        //    //int value = -1;
        //    //if (_SourceCount.TryGetValue(item, out value))
        //    //{
        //    //    _SourceCount[item] = value + 1;
        //    //    return false;
        //    //}

        //    //_SourceCount.Add(item, 1);
        //    //SubscribeToItem(item);
        //    //return true;

        //    if (this._SourceCount.Add(item))
        //    {
        //        SubscribeToItem(item);
        //        return true;
        //    }

        //    return false;
        //}

        //private bool TryUnsubscribeFromItem(TSource item)
        //{
        //    //int value = -1;
        //    //if (!_SourceCount.TryGetValue(item, out value))
        //    //{
        //    //    Trace.WriteLine("Problem ");
        //    //    return false;
        //    //}

        //    //if (value > 1)
        //    //{
        //    //    _SourceCount[item] = value - 1;
        //    //    return false;
        //    //}

           
        //    //return true;

        //    return !(this._Source.Contains(item));
        //}

        //private void FinalizeRemove(TSource item)
        //{
        //    _SourceCount.Remove(item);

        //    UnsubscribeFromItem(item);
        //}

        //protected IEnumerable<TSource> IndividualSources
        //{
        //    get
        //    //{ return _SourceCount.Keys; }
        //    { return _SourceCount; }
        //}

        protected virtual void UnListenAll()
        {

            //IndividualSources.Apply(i => UnsubscribeFromItem(i));
            //_SourceCount.Clear(); //DEM Changes
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
            //INotifyCollectionChanged Listened = _Source as INotifyCollectionChanged;
            if (_SourceListened != null)
            {
                _SourceListened.CollectionChanged -= OnCollectionChanged;
                _SourceListened = null;
            }

             //_SourceCount.Clear();

            //_CollectionListener.Deregister();
          //  UnListenAll();
        }

    }
}
