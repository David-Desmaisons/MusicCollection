using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.WebServices;
using MusicCollection.WebServices.Discogs2;
using MusicCollection.Fundation;

namespace MusicCollection.SettingsManagement
{
    //internal class WebServicesManagerImpl : IWebServicesManager
    //{
    //    public string FreedbServer { get; private set; }

    //    public int DiscogsTimeOut { get; private set; }

    //    public bool DiscogsActivated { get; private set; }

    //    public bool AmazonActivated { get; private set; }

    //    public OAuthManager DiscogsOAuthManager { get; private set; }

    //    public string Amazon_accessKeyId { get; private set; }

    //    public string Amazon_secretKey { get; private set; }

    //    private string _DiscogsConsumerKey;
    //    private string _DiscogsConsumerSecrets;

    //    private IWebUserSettings _IWebUserSettings;
    //    internal WebServicesManagerImpl( IWebUserSettings iIWebUserSettings)
    //        //string iFreedbServer, int iDiscogsTimeOut, bool iDiscogsActivated, bool iAmazonActivated,
    //        //string iDiscogsConsumerKey, string iDiscogsConsumerSecrets, 
    //        //string iDiscogsToken, string iDiscogsTokenSecret,
    //        //string iAmazon_accessKeyId, string iAmazon_secretKey)
    //    {
    //        FreedbServer = iFreedbServer;
    //        DiscogsTimeOut = iDiscogsTimeOut;
    //        DiscogsActivated = iDiscogsActivated;
    //        AmazonActivated = iAmazonActivated;
    //        Amazon_accessKeyId = iAmazon_accessKeyId;
    //        Amazon_secretKey = iAmazon_secretKey;
    //        _DiscogsConsumerKey = iDiscogsConsumerKey;
    //        _DiscogsConsumerSecrets = iDiscogsConsumerSecrets;

    //        if (!string.IsNullOrWhiteSpace(iDiscogsToken) && (!string.IsNullOrWhiteSpace(iDiscogsTokenSecret)))
    //        {
    //            DiscogsOAuthManager = new OAuthManager(_DiscogsConsumerKey, _DiscogsConsumerSecrets, iDiscogsToken, iDiscogsTokenSecret);
    //        }
    //    }

    //    public DiscogsOAuthBuilder GetDiscogsOAuthBuilder()
    //    {
    //        return new DiscogsOAuthBuilder(_DiscogsConsumerKey, _DiscogsConsumerSecrets); 
    //    }
    //}
}
