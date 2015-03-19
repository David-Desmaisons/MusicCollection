using MusicCollection.WebServices;
using MusicCollection.WebServices.Discogs2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IWebUserSettings
    {
        List<string> FreedbServers { get; }
        string FreedbServer { get; set; }
        int DiscogsTimeOut { get; set; }
        bool DiscogsActivated { get; set; }
        bool AmazonActivated { get; set; }
        string DiscogsConsumerKey {get;}
        string DiscogsConsumerSecret {get;}
        string DiscogsToken { get; set; }
        string DiscogsTokenSecret { get; set; }
        string AmazonaccessKeyId {get; }
        string AmazonsecretKey {get; }
        string GraceNoteAplicationID { get; }
        string GraceNoteDeviceID { get; set; }
        string RoviSearchAPIKey {get; }
        string RoviSharedSecret {get; }
        string MuzicBrainzUser { get; }
        string MuzicBrainzPassword { get; }
        void Save();
    }

    public static class IWebUserSettingsExetender
    {

        public static IDiscogsAuthentificationProvider GetDiscogsAutentificator(this IWebUserSettings @this)
        {
            return new DiscogsAuthentificationProvider(@this);
        }

    }
}
