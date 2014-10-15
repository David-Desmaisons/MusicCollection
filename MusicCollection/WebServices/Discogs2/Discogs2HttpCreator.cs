using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MusicCollection.DataExchange;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.ToolBox.Web;


namespace MusicCollection.WebServices.Discogs2
{
    internal abstract class Discogs2HttpCreator
    {
        private static string _UA;
 
        private Nullable<int> _TimeOut = null;

        protected Discogs2HttpCreator( string ua, Nullable<int>  iTimeOut = null)
        {
            _UA = ua;
            _TimeOut = iTimeOut;
        }

        private const string _Base = "http://api.discogs.com/";
    
        protected abstract string Path { get; }

        private IHttpWebRequest Mature(IHttpWebRequest request)
        {
            request.Headers["Accept-Encoding"] = "gzip";
            if (string.IsNullOrEmpty(request.UserAgent))
                request.UserAgent = _UA;
            if (_TimeOut!=null)
                request.Timeout = _TimeOut.Value;
            return request;
        }

        internal IHttpWebRequest FromUrl(string iurl)
        {
            return Mature(InternetProvider.InternetHelper.CreateHttpRequest(iurl));
        }

        internal IHttpWebRequest BuildRequest(IOAuthManager iIOAuthManager)
        {
            return Mature(InternetProvider.InternetHelper.CreateAuthentified(Buildstring, _UA, iIOAuthManager));
        }

        private string Buildstring
        {
            get { return string.Format("{0}{1}?{2}", _Base, Path, Arguments); }
        }

        private string Arguments
        {
            get
            {
                if (_Arguments == null)
                    return string.Empty;

                return string.Join("&", _Arguments.Select(a=> (a.Value == null) ? a.Key : string.Format("{0}={1}", a.Key, a.Value)));
            }
        }

        private Dictionary<string, string> _Arguments;

        internal Discogs2HttpCreator AddArgument(string argumentName, string argumentvalue = null)
        {
            if (_Arguments == null)
                _Arguments = new Dictionary<string, string>();

            _Arguments.Add(argumentName, (argumentvalue == null) ? null : Uri.EscapeDataString(argumentvalue.NormalizeSpace()));
            return this;
        }
     }

    internal class Discogs2HttpRequestCreator : Discogs2HttpCreator
    {
        private IOAuthManager _AuthManager;
        internal Discogs2HttpRequestCreator(string ua, IOAuthManager iIOAuthManager, Nullable<int> iTimeOut = null)
            : base(ua, iTimeOut)
        {
            _AuthManager = iIOAuthManager;
        }

        protected string _Search = @"database/search";
        protected override string Path
        {
            get { return _Search; }
        }


        private Discogs2HttpRequestCreator AddTypeSearch(string itype)
        {
            return AddArgument("type", itype) as Discogs2HttpRequestCreator;
        }

        private Discogs2HttpRequestCreator LookForTitle()
        {
            return AddArgument("title") as Discogs2HttpRequestCreator;
        }

        private Discogs2HttpRequestCreator LookFor(string iSearch)
        {
            return AddArgument("q", iSearch) as Discogs2HttpRequestCreator;
        }

        internal IHttpWebRequest GetSearchRequest(IAlbumDescriptor iad)
        {
            return LookFor(string.Format("{0}+{1}", iad.Artist.Replace(',', ' ').Replace('&', ' '), iad.Name.Replace(',', ' ').Replace('&', ' ')))
                    .AddTypeSearch("release").LookForTitle().BuildRequest(_AuthManager);
        }
    }
}
