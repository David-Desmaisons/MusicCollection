using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace MusicCollectionWPF.Infra
{

    public class EnumPresenter
    {
        public EnumPresenter(object o)
        {
            string valuename = o.ToString();
            Type EnumType = o.GetType();
            FieldInfo fi = EnumType.GetField(valuename);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            EnumValue = o;
            EnumDisplayvalue = (attributes != null && attributes.Length > 0) ? attributes[0].Description : valuename;
        }

        public int NumericValue
        {
            get { return (int)EnumValue; }
        }

        public object EnumValue
        {
            get;
            private set;
        }

        public string EnumDisplayvalue
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return EnumDisplayvalue;
        }

    }

    [MarkupExtensionReturnType(typeof(object[]))]
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.EnumType == null)
                throw new ArgumentException("The enum type is not set");


            return (from v in Enum.GetValues(this.EnumType).Cast<object>()
                    select new EnumPresenter(v)
                    ).ToList();

        }
    }
}
