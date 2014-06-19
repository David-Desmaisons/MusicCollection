using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Implementation
{
    internal interface IOpenDurationTrackDescriptor : ITrackDescriptor
    {
        void SetDuration(TimeSpan Duration);
        void SetPath(string Path);
    }
}
