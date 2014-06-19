using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public interface IObjectAttribute
    {
        //ObjectModifiedArgs this[string iAttributeName] { get; }

        event EventHandler<ObjectModifiedArgs> ObjectChanged;
    }

    public interface IObjectBuildAttributeListener
    {
        void AttributeChanged<T>(string AttributeName, T oldv, T newv);
    }

    public class ObjectModifiedArgs : EventArgs
    {
        public object ModifiedObject
        {
            get;
            private set;
        }

        public string AttributeName
        {
            get;
            private set;
        }

        public object OldAttributeValue
        {
            get;
            private set;
        }

        public object NewAttributeValue
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            ObjectModifiedArgs o = obj as ObjectModifiedArgs;
            if (o == null) return false;

            return Object.ReferenceEquals(ModifiedObject, o.ModifiedObject) && object.Equals(OldAttributeValue, o.OldAttributeValue) && object.Equals(NewAttributeValue, o.NewAttributeValue);
        }

        public override int GetHashCode()
        {
            return (ModifiedObject == null ? 0 : ModifiedObject.GetHashCode()) ^ (OldAttributeValue == null ? 0 : OldAttributeValue.GetHashCode()) ^ (NewAttributeValue == null ? 0 : NewAttributeValue.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("Object:<{3}> Attribute:<{0}> Old:<{1}> New:<{2}>", this.AttributeName, this.OldAttributeValue, this.NewAttributeValue, this.ModifiedObject);
        }

        public ObjectModifiedArgs(object myobj, string iAttributeName, object iOldAttributeValue, object iNewAttributeValue)
        {
            ModifiedObject = myobj;
            AttributeName = iAttributeName;
            OldAttributeValue = iOldAttributeValue;
            NewAttributeValue = iNewAttributeValue;

        }
    }

    public interface IObjectAttributeChanged<T>
    {
        T Old
        {
            get;
        }

        T New
        {
            get;
        }

        object ModifiedObject
        {
            get;
        }

        string AttributeName
        {
            get;
        }
    }



    public class ObjectAttributeChangedArgs<TAtribute> : ObjectModifiedArgs, IObjectAttributeChanged<TAtribute>
    {
        public TAtribute Old
        {
            get;
            private set;
        }

        public TAtribute New
        {
            get;
            private set;
        }


        public ObjectAttributeChangedArgs(object myobj, string iAttributeName, TAtribute iOld, TAtribute iNew)
            : base(myobj, iAttributeName, iOld, iNew)
        {
            Old = iOld;
            New = iNew;
        }
    }


}
