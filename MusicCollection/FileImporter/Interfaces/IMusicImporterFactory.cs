using MusicCollection.Fundation;
using MusicCollection.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.FileImporter
{
    internal interface IMusicImporterFactory
    {
        IMusicImporter GetITunesService(bool ImportBrokenTunes, string Directory, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetDBImporter();

        IMusicImporter GetFileService(IInternalMusicSession iconv, string Directory, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetRarImporter(string FileName, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetXMLImporter(IEnumerable<string> FileName, bool ImportAllMetaData, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetMultiRarImporter(IEnumerable<string> FileName, AlbumMaturity iDefaultMaturity);

        IMusicImporter GetCDImporter(AlbumMaturity iDefaultMaturity, bool iOpenCDDoorOnComplete);
    }
}
