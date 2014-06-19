using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MusicCollection.Fundation;

namespace MusicCollectionWPF.Infra
{
    internal class ToogleAdaptor2 : IToogleProvider
    {

        //CheckBox IncludePhysicalRemove

        private bool? _Result;

        private const string _CheckValueProperty = "CheckValue";
        private const string _IsValidProperty = "IsValid";
        private const string _IsInValidProperty = "IsInValid";


        internal ToogleAdaptor2(BasicBehaviour Init)
        {
            switch (Init)
            {
                case BasicBehaviour.AskEndUser:
                    _Result = null;
                    break;

                case BasicBehaviour.Yes:
                    _Result = true;
                    break;

                case BasicBehaviour.No:
                    _Result = false;
                    break;
            }
        }

        public bool? CheckValue
        {
            get { return _Result; }
            set { _Result = value; OnChange(_CheckValueProperty); OnChange(_IsValidProperty); OnChange(_IsInValidProperty); }
        }

        public bool IsValid
        {
            get { return _Result != null; }
        }

        public bool IsInValid
        {
            get { return !IsValid; }
        }

        private void OnChange(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return IsValid ? null : "Must Define a Behaviour for broken tracks."; }
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

        public string this[string columnName]
        {
            get
            {
                if ((_CheckValueProperty == columnName) && (_Result == null))
                {
                    return "Define a Behaviour for files";
                }

                return null;
            }
        }
    }


    internal class ToogleAdaptor3 : IToogleProvider
    {

        //CheckBox IncludePhysicalRemove

        private bool? _Result;

        private const string _CheckValueProperty = "CheckValue";
        private const string _IsValidProperty = "IsValid";
        private const string _IsInValidProperty = "IsInValid";


        internal ToogleAdaptor3()
        {
        }

        public bool? CheckValue
        {
            get { return _Result; }
            set { _Result = value; OnChange(_CheckValueProperty); OnChange(_IsValidProperty); OnChange(_IsInValidProperty); }
        }

        public bool IsValid
        {
            get { return _Result != null; }
        }

        public bool IsInValid
        {
            get { return !IsValid; }
        }

        private void OnChange(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return IsValid ? null : "Must Define a Behaviour for broken tracks."; }
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

        public string this[string columnName]
        {
            get
            {
                if ((_CheckValueProperty == columnName) && (_Result == null))
                {
                    return "Define a Behaviour for files";
                }

                return null;
            }
        }
    }
}
