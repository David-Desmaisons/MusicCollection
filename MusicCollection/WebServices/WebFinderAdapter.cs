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
        public event EventHandler<InternetFailedArgs> OnInternetError;

        public void OnStatusCodeKO(Nullable<HttpStatusCode> code)
        {
            OnEvent(InternetFailedArgs.WebServiceDown(null, code));
        }

        public void OnUnExpectedUnreadableResult(string AdditionalInfo = null)
        {
            OnEvent(InternetFailedArgs.WebServiceDown(AdditionalInfo));
        }

        public void OnWebExeption(Exception e)
        {
            OnEvent(InternetFailedArgs.InternetDown(e));
        }

        protected abstract void OnEvent(InternetFailedArgs ifa);


        protected void FireEvent(InternetFailedArgs ifa)
        {
            EventHandler<InternetFailedArgs> InternetError = OnInternetError;
            if (InternetError != null)
                InternetError(this, ifa);
        }
    }

    internal class WebFinderAdapter : WebFinderAdapterBase
    {

        protected override void OnEvent(InternetFailedArgs ifa)
        {
            FireEvent(ifa);
        }
    }
}
