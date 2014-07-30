using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
       }

        public IEnumerable<Infra.Match<DataExchange.AlbumDescriptor>> Search(Fundation.IWebQuery query, System.Threading.CancellationToken iCancellation)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
