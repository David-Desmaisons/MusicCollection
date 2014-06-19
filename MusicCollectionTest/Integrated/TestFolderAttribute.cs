using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollectionTest.Integrated
{
   
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class TestFolderAttribute : Attribute
    {
        internal string InFolderName
        {
            private set;
            get;
        }

        internal string DBFolderName
        {
            private set;
            get;
        }

        internal TestFolderAttribute(string Name)
        {
            InFolderName = Name;
            DBFolderName = null;
        }

        internal TestFolderAttribute(string Name, string DBName)
        {
            InFolderName = Name;
            DBFolderName = DBName ?? Name;
        }
    }
}
