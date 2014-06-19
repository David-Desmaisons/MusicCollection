using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.WebServices.Amazon.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.WebServices.Amazon
{
    public partial class AWSECommerceServicePortTypeClient
    {
        public AWSECommerceServicePortTypeClient(IWebUserSettings iwsm)
            : this()
        {
            this.Endpoint.EndpointBehaviors.Add(new AmazonSigningEndpointBehavior(iwsm.AmazonaccessKeyId, iwsm.AmazonsecretKey));
        }
    }
}
