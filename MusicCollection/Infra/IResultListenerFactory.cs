using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MusicCollection.Infra
{
    internal interface IResultListenerFactory<Tor, TDes>: IRawResultListenerFactory, IDisposable where Tor : class
    {
        IResultListener<TDes> CreateListener(Tor origine);      
    }

    internal interface IRawResultListenerFactory
    {
        IRawResultListener CreateRawListener(object origine);
    }

    internal interface IResultListener<TDes> : IRawResultListener, IDisposable
    {
        TDes Value { get; }
    }

    internal interface IRawResultListener : IDisposable
    {
        void Register();

        void UnRegister();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EventHandler<ObjectModifiedArgs> ListenedObject
        {
            get;
        }
    }
}
