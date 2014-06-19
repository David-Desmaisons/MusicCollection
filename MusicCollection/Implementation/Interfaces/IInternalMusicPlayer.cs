using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    internal interface IInternalMusicPlayer : IMusicPlayer
    {
        void OnLockEvent(object sender, ObjectStateChangeArgs e);

    }
}
