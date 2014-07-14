using MusicCollection.Fundation;
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

        private MusicImportExportType _Option;
        public MusicImportExportType Option
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
            Importer = _Builder.BuildImporter();

            _IMusicSettings.MusicImporterExporter.LastImportType = Option;
            if (Option == MusicImportExportType.iTunes)
            {
                _IMusicSettings.iTunesSetting.ImportBrokenTrack = ((_Builder as IiTunesImporterBuilder).ImportBrokenTracks == true) ?
                     BasicBehaviour.Yes : BasicBehaviour.No;
            }

            Continue = true;

            Window.Close();
        }

        public string Directory
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IDirectoryImporterBuilder) == false) ? null : (t.Builder as IDirectoryImporterBuilder).Directory); }
            set { (Builder as IDirectoryImporterBuilder).Directory = value; }
        }

        #region IFilesImporterBuilder

        public string DefaultFolder
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).DefaultFolder); }
            set { (Builder as IFilesImporterBuilder).DefaultFolder = value; }
        }

        public string[] Files
        {
            get { return Get<ImporterViewModel, string[]>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).Files); }
            set { (Builder as IFilesImporterBuilder).Files = value; }
        }

        public string FileExtensions
        {
            get { return Get<ImporterViewModel, string>(() => (t) => ((t.Builder is IFilesImporterBuilder) == false) ? null : (t.Builder as IFilesImporterBuilder).FileExtensions); }
        }

        #endregion

        public bool OpenCDDoorOnComplete
        {
            get { return Get<ImporterViewModel, bool>(() => (t) => ((t.Builder is ICDImporterBuilder) == false) ? false : (t.Builder as ICDImporterBuilder).OpenCDDoorOnComplete); }
            set { (Builder as ICDImporterBuilder).OpenCDDoorOnComplete = value; }
        }
       
        public bool? ImportBrokenTracks
        {
            get { return Get<ImporterViewModel,bool?>(() => (t) => ((t.Builder is IiTunesImporterBuilder)==false) ? new Nullable<bool>() : (t.Builder as IiTunesImporterBuilder).ImportBrokenTracks); }
            set { (Builder as IiTunesImporterBuilder).ImportBrokenTracks = value; }
        }

        public bool ImportAllMetaData
        {
            get { return Get<ImporterViewModel, bool>(() => (t) => ((t.Builder is ICustoFilesImporterBuilder) == false) ? false : (t.Builder as ICustoFilesImporterBuilder).ImportAllMetaData); }
            set { (Builder as ICustoFilesImporterBuilder).ImportAllMetaData = value; }
        }
    }
}
