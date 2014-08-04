using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using System.Threading;

namespace MusicCollection.FileImporter
{
    internal abstract class ImporterConverterAbstract : IImporter
    {
        internal enum ImportState {OK,KO,Partial,NotFinalized};

        protected class EndImport
        {
            internal ImportState State
            {
                get 
                {
                    if (!FilesImported.Any())
                        return (FilesNotimported.Any() ? ImportState.KO : ImportState.NotFinalized);

                    return (FilesNotimported.Any()? ImportState.Partial : ImportState.OK);
                }
            }


            private List<string> _FilesImported;
            private IEnumerable<string> FilesImported
            {
                get { return _FilesImported ?? Enumerable.Empty<string>(); }
            }


            private List<string> _FilesAlreadyInCollection;
            private IEnumerable<string> FilesAlreadyInCollection
            {
                get { return _FilesAlreadyInCollection ?? Enumerable.Empty<string>(); }
            }

            private List<string> _FilesKODuringImport;
            private IEnumerable<string> FilesKODuringImport
            {
                get { return _FilesKODuringImport ?? Enumerable.Empty<string>(); }
            }

            internal void AddTrackAlreadyInCollection(string iaic)
            {
                if (_FilesAlreadyInCollection==null)
                     _FilesAlreadyInCollection = new List<string>();

                _FilesAlreadyInCollection.Add(iaic);
            }

            internal void AddTrackKODuringImport(string iaic)
            {
                if (_FilesKODuringImport == null)
                    _FilesKODuringImport = new List<string>();

                _FilesKODuringImport.Add(iaic);
            }

            internal void AddTrackImported(string iaic)
            {
                if (_FilesImported == null)
                    _FilesImported = new List<string>();

                _FilesImported.Add(iaic);
            }

          

            internal IEnumerable<string> FilesNotimported
            {
                get
                {
                    return FilesAlreadyInCollection.Concat(FilesKODuringImport);
                }
            }

            internal ImportType LastOperation
            {
                get;
                private set;
            }

            internal EndImport(ImportType iLastOperation)
            {
                LastOperation = iLastOperation;
            }

            static internal EndImport KO(ImportType iLastOperation)
            {
                return new EndImport(iLastOperation);
            }

        }

        private EndImport KOEndImport()
        {
            return EndImport.KO(Type);
        }

        private ImporterConverterAbstract Previous
        {
            get;
            set;
        }

        abstract protected bool ExpectedNotNullResult
        {
            get;
        }

        private IEnumerable<ImporterConverterAbstract> Previouses
        {
            get
            {
                ImporterConverterAbstract p = this;
                while (p != null)
                {
                    yield return p;
                    p = p.Previous;
                }

                yield break;
            }
        }

        protected ImportType GetTransactionContext()
        {
            return Previouses.Last().Type;
        }


        public IImportContext Context
        {
            get;
            set;
        }

        internal ImportType? Next
        {
            get;
            private set;
        }

        abstract protected ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken);

        public IImporter Action(IEventListener iel, CancellationToken iCancellationToken)
        {
            ImporterConverterAbstract next = GetNext(iel, iCancellationToken);

            if (next == null)
            {
                if (ExpectedNotNullResult)
                    RawImportEnded(KOEndImport());
            }
            else
            {
                next.Previous = this;
                this.Next = next.Type;
            }

            return next;
        }

        abstract public ImportType Type { get; }

        abstract protected void OnEndImport(EndImport EI);

        //protected void ImportEnded()
        //{
        //    ImportEnded();
        //}

        protected void RawImportEnded(EndImport EI)
        {

            foreach (ImporterConverterAbstract p in Previouses)
            {
                p.OnEndImport(EI);
            }
        }


        protected abstract IEnumerable<string> InFiles
        {
            get;
        }

        protected abstract IEnumerable<string> OutFilesFiles
        {
            get;
        }
    }


    internal abstract class FinalizerConverterAbstract : ImporterConverterAbstract,ITrackStatusVisitor
    {
        private EndImport _EI = null;

        private EndImport EndImporter
        {
            get { if (_EI == null) _EI = new EndImport(Type); return _EI; }
        }

        protected void ImportEnded()
        {
            RawImportEnded(EndImporter);
        }

        protected override IEnumerable<string> OutFilesFiles
        {
            get { return Enumerable.Empty<string>(); }
        }

        public void Visit(string Path, TrackStatus ts)
        {
            if (ts == null)
            {
                EndImporter.AddTrackKODuringImport(Path);
                return;
            }

            if (ts.Continue)
            {
                EndImporter.AddTrackImported(Path);
                return;
            }

            if ((ts.AbortAlbumExist) || (ts.Found != null))
            {
                EndImporter.AddTrackAlreadyInCollection(Path);
                return;
            }

            EndImporter.AddTrackKODuringImport(Path);
        }
    }
}
