using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{

    internal abstract class MusicImporterBuilder : NotifyCompleteListenerObject
    {
        //private static string _IsValidProperty = "IsValid";

        public AlbumMaturity DefaultAlbumMaturity { get; set; }

        protected IMusicImporterFactory Factory { get; private set; }

        internal MusicImporterBuilder(IInternalMusicSession Session)
        {
            Factory = LazyLoadingMusicImporter.GetFactory(Session);
        }

        static internal IMusicImporterBuilder GetFromType(IInternalMusicSession Session, MusicImportExportType typei)
        {
            switch (typei)
            {
                case MusicImportExportType.CD:
                    return new CDMusicImporterBuilder(Session);

                case MusicImportExportType.Compressed:
                    return new CompressedMusicImporterBuilder(Session);

                case MusicImportExportType.Custo:
                    return new CustoMusicImporterBuilder(Session);

                case MusicImportExportType.Directory:
                    return new DirectoryMusicImporterBuilder(Session);

                case MusicImportExportType.iTunes:
                    return new iTunesMusicImporterBuilder(Session);
            }

            throw new NotImplementedException();
        }
    }


    internal class iTunesMusicImporterBuilder : MusicImporterBuilder, IiTunesImporterBuilder
    {
        internal iTunesMusicImporterBuilder(IInternalMusicSession msi)
            : base(msi)
        {

            switch (msi.Setting.iTunesSetting.ImportBrokenTrack)
            {
                case BasicBehaviour.AskEndUser:
                    ImportBrokenTracks = null;
                    break;

                case BasicBehaviour.Yes:
                    ImportBrokenTracks = true;
                    break;

                case BasicBehaviour.No:
                    ImportBrokenTracks = false;
                    break;
            }

            ItunesDirectory = null;
        }


        private bool? _ImportBTracks;
        public bool? ImportBrokenTracks
        {
            get { return _ImportBTracks; }
            set { Set(ref _ImportBTracks , value); }
        }

        public MusicImportExportType Type
        {
            get { return MusicImportExportType.iTunes; }
        } 

        public bool IsValid
        {
            get { return Get<iTunesMusicImporterBuilder, bool>(() => (t) => (t.ImportBrokenTracks != null) && ((t.ItunesDirectory == null) || (Directory.Exists(t.ItunesDirectory)))); }
            //get { return (ImportBrokenTracks != null) && ((ItunesDirectory == null) || (Directory.Exists(ItunesDirectory))); }
        }

        private string _ItunesDirectory;
        public string ItunesDirectory
        {
            get { return _ItunesDirectory; }
            set { Set(ref _ItunesDirectory, value); }
        }

        public IMusicImporter BuildImporter()
        {
            if (!IsValid)
                return null;

            return Factory.GetITunesService((bool)ImportBrokenTracks, ItunesDirectory, DefaultAlbumMaturity);
        }
    }

    internal class CompressedMusicImporterBuilder : MusicImporterBuilder, IFilesImporterBuilder
    {
        static private readonly string _FileExtensions;

        private IInternalMusicSession _Session;
        internal CompressedMusicImporterBuilder(IInternalMusicSession msi)
            : base(msi)
        {
            _Session = msi;
        }

        static CompressedMusicImporterBuilder()
        {
            _FileExtensions = "Rar/Zip Files | " + FileServices.GetRarFilesSelectString();
        }

        public string FileExtensions
        {
            get { return _FileExtensions; }
        }

        private string[] _Files;
        public string[] Files
        {
            get { return _Files; }
            set { Set (ref _Files , value); }
        }

        public MusicImportExportType Type
        {
            get { return MusicImportExportType.Compressed; }
        }

        public bool IsValid
        {
            get { return Get<CompressedMusicImporterBuilder, bool>(() => (t) => (t.Files != null)); }
           //get { return Files != null; }
        }


        public IMusicImporter BuildImporter()
        {
            if (!IsValid)
                return null;

            return Factory.GetMultiRarImporter(Files, DefaultAlbumMaturity);
        }

        public string DefaultFolder
        {
            get { return _Session.Setting.PathUserSettings.PathRar; }
            set { _Session.Setting.PathUserSettings.PathRar = value; }
        }
    }

    internal class CustoMusicImporterBuilder : MusicImporterBuilder, ICustoFilesImporterBuilder
    {
        static private readonly string _FileExtensions;
        private IInternalMusicSession _Session;

        internal CustoMusicImporterBuilder(IInternalMusicSession msi)
            : base(msi)
        {
            _Session = msi;
        }

        public bool IsValid
        {
            get { return Get<CustoMusicImporterBuilder, bool>(() => (t) => (t.Files != null)); }
            //get { return Files != null; }
        }

        static CustoMusicImporterBuilder()
        {
            _FileExtensions = "Files | " + "*.xml;*.mcc";
        }


        public string FileExtensions
        {
            get { return _FileExtensions; }
        }

        public bool ImportAllMetaData { get; set; }

        private string[] _Files;
        public string[] Files
        {
            get { return _Files; }
            set { Set(ref _Files, value); }
        }

        public MusicImportExportType Type
        {
            get { return MusicImportExportType.Custo; }
        }


        public IMusicImporter BuildImporter()
        {
            return Factory.GetXMLImporter(Files, ImportAllMetaData, DefaultAlbumMaturity);
        }

        public string DefaultFolder
        {
            get { return _Session.Setting.PathUserSettings.PathCusto; }
            set { _Session.Setting.PathUserSettings.PathCusto = value; }
        }
    }

    internal class DirectoryMusicImporterBuilder : MusicImporterBuilder, IDirectoryImporterBuilder
    {
        private IInternalMusicSession _Session;
        internal DirectoryMusicImporterBuilder(IInternalMusicSession msi)
            : base(msi)
        {
            _Session = msi;
            _Directory = _Session.Setting.PathUserSettings.PathFolder;
        }

        private string _Directory;
        public string Directory
        {
            get { return _Directory; }
            set { if (Set(ref _Directory,value))  _Session.Setting.PathUserSettings.PathFolder = value;}
        }

        public bool IsValid
        {
            get { return Get<DirectoryMusicImporterBuilder, bool>(() => (t) => (t.Directory != null)); }
            //get { return Directory != null; }
        }

        public MusicImportExportType Type
        {
            get { return MusicImportExportType.Directory; }
        }

        public IMusicImporter BuildImporter()
        {
            return Factory.GetFileService(_Session, Directory, DefaultAlbumMaturity);
        }
    }

    internal class CDMusicImporterBuilder : MusicImporterBuilder, IMusicImporterBuilder
    {
        private IMusicImporterExporterUserSettings _IMusicImporterExporterUserSettings;
        internal CDMusicImporterBuilder(IInternalMusicSession msi)
            : base(msi)
        {
            _IMusicImporterExporterUserSettings = msi.Setting.MusicImporterExporter;
        }

        public bool IsValid
        {
            get { return true; }
        }

        public MusicImportExportType Type
        {
            get { return MusicImportExportType.CD; }
        }

        public bool OpenCDDoorOnComplete
        {
            //get { return Properties.Settings.Default.OpenCDDoorOnEndImport; }
            //set { Properties.Settings.Default.OpenCDDoorOnEndImport = value; }
            get { return _IMusicImporterExporterUserSettings.OpenCDDoorOnEndImport; }
            set { _IMusicImporterExporterUserSettings.OpenCDDoorOnEndImport = value; }  
        }

        public IMusicImporter BuildImporter()
        {
            return Factory.GetCDImporter(DefaultAlbumMaturity, OpenCDDoorOnComplete);
        }
    }
}
