using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MusicCollection.Infra
{
    public static class AttributeExtender
    {
        static public IEnumerable<Tuple<T, Type>> GetMarkedTypeInTheAssembly<T>() where T : Attribute
        {
            //Type attributetype = typeof(T);
            //return from type in attributetype.Assembly.GetTypes() let Cus = type.GetCustomAttributes(attributetype, false)
            //       where (Cus != null && Cus.Length > 0)
            //       select new Tuple<T, Type>(Cus[0] as T, type);

            return typeof(T).Assembly.GetMarkedType<T>();
        }

        static public IEnumerable<Tuple<T, Type>> GetMarkedType<T>(this Assembly iAssembly) where T : Attribute
        {
            Type attributetype = typeof(T);
            return from type in iAssembly.GetTypes()
                   let Cus = type.GetCustomAttributes(attributetype, false)
                   where (Cus != null && Cus.Length > 0)
                   select new Tuple<T, Type>(Cus[0] as T, type);
        }

        //static public IEnumerable<Tuple<T, Type>> GetMarkedTypeInTheAssembly<T>(this Assembly ass) where T : Attribute
        //{
        //    Type attributetype = typeof(T);
        //    return from type in ass.GetTypes()
        //           let Cus = type.GetCustomAttributes(attributetype, false)
        //           where (Cus != null && Cus.Length > 0)
        //           select new Tuple<T, Type>(Cus[0] as T, type);
        //}
    }
}
