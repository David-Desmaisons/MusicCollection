using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MusicCollection.Infra;
using MusicCollection.Implementation;

namespace MusicCollection.ToolBox
{
    public enum LockerStatus { OK, AlreadyLock_SameThread, NotAvailable, AlreadyLock_InstaneousRequest }

    //public struct LockerResult : IDisposable
    //{
    //    private IObjectStateLocker InternalLocker
    //    {
    //        get;
    //        set;
    //    }

    //    public LockerStatus Diagnostic
    //    {
    //        get;
    //        private set;
    //    }

    //    public void Remove()
    //    {
    //        if (InternalLocker == null)
    //            throw new Exception("Locker basic problem: Remove");

    //        InternalLocker.Remove();
    //    }

    //    internal LockerResult(LockerStatus ilockerLockerStatus)
    //        : this()
    //    {
    //        if (LockerStatus.OK == ilockerLockerStatus)
    //            throw new Exception("Locker basic problem");

    //        InternalLocker = null;
    //        Diagnostic = ilockerLockerStatus;
    //    }

    //    internal LockerResult(IObjectStateLocker ilocker)
    //        : this()
    //    {
    //        InternalLocker = ilocker;
    //        Diagnostic = LockerStatus.OK;
    //    }

    //    public void Dispose()
    //    {
    //        if (InternalLocker != null)
    //        {
    //            InternalLocker.Dispose();
    //            InternalLocker = null;
    //        }
    //    }
    //}



    public abstract class StateObjectAdapter : NotifyCompleteAdapterNoCache, IObjectStateCycle
    {
        private event EventHandler<ObjectStateChangeArgs> _LF;

        private bool? _FinallyRemoved = null;
        private bool _Locked = false;
        private ObjectState _State = ObjectState.Available;
        private bool _Init = false;
        //private readonly object _SyncLocker = new object();

        const string _StateProperty = "State";

        public StateObjectAdapter(): base()
        {
        }

        protected abstract bool IsFileBroken(bool UpdateStatusFile);

        internal abstract void OnRemove(IImportContext iic);

        protected virtual void OnFirstCompute()
        {
        }

        #region Synchro locker

        //private LifeCycleLocker _AvailabilityLocker;

        //private class LifeCycleLocker : IObjectStateLocker
        //{
        //    private StateObjectAdapter _Father;
        //    private int _Count = 1;
        //    private bool _Disp = false;
        //    private bool _Canceled = false;
        //    private Thread _CurrentThread;
        //    private IImportContext _Iic;

        //    internal LifeCycleLocker(StateObjectAdapter iFather, IImportContext iic)
        //    {
        //        _Father = iFather;
        //        _Father.Locked();
        //        _CurrentThread = Thread.CurrentThread;
        //        _Iic = iic;
        //    }

        //    internal LockerResult Queue()
        //    {
        //        lock (MyLocker)
        //        {
        //            Thread current = Thread.CurrentThread;
        //            if (current == _CurrentThread)
        //                return new LockerResult(LockerStatus.AlreadyLock_SameThread);

        //            _Count++;
        //            while (!_Disp)
        //                Monitor.Wait(MyLocker);

        //            if (_Canceled)
        //                return new LockerResult(LockerStatus.NotAvailable);

        //            _CurrentThread = current;

        //            _Disp = false;
        //            return new LockerResult(this);
        //        }
        //    }

        //    public void Remove()
        //    {
        //        lock (MyLocker)
        //        {

        //            if (_Canceled)
        //                return;

        //            _Father.PrivateRemove(_Iic);
        //            _Disp = true;
        //            _Canceled = true;
        //            _Father._AvailabilityLocker = null;
        //            _CurrentThread = null;
        //            Monitor.PulseAll(MyLocker);
        //        }
        //    }

        //    private object MyLocker
        //    {
        //        get
        //        {
        //            return _Father._Locker;
        //        }
        //    }

        //    public void Dispose()
        //    {
        //        lock (MyLocker)
        //        {
        //            if (--_Count == 0)
        //            {
        //                _Father.Unlocked();
        //                _Father._AvailabilityLocker = null;
        //            }
        //            else
        //            {
        //                _Disp = true;
        //                Monitor.Pulse(MyLocker);
        //            }
        //        }
        //    }
        //}

        //private void PrivateRemove(IImportContext iic)
        //{
        //    OnRemove(iic);
        //    _FinallyRemoved = true;

        //    CacheState();
        //}

        //internal LockerResult GetAvailabilityLocker(IImportContext iic, bool Instantaneous)
        //{
        //    lock (_Locker)
        //    {
        //        if (_FinallyRemoved!=null)
        //            return new LockerResult(LockerStatus.NotAvailable);

