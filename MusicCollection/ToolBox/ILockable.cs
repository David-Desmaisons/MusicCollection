//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace MusicCollection.ToolBox
//{
//    internal class LockArgs : EventArgs
//    {
//        internal bool Locked
//        {
//            get;
//            private set;
//        }

//        internal LockArgs(bool iLocked)
//        {
//            Locked = iLocked;
//        }
//    }

//    internal interface ILockable
//    {
//        bool Locked
//        {
//            get;
//            set;
//        }

//        event EventHandler<LockArgs> OnLocked;
//    }

//    internal class Locker: IDisposable
//    {
//        private ILockable _Locked;

//        internal Locker(ILockable Lockable)
//        {
//            _Locked = Lockable;
//            Lockable.Locked = true;
//        }

//        public void Dispose()
//        {
//            _Locked.Locked = false;
//        }
//    }
//}
