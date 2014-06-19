using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public class ObjectStateChangeArgs : EventArgs
    {
        private ObjectState _NLf;
        private ObjectState _OLf;
        private IObjectState _Changed;


        public ObjectState NewState
        {
            get { return _NLf; }
        }

        public ObjectState OldState
        {
            get { return _OLf; }
        }

        public IObjectState Changed
        {
            get { return _Changed; }
        }

        //public override bool Equals(object obj)
        //{
        //    ObjectStateChangeArgs o = obj as ObjectStateChangeArgs;
        //    if (obj == null)
        //        return false;

        //    if (_OLf != o._OLf)
        //        return false;

        //    if (_NLf != o._NLf)
        //        return false;

        //    return (_Changed == o._Changed);
        //}

        //public override int GetHashCode()
        //{
        //    return _Changed.GetHashCode() ^  _OLf.GetHashCode();
        //}

        internal ObjectStateChangeArgs(IObjectState changed, ObjectState OLF, ObjectState NLF)
        {
            _OLf = OLF;
            _NLf = NLF;
            _Changed = changed;
        }
    }

    public enum ObjectState
    {
        Available, FileNotAvailable, UnderEdit, Removed, UnderRemove
    };

    public interface IObjectState
    {
        bool IsAlive {get;}

        ObjectState State { get; }

        ObjectState UpdatedState { get; }

        event EventHandler<ObjectStateChangeArgs> ObjectStateChanges;
    }

    internal interface IObjectStateLocker : IDisposable
    {
        void Remove();
    }


}
