using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModelHelper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewModelBindingAttribute : Attribute
    {
        public Type ViewModelBaseType
        {
            get;
            private set;
        }

        public ViewModelBindingAttribute(Type iMappingType)
        {
            ViewModelBaseType = iMappingType;
        }
    }
}
