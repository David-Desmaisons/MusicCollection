using System;
namespace MusicCollection.Fundation
{
    public interface IDiscogsAuthentificationProvider
    {
        bool AuthorizeDiscogsPin(string iPin);
        string ComputeLinkForDiscogsOAuthAuthorization();
        MusicCollection.WebServices.Discogs2.DiscogsOAuthBuilder GetDiscogsOAuthBuilder();
        MusicCollection.WebServices.OAuthManager GetDiscogsOAuthManager();
        System.Collections.Generic.IDictionary<string, string> GetPrivateKeys();
        bool ImportPrivateKeys(System.Collections.Generic.IDictionary<string, string> ikeys, bool iForce = false);
        bool IsDiscogImageActivated { get; }
    }
}
