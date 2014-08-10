using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;

namespace MusicCollection.FileImporter
{
    internal interface IRarDescompactor:IDisposable
    {
        bool Extract(IEventListener Listener);

        List<String> DescompactedFiles { set; }

        List<String> ArchiveNames { get; }

        IImportHelper Helper { get; }
    }

    internal interface IMccDescompactor : IDisposable
    {
        bool Extract(IEventListener Listener);

        List<String> DescompactedFiles { get; }

        string RootXML { get; }

        IDictionary<string, string> Rerooter { get; }
    }
}
