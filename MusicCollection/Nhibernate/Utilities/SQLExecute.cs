using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using NHibernate;

using MusicCollection.Implementation;
using MusicCollection.Nhibernate.Session;


namespace MusicCollection.Nhibernate.Utilities
{
    public class SQLExecute
    {
        private string _SQL;
        private IInternalMusicSession _MSI = null;
        private ISession _Session = null;

        internal SQLExecute(string isql, IInternalMusicSession iMSI)
        {
            _SQL = isql;
            _MSI = iMSI;
        }


        internal SQLExecute(string isql, ISession iMSI)
        {
            _SQL = isql;
            _Session = iMSI;
        }

        internal void Execute()
        {
            if (_Session != null)
            {
                PrivateExecute();
                return;
            }

            IImportContext iic = _MSI.GetNewSessionContext();

            using (IDBSession session = DBSession.CreateorGetCurrentSession(iic))
            {
                _Session = session.NHSession;
                PrivateExecute();
            }

            iic.Commit();
        }

        private void PrivateExecute()
        {
            IDbConnection connection = _Session.Connection;
            IDbCommand sqlcommand = connection.CreateCommand();
            sqlcommand.CommandText = _SQL;
            _Session.Transaction.Enlist(sqlcommand);
            sqlcommand.ExecuteNonQuery();
        }
    }
}
