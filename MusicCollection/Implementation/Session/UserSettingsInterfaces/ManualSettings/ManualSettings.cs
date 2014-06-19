using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.SettingsManagement;
using MusicCollection.Fundation;
using MusicCollection.Implementation.Session;

namespace MusicCollection.Implementation
{
    [Serializable]
    internal class ManualSettings : IMusicSettings
    {
        private IUnrarUserSettings _IUnrarUserSettings;
        public IUnrarUserSettings RarFileManagement
        {
            get { return _IUnrarUserSettings; }
        }

        private IWebUserSettings _IWebUserSettings;
        public IWebUserSettings WebUserSettings
        {
            get { return _IWebUserSettings; }
        }

        private IMaturityUserSettings _IMaturityUserSettings;
        public IMaturityUserSettings CollectionFileSettings
        {
            get { return _IMaturityUserSettings; }
        }

        private IEmailInformationSettings _IEmailInformationSettings;
        public IEmailInformationSettings EmailInformationSettings
        {
            get { return _IEmailInformationSettings; }
        }

        public IConverterUserSettings ConverterUserSettings
        {
            get;
            set;
        }

        private IImageFormatManagerUserSettings _IImageFormatManagerUserSettings;
        public IImageFormatManagerUserSettings ImageFormatManagerUserSettings
        {
            get { return _IImageFormatManagerUserSettings; }
        }

        private IAparencyUserSettings _IAparencyUserSettings;
        public IAparencyUserSettings AparencyUserSettings
        {
            get { return _IAparencyUserSettings; }
        }

        private IMusicImporterExporterUserSettings _IMusicImporterExporterUser;
        public IMusicImporterExporterUserSettings MusicImporterExporter
        {
            get { return _IMusicImporterExporterUser; }
        }

        private IiTunesUserSettings _IiTunesUserSettings;
        public IiTunesUserSettings iTunesSetting
        {
            get { return _IiTunesUserSettings; }
        }

        private IPathUserSettings _IPathUserSettings;
        public IPathUserSettings PathUserSettings
        {
            get { return _IPathUserSettings; }
        }

        internal ManualSettings()
        {
            //FileCreatedByConvertion = ConvertFileBehaviour.CopyInMananagedFolder;
            //SourceFileUsedForConvertion = PartialFileBehaviour.DoNothing;
            //ConvertedFileExtractedFromRar = PartialFileBehaviour.Delete;
            //DeleteRemovedFile = BasicBehaviour.AskEndUser;

            _IMusicImporterExporterUser = new ManualMusicImporterExporterSettings();
            _IiTunesUserSettings = new ManualIiTunesUserSettings();
            _IPathUserSettings = new ManualPathUserSettings();

            _IImageFormatManagerUserSettings = new ManualmageFormatManagerSettings();
            _IImageFormatManagerUserSettings.ImageSizeMoLimit = 1;
            _IImageFormatManagerUserSettings.ImageNumberLimit = true;
            _IImageFormatManagerUserSettings.ImageNumber = 2;
            //DirForPermanentCollection = string.Empty;
            //ExportCollectionFiles = false;
            //FreedServer = "us.freedb.org:80";
            //DiscogsActivated = true;
            //AmazonActivated = true;

            _IEmailInformationSettings = new ManualEmailInformationSettings();

            ConverterUserSettings = new ManualConverterSettings();
            ConverterUserSettings.FileCreatedByConvertion = ConvertFileBehaviour.CopyInMananagedFolder;
            ConverterUserSettings.SourceFileUsedForConvertion = PartialFileBehaviour.DoNothing;
            ConverterUserSettings.ConvertedFileExtractedFromRar = PartialFileBehaviour.Delete;
    

            _IMaturityUserSettings = new ManualMaturitySettings();
            _IMaturityUserSettings.DirForPermanentCollection = string.Empty;
            _IMaturityUserSettings.ExportCollectionFiles = false;

            _IUnrarUserSettings = new ManualUnrarUserSettings();
            _IUnrarUserSettings.RarZipFileAfterSuccessfullExtract = CompleteFileBehaviour.DoNothing;
            _IUnrarUserSettings.RarZipFileAfterFailedExtract = CompleteFileBehaviour.DoNothing;
            _IUnrarUserSettings.RarExctractManagement = ConvertFileBehaviour.CopyInMananagedFolder;
            _IUnrarUserSettings.AddUseRarPasswordToList = true;


            _IWebUserSettings = new ManualWebSettings();
            _IWebUserSettings.FreedbServer = "us.freedb.org:80";
            _IWebUserSettings.DiscogsActivated = true;
            _IWebUserSettings.AmazonActivated = true;

            _IAparencyUserSettings = new ManualAparencySettings();
        }

       

       

        //internal CompleteFileBehaviour RarZipFileAfterSuccessfullExtract { get; set; }
        //internal CompleteFileBehaviour RarZipFileAfterFailedExtract { get; set; }
        //internal ConvertFileBehaviour RarExctractManagement { get; set; }
        //internal bool AddUseRarPasswordToList { get; set; }
        //internal string FreedServer { get; set; }
        //internal bool DiscogsActivated { get; set; }
        //internal bool AmazonActivated { get; set; }

        //internal string EmailAdress { get; set; }
        //internal string Password { get; set; }
        //internal string EmailReceptor { get; set; }

        public IRarManager GetRarManager(IImportContext msi)
        {
            //return new RarManagerImpl(msi, RarZipFileAfterSuccessfullExtract, RarZipFileAfterFailedExtract,
            //                            RarExctractManagement, AddUseRarPasswordToList);

            return new RarManagerImpl(msi, _IUnrarUserSettings);
        }

        //internal ConvertFileBehaviour FileCreatedByConvertion { get; set; }
        //internal PartialFileBehaviour SourceFileUsedForConvertion { get; set; }
        //internal PartialFileBehaviour ConvertedFileExtractedFromRar { get; set; }

        public IConvertManager GetConvert(IImportContext msi)
        {
            return new ConvertManagerImpl(msi, ConverterUserSettings);
            //return new ConvertManagerImpl(msi,FileCreatedByConvertion, SourceFileUsedForConvertion, ConvertedFileExtractedFromRar);
        }

        //internal BasicBehaviour DeleteRemovedFile { get; set; }
        public IDeleteManager GetDelete()
        {
            return new DeleteManagerImpl( _IMaturityUserSettings);
        }


        //internal double ImageSizeMoLimit { get; set; }
        //internal uint ImageNumberLimit { get; set; }
        //internal bool ImageNumber { get; set; }

        public ImageFormatManager GetImage()
        {
            return new ImageManagerImpl(_IImageFormatManagerUserSettings);

            //return new ImageManagerImpl( ImageSizeMoLimit, ImageNumberLimit, ImageNumber);
        }

        public IXMLImportManager GetXML()
        {
            //done
            return new XMLManagerImpl();
        }

        //internal string DirForPermanentCollection { get; set; }
        //internal bool ExportCollectionFiles { get; set; }

        //public IMaturityManager GetMaturity()
        //{
        //    return new MaturityManagerImpl(DirForPermanentCollection, ExportCollectionFiles);
        //}

        //public IWebServicesManager GetWebService()
        //{
        //    return new WebServicesManagerImpl( FreedServer,100,DiscogsActivated,AmazonActivated,null,null,null,null,null,null);
        //}

        public void SessionEnds()
        {
        }

        //public IEmailInformationManager GetEmailInformationManager()
        //{
        //    return new EmailSettingManagement(EmailAdress,Password,EmailReceptor);
        //}

    }
}
