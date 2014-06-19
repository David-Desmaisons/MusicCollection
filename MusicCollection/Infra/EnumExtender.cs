using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace MusicCollection.Infra
{
    public static class EnumExtender
    {
        public static string GetDescription(this System.Enum o)
        {
            if (o == null)
                return string.Empty;

            string valuename = o.ToString();
            Type EnumType = o.GetType();
            FieldInfo fi = EnumType.GetField(valuename);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes != null && attributes.Length > 0) ? attributes[0].Description : valuename;        
        }
    }
}
