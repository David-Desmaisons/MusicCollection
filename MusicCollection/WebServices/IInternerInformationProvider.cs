using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using System.Threading;

namespace MusicCollection.WebServices
{
    internal interface IInternerInformationProvider : IDisposable
    {
        IEnumerable<Match<AlbumDescriptor>> Search(IWebQuery query, CancellationToken iCancellation);

        event EventHandler<InternetFailedArgs> OnInternetError;    
    }
}
