using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.Implementation;
using MusicCollection.Properties;
using MusicCollection.Fundation;
using MusicCollection.FileImporter;


namespace MusicCollection.SettingsManagement
{
    internal class ConvertManagerImpl : FileChangeManager, IConvertManager
    {
        private ConvertFileBehaviour _FileCreatedByConvertion;
        private PartialFileBehaviour _SourceFileUsedForConvertion;
        private PartialFileBehaviour _ConvertedFileExtractedFromRar;
        private Func<string, IImportHelper, string> _ComputeTargetName;

        //internal ConvertManagerImpl(IImportContext Ms, ConvertFileBehaviour iFileCreatedByConvertion, PartialFileBehaviour iSourceFileUsedForConvertion, 
        //    PartialFileBehaviour iConvertedFileExtractedFromRar)
        internal ConvertManagerImpl(IImportContext Ms, IConverterUserSettings iConverterUserSettings)

            : base(Ms)
        {
            _FileCreatedByConvertion = iConverterUserSettings.FileCreatedByConvertion;
            _SourceFileUsedForConvertion = iConverterUserSettings.SourceFileUsedForConvertion;
            _ConvertedFileExtractedFromRar = iConverterUserSettings.ConvertedFileExtractedFromRar;

            if (_FileCreatedByConvertion == ConvertFileBehaviour.SameFolder)
                _ComputeTargetName = (fn, h) => Path.GetDirectoryName(fn);
            else _ComputeTargetName = (fn, h) => ComputeName(fn, h);

        }

        private Func<string, bool> Action(PartialFileBehaviour pfb,bool rarcontext)
        {
            switch (pfb)
            {
                case PartialFileBehaviour.DoNothing:
                    return (fn) => true;

                case PartialFileBehaviour.Delete:
                    return (fn) => Delete(fn, rarcontext==false);

              }

            throw new Exception("Algo Error");
        }

        private string ComputeName(string OriginalFileName, IImportHelper Cue)
        {
            string Directory = Context.Folders.GetMusicFolder(OriginalFileName);

            if (Directory != null)
                return Directory;

            return ComputeName(Cue);

        }
        public void OnSourceConverted(string FileName, bool Rarcontext)
        {
            bool res = Rarcontext ? Action(_ConvertedFileExtractedFromRar, Rarcontext)(FileName) : Action(_SourceFileUsedForConvertion, Rarcontext)(FileName);
            if (res == false)
                Trace.WriteLine("music convertion clean-up problem");     
        }

        public string PathFromOutput(string OriginalFileName, IImportHelper Cue)
        {
            return _ComputeTargetName(OriginalFileName, Cue);
        }
    }
}
