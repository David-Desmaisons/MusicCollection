using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Threading;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Event
{
    internal class CollectionUISafeEvent 
    {
        private Action _OnEventEimt;

        internal CollectionUISafeEvent(object sender, Action ev=null)
        {
            _OnEventEimt = ev;
            _Owner = sender;
        }

        private void EmitEvent(NotifyCollectionChangedEventArgs e, bool Sync)
        {
            Fire(e, Sync);

            if (_OnEventEimt!=null)
                _OnEventEimt();
        }

        internal void CollectionChanged(NotifyCollectionChangedEventArgs e, bool Sync=true)
        {
            if (_FE != null)
            {
                _FE.RegisterNewEvent(e);
                return;
            }

            EmitEvent(e, Sync);
        }

        #region Differed

        private class EventsFactorizer : IDisposable
        {
            private CollectionUISafeEvent _Father;
            private NotifyCollectionChangedEventArgs _CompileEvent = null;

            internal EventsFactorizer(CollectionUISafeEvent F)
            {
                _Father = F;
            }

            internal void RegisterNewEvent(NotifyCollectionChangedEventArgs eve)
            {
                if (eve == null)
                    return;

                if (_CompileEvent == null)
                {
                    _CompileEvent = eve;
                    return;
                }

                if ((_CompileEvent.Action == NotifyCollectionChangedAction.Remove) && (eve.Action == NotifyCollectionChangedAction.Add))
                {
                    if (object.Equals(_CompileEvent.OldItems[0], eve.NewItems[0]))
                    {
                        if (eve.NewStartingIndex== _CompileEvent.OldStartingIndex)
                        {
                            _CompileEvent = null;
                            return;
                        }

                        _CompileEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, _CompileEvent.OldItems[0], eve.NewStartingIndex, _CompileEvent.OldStartingIndex);
                        return;
                    }
                    else if (_CompileEvent.OldStartingIndex == eve.NewStartingIndex)
                    {
                        _CompileEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, eve.NewItems[0], _CompileEvent.OldItems[0], eve.NewStartingIndex);
                        return;
                    }
                }

                _CompileEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            }

            public void Dispose()
            {
                _Father.OnEndFactorizedEvents(_CompileEvent);
            }
        }

        #endregion

        #region Performance issue rewriting

        private event NotifyCollectionChangedEventHandler _Event;
        public event NotifyCollectionChangedEventHandler Event { add { _Event += value; } remove { _Event -= value; } }

        private object _Owner;


        internal void Fire(NotifyCollectionChangedEventArgs argument, bool Sync)
        {
            NotifyCollectionChangedEventHandler Event = _Event;
            if (Event == null)
                return;

            foreach (NotifyCollectionChangedEventHandler del in Event.GetInvocationList())
            {
                Dispatcher dip = del.GetDispatcher();

                // If the subscriber is a DispatcherObject and different thread
                if (dip != null && ((!Sync) || dip.CheckAccess() == false))
                {
                    //// If the subscriber is a DispatcherObject and different thread
                    // Invoke handler in the target dispatcher's thread
                    if (Sync)
                        dip.Invoke(DispatcherPriority.DataBind, del, _Owner, argument);
                    else
                        dip.BeginInvoke(DispatcherPriority.DataBind, del, _Owner, argument);
                }
                else
                {
                    if (Sync)
                        del(_Owner, argument);
                    else
                        del.BeginInvoke(_Owner, argument, null, null);
                }
            }
        }

        internal void Fire(IEnumerable<NotifyCollectionChangedEventArgs> arguments, bool Sync)
        {
            NotifyCollectionChangedEventHandler Event = _Event;
            if (Event == null)
                return;

            foreach (NotifyCollectionChangedEventHandler del in Event.GetInvocationList())
            {
                Dispatcher dip = del.GetDispatcher();

                Action CallNotifyCollectionChangedEventHandler = () =>
                    {
                        arguments.Apply(ar => del(_Owner, ar));
                    };

                // If the subscriber is a DispatcherObject and different thread
                if (dip != null && ((!Sync) || dip.CheckAccess() == false))
                {
                    //// If the subscriber is a DispatcherObject and different thread
                    // Invoke handler in the target dispatcher's thread
                    if (Sync)
                        dip.Invoke(DispatcherPriority.DataBind, CallNotifyCollectionChangedEventHandler);
                    else
                        dip.BeginInvoke(DispatcherPriority.DataBind, CallNotifyCollectionChangedEventHandler);
                }
                else
                {
                    if (Sync)
                        CallNotifyCollectionChangedEventHandler();
                    else
                        CallNotifyCollectionChangedEventHandler.BeginInvoke(null, null);
                }
            }
        }

        #endregion


        private EventsFactorizer _FE;

        internal IDisposable GetEventFactorizable()
        {
            if (_FE == null)
                _FE = new EventsFactorizer(this);

            return _FE;
        }

        private void OnEndFactorizedEvents(NotifyCollectionChangedEventArgs e)
        {
            _FE = null;
            if (e == null)
                return;

            EmitEvent(e,true);
        }

    }
}
