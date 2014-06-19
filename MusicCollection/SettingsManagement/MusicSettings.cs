////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.ComponentModel;
////using System.IO;

////using MusicCollection.Properties;
////using MusicCollection.Fundation;
////using MusicCollection.Implementation;
////using MusicCollection.Infra;
////using MusicCollection.WebServices.Discogs2;
////using System.Diagnostics;
////using MusicCollection.Implementation.Session;

////namespace MusicCollection.SettingsManagement
////{
////    internal class MusicSettings : IMusicSettings
////    {
////        //private IUnrarUserSettings _RarFileManagement;
////        //private CollectionFileManagementImpl _CollectionFileManagement;
////        //private EmbeddedMusicSettingsImpl _EmbeddedMusicSettingsImpl;
////        //private MusicExporterManagementImpl _MusicExporterManagement;
////        //private UIGridManagement _UIGD;
////        //private WebsiteMusicSettingsImpl _WBM;
////        private IContextualInfraProvider _ISettingFactory;

////        internal MusicSettings(IContextualInfraProvider iSettingFactory)
////        {
////            _ISettingFactory = iSettingFactory;
////            //_RarFileManagement = new RarFileManagementImpl(this);
////            //_MusicExporterManagement = new MusicExporterManagementImpl(this);
////            //_CollectionFileManagement = new CollectionFileManagementImpl(this);
////            //_EmbeddedMusicSettingsImpl = new EmbeddedMusicSettingsImpl(this);
////            //_WBM = new WebsiteMusicSettingsImpl(this);
////            //_UIGD = new UIGridManagement(iSettingFactory);
////        }

////        private IContextualInfraProvider SettingFactory
////        {
////            get{return _ISettingFactory;}
////        }

////        //public IWebServicesSettings WebsiteMusicSettings
////        //{
////        //    get { return _WBM; }
////        //}

////        ////public IUIGridManagement GridManagement
////        //{
////        //    get { return _UIGD; }
////        //}

////        //#region DisplaySizer

////        //public int DisplaySizer
////        //{
////        //    get { return Settings.Default.DisplaySizer; }
////        //    set 
////        //    { 
////        //        if (value == Settings.Default.DisplaySizer) 
////        //            return; 
////        //        Settings.Default.DisplaySizer = value; 

////        //    }
////        //}

////        //#endregion

////        //#region PresenterMode
////        //public AlbumPresenter PresenterMode
////        //{
////        //    get { return Settings.Default.PresenterMode; }
////        //    set { Settings.Default.PresenterMode = value; }
////        //}
////        //#endregion

////        //#region WebsiteMusicSettings

////        ////private class WebsiteMusicSettingsImpl : IWebServicesSettings
////        ////{
////        ////    private DiscogsOAuthBuilder _DiscogsOAuthBuilder;
////        ////    private MusicSettings _MusicSettings;
////        ////    internal WebsiteMusicSettingsImpl(MusicSettings father)
////        ////    {
////        ////        _MusicSettings = father;
////        ////        FreedbServers = Settings.Default.FreedbServers.Cast<string>().ToList();
////        ////     }

////        ////    public bool IsDiscogImageActivated
////        ////    {
////        ////        get { return !string.IsNullOrEmpty(Settings.Default.DiscogsToken); }
////        ////    }

////        ////    public int DiscogsTimeOut
////        ////    {
////        ////        get { return Settings.Default.DiscogsTimeOut; }
////        ////        set { Settings.Default.DiscogsTimeOut = value; }
////        ////    }

////        ////    public string FreedbServer
////        ////    {
////        ////        get { return Settings.Default.FreedbServer; }
////        ////        set { Settings.Default.FreedbServer = value; }
////        ////    }

////        ////    public List<string> FreedbServers{ get; private set;  }

////        ////    public bool DiscogsActivated
////        ////    {
////        ////        get { return Settings.Default.DiscogsActivated; }
////        ////        set { Settings.Default.DiscogsActivated = value; }
////        ////    }

////        ////    public bool AmazonActivated
////        ////    {
////        ////        get { return Settings.Default.AmazonActivated; }
////        ////        set { Settings.Default.AmazonActivated = value; }
////        ////    }

////        ////    public string ComputeLinkForDiscogsOAuthAuthorization()
////        ////    {
////        ////        if (IsDiscogImageActivated)
////        ////            return null;

////        ////        _DiscogsOAuthBuilder = _MusicSettings.SettingFactory.GetWebService().GetDiscogsOAuthBuilder();

////        ////        //_DiscogsOAuthBuilder = new DiscogsOAuthBuilder(Settings.Default.DiscogsConsumerKey, Settings.Default.DiscogsConsumerSecret);
////        ////        return _DiscogsOAuthBuilder.ComputeUrlForAuthorize();
////        ////    }

