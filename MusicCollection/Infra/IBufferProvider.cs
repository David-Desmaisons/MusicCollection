using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MusicCollection.Infra
{
    public interface IBufferProvider: IDisposable
    {
        Stream GetBuffer();

        bool IsOK { get; }

        byte[] RawData
        {
            get;
        }

        string PersistedPath
        {
            get;
        }

        string DefaultExtension
        {
            get;
            set;
        }

        long Length
        {
            get;
        }

        bool CopyTo(string FileName);

        IBufferProvider Clone();

        bool Compare(IBufferProvider Other);

        int GetContentHashCode();

    }

}
