using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using System.Threading;

namespace MusicCollection.FileImporter
{
    internal interface IRarDescompactor:IDisposable
    {
        bool Extract(IEventListener Listener, CancellationToken iCancellationToken);

        List<String> DescompactedFiles { set; }

        List<String> ArchiveNames { get; }

        IImportHelper Helper { get; }
    }

    internal interface IMccDescompactor : IDisposable
    {
        bool Extract(IEventListener Listener, CancellationToken iCancellationToken);

        List<String> DescompactedFiles { get; }

        string RootXML { get; }

        IDictionary<string, string> Rerooter { get; }
    }
}
