using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox
{
    internal class SHA1KeyComputer:IDisposable
    {
        private Stream _str;
        private static   SHA1CryptoServiceProvider _UnmanagedKeyComputer = new SHA1CryptoServiceProvider();
        private static readonly string _Dummy;
        private static long _Limitino = FileSize.FromMB(8).SizeInByte;
        private bool _NeedtoClean = true;

        static SHA1KeyComputer()
        {
            _Dummy = new string('X', 40);
        }

        static internal SHA1KeyComputer FromStream(Stream str)
        {
            return new SHA1KeyComputer(str);
        }

        static internal SHA1KeyComputer FromPath(string str, long Begin, long End)
        {
            return new SHA1KeyComputer(str, Begin, End);
        }

        private SHA1KeyComputer(string Path,long Begin, long End)
        {
            if ((_Limitino > 0) && (((End-Begin)>_Limitino) || (End==-1)) )
            {
                End = Begin + _Limitino;
            }

            _str = new ReadableDuplicateStream(Path, Begin, End);
        }

        private SHA1KeyComputer(Stream str)
        {
            _NeedtoClean = false;
            _str = str;
        }

        static internal bool IsKeyDummy(string key)
        {
            return key == _Dummy;
        }

        static internal string Dummy
        { get { return _Dummy; } }

        private string ComputeKeyFromRaw(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        internal string ComputeKey()
        {
            return ComputeKeyFromRaw(_UnmanagedKeyComputer.ComputeHash(_str));
        }


        public void Dispose()
        {
            if (_NeedtoClean)
            {
                _str.Dispose();
                _NeedtoClean = false;
            }
        }
    }
}
