using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Implementation;
using System.Threading;

namespace MusicCollection.FileImporter
{
    internal class DataBaseImporter : IImporter
    {
        private Nullable<bool> _CleanOnOpen;
        internal DataBaseImporter(Nullable<bool> iCleanOnOpen)
        {
            _CleanOnOpen = iCleanOnOpen;
        }

        IImportContext _IIC;

        IImportContext IImporter.Context { set { _IIC = value; } get { return _IIC; } }

        IImporter IImporter.Action(IEventListener iel, CancellationToken iCancellationToken)
        {
            _IIC.LoadAllFromDB(_CleanOnOpen);
            return null;
        }

        ImportType IImporter.Type
        {
            get { return ImportType.DBConnection; }
        }

    }
}
