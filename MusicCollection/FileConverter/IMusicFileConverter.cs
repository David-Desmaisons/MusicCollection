using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using MusicCollection.Fundation;

namespace MusicCollection.FileConverter
{
    internal interface IMusicFileConverter:IDisposable
    {
        bool ConvertTomp3(bool deleteIfAlreadyExists = false, bool deleteIfSucceed = false);

        string ConvertName
        { get; }

        bool TagetFileAlreadyExist { get; }
    }

    internal class TrackConvertedArgs : EventArgs
    {
        internal ITrackDescriptor Track
        { get; private set; }

        internal bool OK
        { get; private set; }

        internal TrackConvertedArgs(ITrackDescriptor iTrack, bool iOK)
            : base()
        {
            Track = iTrack;
            OK = iOK;
        }

    }

    //internal delegate void TrackConvertedArgs_EventHandler(object sender, TrackConvertedArgs e);

    internal interface IMusicfilesConverter : IDisposable
    {
        bool ConvertTomp3(bool deleteIfSucceed = false);

        event EventHandler<TrackConvertedArgs> TrackHandled;

    }
}
