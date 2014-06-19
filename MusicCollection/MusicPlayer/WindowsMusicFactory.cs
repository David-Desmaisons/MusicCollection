using MusicCollection.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.MusicPlayer
{
    public class WindowsMusicFactory : IMusicFactory
    {
        public IInternalPlayer GetInternalPlayer()
        {
            return new WindowsInternalPlayer();
        }
    }
}
