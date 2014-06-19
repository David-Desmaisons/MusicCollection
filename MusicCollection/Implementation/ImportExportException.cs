using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    [Serializable]
    internal class ImportExportException : Exception
    {
        private ImportExportErrorEventArgs _Reason;
        internal ImportExportException(ImportExportErrorEventArgs ireason)
        {
            _Reason = ireason;
        }

        public ImportExportErrorEventArgs Error
        {
            get { return _Reason; }
        }

        static internal ImportExportException FromError(ImportExportErrorEventArgs ireason)
        {
            return new ImportExportException(ireason);
        }
    }
}
