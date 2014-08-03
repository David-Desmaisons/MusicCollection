using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using System.Threading.Tasks;
using System.Threading;
using MusicCollection.Exporter;

namespace MusicCollection.Utilies
{
    internal class AlbumMover : ExporterAdaptor, IMusicFileExporter
    {
        private IInternalMusicSession _msi;
        internal AlbumMover(IInternalMusicSession msi)
        {
            _msi = msi;
        }

        public string FileDirectory { get; set; }

        protected override void PrivateExport(IImportExportProgress iIImportExportProgress, CancellationToken? iCancellationToken)
        {
            if (AlbumToExport == null)
                return;

            if (AlbumToExport.Count() == 0)
                return;

            if (!Directory.Exists(FileDirectory))
                return;
             
            using (IImportContext Context = _msi.GetNewSessionContext())
            {
                using (Context.SessionLock())
                {
                    Context.Error += (o, e) => iIImportExportProgress.SafeReport(e);

                    iIImportExportProgress.SafeReport(new BeginningMove());

                    using (IMusicTransaction imut = Context.CreateTransaction())
                    {
                        bool res = false;

                        AlbumToExport.Apply(a => { bool r = (a as Album).Reroot(FileDirectory, Context, true); if (r) res = true; });

                        if (res)
                        {
                            imut.Commit();
                        }
                        else
                            imut.Cancel();
                    }

                    iIImportExportProgress.SafeReport(new EndExport(_Alls));
                }
            } 
        }

        private IList<IAlbum> _Alls;
        public IEnumerable<IAlbum> AlbumToExport
        {
            get { return _Alls; }
            set { _Alls = (value == null) ? null : value.ToList(); }
        }


    }
}
