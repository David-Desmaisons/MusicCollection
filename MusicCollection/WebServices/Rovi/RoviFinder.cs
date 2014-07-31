using MusicCollection.DataExchange;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicCollection.WebServices.Rovi
{
    [WebServicesInfoProvider(WebProvider.Rovi)]
    internal class RoviFinder : WebFinderAdapter, IInternerInformationProvider
    {
        private string _SharedSecret;
        private string _SearchAPIKey;
        private IWebUserSettings _IWebUserSettings;
        private RoviClient _RoviClient;
  

        public RoviFinder(IWebUserSettings iwsm)
        {
            _IWebUserSettings = iwsm;
            _SearchAPIKey = iwsm.RoviSearchAPIKey;
            _SharedSecret = iwsm.RoviSharedSecret;

            _RoviClient = new RoviClient(_SearchAPIKey, _SharedSecret);
       }

        public IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellation)
        {
           if (query.Type== QueryType.FromCD)
               return Enumerable.Empty<Match<AlbumDescriptor>>();

           dynamic res = _RoviClient.Limit(query.MaxResult).SearchAlbum(query.AlbumDescriptor).GetAnswer();
           return Enumerable.Empty<Match<AlbumDescriptor>>();
        }

        public void Dispose()
        {
        }
    }
}
