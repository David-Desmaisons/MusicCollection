using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Net.NetworkInformation;

namespace MusicCollection.ToolBox.Web
{
    internal class InternetProvider : IInternetProvider
    {
        public IHttpWebRequest CreateHttpRequest(string iUrl)
        {
            return new HttpWebRequestWrapper((HttpWebRequest)WebRequest.Create(iUrl));
        }

        public IHttpWebRequest CreateHttpRequest(Uri iUrl)
        {
            return new HttpWebRequestWrapper((HttpWebRequest)WebRequest.Create(iUrl));
        }

        static InternetProvider()
        {
            InternetHelper = new InternetProvider();
        }

        static public IInternetProvider InternetHelper
        {
            get;
            set;
        }

        public bool GetIsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
