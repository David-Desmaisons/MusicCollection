using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IDiscInformationProvider
    {
        event EventHandler<EventArgs> OnCompleted;
        event EventHandler<ImportExportErrorEventArgs> OnError;

        WebMatch<IFullAlbumDescriptor> FoundCDInfo { get; }

        void Compute(bool Sync);
    }
}
