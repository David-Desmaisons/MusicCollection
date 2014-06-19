using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Event
{
    internal class UISafeEvent<T> where T:EventArgs
    {
        private event EventHandler<T> _Event;
        public event EventHandler<T> Event { add { _Event += value; } remove { _Event -= value; } }

        //internal void AddEvent(Delegate Del)
        //{
        //    Event += Del.Convert<EventHandler<T>>();
        //}

        //internal void Remove(Delegate Del)
        //{ 
        //    Event -= Del.Convert<EventHandler<T>>();
        //}

        private object _Owner;

        internal UISafeEvent(object Owner)
        {
            _Owner = Owner;
        }

        internal bool IsLoaded
        {
            get { return (_Event!=null); }
        }

  
        internal void Fire(T argument, bool Sync)
        {
            EventHandler<T> Event = _Event;
            if (Event == null)
                return;

            foreach (EventHandler<T> del in Event.GetInvocationList())
            {
                //Dispatcher dip = EventHelper.GetDispatcher(del);
                Dispatcher dip = del.GetDispatcher();

                // If the subscriber is a DispatcherObject and different thread
                if (dip != null && ((!Sync) || dip.CheckAccess() == false))
                {
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
                        del.BeginInvoke(_Owner, argument,null,null);
                }
            }


        }


    }
}
