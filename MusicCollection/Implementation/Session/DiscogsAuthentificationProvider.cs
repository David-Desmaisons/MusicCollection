using MusicCollection.Fundation;
using MusicCollection.WebServices;
using MusicCollection.WebServices.Discogs2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    internal class DiscogsAuthentificationProvider : IDiscogsAuthentificationProvider
    {
        private IWebUserSettings _IWebUserSettings;
        private DiscogsOAuthBuilder _DiscogsOAuthBuilder;
        public DiscogsAuthentificationProvider(IWebUserSettings iIWebUserSettings)
        {
            _IWebUserSettings = iIWebUserSettings;
        }

        public bool IsDiscogImageActivated
        {
            get { return !string.IsNullOrEmpty(_IWebUserSettings.DiscogsToken); }
        }

        public string ComputeLinkForDiscogsOAuthAuthorization()
        {
            if (IsDiscogImageActivated)
                return null;

            _DiscogsOAuthBuilder = GetDiscogsOAuthBuilder();

            //_DiscogsOAuthBuilder = new DiscogsOAuthBuilder(Settings.Default.DiscogsConsumerKey, Settings.Default.DiscogsConsumerSecret);
            return _DiscogsOAuthBuilder.ComputeUrlForAuthorize();
        }

        public bool AuthorizeDiscogsPin(string iPin)
        {
            if (IsDiscogImageActivated)
                return true;

            if (_DiscogsOAuthBuilder == null)
                return false;

            _DiscogsOAuthBuilder.FinalizeFromPin(iPin);

            _IWebUserSettings.DiscogsToken = _DiscogsOAuthBuilder.Token;
            _IWebUserSettings.DiscogsTokenSecret = _DiscogsOAuthBuilder.TokenSecret;

            _IWebUserSettings.Save();

            return true;
        }


        public IDictionary<string, string> GetPrivateKeys()
        {
            if (!IsDiscogImageActivated)
                return null;

            IDictionary<string, string> res = new Dictionary<string, string>();
            res.Add("DiscogsToken", _IWebUserSettings.DiscogsToken);
            res.Add("DiscogsTokenSecret", _IWebUserSettings.DiscogsTokenSecret);

            return res;
        }

        public bool ImportPrivateKeys(IDictionary<string, string> ikeys, bool iForce = false)
        {
            if ((ikeys == null))
                return false;

            if (!iForce && (IsDiscogImageActivated))
                return false;

            try
            {
                _IWebUserSettings.DiscogsToken = ikeys["DiscogsToken"];
                _IWebUserSettings.DiscogsTokenSecret = ikeys["DiscogsTokenSecret"];
                _IWebUserSettings.Save();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Impossible to import keys {0}", ex));
                return false;
            }

            return true;
        }

        public OAuthManager GetDiscogsOAuthManager()
        {
            if (!string.IsNullOrWhiteSpace(_IWebUserSettings.DiscogsToken) && (!string.IsNullOrWhiteSpace(_IWebUserSettings.DiscogsTokenSecret)))
            {
                return new OAuthManager(_IWebUserSettings.DiscogsConsumerKey,
                        _IWebUserSettings.DiscogsConsumerSecret, _IWebUserSettings.DiscogsToken,
                        _IWebUserSettings.DiscogsTokenSecret);
            }

            return null;
        }

        public DiscogsOAuthBuilder GetDiscogsOAuthBuilder()
        {
            return new DiscogsOAuthBuilder(_IWebUserSettings.DiscogsConsumerKey, _IWebUserSettings.DiscogsConsumerSecret);
        }

    }
}
