using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;


namespace MusicCollection.ToolBox.Event
{
    class PropertyChangedEventHandlerUISafeEvent
    {

        internal PropertyChangedEventHandlerUISafeEvent(object sender)
        {
            _Owner = sender;
        }

        #region Performance issue rewriting

        private event PropertyChangedEventHandler _Event;
        public event PropertyChangedEventHandler Event { add { _Event += value; } remove { _Event -= value; } }

        private object _Owner;

        internal bool IsObserved
        {
            get
            {
                return (_Event != null);
            }
        }

        internal void Fire(string iPN, bool Sync)
        {
            PropertyChangedEventHandler Event = _Event;
            if (Event == null)
                return;
            
            PropertyChangedEventArgs argument = new PropertyChangedEventArgs(iPN);

            foreach (PropertyChangedEventHandler del in Event.GetInvocationList())
            {
                //Dispatcher dip = EventHelper.GetDispatcher(del);
                Dispatcher dip = del.GetDispatcher();

                //DispatcherObject dispatcherObject = del.Target as DispatcherObject;

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
                        del.BeginInvoke(_Owner, argument, null, null);
                }
            }
        }

        #endregion

    }
}
