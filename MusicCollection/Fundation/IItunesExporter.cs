using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IItunesExporter : IMusicExporter
    {
        void Synchronize(bool DeleteBrokenItunes);

       // void ExportToItunes(IEnumerable<IAlbum> Albums,bool ExporttoiPod);

        bool Exporttoipod
        {
            set;
            get;
        }

    }
}
