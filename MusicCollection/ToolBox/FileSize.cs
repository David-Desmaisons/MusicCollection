using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox
{
    internal class FileSize
    {
        private long _OctetSize;

        private const long _Base = 1024;

        private FileSize(long Os)
        {
            _OctetSize = Os;
        }

        private FileSize()
        {
            _OctetSize = 0;
        }

        internal long SizeInOctet
        {
            get{return _OctetSize;}
            set{_OctetSize=value;}
        }

        internal long SizeInKo
        {
            get{return (long)Math.Round((decimal)SizeInOctet/_Base);}
            set{SizeInOctet=value*_Base;}
        }

         internal long SizeInMo
        {
            get{return (long)Math.Round((decimal)SizeInKo/_Base);;}
            set{SizeInKo=value*_Base;}
        }

         private double DoubleSizeInMo
         {
             set { SizeInOctet = (long) Math.Round(value * _Base*_Base); }
         }

        internal long SizeInGo
        {
            get{return (long)Math.Round((decimal)SizeInMo/_Base);;}
            set{SizeInMo=value*_Base;}
        }

        static internal FileSize FromO(long O)
        {
            return new FileSize(O);
        }

        public override string ToString()
        {
            return string.Format("Size in Mo:{0}", SizeInMo);
        }

        public static FileSize operator -(FileSize s1, FileSize s2)
        {
            return new FileSize(s1.SizeInOctet-s2.SizeInOctet);
        }

        public static FileSize operator +(FileSize s1, FileSize s2)
        {
            return new FileSize(s1.SizeInOctet + s2.SizeInOctet);
        }

        public static FileSize operator -(FileSize s1)
        {
            return new FileSize(-s1.SizeInOctet);
        }


        static internal FileSize FromKO(long KO)
        {
           FileSize res = new FileSize();
            res.SizeInKo=KO;
            return res;
        }

        static internal FileSize FromMo(long MO)
        {
              FileSize res = new FileSize();
            res.SizeInMo=MO;
            return res;
        }

        static internal FileSize FromMo(double MO)
        {
            FileSize res = new FileSize();
            res.DoubleSizeInMo = MO;
            return res;
        }

        static internal FileSize FromGo(long Go)
        {
            FileSize res = new FileSize();
            res.SizeInGo=Go;
            return res;
        }
    }
}
