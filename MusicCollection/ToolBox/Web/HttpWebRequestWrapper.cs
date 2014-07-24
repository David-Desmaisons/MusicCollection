using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace MusicCollection.ToolBox.Web
{
    internal class HttpWebRequestWrapper : IHttpWebRequest
    {
        private HttpWebRequest _Wrapped;
        internal HttpWebRequestWrapper(HttpWebRequest iWrapped)
        {
            _Wrapped = iWrapped;
        }

        public Uri RequestUri
        {
            get { return _Wrapped.RequestUri; }
        }

        public int Timeout
        {
            get { return _Wrapped.Timeout; }
            set { _Wrapped.Timeout = value; }
        }

        public int ReadWriteTimeout
        {
            get { return _Wrapped.ReadWriteTimeout; }
            set { _Wrapped.ReadWriteTimeout = value; }
        }

        public string UserAgent
        {
            get { return _Wrapped.UserAgent; }
            set { _Wrapped.UserAgent = value; }
        }

        public string Referer
        {
            get { return _Wrapped.Referer; }
            set { _Wrapped.Referer = value; }
        }

        public ICredentials Credentials
        {
            get { return _Wrapped.Credentials; }
            set { _Wrapped.Credentials = value; }
        }

        public string Accept
        {
            get { return _Wrapped.Accept; }
            set { _Wrapped.Accept = value; }
        }

        public string ContentType
        {
            get { return _Wrapped.ContentType; }
            set { _Wrapped.ContentType = value; }
        }

        public string Method
        {
            get { return _Wrapped.Method; }
            set { _Wrapped.Method = value; }
        }

        public Stream GetRequestStream()
        {
            return _Wrapped.GetRequestStream();
        }

        public NameValueCollection Headers
        {
            get { return _Wrapped.Headers; }
        }

        public IHttpWebResponse GetResponse()
        {
            return new HttpWebResponseWrapped((HttpWebResponse)_Wrapped.GetResponse());
        }


        public bool PreAuthenticate
        {
            get {   return _Wrapped.PreAuthenticate;}
            set {   _Wrapped.PreAuthenticate=value;}
        }


        public DecompressionMethods AutomaticDecompression
        {
            get {   return _Wrapped.AutomaticDecompression;}
            set {   _Wrapped.AutomaticDecompression=value;}
        }
    }
}
