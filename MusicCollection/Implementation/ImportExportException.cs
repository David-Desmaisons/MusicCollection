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
        private ImportExportError _Reason;
        internal ImportExportException(ImportExportError ireason)
        {
            _Reason = ireason;
        }

        public ImportExportError Error
        {
            get { return _Reason; }
        }

        static internal ImportExportException FromError(ImportExportError ireason)
        {
            return new ImportExportException(ireason);
        }
    }
}
