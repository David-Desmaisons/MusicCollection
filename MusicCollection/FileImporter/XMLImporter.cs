using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using System.Threading;

namespace MusicCollection.FileImporter
{
    internal class XMLImporter : FinalizerConverterAbstract, IImporter
    {
        private string _FN;
        private string _Reroot;
        private bool _ImportAllMetaData;

        internal XMLImporter(string fn, bool ImportAllMetaData ,string Reroot = null)
        {
            _FN = fn;
            _Reroot = Reroot;
            _ImportAllMetaData = ImportAllMetaData;
        }

      
        public override ImportType Type
        {
            get { return ImportType.Import; }
        }

        protected override bool ExpectedNotNullResult
        {
            get { return false; }
        }

        internal IDictionary<string, string> Rerooter { get; set; }

        protected override IEnumerable<string> InFiles
        {
            get { yield return _FN; }
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            iel.Report(new ImportProgessEventArgs(_FN));

            IList<AlbumDescriptor> Als = AlbumDescriptorExchanger.Import(_FN, true, _Reroot, Rerooter);

            if (Als == null)
            {
                iel.Report(new UnsupportedFormat(_FN));
                return null;
            }

            Als.Apply(al => Album.GetAlbumFromExportAlbum(al, Context, this, _ImportAllMetaData));

            ImportEnded();

            return null;
        }

        protected override void OnEndImport(ImporterConverterAbstract.EndImport EI)
        {
        }
    }



}
