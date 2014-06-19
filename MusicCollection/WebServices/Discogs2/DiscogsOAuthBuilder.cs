using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.WebServices.Discogs2
{
    public class DiscogsOAuthBuilder
    {
        private const string _RequestToken=@"http://api.discogs.com/oauth/request_token";
        private const string _Authorize = @"http://www.discogs.com/oauth/authorize?oauth_token=";
        private const string _AcessToken = @"http://api.discogs.com/oauth/access_token";

        private string _ConsumerKey;
        private string _ConsumerSecret;
        private OAuthManager _OAuthManager;

        public DiscogsOAuthBuilder(string iConsumerKey,string iConsumerSecret)
        {
            _ConsumerKey=iConsumerKey;
            _ConsumerSecret=iConsumerSecret;
            _OAuthManager = new OAuthManager(iConsumerKey, iConsumerSecret);
        }

        public string ComputeUrlForAuthorize()
        {
            try 
            { 
                _OAuthManager.AcquireRequestToken(_RequestToken, "POST");
                return _Authorize + _OAuthManager["token"];
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Problem during Discogs url building");
                Trace.WriteLine(string.Format("Corresponding exception: {0}",ex));
                return null;
            }
        }

        public void FinalizeFromPin(string iPin)
        {
            _OAuthManager.AcquireAccessToken(_AcessToken, "POST", iPin);
        }

        public string Token
        {
            get { return _OAuthManager["token"]; }
        }

        public string TokenSecret
        {
            get { return _OAuthManager["token_secret"]; }
        }

        public OAuthManager OAuthManager
        {
            get { return _OAuthManager; }
        }
    }
}
