using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Itunes;
using MusicCollection.Utilies;
using MusicCollection.WindowsPhone;

namespace MusicCollection.Implementation
{
    internal class MusicExporterFactory:IMusicExporterFactory
    {
        private MusicSessionImpl _MSI;

        internal MusicExporterFactory(MusicSessionImpl MSI)
        {
            _MSI = MSI;
        }

        public IMusicExporter FromType(MusicImportExportType type)
        {
           
            switch (type)
            {
                case MusicImportExportType.iTunes:
                    return new ITunesExporter(_MSI);

                case MusicImportExportType.WindowsPhone:
                    return new WindowsPhoneExporter(_MSI);

                case MusicImportExportType.Compressed:
                case MusicImportExportType.Custo:
                case MusicImportExportType.Directory:
                        //IMusicCompleteFileExporter res = null;
                        //res = new MusicExporter(_MSI,type);
                        return new MusicExporter(_MSI, type);
            }

            return null;
        }

        public IMusicFileExporter GetMover()
        {
            return new AlbumMover(_MSI);
        }

    
    }
}
