using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.Nhibernate.Session
{
    internal static class ISessionPersistentObjectExtension
    {

        internal static bool ChangeCommitted(this ISessionPersistentObject al,CRUD Operation, IImportContext iic,  ISession session)
        {
            if (al == null)
                throw new Exception("Algo Error");

            bool needtoregister = false;

            IObjectStateCycle oa = al;

            switch (Operation)
            {
                case CRUD.Created:
                    needtoregister = true;
                    session.Save(al);
                    break;

                case CRUD.Update:
                    session.Update(al);         
                    oa.HasBeenUpdated();
                    break;

                case CRUD.Delete:
                    session.Delete(al);
                    oa.SetInternalState(ObjectState.Removed,iic);
                    break;
            }

            al.Context = null;

            return needtoregister;
        }



        internal static bool ChangeRequested(this ISessionPersistentObject Al, CRUD Operation, IImportContext iit)
        {
            if (Al == null)
                throw new Exception("Algo Error");

            switch (Operation)
            {
                case CRUD.Created:
                    Al.Register(iit);
                    break;

                case CRUD.Delete:
                    Al.SetInternalState(ObjectState.UnderRemove, iit);
                    Al.UnRegisterFromSession(iit);
                    break;

                case CRUD.Ignore:
                    if (Al.Context != null)
                        throw new Exception("Session transaction managment");
                    return false;
            }

            Al.Context = iit;
            return true;
        }

        internal static bool RollBackChanges(this ISessionPersistentObject Al, CRUD Operation, IImportContext iit)
        {
            if (Al == null)
                throw new Exception("Algo Error");

            switch (Operation)
            {
                case CRUD.Created: 
                    Al.UnRegisterFromSession(iit);
                    
                    break;

                case CRUD.Delete:
                    Al.Register(iit);
                    Al.SetInternalState(ObjectState.Available, iit);
                    break;
             }

            Al.Context = null;
            return true;
        }
    }

}
