using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Implementation;
using MusicCollection.Nhibernate.Mapping;

namespace MusicCollectionTest.TestObjects
{
    internal class MyObject : StateObjectAdapter, IObject, IObjectStateCycle, ISessionPersistentObject, IComparable
    {
        static private string _NP = "Name";
        static private string _NV = "Value";
        static private string _NF = "Friend";
        static int _Count = 0;


        private string _Name;
        public string Name
        {
            set { if (_Name == value) return; string old = Name; _Name = value; PropertyHasChanged(_NP,old,_Name); }
            get { return _Name; }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
        }

        IObject IObject.MyFriend
        {
            set { Friend = value as MyObject; }
            get { return _Friend; }
        }

        public bool IsObserved
        {
            get { return base.IsObjectObserved; }
        }

        private MyObject _Friend;
        public MyObject Friend
        {
            set { if (_Friend == value) return; MyObject old = _Friend; _Friend = value; PropertyHasChanged(_NF, old, _Friend); }
            get { return _Friend; }
        }

        private ObservableCollection<MyObject> _MyFriends= new ObservableCollection<MyObject>();
        public ObservableCollection<MyObject> MyFriends
        {
            get { return _MyFriends; }
            set 
            {
                var old = _MyFriends;
                _MyFriends=value;
                PropertyHasChanged("MyFriends", old, value);
            }
        }


        private int _Value;
        public int Value
        {
            set { if (_Value == value) return; int old = _Value; _Value = value; PropertyHasChanged(_NV, old, _Value); }
            get { return _Value; }
        }

        internal MyObject(string iN, int iv)
        {
            _Name = iN;
            _Value = iv;
            _ID = _Count++;
        }

        public override string ToString()
        {
            return string.Format("Name {0} Value {1}", Name, Value);
        }

        private bool _B = false;

        protected override bool IsFileBroken(bool UpdateStatusFile)
        {
            return _B;
        }

        internal ObjectState Break()
        {
            _B = true;
            return this.UpdatedState;
        }

        internal override void OnRemove(IImportContext iic)
        {
        }


        public IImportContext Context
        {
            get;
            set;
        }

        public void UnRegisterFromSession(IImportContext session)
        {
        }

        public void Publish()
        {
        }


        public void OnLoad(IImportContext iic)
        {
        }

        public void Register(IImportContext session)
        {
        }

        public int CompareTo(object obj)
        {
            MyObject o = obj as MyObject;
            return (ID - o.ID);
        }
    }

    internal class MyDummyObject : NotifySimpleAdapter, IObjectStateCycle
    {
        static private string _NP = "Name";
        static private string _NV = "Value";

        private string _Name;
        internal string Name
        {
            set { if (_Name == value) return; string old = Name; _Name = value; PropertyHasChanged(_NP); }
            get { return _Name; }
        }

        private int _Value;
        internal int Value
        {
            set { if (_Value == value) return; int old = _Value; _Value = value; PropertyHasChanged(_NV); }
            get { return _Value; }
        }

        internal MyDummyObject(string iN, int iv)
        {
            _Name = iN;
            _Value = iv;
            _State = ObjectState.Available;
        }

        public override string ToString()
        {
            return string.Format("Name {0} Value {1}", Name, Value);
        }

        private ObjectState _State;

        public ObjectState State
        {
            set
            {
                if (_State == value)
                    return;

                ObjectState old = _State;
                _State = value;

                if (ObjectStateChanges != null)
                    ObjectStateChanges(this, new ObjectStateChangeArgs(this, old, _State));
            }
        }



        public ObjectState UpdatedState
        {
            get { return _State; }
        }

        public bool IsAlive
        {
            get { return _State != ObjectState.Removed; }
        }

        ObjectState IObjectState.State
        {
            get { return _State; }
        }

       

        public event EventHandler<ObjectStateChangeArgs> ObjectStateChanges;

        public void SetInternalState(ObjectState value, IImportContext iic)
        {
            State = value;
        }

        public ObjectState InternalState
        {
            get { return _State; }
        }

        public void HasBeenUpdated()
        {
        }
    }
}
