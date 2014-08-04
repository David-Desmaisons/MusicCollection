using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.FileConverter;
using System.Threading;


namespace MusicCollection.FileImporter
{
    internal class RarImporter : ImporterConverterAbstract, IImporter
    {
        private string _FileName;
        private List<string> _RarFileNames;
        private List<string> _ExtractedFiles;
        private IInternalMusicSession _IInternalMusicSession;

        internal RarImporter(IInternalMusicSession iconv, string FileName)
        {
            _IInternalMusicSession = iconv;
            _FileName = FileName;
            _RarFileNames = new List<string>();
            _ExtractedFiles = new List<string>();
            _RarFileNames.Add(FileName);
        }

        override protected bool ExpectedNotNullResult
        {
            get { return true; }
        }

        protected override IEnumerable<string> InFiles
        {
            get { return _RarFileNames; }
        }

        protected override IEnumerable<string> OutFilesFiles
        {
            get { return _ExtractedFiles; }
        }

        protected override void OnEndImport(EndImport EI)
        {
            Context.RarManager.OnUnrar(_RarFileNames, ((EI.State == ImportState.OK) || (EI.State == ImportState.Partial)));


            if (EI.State == ImportState.OK)
            {
                return;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            
            if ((EI.State == ImportState.NotFinalized) || (EI.State == ImportState.KO))
            {
                //je ne suis pas arrive a tenter les imports ou tous les import sont ko
                //on clean tout
                Context.Folders.GetFileCleanerFromFiles(OutFilesFiles, n => false, false).Remove();
                //FileCleaner.FromFiles(OutFilesFiles, n => false, false).Remove();
                return;
            }

            //ImportState partiel

            if (EI.FilesNotimported.Any())
            {
                Context.Folders.GetFileCleanerFromFiles(from t in OutFilesFiles where EI.FilesNotimported.Contains(t) select t, n => false, false).Remove();
                return;
            }   
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
         {
 
            string dp = Path.GetFileName(_FileName);
            iel.Report(new ExtractProgessEventArgs(dp));

            ImporterConverterAbstract next = null;            

            try
            {

                IRarDescompactor Sex = Context.RarManager.InstanciateExctractorWithPassword(_FileName, iel);


                if (Sex == null)
                {
                    return next;
                }

               

                using (Sex)
                {

                    Sex.DescompactedFiles = _ExtractedFiles;

                    bool res = Sex.Extract(iel);

                    _RarFileNames =Sex.ArchiveNames;

                    if (res)
                    {

                        NonRecursiveFolderInspector nfr = new NonRecursiveFolderInspector(_IInternalMusicSession,_ExtractedFiles, Sex.Helper, iel);
                        ImporterConverterAbstract[] Importers = nfr.Importers;

                        if (Importers.Length == 0)
                        {
                            iel.Report(new NoMusicImportErrorEventArgs(Sex.Helper.DisplayName));
                        }
                        else if (Importers.Length > 1)
                        {
                            Trace.WriteLine("Unhandled configuration in a rar file");
                            iel.Report(new UnhandledRarFile(Sex.Helper.DisplayName));
                        }
                        else
                            next = Importers[0];
                    }
                }
            }
            catch(Exception e)
            {
                iel.Report(new UnknownRarError(dp));
                Trace.WriteLine("Decompressing problem " + e.ToString());
                next = null;
            }

            return next;
        }

        
        override public  ImportType Type
        {
            get { return ImportType.UnRar; }
        }

    }
}
