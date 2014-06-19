using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface ICDInfoHandler
    {
        bool IsReady { get; }

        int TrackNumbers { get; }

        IDiscIDs IDs { get; }

        string Driver { get; }

        TimeSpan Duration(int traknumber);
    }
}
