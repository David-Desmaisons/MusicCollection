using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.ToolBox;
using MusicCollection.Infra;

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

        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
            switch (EI.State)
            { 
                case ImportState.OK:
                    bool RarC = GetTransactionContext() == ImportType.UnRar;
                    InFiles.Apply(mc =>   Context.ConvertManager.OnSourceConverted(mc, RarC));
                    //foreach (string MC in InFiles)
                    //    Context.ConvertManager.OnSourceConverted(MC, RarC);
                    break;

                case ImportState.NotFinalized:
                case ImportState.KO:
                    //je ne suis pas arrive a tenter les imports ou tous les import sont ko
                    //on clean tout      
                    Context.Folders.GetFileCleanerFromFiles(OutFilesFiles, n => false, false).Remove();
                    break;

                case ImportState.Partial:
                    if (EI.FilesNotimported.Any())
                    {
                        //add to file not imported file which convertion result in a not imported file
                        var notimportedfiles = InFiles.Where(t => ConvertedFiles(t).Any() && ConvertedFiles(t).All(f => EI.FilesNotimported.Contains(f)));
                        notimportedfiles.Apply( to => EI.AddTrackKODuringImport(to));
                    
                        //foreach(string to in from t in InFiles where ( ConvertedFiles(t).Any() && ConvertedFiles(t).All(f=>EI.FilesNotimported.Contains(f))) select t)
                        //{
                        //    EI.AddTrackKODuringImport(to);
                        //}
                        Context.Folders.GetFileCleanerFromFiles(EI.FilesNotimported, n => false, false).Remove();
                    }
                    break;
            }

        }
    }
}
