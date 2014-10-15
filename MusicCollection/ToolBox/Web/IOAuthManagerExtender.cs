using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox.Web
{
    public static class IOAuthManagerExtender
    {

        public static IHttpWebRequest CreateAuthentified(this  IInternetProvider @this, string myuri, string UserAgent, IOAuthManager iOAuthManager)
        {
            IHttpWebRequest request = @this.CreateHttpRequest(myuri);
            request.UserAgent = UserAgent;
            request.Headers.Add("Authorization", iOAuthManager.GenerateAuthzHeader(myuri, "GET"));
            request.PreAuthenticate = true;
            return request;
        }
    }
}
