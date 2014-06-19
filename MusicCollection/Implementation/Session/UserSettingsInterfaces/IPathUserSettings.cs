using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IPathUserSettings
    {
        string PathRar { get; set; }

        string PathCusto { get; set; }

        string PathExport { get; set; }

        string PathFolder { get; set; }
    }
}
