using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.SettingsManagement;
using MusicCollection.Properties;
using System.Configuration;
using MusicCollection.Fundation;

namespace MusicCollection.Implementation.Session
{
    static internal class SettingsBuilder
    {

        private class SettingsUsers : IMusicSettings
         //IContextualInfraProvider, 
        {
            internal SettingsUsers()
            {
                _IUnrarUserSettings = new StandardRarSettings();
                _IWebUserSettings = new StandardWebSettings();
                _IMaturityUserSettings = new StandardMaturitySetting();
                _IConverterUserSettings = new StandardConverterSettings();
                _IEmailInformationSettings = new StandardEmailInformationSettings();
                _IImageFormatManagerUserSettings = new StandardImageFormatManagerSettings();
                _IAparencyUserSettings = new StandardAparencySettings();
                _IMusicImporterExporterUser = new StandardMusicExporterSettings();
                _IiTunesUserSettings = new StandardiTunesUserSettings();
                _IPathUserSettings = new StandardPathUserSettings();
            }

            private IAparencyUserSettings _IAparencyUserSettings;
            public IAparencyUserSettings AparencyUserSettings
            {
                get { return _IAparencyUserSettings; }
            }

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

            private IConverterUserSettings _IConverterUserSettings;
            public IConverterUserSettings ConverterUserSettings
            {
                get { return _IConverterUserSettings; }
            }

            private IEmailInformationSettings _IEmailInformationSettings;
            public IEmailInformationSettings EmailInformationSettings
            {
                get { return _IEmailInformationSettings; }
            }

            private IImageFormatManagerUserSettings _IImageFormatManagerUserSettings;
            public IImageFormatManagerUserSettings ImageFormatManagerUserSettings
            {
                get { return _IImageFormatManagerUserSettings; }
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

            public IRarManager GetRarManager(IImportContext msi)
            {
                return new RarManagerImpl(msi, _IUnrarUserSettings);
            }

            //public IConvertManager GetConvert(IImportContext msi)
            //{
            //    return new ConvertManagerImpl(msi, _IConverterUserSettings);
            //    //return new ConvertManagerImpl(msi, Settings.Default.FileCreatedByConvertion, Settings.Default.SourceFileUsedForConvertion, Settings.Default.ConvertedFileExtractedFromRar);
            //}

            //public IDeleteManager GetDelete()
            //{
            //    return new DeleteManagerImpl(_IMaturityUserSettings);
            //}

            //public ImageFormatManager GetImage()
            //{
            //    return new ImageManagerImpl(_IImageFormatManagerUserSettings);
            //    //return new ImageManagerImpl(Settings.Default.ImageSizeMoLimit, Settings.Default.ImageNumberLimit, Settings.Default.ImageNumber);
            //}

            //public IWebServicesManager GetWebService()
            //{
            //    //return new WebServicesManagerImpl(msi, Settings.Default.FreedbServer, Settings.Default.DiscogsTimeOut,
            //    //    Settings.Default.DiscogsActivated, Settings.Default.AmazonActivated,
            //    //    Settings.Default.DiscogsConsumerKey,Settings.Default.DiscogsConsumerSecret,
            //    //    Settings.Default.DiscogsToken,Settings.Default.DiscogsTokenSecret,
            //    //    //Settings.Default.Amazon_accessKeyId, Settings.Default.Amazon_secretKey);

            //    //    ConfigurationManager.AppSettings["AmazonaccessKeyId"],
            //    //    ConfigurationManager.AppSettings["AmazonsecretKey"]);

            //    return new WebServicesManagerImpl( 
            //       Settings.Default.FreedbServer, 
            //       Settings.Default.DiscogsTimeOut,
            //       Settings.Default.DiscogsActivated, 
            //       Settings.Default.AmazonActivated,
            //       ConfigurationManager.AppSettings["DiscogsConsumerKey"],
            //       ConfigurationManager.AppSettings["DiscogsConsumerSecret"],
            //       Settings.Default.DiscogsToken, 
            //       Settings.Default.DiscogsTokenSecret,
            //       ConfigurationManager.AppSettings["AmazonaccessKeyId"],
            //       ConfigurationManager.AppSettings["AmazonsecretKey"]);
            //}

            //public IXMLImportManager GetXML()
            //{
            //    //done
            //    return new XMLManagerImpl();
            //}

            private IMaturityUserSettings _IMaturityUserSettings;
            public IMaturityUserSettings CollectionFileSettings
            {
                get { return _IMaturityUserSettings; }
            }

            //public IMaturityManager GetMaturity()
            //{
            //    //return new MaturityManagerImpl(Settings.Default.DirForPermanentCollection,Settings.Default.ExportCollectionFiles);

            //    return new MaturityManagerImpl();
            //}

            public void SessionEnds()
            {
                Settings.Default.Save();
            }



            //public IEmailInformationManager GetEmailInformationManager()
            //{
            //    return new EmailSettingManagement( 
            //        ConfigurationManager.AppSettings["AdministrativeEmail"],
            //       ConfigurationManager.AppSettings["AdministrativeEmailPassword"],
            //       ConfigurationManager.AppSettings["AdministrativeEmailReceptor"]);
            //}          

        }

        static internal IMusicSettings FromUserSetting()
        {
            return new SettingsUsers();
        }
    }
}
