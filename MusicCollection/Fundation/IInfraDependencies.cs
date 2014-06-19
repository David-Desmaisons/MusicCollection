using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra.File;
using MusicCollection.ToolBox.ZipTools;
using MusicCollection.MusicPlayer;
using MusicCollection.Implementation;

namespace MusicCollection.Fundation
{
    public interface IInfraDependencies 
    {
        IZipper Zip { get; }

        IFileTools File { get; }

        IMusicFactory MusicFactory { get; }

      //  IInternalPlayer GetInternalPlayer();
    }
}
