using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using System.Threading;

namespace MusicCollection.FileImporter
{       
    
    internal enum ImportType {Import,Convertion,CDImport,UnRar,DBConnection,ItunesImport};

    internal interface IImporter
    {
        IImportContext Context { set; get; }

        IImporter Action(IEventListener iel, CancellationToken iCancellationToken);

        ImportType Type
        {
            get;
        }

    }
}
