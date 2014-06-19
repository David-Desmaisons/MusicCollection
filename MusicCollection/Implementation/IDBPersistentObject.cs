using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;
using MusicCollection.ToolBox;

namespace MusicCollection.Implementation
{
    internal interface ISessionPersistentObject:IObjectStateCycle
    {
  
        IImportContext Context
        {
            set;
            get;
        }

        //DataBase ID
        int ID
        { get; }

        //need to think of specs
        //current spec remove from corecollection and visible collection
        //need to be propagated to children 
        //called by import transaction on each object addfor remove
        void UnRegisterFromSession(IImportContext session);

        //called after Database load
        //object has to be registered in session and has to be visible
        //not propagated to child element
        //called at end of nhibernate transation via event
        void OnLoad(IImportContext iic);

        //Called after object construction for object being constructed
        //object not yet visible in session
        //not propagated to children
        //called by import transaction for album and by track static constructor
        void Register(IImportContext iic);  
        
        //Called after object creation to make object visible 
        //need to be propagated to children
        //called by import transaction on root object created 
        void Publish();
    }
}
