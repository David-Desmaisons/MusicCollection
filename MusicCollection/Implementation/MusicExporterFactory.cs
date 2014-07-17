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

        public IMusicExporter FromType(MusicExportType type)
        {
           
            switch (type)
            {
                case MusicExportType.iTunes:
                    return new ITunesExporter(_MSI);

                case MusicExportType.WindowsPhone:
                    return new WindowsPhoneExporter(_MSI);

                case MusicExportType.Compressed:
                case MusicExportType.Custo:
                case MusicExportType.Directory:
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
