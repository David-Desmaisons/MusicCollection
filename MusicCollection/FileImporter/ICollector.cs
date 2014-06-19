using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.FileImporter
{
    internal interface ICollector
    {
        ImporterConverterAbstract Importer
        {
            get;
        }

        bool IsSealed
        {
            get;
        }

        ICollector Merge(ICollector coll);

        bool IsMergeable(ICollector coll);
    }
}
