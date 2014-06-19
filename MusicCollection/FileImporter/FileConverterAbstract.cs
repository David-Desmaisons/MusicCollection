using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.ToolBox;

namespace MusicCollection.FileImporter
{
    
    internal abstract class FileConverterAbstract: ImporterConverterAbstract, IImporter
    {
        protected override bool ExpectedNotNullResult
        {
            get { return true; }
        }
    
        public override ImportType Type
        {
            get { return ImportType.Convertion; }
        }

        private Dictionary<string, List<string>> _Correspondance = new Dictionary<string, List<string>>();

        protected void AddConvertedFiles(string Infile,string outfile)
        {
            List<string> l = null;

            if (!_Correspondance.TryGetValue(Infile,out l))
            {
               
                l = new List<string>();
                _Correspondance.Add(Infile,l);
            }

            l.Add(outfile);
        }

        private IEnumerable<string> ConvertedFiles(string inFile)
        {
            List<string> l = null;

            if (!_Correspondance.TryGetValue(inFile, out l))
            {
                return Enumerable.Empty<string>();
            }

            return l;
        }


        protected override IEnumerable<string> OutFilesFiles
        {
            get { return  from t in _Correspondance.Values from s in t select s; }
        }

        //protected bool CheckSize(IEnumerable<string> Filesource, string iDertarget)
        //{
        //    string drive = Path.GetPathRoot(iDertarget);
        //    long sad = FileInternalToolBox.AvailableFreeSpace(drive);
        //    long sn = (from f in Filesource select new FileInfo(f).Length).Sum();

        //    if (sn > sad)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
            if (EI.State==ImportState.OK)
            {
                bool RarC = GetTransactionContext() == ImportType.UnRar;
                foreach (string MC in InFiles)
                    Context.ConvertManager.OnSourceConverted(MC, RarC);
                return;
            }

            //je ne suis pas arrive a tenter les imports ou tous les import sont ko
            //on clean tout
            if ((EI.State == ImportState.NotFinalized) || (EI.State == ImportState.KO))
            {
                Context.Folders.GetFileCleanerFromFiles(OutFilesFiles, n => false, false).Remove();
                //FileCleaner.FromFiles(OutFilesFiles, n => false, false).Remove();
                return;
            }

            //ImportState partiel

            if (EI.FilesNotimported.Any())
            {
                //add to file not imported file which convertion result in a not imported file
                foreach(string to in from t in InFiles where ( ConvertedFiles(t).Any() && ConvertedFiles(t).All(f=>EI.FilesNotimported.Contains(f))) select t)
                {
                    EI.AddTrackKODuringImport(to);
                }
                Context.Folders.GetFileCleanerFromFiles(EI.FilesNotimported, n => false, false).Remove();
                //FileCleaner.FromFiles(EI.FilesNotimported, n => false, false).Remove();
                return;
            }

        }
    }
}
