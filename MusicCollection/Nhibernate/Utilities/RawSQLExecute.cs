//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
////using System.Data.SQLite;

//using NHibernate;

//namespace MusicCollection.Nhibernate.Utilities
//{
//    internal class RawSQLExecute
//    {
//        private string _SQL;
//        private ISession _Session;
//        //public int Timeout
//        //{
//        //    get;
//        //    set;
//        //}

//        internal RawSQLExecute(string isql, ISession iMSI)
//        {
//            _SQL = isql;
//            _Session = iMSI;
//            //Timeout = 600;
//        }

//        internal bool Execute()
//        {
//            IDbConnection connection = _Session.Connection;
//            IDbCommand sqlcommand = connection.CreateCommand();

//            //SQLiteCommand  sqlcommand = command as SQLiteCommand;

//            //if (sqlcommand==null)
//            //    return false;

//            //command.CommandTimeout = Timeout;
//            //command.CommandType = CommandType.Text; 
//            //command.CommandText = "vacuum;";
//            sqlcommand.CommandText = _SQL;
//            _Session.Transaction.Enlist(sqlcommand);
//            sqlcommand.ExecuteNonQuery();

//            return true;
//        }
//    }
//}