        //        switch (_State)
        //        {
        //            case ObjectState.UnderEdit:
        //                if (Instantaneous)
        //                    return new LockerResult(LockerStatus.AlreadyLock_InstaneousRequest);
        //                break;

        //            case ObjectState.FileNotAvailable:
        //                return new LockerResult(LockerStatus.NotAvailable);
        //        }


        //        if (_AvailabilityLocker == null)
        //        {
        //            _AvailabilityLocker = new LifeCycleLocker(this,iic);
        //            return new LockerResult(_AvailabilityLocker);
        //        }

        //        return _AvailabilityLocker.Queue();
        //    }
        //}

        //private void Locked()
        //{
        //    if (_Locked == true)
        //        throw new Exception("Algo error");

        //    _Locked = true;
        //    CacheState();
        //}

        //private void Unlocked()
        //{
        //    if (_Locked == false)
        //        throw new Exception("Algo error");

        //    _Locked = false;
        //    CacheState();
        //}

        #endregion

        private ObjectState UpdateState(bool UpdateStatusFile)
        {
            if (_FinallyRemoved == false)
                return ObjectState.UnderRemove;

            if (_FinallyRemoved==true)
                return ObjectState.Removed;

            if (_Locked)
                return ObjectState.UnderEdit;

            return IsFileBroken(UpdateStatusFile) ? ObjectState.FileNotAvailable : ObjectState.Available;
        }

        protected ObjectState CacheState(bool UpdateStatusFile = false)
        {
            //lock (_SyncLocker)
            //{
                var oldalive = IsAlive;
                ObjectState old = _State;

                ObjectState os = UpdateState(UpdateStatusFile);
                bool res = os != _State;

                _State = os;

                if (res)
                {
                    if (_LF != null)
                        _LF(this, new ObjectStateChangeArgs(this, old, _State));

                    PropertyHasChanged(_StateProperty, old, _State);
                    PropertyHasChanged("IsAlive", oldalive, IsAlive);
                }

                return _State;
            //}
        }


        public bool IsAlive
        {
            get
            {
                //lock (_SyncLocker)
                //{
                    return ((_State != ObjectState.Removed) && ((_State != ObjectState.UnderRemove)));
                //}
            }
        }

        public ObjectState UpdatedState
        {
            get
            {
                return CacheState(true);
            }
        }

        public ObjectState State
        {
            get
            {
                //lock (_SyncLocker)
                //{
                    return InternalState;
                //}
            }
        }

        public event EventHandler<ObjectStateChangeArgs> ObjectStateChanges
        {
            add { _LF += value; }
            remove { _LF -= value; }
        }

        protected bool IsStatusInitialized
        {
            get
            {
                //lock (_SyncLocker)
                //{
                    return _Init;
                //}
            }
        }

        void IObjectStateCycle.SetInternalState(ObjectState value, IImportContext iic)
        {
            //lock (_SyncLocker)
            //{
                switch (value)
                {
                    case ObjectState.FileNotAvailable:
                        throw new Exception("File management");

                    case ObjectState.Available:
                        ObjectState inter = InternalState;
                        if ((inter != ObjectState.UnderEdit) && (inter != ObjectState.UnderRemove))
                            throw new Exception("File management");
                        break;
                }

                if (value == _State)
                {
                    if (value == ObjectState.UnderEdit)
                        throw new Exception("object state management");

                    return;
                }

                if (_State == ObjectState.Removed)
                    throw new Exception("Album remove problem");

                switch (value)
                {
                    case (ObjectState.UnderRemove):
                        _FinallyRemoved = false;
                        break;

                    case (ObjectState.Removed):
                        OnRemove(iic);
                        _FinallyRemoved = true;
                        break;

                    case (ObjectState.UnderEdit):
                        _Locked = true;
                        break;

                    case (ObjectState.Available):
                        if (_FinallyRemoved == false)
                        {
                            _FinallyRemoved = null;
                        }
                        else
                        {
                            _Locked = false;
                        }
                        break;
                }

                CacheState();
            //}
        }

        //ObjectState IObjectStateCycle.InternalState
        //{
        //    get { return InternalState; }
        //}

        public ObjectState InternalState
        {
            get
            {
                //lock (_SyncLocker)
                //{
                if (_Init == false)
                {
                    _Init = true;
                    CacheState();
                    OnFirstCompute();
                }

                return _State;
                //}
            }

        }

        public virtual void HasBeenUpdated()
        {
        }
    }
}
