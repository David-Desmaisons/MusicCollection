using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox
{
    
    internal class Locker : IDisposable
    {
        private IObjectStateCycle _Locked;
        private IImportContext _Context;

        private Locker(IObjectStateCycle Lockable, IImportContext iContext)
        {
            _Locked = Lockable;
            Lockable.SetInternalState(ObjectState.UnderEdit,null);
            _Locked.ObjectStateChanges += Listener;
            _Context = iContext;
        }

        internal static IDisposable GetLocker(IObjectStateCycle Lockable, IImportContext Context )
        {
            if ((Lockable as IObjectState).State == ObjectState.Removed)
                return null;

            return new Locker(Lockable, Context);
        }

        private void Listener(object o, ObjectStateChangeArgs e)
        {
            throw new Exception("Ca pue");
        }


        public void Dispose()
        {
            _Locked.ObjectStateChanges -= Listener;
            _Locked.SetInternalState(ObjectState.Available, _Context);
        }
    }
    
    internal interface IObjectStateCycle : IObjectState
    {
        void HasBeenUpdated();

        ObjectState InternalState { get; }

        void SetInternalState(ObjectState value, IImportContext iic);
    }
}
