using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public enum MusicImportExportType
    {
        iTunes,
        CD,
        Directory,
        Compressed,
        WindowsPhone,
        Custo
    }

    public interface IMusicImporterBuilder
    {
        MusicImportExportType Type { get; }

        AlbumMaturity DefaultAlbumMaturity { get; set; }

        bool IsValid { get; }

        IMusicImporter BuildImporter();
    }

    public interface ICDImporterBuilder : IMusicImporterBuilder
    {
        bool OpenCDDoorOnComplete { get; set; }
    }

    public interface IiTunesImporterBuilder : IMusicImporterBuilder
    {
        string ItunesDirectory { get; set; }

        bool? ImportBrokenTracks { get; set; }
    }

    public interface IFilesImporterBuilder : IMusicImporterBuilder
    {
        string FileExtensions { get;}

        string DefaultFolder { get; set; }
       
        string[] Files { get; set; }
    }

    public interface ICustoFilesImporterBuilder : IFilesImporterBuilder
    {
        bool ImportAllMetaData { get; set; }
    }

    public interface IDirectoryImporterBuilder : IMusicImporterBuilder
    {
        string Directory { get; set; }
    }
}
