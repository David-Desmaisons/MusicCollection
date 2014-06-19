using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    [Serializable]
    internal class ManualConverterSettings : IConverterUserSettings
    {
        public ConvertFileBehaviour FileCreatedByConvertion { get; set; }

        public PartialFileBehaviour SourceFileUsedForConvertion { get; set; }

        public PartialFileBehaviour ConvertedFileExtractedFromRar { get; set; }

        public string BassUser { get; set; }

        public string BassPassword { get; set; }
    }
}
