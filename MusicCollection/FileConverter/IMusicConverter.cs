using MusicCollection.DataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.FileConverter
{
    internal interface IMusicConverter : IDisposable
    {
        int GetDriverNumber(char DriverName);

        double GetFileLengthInSeconds(string iFileName);

        IMusicfilesConverter GetMusicConverter(string FileName, List<TrackDescriptor> Cues,
                                string Outdir, string temp);

        IMusicFileConverter GetMusicConverter(string FileName, string Outdir, string temp);

        IMusicfilesConverter GetCDMusicConverter(AlbumDescriptor ICDInfo, string Outdir,
                                bool BigBrute = false, int Cdnumber = 0);
    }
}
