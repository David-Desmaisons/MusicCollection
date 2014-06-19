using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MusicCollection.ToolBox.Web
{
    internal class HttpWebResponseWrapped : IHttpWebResponse, IDisposable
    {
        private HttpWebResponse _Wrapped;
        internal HttpWebResponseWrapped(HttpWebResponse iWrapped)
        {
            _Wrapped = iWrapped;
        }

        public HttpStatusCode StatusCode { get { return _Wrapped.StatusCode; } }

        public Stream GetResponseStream()
        {
            return _Wrapped.GetResponseStream();
        }

        public string ContentEncoding { get { return _Wrapped.ContentEncoding; } }

        public void Dispose()
        {
            IDisposable id = _Wrapped;
            id.Dispose();
        }

        public void Close()
        {
            _Wrapped.Close();
        }
    }
}
