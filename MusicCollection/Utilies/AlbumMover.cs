using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.Utilies
{
    internal class AlbumMover : UIThreadSafeImportEventAdapter, IMusicFileExporter
    {
        private IInternalMusicSession _msi;
        internal AlbumMover(IInternalMusicSession msi)
        {
            _msi = msi;
        }

        public string FileDirectory
        {
            get;
            set;
        }

        private void Export()
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
                    Context.Error += (o, e) => OnError(e);

                    OnProgress(new BeginningMove());

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

                    OnProgress(new EndExport(_Alls));
                }
            }

           
        }

        public void Export(bool Sync)
        {
            if (Sync)
                Export();
            else
            {
                Action Ac = () => Export();
                Ac.BeginInvoke(null, null);
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
