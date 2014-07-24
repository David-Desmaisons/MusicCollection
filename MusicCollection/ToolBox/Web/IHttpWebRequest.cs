using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace MusicCollection.ToolBox.Web
{
    public interface IInternetProvider
    {
        IHttpWebRequest CreateHttpRequest(string iUrl);

        IHttpWebRequest CreateHttpRequest(Uri iUrl);

        bool GetIsNetworkAvailable();
    }

    public interface IHttpWebRequest
    {
        Uri RequestUri { get; }

        int Timeout { get; set; }

        int ReadWriteTimeout { get; set; }

        string UserAgent { get; set; }

        string Accept { get; set; }

        string ContentType { get; set; }

        string Method { get; set; }

        string Referer { get; set; }

        DecompressionMethods AutomaticDecompression { get; set; }

        ICredentials Credentials { get; set; }

        NameValueCollection Headers { get; }

        Stream GetRequestStream();

        IHttpWebResponse GetResponse();

        bool PreAuthenticate { get; set; }
    }

    public interface IHttpWebResponse : IDisposable
    {
        HttpStatusCode StatusCode { get; }

        Stream GetResponseStream();

        string ContentEncoding { get; }

        void Close();
    }
}
