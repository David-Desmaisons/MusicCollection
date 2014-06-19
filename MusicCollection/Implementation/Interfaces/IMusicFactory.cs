using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Fundation;
using MusicCollection.MusicPlayer;

namespace MusicCollection.Implementation
{
    public interface IMusicFactory 
        //: IMusicSession
    {
        IInternalPlayer GetInternalPlayer();
    }
}
