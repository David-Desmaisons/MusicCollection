using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IMaturityUserSettings
    {
        bool ExportCollectionFiles { get; set; }

        string DirForPermanentCollection { get; set; }

        BasicBehaviour DeleteRemovedFile { get; set; }
    }
}
