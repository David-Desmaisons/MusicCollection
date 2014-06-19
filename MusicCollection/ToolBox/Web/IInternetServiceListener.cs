using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MusicCollection.ToolBox.Web
{
    public interface IInternetServiceListener
    {
        void OnStatusCodeKO(Nullable<HttpStatusCode> code=null);

        void OnUnExpectedUnreadableResult(string AdditionalMessage=null);

        void OnWebExeption(Exception e);

    }
}
