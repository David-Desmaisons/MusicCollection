using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    internal interface IInternalTrack : ITrack, IObjectStateCycle
    {
        bool IsBroken { get; }

        void AddPlayer(IInternalMusicPlayer pl);

        void RemovePlayer(IInternalMusicPlayer pl);
    }
}
