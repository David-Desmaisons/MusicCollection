﻿using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicCollectionWPF.ViewModel
{
    public class ImporterViewModel : ViewModelBase
    {
        public IMusicImporter Importer { get; private set; }

        private IMusicSession _Session;
        private IMusicSettings _IMusicSettings;

        public ImporterViewModel(IMusicSession iSession)
        {
            Continue = false;
            _Session = iSession;
            _IMusicSettings = _Session.Setting;
            Option = _IMusicSettings.MusicImporterExporter.LastImportType;

            UpdateBuilder();

            OK = Register(RelayCommand.Instanciate(
                Commit,
                () => (Builder != null) && (Builder.IsValid)
                ));
        }

        public bool Continue { get; private set; }

        private void UpdateBuilder()
        {
            Builder = _Session.GetImporterBuilder(Option);
        }

        private IMusicImporterBuilder _Builder = null;
        public IMusicImporterBuilder Builder
        {
            get { return _Builder; }
            set { Set(ref _Builder, value); }
        }

        private MusicImportType _Option;
        public MusicImportType Option
        {
            get { return _Option; }
            set
            {
                if (Set(ref _Option, value))
                    UpdateBuilder();
            }
        }

        public ICommand OK { get; private set; }

        private void Commit()
        {
            Importer = Builder.BuildImporter();

            _IMusicSettings.MusicImporterExporter.LastImportType = Option;
            if (Option == MusicImportType.iTunes)
            {
                _IMusicSettings.iTunesSetting.ImportBrokenTrack = ((Builder as IiTunesImporterBuilder).ImportBrokenTracks == true) ?
                     BasicBehaviour.Yes : BasicBehaviour.No;
            }

            Continue = true;

            Window.Close();
        }

        public string Directory
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IDirectoryImporterBuilder) == false) ? null : (t.Builder as IDirectoryImporterBuilder).Directory); }
            set { if (!(Builder is IDirectoryImporterBuilder)) return; (Builder as IDirectoryImporterBuilder).Directory = value; }
        }

        #region IFilesImporterBuilder

        public string DefaultFolder
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).DefaultFolder); }
            set { if (!(Builder is IFilesImporterBuilder)) return; (Builder as IFilesImporterBuilder).DefaultFolder = value; }
        }

        public string[] Files
        {
            get { return Get<ImporterViewModel, string[]>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).Files); }
            set { if (!(Builder is IFilesImporterBuilder)) return; (Builder as IFilesImporterBuilder).Files = value; }
        }

        public string FileExtensions
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).FileExtensions); }
        }

        #endregion

        public bool OpenCDDoorOnComplete
        {
            get { return Get<ImporterViewModel, bool>(() => (t) => ((t.Builder is ICDImporterBuilder) == false) ? false : (t.Builder as ICDImporterBuilder).OpenCDDoorOnComplete); }
            set { if (!(Builder is ICDImporterBuilder)) return; (Builder as ICDImporterBuilder).OpenCDDoorOnComplete = value; }
        }
       
        public bool? ImportBrokenTracks
        {
            get { return Get<ImporterViewModel,bool?>(() => (t) => ((t.Builder is IiTunesImporterBuilder)==false) ? new Nullable<bool>() : (t.Builder as IiTunesImporterBuilder).ImportBrokenTracks); }
            set { if (!(Builder is IiTunesImporterBuilder)) return; (Builder as IiTunesImporterBuilder).ImportBrokenTracks = value; }
        }

        public bool ImportAllMetaData
        {
            get { return Get<ImporterViewModel, bool>(() => (t) => ((t.Builder is ICustoFilesImporterBuilder) == false) ? false : (t.Builder as ICustoFilesImporterBuilder).ImportAllMetaData); }
            set { if (!(Builder is ICustoFilesImporterBuilder)) return; (Builder as ICustoFilesImporterBuilder).ImportAllMetaData = value; }
        }
    }
}
