using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    //public enum MusicImportExportType
    //{
    //    [Description("Import from Itunes")]
    //    iTunes,

    //    [Description("Import from CD")]
    //    CD,

    //    [Description("Import Directory")]
    //    Directory,

    //    [Description("Import Rar/zip")]
    //    Compressed,

    //    [Description("Import from WindowsPhone")]
    //    WindowsPhone,

    //    [Description("Import from XML")]
    //    Custo
    //}

    public enum MusicImportType
    {
        [Description("Import from Itunes")]
        iTunes,

        [Description("Import from CD")]
        CD,

        [Description("Import Directory")]
        Directory,

        [Description("Import Rar/zip")]
        Compressed,

        [Description("Import from Mcc")]
        Custo
    }

    public enum MusicExportType
    {
        [Description("Export to Itunes")]
        iTunes,

        [Description("Export to Directory")]
        Directory,

        [Description("Export to 7z")]
        Compressed,

        [Description("Export to WindowsPhone")]
        WindowsPhone,

        [Description("Export to Mcc")]
        Custo
    }

    public interface IMusicImporterBuilder
    {
        MusicImportType Type { get; }

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
