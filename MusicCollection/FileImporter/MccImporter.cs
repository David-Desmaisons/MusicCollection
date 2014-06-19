using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;


namespace MusicCollection.FileImporter
{
    internal class MccImporter : ImporterConverterAbstract, IImporter
    {
        private string _FN;
        private bool _ImportAllMetaData;
        //private string _Dest;

        internal MccImporter(string Filename, bool ImportAllMetaData)
        {
            _FN = Filename;
            _ImportAllMetaData = ImportAllMetaData;
        }


        protected override bool ExpectedNotNullResult
        {
            get { return true; }
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel)
        {
            ImporterConverterAbstract next = null;            

            try
            {
                //string dest = FileInternalToolBox.CreateNewAvailableName(Path.ChangeExtension(_FN, MusicExporter.RealExtName));
                //File.Move(_FN, dest);

                //_Dest = dest;
                //_Dest = _FN;

                IMccDescompactor Sex = Context.RarManager.InstanciateExctractor(_FN, iel,Context);

                if (Sex == null)
                    return null;         

                using (Sex)
                {
                    if (Sex.Extract(iel) == false)
                        return next;
                } 
                
                OutPutFiles = Sex.DescompactedFiles;

                XMLImporter xxml = new XMLImporter(Sex.RootXML, _ImportAllMetaData, Context.Folders.File);// FileHelper.GetMusicFileDirectoty());
                xxml.Rerooter = Sex.Rerooter;
                next = xxml;
            }
            catch (Exception e)
            {
                iel.OnError(new UnknownRarError(_FN));
                Trace.WriteLine("Decompressing problem " + e.ToString());
                next = null;
            }

            return next;
        }

        public override ImportType Type
        {
            get { return ImportType.UnRar; }
        }

        private List<string>  _OutPutFiles;
        private List<string> OutPutFiles
        {
            get { return _OutPutFiles; }
            set { _OutPutFiles=value; }
        }

        protected override IEnumerable<string> InFiles
        {
            get { yield return _FN; }
        }

        protected override IEnumerable<string> OutFilesFiles
        {
            get { return _OutPutFiles; }
        }


        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
            // File.Move(_Dest, _FN);

            Context.RarManager.OnUnrar(_FN, ((EI.State == ImportState.OK) || (EI.State == ImportState.Partial)));


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
                //FileCleaner.FromFiles(from t in OutFilesFiles where EI.FilesNotimported.Contains(t) select t, n => false, false).Remove();
                return;
            }   
        }
    }
}
