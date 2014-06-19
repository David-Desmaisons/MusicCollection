using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Diagnostics;

using NHibernate;

using MusicCollection.Implementation;

namespace MusicCollection.Nhibernate.Session
{
    internal class ObjectInSessionArgs : EventArgs
    {
        internal IList<ISessionPersistentObject> NewObjectInSession
        {
            get;
            private set;
        }

        internal ObjectInSessionArgs(IList<ISessionPersistentObject> iNewObjectInSession)
        {
            NewObjectInSession = iNewObjectInSession;
        }
    }

    internal interface IDBSession : IDisposable
    {
        ISession NHSession
        {
            get;
        }



        event EventHandler<ObjectInSessionArgs> OnObjectLoads;
    }
}
