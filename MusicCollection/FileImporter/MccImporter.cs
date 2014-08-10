using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using System.Threading;


namespace MusicCollection.FileImporter
{
    internal class MccImporter : ImporterConverterAbstract, IImporter
    {
        private string _FN;
        private bool _ImportAllMetaData;

        internal MccImporter(string Filename, bool ImportAllMetaData)
        {
            _FN = Filename;
            _ImportAllMetaData = ImportAllMetaData;
        }


        protected override bool ExpectedNotNullResult
        {
            get { return true; }
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            ImporterConverterAbstract next = null;

            try
            {
                IMccDescompactor Sex = Context.RarManager.InstanciateExctractor(_FN, iel, Context);

                if (Sex == null)
                    return null;

                using (Sex)
                {
                    if (Sex.Extract(iel) == false)
                        return next;
                }

                OutPutFiles = Sex.DescompactedFiles;

                XMLImporter xxml = new XMLImporter(Sex.RootXML, _ImportAllMetaData, Context.Folders.File);
                xxml.Rerooter = Sex.Rerooter;
                next = xxml;
            }
            catch (Exception e)
            {
                iel.Report(new UnknownRarError(_FN));
                Trace.WriteLine("Decompressing problem " + e.ToString());
                next = null;
            }

            return next;
        }

        public override ImportType Type
        {
            get { return ImportType.UnRar; }
        }

        private List<string> _OutPutFiles;
        private List<string> OutPutFiles
        {
            get { return _OutPutFiles; }
            set { _OutPutFiles = value; }
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
            Context.RarManager.OnUnrar(_FN, ((EI.State == ImportState.OK) || (EI.State == ImportState.Partial)));

            GC.Collect();
            GC.WaitForPendingFinalizers();

            switch(EI.State)
            {
                case ImportState.OK:
                    break;

                case ImportState.NotFinalized:
                case ImportState.KO:
                    //je ne suis pas arrive a tenter les imports ou tous les import sont ko
                    //on clean tout
                    Context.Folders.GetFileCleanerFromFiles(OutFilesFiles, n => false, false).Remove();
                    break;

                case ImportState.Partial:
                    //ImportState partiel
                    if (EI.FilesNotimported.Any())
                    {
                        Context.Folders.GetFileCleanerFromFiles(OutFilesFiles.Where(t=> EI.FilesNotimported.Contains(t)), n => false, false).Remove();
                    }   
                    break;
            }
        }
    }
}
