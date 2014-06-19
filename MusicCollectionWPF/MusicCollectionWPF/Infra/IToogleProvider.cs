using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MusicCollectionWPF.Infra
{
    public interface IToogleProvider : INotifyPropertyChanged, IDataErrorInfo
    {

        bool? CheckValue
        {
            get;
            set;
        }

        bool IsValid
        {
            get;
        }

        bool ResultValue
        {
            get;
        }
    }
}
