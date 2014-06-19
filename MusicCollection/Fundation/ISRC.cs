//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace MusicCollection.Fundation
//{
//    public class ISRC
//    {
//        private readonly static Regex _ISRCRegEx;

//        static ISRC()
//        {
//            _ISRCRegEx = new Regex(@"([A-Z]{2}-?\w{3}-?\d{2}-?\d{5})");
//        }

//        public ISRC()
//        {
//        }

//        public string Name
//        {
//            get; 
//            set;
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        static internal ISRC Fromstring(string name)
//        {
//            if (string.IsNullOrEmpty(name))
//                return null;

//            if (!_ISRCRegEx.IsMatch(name))
//                return null;

//            return new ISRC(_ISRCRegEx.Match(name).Groups[1].Value.Replace("-",""));
//        }

//        private ISRC(string iName)
//        {
//            Name = iName;
//        }
//    }
//}
