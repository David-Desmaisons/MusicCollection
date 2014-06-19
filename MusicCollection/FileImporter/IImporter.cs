using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Implementation;

namespace MusicCollection.FileImporter
{       
    
    internal enum ImportType {Import,Convertion,CDImport,UnRar,DBConnection,ItunesImport};

    internal interface IImporter
    {
        IImportContext Context { set; get; }

        IImporter Action(IEventListener iel);

        ImportType Type
        {
            get;
        }

    }
}
