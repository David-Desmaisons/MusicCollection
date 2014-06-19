using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Web;

namespace MusicCollection.ToolBox.Buffer
{
    static public class BufferFactory
    {

        static public IBufferProvider GetBufferProviderFromStream(Stream iBuffer)
        {
            return InternalBufferFactory.GetBufferProviderFromStream(iBuffer);
        }

        static public IBufferProvider GetBufferProviderFromArray(byte[] iBuffer)
        {
            return InternalBufferFactory.GetBufferProviderFromArray(iBuffer);
        }

        static public IBufferProvider GetBufferProviderFromFile(string FileName)
        {
            return InternalBufferFactory.GetBufferProviderFromFile(FileName);
        }

        static public IBufferProvider GetBufferProviderFromURI(Uri FileName, IHttpContextFurnisher useragent = null)
        {
            return InternalBufferFactory.GetBufferProviderFromURI(FileName, useragent);
        }

        static public IBufferProvider GetBufferProviderFromHttpRequest(IHttpWebRequest iIHttpWebRequest)
        {
            return InternalBufferFactory.GetBufferProviderFromHttpRequest(iIHttpWebRequest);
        }
    }
}
