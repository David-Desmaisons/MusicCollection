using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using System.Threading;

namespace MusicCollection.FileConverter
{
    internal interface IMusicFileConverter:IDisposable
    {
        bool ConvertTomp3(bool deleteIfAlreadyExists = false, bool deleteIfSucceed = false);

        string ConvertName { get; }

        bool TagetFileAlreadyExist { get; }
    }

    internal class TrackConverted
    {
        internal TrackConverted(ITrackDescriptor iTrack, bool iOK)
        {
            Track = iTrack;
            OK = iOK;
        }

        internal ITrackDescriptor Track { get; private set; }

        internal bool OK { get; private set; }
    }

    internal interface IMusicfilesConverter : IDisposable
    {
        bool ConvertTomp3(IProgress<TrackConverted> iprogress, CancellationToken iCancellationToken, bool deleteIfSucceed = false);

    }
}
