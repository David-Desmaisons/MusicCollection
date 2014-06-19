using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using NHibernate.UserTypes;
using NHibernate.SqlTypes;
using NHibernate;

using FluentNHibernate.Conventions;

using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Buffer;
using MusicCollection.Infra;

namespace MusicCollectionTest.Integrated.Session_Accessor
{
    //dem danger
    public class DummyBufferConvention : UserTypeConvention<DummyBufferType>
    { }


    [Serializable]
    public class DummyBufferType: IUserType
    {
        static DummyBufferType()
        {
            IsOK = true;
        }

        static public bool IsOK
        {
            get;
            set;
        }
        // Summary:
        //     Are objects of this type mutable?
        public bool IsMutable { get { return false; } }

        //
        // Summary:
        //     The type returned by NullSafeGet()
        public Type ReturnedType { get { return typeof(IBufferProvider); } }


        private static readonly SqlType[] _Types = new SqlType[] { NHibernateUtil.String.SqlType };


        //
        // Summary:
        //     The SQL types for the columns mapped by this type.
        public SqlType[] SqlTypes { get { return _Types; } }


        public object Assemble(object cached, object owner)
        {
            return cached;
        }


        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Disassemble(object value)
        {
            return value;
        }
       
        public new bool Equals(object x, object y)
        {
            if (x == null)
                return (y == null);


            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            if (x == null)
                return 0;

            return x.GetHashCode();
        }
        //
        // Summary:
        //     Retrieve an instance of the mapped class from a JDBC resultset.  Implementors
        //     should handle possibility of null values.
        //
        // Parameters:
        //   rs:
        //     a IDataReader
        //
        //   names:
        //     column names
        //
        //   owner:
        //     the containing entity
        //
        // Exceptions:
        //   NHibernate.HibernateException:
        //     HibernateException
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            if (rs == null)
                return null;

            string res = (string)NHibernateUtil.String.NullSafeGet(rs, names[0]);

            if (res == null)
                return null;

            IPersistentBufferProvider IPBF = InternalBufferFactory.GetBufferProviderFromFile(res);
            if (IPBF!=null)
                IPBF.IsPersistent = true;

            return IPBF;
        }


        //
        // Summary:
        //     Write an instance of the mapped class to a prepared statement.  Implementors
        //     should handle possibility of null values.  A multi-column type should be
        //     written to parameters starting from index.
        //
        // Parameters:
        //   cmd:
        //     a IDbCommand
        //
        //   value:
        //     the object to write
        //
        //   index:
        //     command parameter index
        //
        // Exceptions:
        //   NHibernate.HibernateException:
        //     HibernateException
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {

            if ((!IsOK) || (value == null))
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                IPersistentBufferProvider Buff = value as IPersistentBufferProvider;
                if (!Buff.IsPersistent)
                    throw new Exception("Internal Error Blob management");

                ((IDataParameter)cmd.Parameters[index]).Value = Buff.PersistedPath;
            }


        }


    }
 
}

