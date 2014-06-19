using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Event
{
    static public class EventHelper
    {
        static public Dispatcher GetDispatcher(this Delegate eventmethod)
        {
            DispatcherObject dispatcherObject = eventmethod.Target as DispatcherObject;

            if (dispatcherObject != null)
                //&& ((!Sync) || dispatcherObject.CheckAccess() == false))
                return dispatcherObject.Dispatcher;

            IDispatcher id = eventmethod.Target as IDispatcher;
            if (id == null)
                return null;

            return id.GetDispatcher();
        }

    }
}
