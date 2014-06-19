using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Infra
{
    public class FileSize
    {
        private long _ByteSize;

        private const long _Base = 1024;

        public FileSize(long Os)
        {
            _ByteSize = Os;
        }

        private FileSize()
        {
            _ByteSize = 0;
        }

        public long SizeInByte
        {
            get { return _ByteSize; }
            set { _ByteSize = value; }
        }

        public long SizeInKB
        {
            get { return (long)Math.Truncate((decimal)SizeInByte / _Base); }
            set { SizeInByte = value * _Base; }
        }

        public long SizeInMB
        {
            get { return (long)Math.Truncate((decimal)SizeInKB / _Base); }
            set { SizeInKB = value * _Base; }
        }

        public decimal SizeInMBD
        {
            get { return ((decimal)SizeInKB / _Base); }
        }

        public double DoubleSizeInMB
        {
            set { SizeInByte = (long)Math.Truncate(value * _Base * _Base); }
        }

        public long SizeInGB
        {
            get { return (long)Math.Truncate((decimal)SizeInMB / _Base); }
            set { SizeInMB = value * _Base; }
        }

        public decimal SizeInGBD
        {
            get { return ((decimal)SizeInMB / _Base); }
        }

        static public FileSize FromBytes(long O)
        {
            return new FileSize(O);
        }

        public override string ToString()
        {
            if (SizeInGB >= 1)
                return string.Format("{0:0.0} GB", SizeInGBD);

            if (SizeInMB >= 1)
                return string.Format("{0:0.0} MB", SizeInMBD);

            return string.Format("{0} KB", SizeInKB);
        }

        public static FileSize operator -(FileSize s1, FileSize s2)
        {
            return new FileSize(s1.SizeInByte - s2.SizeInByte);
        }

        public static FileSize operator +(FileSize s1, FileSize s2)
        {
            return new FileSize(s1.SizeInByte + s2.SizeInByte);
        }

        public static FileSize operator -(FileSize s1)
        {
            return new FileSize(-s1.SizeInByte);
        }


        static internal FileSize FromKB(long KO)
        {
            FileSize res = new FileSize();
            res.SizeInKB = KO;
            return res;
        }

        static public FileSize FromMB(long MO)
        {
            FileSize res = new FileSize();
            res.SizeInMB = MO;
            return res;
        }

        static public FileSize FromMB(double MO)
        {
            FileSize res = new FileSize();
            res.DoubleSizeInMB = MO;
            return res;
        }

        static public FileSize FromGB(long Go)
        {
            FileSize res = new FileSize();
            res.SizeInGB = Go;
            return res;
        }
    }
}