////        ////    public bool AuthorizeDiscogsPin(string iPin)
////        ////    {
////        ////         if (IsDiscogImageActivated)
////        ////            return true;

////        ////         if (_DiscogsOAuthBuilder == null)
////        ////             return false;

////        ////        _DiscogsOAuthBuilder.FinalizeFromPin(iPin);

////        ////        Settings.Default.DiscogsToken = _DiscogsOAuthBuilder.Token;
////        ////        Settings.Default.DiscogsTokenSecret = _DiscogsOAuthBuilder.TokenSecret;

////        ////        Settings.Default.Save();

////        ////        return true;
////        ////    }


////        ////    public IDictionary<string, string> GetPrivateKeys()
////        ////    {
////        ////        if (!IsDiscogImageActivated)
////        ////            return null;

////        ////        IDictionary<string, string> res = new Dictionary<string, string>();
////        ////        res.Add("DiscogsToken",Settings.Default.DiscogsToken);
////        ////        res.Add("DiscogsTokenSecret",Settings.Default.DiscogsTokenSecret);

////        ////        return res;
////        ////    }

////        ////    public bool ImportPrivateKeys(IDictionary<string, string> ikeys, bool iForce = false)
////        ////    {
////        ////        if ((ikeys == null))
////        ////            return false;

////        ////        if (!iForce && (IsDiscogImageActivated))
////        ////            return false;

////        ////        try
////        ////        {
////        ////            Settings.Default.DiscogsToken = ikeys["DiscogsToken"];
////        ////            Settings.Default.DiscogsTokenSecret = ikeys["DiscogsTokenSecret"];
////        ////            Settings.Default.Save();
////        ////        }
////        ////        catch (Exception ex)
////        ////        {
////        ////            Trace.WriteLine(string.Format("Impossible to import keys {0}",ex));
////        ////            return false;
////        ////        }

////        ////        return true;
////        ////    }
////        ////}

////        //#endregion

////        //#region EmbeddedMusicSettings

////        ////private class EmbeddedMusicSettingsImpl : IEmbeddedMusicSettings
////        ////{
  
////        ////    internal EmbeddedMusicSettingsImpl(MusicSettings father)
////        ////    {
////        ////    }

////        ////    public double ImageSizeMoLimit
////        ////    {
////        ////        get { return Settings.Default.ImageSizeMoLimit; }
////        ////        set { Settings.Default.ImageSizeMoLimit = value; }
////        ////    }

////        ////    public uint ImageNumber
////        ////    {
////        ////        get { return Settings.Default.ImageNumberLimit ; }
////        ////        set { Settings.Default.ImageNumberLimit = value;}
////        ////    }

////        ////    public bool ImageNumberLimit
////        ////    {
////        ////        get { return Settings.Default.ImageNumber; }
////        ////        set { Settings.Default.ImageNumber = value;}
////        ////    }
////        ////}

////        ////public IEmbeddedMusicSettings EmbeddedMusicSettings
////        ////{
////        ////    get { return _EmbeddedMusicSettingsImpl; }
////        ////}

////        //#endregion

////        //#region CollectionFileManagementImpl

////        ////private class CollectionFileManagementImpl : ICollectionFileManagement
////        ////{
////        ////    internal CollectionFileManagementImpl(MusicSettings father)
////        ////    {
////        ////    }

          
////        ////    public bool ExportCollectionFiles
////        ////    {
////        ////        get { return Settings.Default.ExportCollectionFiles; }
////        ////        set {Settings.Default.ExportCollectionFiles = value;}
////        ////    }

////        ////    public string DirForPermanentCollection
////        ////    {
////        ////        get { return Settings.Default.DirForPermanentCollection;}
////        ////        set { Settings.Default.DirForPermanentCollection = value;}
////        ////    }

////        ////    public BasicBehaviour DeleteRemovedFile
////        ////    {
////        ////        get { return Settings.Default.DeleteRemovedFile; }
////        ////        set { Settings.Default.DeleteRemovedFile = value; }
////        ////    }
////        ////}

////        //#endregion

////        #region MusicExporterManagement



////        #endregion

////        #region RarFileManagementImpl

////        //private class RarFileManagementImpl : IRarFileManagement
////        //{
////        //    internal RarFileManagementImpl(MusicSettings Ms)
////        //    {
////        //        _Passwords = (Settings.Default.RarPasswords == null) ? new string[] { } : (from s in Settings.Default.RarPasswords.Cast<string>() where !String.IsNullOrEmpty(s) select s).ToArray();
////        //    }

////        //    private string[] _Passwords;
////        //    public string[] RarPasswords
////        //    {
////        //        get { return _Passwords; }
////        //        set 
////        //        { 
////        //            if (value == null) { value = new string[] { }; } 
////        //            _Passwords = value;
////        //            Settings.Default.RarPasswords = new System.Collections.Specialized.StringCollection();
////        //            Settings.Default.RarPasswords.AddRange((from s in _Passwords where !string.IsNullOrEmpty(s) select s).Distinct().ToArray());
////        //        }
////        //    }

