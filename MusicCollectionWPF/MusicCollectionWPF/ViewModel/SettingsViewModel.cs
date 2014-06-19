using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

using MusicCollection.Properties;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using System.Windows.Input;
using System.Diagnostics;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    #region RarFileManagement

    public class RarFileManagementModelView : ViewModelBase, IDataErrorInfo
    {
        private bool _RarPasswordChanged = false;
        //private IRarFileManagement _IRarFileManagement;
        private IUnrarUserSettings _IRarFileManagement;

        internal RarFileManagementModelView(IUnrarUserSettings iIRarFileManagement)
        {
            _IRarFileManagement = iIRarFileManagement;
            _RarZipFileAfterSuccessfullExtract = _IRarFileManagement.RarZipFileAfterSuccessfullExtract;
            _RarZipFileAfterFailedExtract = _IRarFileManagement.RarZipFileAfterFailedExtract;
            _RarExctractManagement = _IRarFileManagement.RarExctractManagement;
            _Passwords = _IRarFileManagement.RarPasswords;
            //InitPasswords();

            CancelChanges = RelayCommand.Instanciate(DoCancelChanges);
            ClearPassWords = RelayCommand.Instanciate(DoClearPassWords);
            CommitPassWordsChanges = RelayCommand.Instanciate(DoCommitPassWordsChange);
            ImportFromTextFile = RelayCommand.Instanciate(DoImportFromTextFile);
        }

        public ICommand CommitPassWordsChanges { get; private set; }

        public ICommand ClearPassWords { get; private set; }

        public ICommand CancelChanges { get; private set; }

        public ICommand ImportFromTextFile { get; private set; }

        //private void InitPasswords()
        //{
        //    _Passwords = (Settings.Default.RarPasswords == null) ? new string[] { } : (from s in Settings.Default.RarPasswords.Cast<string>() where !String.IsNullOrEmpty(s) select s).ToArray();
        //    _RarPasswordChanged = false;
        //}

        private string[] _Passwords;
        public string[] RarPasswords
        {
            get { return _Passwords; }
            set
            {
                //var old = _Passwords;
                if (value == null) { value = new string[] { }; }
                //_Passwords = value;
                _RarPasswordChanged = true;
                //OnPropertyHasChanged(old, value);
                Set(ref _Passwords, value);
            }
        }

        public void DoClearPassWords()
        {
            RarPasswords = null;
        }

        internal bool Valid
        {
            get { return true; }
        }

        private void PrivateCommitPassWordsChange()
        {
            if (_RarPasswordChanged)
            {
                _IRarFileManagement.RarPasswords = _Passwords;
            }
        }

        private void DoCommitPassWordsChange()
        {
            PrivateCommitPassWordsChange();
            Window.Close();
        }

        public void DoCancelChanges()
        {
            _Passwords = _IRarFileManagement.RarPasswords;
            _RarPasswordChanged = false;
            Window.Close();
        }

        private void DoImportFromTextFile()
        {
            string targetpath = Window.ChooseFile("Select text file to be imported", "Text Files | *.txt");

            if (targetpath == null)
                return;

            List<string> PWR = new List<string>(RarPasswords);

            try
            {
                using (StreamReader sr = new StreamReader(targetpath))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                            PWR.Add(line);
                    }
                }

                RarPasswords = PWR.ToArray();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(ex.Message);
            }
        }

        internal void Commit()
        {
            if (!Valid)
                return;

            _IRarFileManagement.RarZipFileAfterSuccessfullExtract = _RarZipFileAfterSuccessfullExtract;
            _IRarFileManagement.RarZipFileAfterFailedExtract = _RarZipFileAfterFailedExtract;
            _IRarFileManagement.RarExctractManagement = _RarExctractManagement;
        }

        #region accessors

        private CompleteFileBehaviour _RarZipFileAfterSuccessfullExtract;
        public CompleteFileBehaviour RarZipFileAfterSuccessfullExtract
        {
            get { return _RarZipFileAfterSuccessfullExtract; }
            set { this.Set(ref _RarZipFileAfterSuccessfullExtract, value); }
        }

        private CompleteFileBehaviour _RarZipFileAfterFailedExtract;
        public CompleteFileBehaviour RarZipFileAfterFailedExtract
        {
            get { return _RarZipFileAfterFailedExtract; }
            set { this.Set(ref _RarZipFileAfterFailedExtract, value); }
        }

        private ConvertFileBehaviour _RarExctractManagement;
        public ConvertFileBehaviour RarExctractManagement
        {
            get { return _RarExctractManagement; }
            set { this.Set(ref _RarExctractManagement, value); }
        }

        #endregion

        #region Error

        public string Error
        {
            get { return Valid ? null : "Must inform valid Output Directory"; }
        }

        public string this[string columnName] { get { return null; } }

        #endregion
    }

    #endregion

    #region CollectionFileManagement

    public class CollectionFileManagement : ViewModelBase, IDataErrorInfo
    {
        private bool _Modified = false;
        private IMaturityUserSettings _CollectionManagement;

        internal CollectionFileManagement(IMaturityUserSettings iCollectionManagement)
        {
            _CollectionManagement = iCollectionManagement;
            _DirForPermanentCollection = _CollectionManagement.DirForPermanentCollection;
            _ExportCollectionFiles = _CollectionManagement.ExportCollectionFiles;
            _DeleteRemovedFile = _CollectionManagement.DeleteRemovedFile;
        }

        internal bool Valid
        {
            get
            {
                if (!_Modified)
                    return true;

                if (ExportCollectionFiles && ((String.IsNullOrEmpty(DirForPermanentCollection)) || (!Directory.Exists(DirForPermanentCollection))))
                    return false;

                return true;
            }
        }

        internal bool Commit()
        {
            if (!Valid)
                return false;

            _CollectionManagement.DirForPermanentCollection = _DirForPermanentCollection;
            _CollectionManagement.ExportCollectionFiles = _ExportCollectionFiles;
            _CollectionManagement.DeleteRemovedFile = _DeleteRemovedFile;
            return true;
        }

        private bool _ExportCollectionFiles;
        public bool ExportCollectionFiles
        {
            get { return _ExportCollectionFiles; }
            set { this.Set(ref _ExportCollectionFiles, value); }
        }

        private string _DirForPermanentCollection;
        public string DirForPermanentCollection
        {
            get { return _DirForPermanentCollection; }
            set { this.Set(ref _DirForPermanentCollection, value); }
        }

        private BasicBehaviour _DeleteRemovedFile;
        public BasicBehaviour DeleteRemovedFile
        {
            get { return _DeleteRemovedFile; }
            set { this.Set(ref _DeleteRemovedFile, value); }
        }

        public string Error
        {
            get { return Valid ? null : "Must inform valid Output Directory"; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == _DirForPermanentCollection)
                {
                    if (String.IsNullOrEmpty(_DirForPermanentCollection))
                        return null;

                    if (!Directory.Exists(_DirForPermanentCollection))
                        return "a valid Destination Directory should be specified";

                    return null;
                }

                return null;
            }
        }
    }

    #endregion

    #region WebsiteMusicSettings

    public class WebsiteMusicSettings : ViewModelBase
    {
        private int _DiscogsTimeOut;
        private string _FDBS;
        private bool _AA;
        private bool _DA;
        private bool _IsDiscogImageActivated;

        private IWebUserSettings _IWebServicesSettings;
        private IDiscogsAuthentificationProvider _IWebServicesSettingsWrapper;

        private IInfraDependencies _IInfraDependencies;
        //private ViewModelBase Father;

        internal WebsiteMusicSettings(IWebUserSettings iIMusicSettings, IInfraDependencies ims, ViewModelBase iFather)
        {
            _IWebServicesSettingsWrapper = iIMusicSettings.GetDiscogsAutentificator();
            Father = iFather;
            _IInfraDependencies = ims;
            _IWebServicesSettings = iIMusicSettings;
            _DiscogsTimeOut = _IWebServicesSettings.DiscogsTimeOut;
            _FDBS = _IWebServicesSettings.FreedbServer;
            _AA = _IWebServicesSettings.AmazonActivated;
            _DA = _IWebServicesSettings.DiscogsActivated;
            _IsDiscogImageActivated = _IWebServicesSettingsWrapper.IsDiscogImageActivated;
            FreedbServers = _IWebServicesSettings.FreedbServers;

            ActivateDiscogs = this.Register(RelayCommand.Instanciate(DoActivateDiscogs, () => !IsDiscogImageActivated));
            ExportDiscogs = this.Register(RelayCommand.Instanciate(DoExportDiscogs, () => IsDiscogImageActivated));
            ImportDiscogs = RelayCommand.Instanciate(DoImportDiscogs);
        }

        public ICommand ActivateDiscogs { get; private set; }
        public ICommand ImportDiscogs { get; private set; }
        public ICommand ExportDiscogs { get; private set; }

        internal void Commit()
        {
            _IWebServicesSettings.DiscogsTimeOut = _DiscogsTimeOut;
            _IWebServicesSettings.FreedbServer = _FDBS;
            _IWebServicesSettings.AmazonActivated = _AA;
            _IWebServicesSettings.DiscogsActivated = _DA;
        }

        private void DoImportDiscogs()
        {
            bool Force = false;

            if (_IWebServicesSettingsWrapper.IsDiscogImageActivated)
            {
                if (!this.Window.ShowConfirmationMessage("Caution the current keys will be overwritten and lost.\nDo not proceed if Discogs is working correctly.","Do you want to proceed?"))
                    return;

                Force = true;
            }

            var import = new ImportKeyViewModel(_IWebServicesSettings.GetDiscogsAutentificator(), _IInfraDependencies) { Force = Force };
            var importwndow = this.Window.CreateFromViewModel(import);
            importwndow.ShowDialog();
            IsDiscogImageActivated = _IWebServicesSettingsWrapper.IsDiscogImageActivated;
        }

        private void DoExportDiscogs()
        {
            var export = new ExportKeyViewModel(_IWebServicesSettingsWrapper, _IInfraDependencies);
            var exportwndow = this.Window.CreateFromViewModel(export);
            exportwndow.ShowDialog();
        }

        private void DoActivateDiscogs()
        {
            DiscogsOAuthViewModel dovm = new DiscogsOAuthViewModel(_IWebServicesSettingsWrapper);

            if (dovm.Url!=null)
            { 
                this.Window.CreateFromViewModel(dovm).ShowDialog();
                IsDiscogImageActivated = _IWebServicesSettingsWrapper.IsDiscogImageActivated;
            }
            else
            {
                this.Window.ShowMessage("Please check that you have not already authorized Discogs.\n If so import discogs key instead.", "Problem with Discogs services", true);
            }
        }

        public bool IsDiscogImageActivated
        {
            get { return _IsDiscogImageActivated; }
            set { this.Set(ref _IsDiscogImageActivated, value); }
        }

        public int DiscogsTimeOut
        {
            get { return _DiscogsTimeOut; }
            set { this.Set(ref _DiscogsTimeOut, value); }
        }

        public string FreedbServer
        {
            get { return _FDBS; }
            set { this.Set(ref _FDBS, value); }
        }

        public List<string> FreedbServers
        {
            get;
            private set;
        }

        public bool DiscogsActivated
        {
            get { return _DA; }
            set { this.Set(ref _DA, value); }
        }

        public bool AmazonActivated
        {
            get { return _AA; }
            set { this.Set(ref _AA, value); }
        }
    }

    #endregion

    #region EmbeddedMusicSettings

    public class EmbeddedMusicSettings : ViewModelBase
    {
        private IImageFormatManagerUserSettings _EmbeddedMusicSettings;

        internal EmbeddedMusicSettings(IImageFormatManagerUserSettings iEmbeddedMusicSettingsImpl)
        {
            _EmbeddedMusicSettings = iEmbeddedMusicSettingsImpl;
            _ImageSizeMoLimit = _EmbeddedMusicSettings.ImageSizeMoLimit;
            _ImageNumber = _EmbeddedMusicSettings.ImageNumber;
            _ImageNumberLimit = _EmbeddedMusicSettings.ImageNumberLimit;
        }

        internal void Commit()
        {
            _EmbeddedMusicSettings.ImageSizeMoLimit = _ImageSizeMoLimit;
            _EmbeddedMusicSettings.ImageNumberLimit = _ImageNumberLimit;
            _EmbeddedMusicSettings.ImageNumber = _ImageNumber;
        }

        private double _ImageSizeMoLimit;
        public double ImageSizeMoLimit
        {
            get { return _ImageSizeMoLimit; }
            set { this.Set(ref _ImageSizeMoLimit, value); }
        }

        private uint _ImageNumber;
        public uint ImageNumber
        {
            get { return _ImageNumber; }
            set { this.Set(ref _ImageNumber, value); }
        }

        private bool _ImageNumberLimit;
        public bool ImageNumberLimit
        {
            get { return _ImageNumberLimit; }
            set { this.Set(ref _ImageNumberLimit, value); }
        }
    }

    #endregion

    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        private const string _CollectionFileManagementProperty = "CollectionFileManagement";
        private const string _RarFileManagementProperty = "RarFileManagement";

        private RarFileManagementModelView _RarFileManagement;
        private CollectionFileManagement _CollectionFileManagement;
        private EmbeddedMusicSettings _EmbeddedMusicSettingsImpl;
        private WebsiteMusicSettings _WBM;
        private IMusicSettings _IMusicSettings;

        internal SettingsViewModel(IMusicSettings iIMusicSettings, IInfraDependencies ims)
        {
            _IMusicSettings = iIMusicSettings;

            _RarFileManagement = new RarFileManagementModelView(_IMusicSettings.RarFileManagement);
            _CollectionFileManagement = new CollectionFileManagement(_IMusicSettings.CollectionFileSettings);
            _EmbeddedMusicSettingsImpl = new EmbeddedMusicSettings(_IMusicSettings.ImageFormatManagerUserSettings);
            _WBM = new WebsiteMusicSettings(_IMusicSettings.WebUserSettings, ims, this);

            _FileCreatedByConvertion = _IMusicSettings.ConverterUserSettings.FileCreatedByConvertion;
            _SourceFileUsedForConvertion = _IMusicSettings.ConverterUserSettings.SourceFileUsedForConvertion;
            _ConvertedFileExtractedFromRar = _IMusicSettings.ConverterUserSettings.ConvertedFileExtractedFromRar;
            _ImportBrokenItunesTrack = _IMusicSettings.iTunesSetting.ImportBrokenTrack;

            CommitChanges = RelayCommand.Instanciate(DoCommitChanges);
            CloseWindow = RelayCommand.Instanciate(DoCloseWindow);
            EditRarPassword = RelayCommand.Instanciate(DoEditRarPassword);
        }

        public ICommand CommitChanges { get; private set; }

        public ICommand CloseWindow { get; private set; }

        public ICommand EditRarPassword { get; private set; }

        public WebsiteMusicSettings WebsiteMusicSettings
        {
            get { return _WBM; }
        }

        public EmbeddedMusicSettings EmbeddedMusicSettings
        {
            get { return _EmbeddedMusicSettingsImpl; }
        }

        public RarFileManagementModelView RarFileManagement
        {
            get { return _RarFileManagement; }
        }

        public CollectionFileManagement CollectionFileManagement
        {
            get { return _CollectionFileManagement; }
        }

        #region Error

        private bool _IsValid = true;
        public bool IsValid
        {
            get { return _IsValid; }
            set { this.Set(ref _IsValid, value); }
        }

        private Dictionary<String, String> _Errors = new Dictionary<string, string>();

        private string OnError(string PropertyName, string ErrorName)
        {
            if (_Errors.ContainsKey(PropertyName))
                return _Errors[PropertyName];

            _Errors.Add(PropertyName, ErrorName);
            return ErrorName;
        }

        private void ResetError(string PropertyName)
        {
            if (_Errors.ContainsKey(PropertyName))
                _Errors.Remove(PropertyName);
        }

        public string Error
        {
            get { return IsValid ? null : "Configuration has Error"; }
        }

        public string this[string columnName]
        {
            get
            {
                string Res = privateError(columnName);
                IsValid = _Errors.Count == 0;
                return Res;
            }
        }

        private string SetError(string iColumName, string res)
        {
            if (res != null)
            {
                return OnError(iColumName, res);
            }

            ResetError(iColumName);
            return null;
        }

        private string privateError(string columnName)
        {
            if (columnName == _RarFileManagementProperty)
            {
                return SetError(_RarFileManagementProperty, _RarFileManagement.Error);
            }

            if (columnName == _CollectionFileManagementProperty)
            {
                return SetError(_CollectionFileManagementProperty, _CollectionFileManagement.Error);
            }

            return null;
        }

        #endregion

        private void DoEditRarPassword()
        {
            IWindow newwindow = Window.CreateFromViewModel(RarFileManagement);
            newwindow.ShowDialog();
        }

        private void DoCommitChanges()
        {
            if (!IsValid)
                return;

            if (!_RarFileManagement.Valid)
                return;

            if (!_CollectionFileManagement.Valid)
                return;

            _CollectionFileManagement.Commit();
            _RarFileManagement.Commit();
            _EmbeddedMusicSettingsImpl.Commit();
            _WBM.Commit();

            _IMusicSettings.ConverterUserSettings.FileCreatedByConvertion = _FileCreatedByConvertion;
            _IMusicSettings.ConverterUserSettings.SourceFileUsedForConvertion = _SourceFileUsedForConvertion;
            _IMusicSettings.ConverterUserSettings.ConvertedFileExtractedFromRar = _ConvertedFileExtractedFromRar;
            _IMusicSettings.iTunesSetting.ImportBrokenTrack = _ImportBrokenItunesTrack;

            this.Window.Close();
        }

        private void DoCloseWindow()
        {
            Window.Close();
        }

        private ConvertFileBehaviour _FileCreatedByConvertion;
        public ConvertFileBehaviour FileCreatedByConvertion
        {
            get { return _FileCreatedByConvertion; }
            set { this.Set(ref _FileCreatedByConvertion, value); }
        }

        private PartialFileBehaviour _SourceFileUsedForConvertion;
        public PartialFileBehaviour SourceFileUsedForConvertion
        {
            get { return _SourceFileUsedForConvertion; }
            set { this.Set(ref _SourceFileUsedForConvertion, value); }
        }

        private PartialFileBehaviour _ConvertedFileExtractedFromRar;
        public PartialFileBehaviour ConvertedFileExtractedFromRar
        {
            get { return _ConvertedFileExtractedFromRar; }
            set { this.Set(ref _ConvertedFileExtractedFromRar, value); }
        }

        private BasicBehaviour _ImportBrokenItunesTrack;
        public BasicBehaviour ImportBrokenItunesTrack
        {
            get { return _ImportBrokenItunesTrack; }
            set { this.Set(ref _ImportBrokenItunesTrack, value); }
        }
    }
}
