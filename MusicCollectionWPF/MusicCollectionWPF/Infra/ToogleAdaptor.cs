using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.Infra
{
    internal class ToogleAdaptor : IToogleProvider
    {

        //CheckBox IncludePhysicalRemove

        private IMusicRemover _IMR;

        private const string _CheckValueProperty = "CheckValue";
        private const string _IsValidProperty = "IsValid";
        private const string _IsInValidProperty = "IsInValid";
 

        internal ToogleAdaptor(IMusicRemover IMU)
        {
            _IMR = IMU;
        }

        public bool? CheckValue
        {
            get { return _IMR.IncludePhysicalRemove; ; }
            set { _IMR.IncludePhysicalRemove = value; OnChange(_CheckValueProperty); OnChange(_IsValidProperty); OnChange(_IsInValidProperty); }
        }

        public bool IsValid
        {
            get { return _IMR.IsValid; }
        }

        public bool IsInValid
        {
            get { return !IsValid; }
        }

        private void OnChange(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(Name));
        }

        public bool ResultValue
        {
            get
            {
                if (CheckValue == null)
                    throw new Exception("Algo Error");

                return (bool)CheckValue;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return IsValid? null :"Must Define a Behaviour for file."; }
        }

        public string this[string columnName]
        {
            get 
            { 
                if ((_CheckValueProperty==columnName) && ((_IMR.IncludePhysicalRemove)==null))
                {
                    return "Define a Behaviour for files";
                }
                
                return null;
             }
        }
    }
}
