using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MusicCollection.SettingsManagement;

namespace MusicCollection.Fundation
{
    //[Serializable]
    //public enum CompleteFileBehaviour
    //{
    //    [Description("Delete File")]
    //    Delete,

    //    [Description("No Action")]
    //    DoNothing,
    //};

    //[Serializable]
    //public enum ConvertFileBehaviour
    //{
    //    [Description("Music Collection Folder")]
    //    CopyInMananagedFolder,

    //    [Description("Original Folder")]
    //    SameFolder
    //};

    //[Serializable]
    //public enum PartialFileBehaviour
    //{
    //    [Description("Delete")]
    //    Delete,

    //    [Description("No Action")]
    //    DoNothing
    //};

    //[Serializable]
    //public enum BasicBehaviour
    //{
    //    [Description("Always")]
    //    Yes,

    //    [Description("Ask")]
    //    AskEndUser,

    //    [Description("Never")]
    //    No
    //};

    //public interface IRarFileManagement 
    //{
    //    CompleteFileBehaviour RarZipFileAfterSuccessfullExtract { get; set; }

    //    CompleteFileBehaviour RarZipFileAfterFailedExtract { get; set; }

    //    ConvertFileBehaviour RarExctractManagement { get; set; }

    //    string[] RarPasswords { get; set; }
    //}


    //public interface ICollectionFileManagement 
    //{
    //    bool ExportCollectionFiles { get; set; }

    //    string DirForPermanentCollection { get; set; }

    //    BasicBehaviour DeleteRemovedFile { get; set; }
    //}


    //public interface IWebServicesSettings
    //{
    //    string FreedbServer { get; set; }

    //    List<string> FreedbServers { get; }  
        
    //    bool AmazonActivated { get; set; }

    //    int DiscogsTimeOut { get; set; }

    //    bool DiscogsActivated { get; set; }

    //    bool IsDiscogImageActivated { get; }

    //    string ComputeLinkForDiscogsOAuthAuthorization();

    //    bool AuthorizeDiscogsPin(string iPin);

    //    IDictionary<string, string> GetPrivateKeys();

    //    bool ImportPrivateKeys(IDictionary<string, string> ikeys, bool iForce=false);
    //}


    //public interface IEmbeddedMusicSettings 
    //{
    //    double ImageSizeMoLimit { get; set; }

    //    uint ImageNumber { get; set; }

    //    bool ImageNumberLimit { get; set; }
    //}


   

    public interface IMusicSettings
    {
        IMusicImporterExporterUserSettings MusicImporterExporter { get; }

        //IUIGridManagement GridManagement { get; }

        //IRarFileManagement RarFileManagement { get; }
        IUnrarUserSettings RarFileManagement { get; }

        //ICollectionFileManagement CollectionFileManagement { get; }

        IMaturityUserSettings CollectionFileSettings { get; }

        IImageFormatManagerUserSettings ImageFormatManagerUserSettings { get; }

        IWebUserSettings WebUserSettings { get; }

        IConverterUserSettings ConverterUserSettings { get; }

        IAparencyUserSettings AparencyUserSettings { get; }

        IiTunesUserSettings iTunesSetting { get;}

        IPathUserSettings PathUserSettings { get; }

        IEmailInformationSettings EmailInformationSettings { get; }

        void SessionEnds();

        //ConvertFileBehaviour FileCreatedByConvertion { get; set; }

        //PartialFileBehaviour SourceFileUsedForConvertion { get; set; }

        //PartialFileBehaviour ConvertedFileExtractedFromRar { get; set; }

        //BasicBehaviour ImportBrokenItunesTrack { get; set; }

        //int DisplaySizer { get; set; }

        //AlbumPresenter PresenterMode { get; set; }

        //string PathRar { get; set; }

        //string PathCusto { get; set; }

        //string PathExport { get; set; }

        //string PathFolder { get; set; }
    }

    public static class IMusicSettingsExtension
    {
        public static IUIGridManagement GetIUIGridManagement(this  IMusicSettings iIMusicSettings )
        {
            return new UIGridManagement(iIMusicSettings);
        }
    }
}
