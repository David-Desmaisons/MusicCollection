using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IConverterUserSettings
    {
        ConvertFileBehaviour FileCreatedByConvertion { get; set; }

        PartialFileBehaviour SourceFileUsedForConvertion { get; set; }

        PartialFileBehaviour ConvertedFileExtractedFromRar { get; set; }

        string BassUser{get;}

        string BassPassword{get;}
    }
}
