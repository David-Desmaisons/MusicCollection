using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation.Session;

namespace MusicCollection.Fundation
{
    public static class MusicSession
    {
        public static IMusicSession GetSession(IMainWindowHwndProvider imp)
        {
            return Implementation.MusicSessionImpl.GetSession(SessionBuilder.FromSettings(),  imp);
        }
    }
}
