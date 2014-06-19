using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MusicCollection.Nhibernate.Session
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class MappingClassAttribute : Attribute
    {
        internal MappingClassAttribute()
        {
        }

        //static internal IEnumerable<Type> MapingType
        //{
        //    get
        //    {
        //        Type MyType = typeof(MappingClassAttribute);
        //        return from T in MyType.Assembly.GetTypes() let Cus = T.GetCustomAttributes(MyType, false) where (Cus != null && Cus.Length > 0) select T;
        //    }
        //}
    }
}
