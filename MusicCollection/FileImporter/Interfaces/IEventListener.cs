using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.FileImporter
{
    internal interface IEventListener : IImportExportProgress
    {
        void OnFactorisableError<T>(string message) where T : ImportExportErrorEventListItemsArgs;
    }
}
