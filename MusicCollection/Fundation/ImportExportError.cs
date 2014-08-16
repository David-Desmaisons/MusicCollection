using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using MusicCollection.Infra;
using MusicCollection.Implementation;
using MusicCollection.WebServices;

namespace MusicCollection.Fundation
{
    public abstract class ImportExportError
    {
        public abstract string WindowName { get; }

        public abstract string What { get; }

        public abstract string Who { get; }

        public override string ToString()
        {
            if (Who == null)
                return string.Format("{0} : {1}", WindowName, What);

            return string.Format("{0} : {1} {2}", WindowName, What, Who); ;
        }
    }

    public class ImportErrorItem
    {
        private string _ItemName;
        public ImportErrorItem(string iName)
        {
            _ItemName = iName;
        }

        public string ItemName
        {
            get { return _ItemName; }
        }

        public override string ToString()
        {
            return _ItemName;
        }
    }


    public abstract class ImportExportErrorEventListItemsArgs : ImportExportError
    {
        protected ImportExportErrorEventListItemsArgs(List<ImportErrorItem> errors)
        {
        }
    }

    public abstract class WhoImportErrorEventArgs : ImportExportErrorEventListItemsArgs
    {  
        protected WhoImportErrorEventArgs(string Who) : base(null)
        {
            _Who = Who;
        }
    
        protected WhoImportErrorEventArgs(IEnumerable<ImportErrorItem> Who): base(null)
        {
            StringBuilder SB = new StringBuilder();

            foreach (ImportErrorItem s in Who)
            {
                SB.Append(s.ItemName);
                SB.Append(Environment.NewLine);
            }

            _Who = SB.ToString();
        }

        private string _Who;

        public override string Who
        {
            get { return _Who; }
        }
    }

    public class OutputDirectoryNotFound : WhoImportErrorEventArgs
    {
        public OutputDirectoryNotFound(string Who) : base(Who)
        {
        }

        public OutputDirectoryNotFound(IEnumerable<ImportErrorItem> Who) : base(Who)
        {
        }

        public override string WindowName
        {
            get { return "Import Warning"; }
        }


        override public string What
        {
            get { return "Output Directory Not Found"; }
        }
    }

    public class NullMusicImportErrorEventArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }


        public override string What
        {
            get { return "No Music to Import"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public NullMusicImportErrorEventArgs()
        {
        }
    }

    public class NoMusicImportErrorEventArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get  { return "No Music to Import"; }
        }

        public NoMusicImportErrorEventArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class FileInUse : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "File used by another process."; }
        }

        public FileInUse(string iPath)
            : base(iPath)
        {
        }
    }

    public class UnhandledRarFile : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get  { return "Unhandled compressed file configuration: Please request develloper support"; }
        }

        public UnhandledRarFile(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnknownRarError : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "Unknown error while decompressing file"; }
        }

        public UnknownRarError(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnsupportedFormat : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "This file format is not supported"; }
        }

        public UnsupportedFormat(string iEntity)
            : base(iEntity)
        {
        }
    }


    public class ExportDirectoryNotFound : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get  { return "Export directory not found"; }
        }

        public ExportDirectoryNotFound(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class FileBrokenCannotBeExported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get { return "Original Track not found"; }
        }

        public FileBrokenCannotBeExported(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class FileBrokenCannotBeImported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get  { return "Original Track not found"; }
        }

        public FileBrokenCannotBeImported(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class PathTooLong : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get  { return "File path too long."; }
        }

        public PathTooLong(string iPath)
            : base(iPath)
        {
        }
    }

    public class FileAlreadyExported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get { return "Track(s) already exist"; }
        }

        public FileAlreadyExported(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    #region windowsphone

    public class WindowsPhoneFound : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Impossible to found WindowsPhone"; }
        }

        public override string What
        {
            get
            { return "Check WindowsPhone connection"; }
        }

        public WindowsPhoneFound()
            : base(string.Empty)
        {
        }
    }




    public class UnableToExportFileToWindowsPhone : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get
            { return "Unknown Error in copying file to windows phone"; }
        }

        public UnableToExportFileToWindowsPhone(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }

        public UnableToExportFileToWindowsPhone(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnknowErrorWindowsPhone : ImportExportError
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }


        public override string What
        {
            get
            { return "Unexpected error during transfer to WindowsPhone"; }
        }

        public override string Who
        {
            get
            { return null; }
        }

        public UnknowErrorWindowsPhone()
        {
        }
    }

    #endregion

    public class UnableToCopyFile : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get  { return "Unknown Error in copying file"; }
        }

        public UnableToCopyFile(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }

        public UnableToCopyFile(string iEntity)
            : base(iEntity)
        {
        }
    }


    public class UnableToCopyFile2 : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Warning"; }
        }

        public override string What
        {
            get { return "Unknown Error in copying file"; }
        }

        public UnableToCopyFile2(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToCopyFile2(List<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnableToDeleteFile : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Warning"; }
        }

        public override string What
        {
            get  { return "Unknown Error in deleting file"; }
        }

        public UnableToDeleteFile(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToDeleteFile(List<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }


    public class AlbumNotExported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get { return "Album Not Exported: Original Tracks not found"; }
        }

        public AlbumNotExported(List<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }




    public class FailedToImportFileEventArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "Failed To Import File"; }
        }

        public FailedToImportFileEventArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class FileAlreadyImported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get   { return "File already imported"; }
        }

        public FileAlreadyImported(string iEntity)
            : base(iEntity)
        {
        }

        public FileAlreadyImported(IEnumerable<ImportErrorItem> Corrupted)
            : base(Corrupted)
        {
        }
    }

    public class AlbumAlreadyImported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "Album already imported"; }
        }

        public AlbumAlreadyImported(string iEntity)
            : base(iEntity)
        {
        }

        public AlbumAlreadyImported(IEnumerable<ImportErrorItem> Corrupted)
            : base(Corrupted)
        {
        }
    }

    public class FileCorrupted : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get  { return "Unable to read file"; }
        }

        public FileCorrupted(string iEntity)
            : base(iEntity)
        {
        }

        public FileCorrupted(IEnumerable<ImportErrorItem> Corrupted)
            : base(Corrupted)
        {
        }
    }

    public class ExportFileError : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get  { return "Unable to export file"; }
        }

        public ExportFileError(string iEntity)
            : base(iEntity)
        {
        }

        public ExportFileError(IEnumerable<ImportErrorItem> Corrupted)
            : base(Corrupted)
        {
        }
    }

    public class MissingRarVolumeArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "Missing volume to export"; }
        }

        public MissingRarVolumeArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class FileNotFoundArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "File not found"; }
        }

        public FileNotFoundArgs(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class ItunesImportError : ImportExportError
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string Who
        {
            get  { return null; }
        }

        public override string What
        {
            get
            { return "Unable to import Itunes collections. Unexpected error"; }
        }

        public ItunesImportError()
        {
        }
    }

    public class ItunesCDNotFoundError : ImportExportError
    {
        public override string WindowName
        {
            get { return "iTunes Error"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public override string What
        {
            get { return "Unable to find CD in Itunes."; }
        }

        public ItunesCDNotFoundError()
        {
        }
    }


    public class ItunesUnknownError : ImportExportError
    {
        public override string WindowName
        {
            get { return "iTunes Error"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public override string What
        {
            get { return "Unable to get CD from Itunes. Unexpected error"; }
        }

        public ItunesUnknownError()
        {
        }
    }


    public class UnknowError : ImportExportError
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }


        public override string What
        {
            get { return "Unexpected error during import"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public UnknowError()
        {
        }
    }



    public class FileTooLongArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Unable to import file"; }
        }

        public override string What
        {
            get { return "Can not import file. File path would exceed 260 charateres"; }
        }

        public FileTooLongArgs(string iEntity)
            : base(iEntity)
        {
        }
    }


    public class UnableToCreateFile : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export Error"; }
        }

        public override string What
        {
            get { return "Unable to create file. Check target disk free available space."; }
        }

        public UnableToCreateFile(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToCreateFile(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnableToExtractFileFromRar : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Extract from Rar/Zip Error"; }
        }

        public override string What
        {
            get { return "Unable to extract file from Rar"; }
        }

        public UnableToExtractFileFromRar(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToExtractFileFromRar(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnableToExtractFileFromRarFileAlreadyExist : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Extract from Rar/Zip Error"; }
        }

        public override string What
        {
            get { return "Unable to extract file from Rar: File Already Exists"; }
        }

        public UnableToExtractFileFromRarFileAlreadyExist(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToExtractFileFromRarFileAlreadyExist(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnableToConvertFile : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get { return "Unable to convert file to mp3"; }
        }

        public UnableToConvertFile(string iEntity)
            : base(iEntity)
        {
        }

        public UnableToConvertFile(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class UnableToDowloadCoverArt : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Import Error"; }
        }

        public override string What
        {
            get
            { return "Unable to dowload artwork (maybe due to a slow internet connection). Please try later."; }
        }

        public UnableToDowloadCoverArt(string iEntity)
            : base(iEntity)
        {
        }
    }

    public class ZipNotImported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "Zip/Rar not imported into collection"; }
        }

        public ZipNotImported(string iEntity)
            : base(iEntity)
        {
        }

        public ZipNotImported(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class MccNotImported : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "Mcc not imported into collection"; }
        }

        public MccNotImported(string iEntity)
            : base(iEntity)
        {
        }

        public MccNotImported(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class ITunesNotResponding : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Impossible to connect to iTunes."; }
        }

        public override string What
        {
            get { return "Check if iTunes is installed.Alternativelly close all iTunes dialog(s) and try again."; }
        }

        public ITunesNotResponding()
            : base(string.Empty)
        {
        }
    }

    public class ITunesIPodPlaylistreadonly : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Impossible to synchronize your iPod."; }
        }

        public override string What
        {
            get  { return "Check if manual synchronization is allowed."; }
        }

        public ITunesIPodPlaylistreadonly()
            : base(string.Empty)
        {
        }
    }



    public class NotEnougthSpace : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Impossible to import music."; }
        }

        public override string What
        {
            get  { return "Not Enougth space on disk"; }
        }

        public NotEnougthSpace(string et)
            : base(et)
        {
        }
    }

    public class iPodNotFound : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Impossible to found iPod"; }
        }

        public override string What
        {
            get { return "Check iPod connection"; }
        }

        public iPodNotFound()
            : base(string.Empty)
        {
        }
    }


    public class ImpossibleToTransferMusicToIPod : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Export to iPod failed"; }
        }

        public override string What
        {
            get  { return "Check iPod available space"; }
        }

        public ImpossibleToTransferMusicToIPod(string iEntity)
            : base(iEntity)
        {
        }

        public ImpossibleToTransferMusicToIPod(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }


    public class FileAlreadyConverted : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "File already converted to mp3"; }
        }

        public FileAlreadyConverted(string iEntity)
            : base(iEntity)
        {
        }

        public FileAlreadyConverted(IEnumerable<ImportErrorItem> iEntity)
            : base(iEntity)
        {
        }
    }

    public class CorruptedRarOrMissingPasswordArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Rar or zip file not imported"; }
        }

        public override string What
        {
            get { return "File not imported maybe due to a missing password"; }
        }

        public bool accept { get; set; }

        public string Password { get; set; }

        public bool SavePassword { get; set; }

        public CorruptedRarOrMissingPasswordArgs(string iEntity, bool RarAddPasword)
            : base(iEntity)
        {
            accept = false;
            SavePassword = RarAddPasword;
        }
    }

    public class NoCDInsertedArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "No CD Inserted"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public NoCDInsertedArgs()
        {
        }
    }

    public class NoCDAudioInsertedArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get
            { return "No Audio CD Inserted"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public NoCDAudioInsertedArgs()
        {
        }
    }

    public class CDUnknownErrorArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Error"; }
        }

        public override string What
        {
            get { return "Unable to convert CD audio. Please check if any application is using the CD driver."; }
        }

        public override string Who
        {
            get { return null; }
        }

        public CDUnknownErrorArgs()
        {
        }
    }

    public class CDInUse : ImportExportError
    {
        public override string WindowName
        {
            get { return "Error"; }
        }

        public override string What
        {
            get {  return "CD in use by other application"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public CDInUse()
        {
        }
    }

    public class AmbigueousCDInformationArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get  { return "CD name ambiguity"; }
        }

        public override string Who
        {
            get  { return null; }
        }


        public bool Continue { get; set; }

        public IList<WebMatch<IFullAlbumDescriptor>> CDInfos { get; private set; }

        public IFullAlbumDescriptor Default { get; private set; }

        public IFullAlbumDescriptor SelectedInfo { get; set; }

        public WebProvider Provider { get; set; }

        public IList<WebMatch<IFullAlbumDescriptor>> PreprocessedWebInfo { get; set; }

        public AmbigueousCDInformationArgs(IList<WebMatch<IFullAlbumDescriptor>> iCDInfos, IFullAlbumDescriptor ifad)
        {
            CDInfos = iCDInfos;
            SelectedInfo = null;
            Continue = false;
            Default = ifad;
        }
    }

    public class CDCoverInformationArgs : ImportExportError
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        public override string What
        {
            get { return "Choose a cover art for album"; }
        }

        public override string Who
        {
            get { return null; }
        }

        public IList<WebMatch<IFullAlbumDescriptor>> CDInfos { get; private set; }

        public IFullEditableAlbumDescriptor Current { get; private set; }

        public CDCoverInformationArgs(IList<WebMatch<IFullAlbumDescriptor>> iCDInfos, IFullEditableAlbumDescriptor ifad)
        {
            CDInfos = iCDInfos;
            Current = ifad;
        }
    }

    public abstract class ConfirmationNeededEventArgs : ImportExportError
    {
        public bool Continue { get; set; }

        public override string WindowName
        {
            get { return "Confirmation needed"; }
        }

        protected ConfirmationNeededEventArgs()
        {
            Continue = false;
        }
    }


    public class CueWillbeDiscarded : ConfirmationNeededEventArgs
    {
        public override string WindowName
        {
            get { return "Cue file corrupted"; }
        }

        override public string What
        {
            get { return string.Format(@"Do you want to continue convertion without cue information?"); }
        }

        override public string Who
        {
            get { return string.Format(@"Cue File corrupted: ""{0}"" {1}File to convert: ""{2}""", _CN, Environment.NewLine, _FN); }
        }

        private string _CN;
        private string _FN;
        public CueWillbeDiscarded(string cueName, string fn)
            : base()
        {
            _CN = cueName;
            _FN = fn;
        }
    }



    public abstract class WhoConfirmationNeededEventArgs : ConfirmationNeededEventArgs
    {
        private string _Who;

        override public string Who
        {
            get { return _Who; }
        }

        protected WhoConfirmationNeededEventArgs(string Who)
        {
            _Who = Who;
        }

        protected WhoConfirmationNeededEventArgs(IEnumerable<string> Who)
        {
            StringBuilder SB = new StringBuilder();

            foreach (string s in Who)
            {
                SB.Append(s);
                SB.Append(Environment.NewLine);
            }

            _Who = SB.ToString();
        }
    }

    public class AmbigueousTrackImportArgs : ConfirmationNeededEventArgs
    {
        private List<ITrack> _Tracks;

        private string _Who;
        private string _Path;


        override public string Who
        {
            get { return _Who; }
        }

        public override string WindowName
        {
            get { return "Do you want to import this file?"; }
        }

        override public string What
        {
            get { return string.Format(@"Warning: the tracK: ""{0}"" may be already imported in collection:", _Path); }
        }

        internal AmbigueousTrackImportArgs(string Path, IEnumerable<ITrack> Tracks)
        {
            _Tracks = Tracks.ToList();
            _Path = Path;

            StringBuilder SB = new StringBuilder();


            SB.Append("Potential Duplicate(s) are:");
            SB.Append(Environment.NewLine);

            foreach (ITrack s in Tracks)
            {
                SB.Append(string.Format(@"Track:""{0}"" Path:""{1}""", s.Name, s.Path));
                SB.Append(Environment.NewLine);
            }

            _Who = SB.ToString();
        }

    }


    public class DeleteAssociatedFiles : WhoConfirmationNeededEventArgs
    {
        override public string What
        {
            get { return "Do you want to delete corresponding files?"; }
        }

        public DeleteAssociatedFiles(IEnumerable<string> Display)
            : base(Display)
        {
        }
    }

    public class OtherAlbumConfirmationNeededEventArgs : WhoConfirmationNeededEventArgs
    {
        private IAlbum _Album;

        override public string What
        {
            get { return "Another Album with the same name is already in the collection. Do you want to continue?"; }
        }

        public OtherAlbumConfirmationNeededEventArgs(IAlbum Display)
            : base(Display.ToString())
        {
            _Album = Display;
        }

        public override bool Equals(object obj)
        {
            OtherAlbumConfirmationNeededEventArgs am = obj as OtherAlbumConfirmationNeededEventArgs;

            if (am == null)
                return false;

            return Object.ReferenceEquals(_Album, am._Album);
        }

        public override int GetHashCode()
        {
            return _Album.GetHashCode();
        }

    }

    public class OtherAlbumsConfirmationNeededEventArgs : ConfirmationNeededEventArgs
    {
        private IEnumerable<MatchAlbum> _albums;
        private string _Who;


        override public string Who
        {
            get { return _Who; }
        }

        override public string What
        {
            get { return "This Album seems to be already in the collection. Do you want to continue?"; }
        }

        private IEnumerable<IAlbum> Albums
        {
            get { return from a in _albums select a.FindItem; }
        }

        internal OtherAlbumsConfirmationNeededEventArgs(IEnumerable<MatchAlbum> albums)
            : base()
        {
            _albums = albums.ToList();

            StringBuilder SB = new StringBuilder();


            SB.Append("Probable Duplicate:");
            SB.Append(Environment.NewLine);

            foreach (IAlbum s in Albums)
            {
                SB.Append(string.Format(@"Name: {0} Authour: {1}", s.Name, s.Author));
                SB.Append(Environment.NewLine);
            }

            _Who = SB.ToString();
        }

    }

    public class ConfirmItunesImport : ConfirmationNeededEventArgs
    {
        public override string WindowName
        {
            get { return "Warning"; }
        }

        override public string Who
        {
            get { return null; }
        }

        override public string What
        {
            get { return "You are about to import your Itunes collection. Do you want to continue?"; }
        }

        public ConfirmItunesImport()
        {
        }

    }



    public class UnknownNameChangedEventArgs : WhoImportErrorEventArgs
    {
        public override string WindowName
        {
            get { return "Unable to update album"; }
        }

        override public string What
        {
            get { return "Files removed or corrupted"; }
        }

        public UnknownNameChangedEventArgs(string Album)
            : base(Album)
        {
        }
    }


    public class CancelledImportEventArgs : ImportExportError
    {

        public CancelledImportEventArgs()
        {
        }

        public override string WindowName
        {
            get { return "Unable to import music"; }
        }

        public override string What
        {
            get { return "Operation has been cancelled"; }
        }
       
        public override string Who
        {
            get { return null; }
        }
    }

}
