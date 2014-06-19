using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal enum ProviderType { Freedb, MusicBrainz };

    internal interface ICompleteCDInfos : ICDInfos
    {
        int? Year { get; }

        ProviderType? Type { get; }

        IEnumerable<IOpenDurationTrackDescriptor> Tracks { get; }
    }
}
