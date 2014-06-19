using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Dynamic;
using System.ComponentModel;
using System.Reflection;


namespace MusicCollection.DataExchange
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal class MusicObjectAttributeMappingAttribute : Attribute
    {
        internal string AttributeName
        {
            private set;
            get;
        }

        internal DataExportImportType TypeofImport
        {
            private set;
            get;
        }

        internal MusicObjectAttributeMappingAttribute(DataExportImportType iTypeofImport, string Name)
        {
            AttributeName = Name;
            TypeofImport = iTypeofImport;
        }
    }
}
