using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualMaturitySettings : IMaturityUserSettings
    {
        public bool ExportCollectionFiles {get;set;}

        public string DirForPermanentCollection { get; set; }

        public BasicBehaviour DeleteRemovedFile { get; set; }
    }
}
