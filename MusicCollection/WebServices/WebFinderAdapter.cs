using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MusicCollection.Fundation;

namespace MusicCollection.WebServices
{
    internal abstract class WebFinderAdapterBase
    {
        public event EventHandler<InternetFailed> OnInternetError;

        public void OnStatusCodeKO(Nullable<HttpStatusCode> code)
        {
            OnEvent(InternetFailed.WebServiceDown(null, code));
        }

        public void OnUnExpectedUnreadableResult(string AdditionalInfo = null)
        {
            OnEvent(InternetFailed.WebServiceDown(AdditionalInfo));
        }

        public void OnWebExeption(Exception e)
        {
            OnEvent(InternetFailed.InternetDown(e));
        }

        protected abstract void OnEvent(InternetFailed ifa);


        protected void FireEvent(InternetFailed ifa)
        {
            EventHandler<InternetFailed> InternetError = OnInternetError;
            if (InternetError != null)
                InternetError(this, ifa);
        }
    }

    internal class WebFinderAdapter : WebFinderAdapterBase
    {

        protected override void OnEvent(InternetFailed ifa)
        {
            FireEvent(ifa);
        }
    }
}