////        //    #region accessors

////        //    public CompleteFileBehaviour RarZipFileAfterSuccessfullExtract
////        //    {
////        //        get { return Settings.Default.RarZipFileAfterSuccessfullExtract; }
////        //        set { Settings.Default.RarZipFileAfterSuccessfullExtract = value; }
////        //    }

////        //    public CompleteFileBehaviour RarZipFileAfterFailedExtract
////        //    {
////        //        get { return Settings.Default.RarZipFileAfterFailedExtract; }
////        //        set { Settings.Default.RarZipFileAfterFailedExtract = value; }
////        //    }

////        //    public ConvertFileBehaviour RarExctractManagement
////        //    {
////        //        get { return Settings.Default.RarExctractManagement; }
////        //        set { Settings.Default.RarExctractManagement = value; }
////        //    }

////        //    #endregion
////        //}

////        #endregion

////        //public IRarFileManagement RarFileManagement
////        //{
////        //    get { return _RarFileManagement; }
////        //}

////        public IUnrarUserSettings RarFileManagement
////        {
////            get { return _ISettingFactory.RarFileManagement; }
////        }

////        public IWebUserSettings WebUserSettings
////        {
////            get { return _ISettingFactory.WebUserSettings; }
////        }

////        public IMaturityUserSettings CollectionFileSettings
////        {
////            get { return _ISettingFactory.CollectionFileSettings; }
////        }

////        public IAparencyUserSettings AparencyUserSettings
////        {
////            get { return _ISettingFactory.AparencyUserSettings; }
////        }

////        public IMusicImporterExporterUserSettings MusicImporterExporter
////        {
////            get { return _ISettingFactory.MusicImporterExporter; }
////        }

////        public IImageFormatManagerUserSettings ImageFormatManagerUserSettings
////        {
////            get { return _ISettingFactory.ImageFormatManagerUserSettings; }
////        }

////        public IConverterUserSettings ConverterUserSettings
////        {
////            get { return _ISettingFactory.ConverterUserSettings; }
////        }

////        public IiTunesUserSettings iTunesSetting
////        {
////            get { return _ISettingFactory.iTunesSetting; }
////        }

////        public IPathUserSettings PathUserSettings
////        {
////            get { return _ISettingFactory.PathUserSettings; }
////        }


////        //public ICollectionFileManagement CollectionFileManagement
////        //{
////        //    get { return _CollectionFileManagement; }
////        //}

////        //public ConvertFileBehaviour FileCreatedByConvertion
////        //{
////        //    get { return Settings.Default.FileCreatedByConvertion; }
////        //    set { Settings.Default.FileCreatedByConvertion = value; }
////        //}

////        //public string PathFolder
////        //{
////        //    get { string res = Settings.Default.LastPathImportFolder; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
////        //    set { Settings.Default.LastPathImportFolder = value; }//Settings.Default.Save(); }
////        //}

////        //public string PathCusto
////        //{
////        //    get { string res = Settings.Default.LastPathImportCusto; if ((string.IsNullOrEmpty(res)) || !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
////        //    set { Settings.Default.LastPathImportCusto = value; }// Settings.Default.Save(); }
////        //}

////        //public string PathRar
////        //{
////        //    get { string res = Settings.Default.LastPathImportRar; if ((string.IsNullOrEmpty(res))|| !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
////        //    set { Settings.Default.LastPathImportRar = value;}// Settings.Default.Save(); }
////        //}

////        //public string PathExport
////        //{
////        //    get { string res = Settings.Default.LastPathExportFile; if ((string.IsNullOrEmpty(res))|| !Directory.Exists(res)) return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); return res; }
////        //    set { Settings.Default.LastPathExportFile = value; }// Settings.Default.Save(); }
////        //}

////        //public PartialFileBehaviour SourceFileUsedForConvertion
////        //{
////        //    get { return Settings.Default.SourceFileUsedForConvertion; }
////        //    set { Settings.Default.SourceFileUsedForConvertion = value; }
////        //}

////        //public PartialFileBehaviour ConvertedFileExtractedFromRar
////        //{
////        //    get { return Settings.Default.ConvertedFileExtractedFromRar; }
////        //    set { Settings.Default.ConvertedFileExtractedFromRar = value; }
////        //}

////        //public BasicBehaviour ImportBrokenItunesTrack
////        //{
////        //    get { return Settings.Default.ImportBrokenItunesTrack; }
////        //    //set { Settings.Default.ImportBrokenItunesTrack = value; }
////        //}

////        public void SessionEnds()
////        {
////        }
////    }
////}
