using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyBinding
{
    public class TypeFromStringSolver
    {

        private static readonly Dictionary<string, Type> _CachedType;

        static TypeFromStringSolver()
        {
            _CachedType = new Dictionary<string, Type>();
        }

        static public Type FromString(string myType)
        {
            Type res = null;
            if (_CachedType.TryGetValue(myType, out res)) 
            {
                return res;
            }

            var domain = AppDomain.CurrentDomain;
            if (domain == null) 
                return null;

            foreach (var assembly in domain.GetAssemblies())
            {
                res = assembly.GetType(myType);
                if (res != null)
                    break;
            }

            _CachedType.Add(myType,res);
            return res;
        }
          
    }
}
