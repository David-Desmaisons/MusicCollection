using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using iTunesLib;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;

namespace MusicCollection.Itunes
{
    internal class ITunesExporter : UIThreadSafeImportEventAdapter, IItunesExporter
    {
        private IImportContext _IIC;
        private IInternalMusicSession _MSI;

        internal ITunesExporter(IInternalMusicSession MSI)
        {
            _IIC = MSI.GetNewSessionContext();
            _IIC.Error += ((o, e) => OnError(e));
            _MSI = MSI;
        }

        #region event

        //private UISafeEvent<ImportExportErrorEventArgs> _Error;

        //public event EventHandler<ImportExportErrorEventArgs> Error
        //{
        //    add { _Error.Event += value; }
        //    remove { _Error.Event -= value; }
        //}

        //protected void OnError(ImportExportErrorEventArgs Error)
        //{
        //    _Error.Fire(Error, true);
        //}

        #endregion


        public void Synchronize(bool DeleteBrokenItunes)
        {
            Action Ac = () => PrivateSynchronize(DeleteBrokenItunes);
            Ac.BeginInvoke(null, null);
        }

        private IEnumerable<IAlbum>  _AlbumToExport;
        public IEnumerable<IAlbum> AlbumToExport
        {
            set { _AlbumToExport = value; }
            get { return _AlbumToExport; }
        }

        private bool _Exporttoipod;
        public bool Exporttoipod
        {
            set { _Exporttoipod = value; }
            get { return _Exporttoipod; }
        }

        public void Export(bool Sync)
        {
            if (Sync)
                PrivateExportToItunes();
            else
            {
                Action Ac = () => PrivateExportToItunes();
                Ac.BeginInvoke(null, null);
            }
        }


        private void PrivateSynchronize(bool DeleteBrokenItunes)
        {
            try
            {
                var MyMusicCollect = (from al in _MSI.Albums from t in al.Tracks let fi = new FileInfo(t.Path) where fi.Exists select fi.FullName ).ToList();

                iTunesApp iTunesApp = new iTunesApp();

                if (iTunesApp == null)
                    return;

                IITLibraryPlaylist mainLibrary = iTunesApp.LibraryPlaylist;

                OnProgress(new ITunesIdentifyingProgessEventArgs());

                var ItunesCollect = (from track in mainLibrary.Tracks.Cast<IITTrack>()
                                     let filetrack = track as IITFileOrCDTrack
                                     where ((filetrack != null) && (filetrack.Kind == ITTrackKind.ITTrackKindFile))
                                     select new { Location = filetrack.Location, TunesTrack = filetrack }).ToList();

                var cleanitunescollect = (from tunetrack in ItunesCollect where tunetrack.Location != null select tunetrack.Location).ToHashSet();
                var hashedcollect = MyMusicCollect.ToHashSet();

                var ToAdd = MyMusicCollect.Where(path => (!cleanitunescollect.Contains(path)) && (Path.GetExtension(path) != ".wma"));
                //from path in MyMusicCollect where  select path;

                var ToRemove = from tunetrack in ItunesCollect
                               let rem = (DeleteBrokenItunes ? ((tunetrack.Location == null) || !(hashedcollect.Contains(tunetrack.Location))) : ((tunetrack.Location != null) && !(hashedcollect.Contains(tunetrack.Location))))
                               where rem
                               select tunetrack.TunesTrack;

                OnProgress(new ITunesExportingProgessEventArgs(0));

                foreach (string res in ToAdd)
                {
                    try
                    {
                        mainLibrary.AddFile(res);
                    }
                    catch(Exception e)
                    {
                        Trace.WriteLine(string.Format("Export error to itunes {0}", e));
                        _IIC.OnFactorisableError<ExportFileError>(res);
                    }
                }

                foreach (IITFileOrCDTrack res in ToRemove)
                {
                    res.Delete();
                }

                OnProgress(new ITunesExportingProgessEventArgs(100));

                OnProgress(new EndExport("Collection Synchronised with iTunes"));

            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem to connect to iPod "+e.ToString());
                OnError(new ITunesNotResponding());

            }
        }

        private void PrivateExportToItunes()
        {
            try
            {
                iTunesApp iTunesApp = new iTunesApp();

                if (iTunesApp == null)
                    return;

                IITLibraryPlaylist mainLibrary = iTunesApp.LibraryPlaylist;

                List<IITTrack> list = new List<IITTrack>();

                foreach (string p in from al in AlbumToExport from tr in al.Tracks select tr.Path)
                {
                    IITOperationStatus iops = mainLibrary.AddFile(p);
                    list.AddCollection(iops.Tracks.Cast<IITTrack>());
                }

                if (!Exporttoipod)
                    return;

                bool Transfer = false;

                IITSourceCollection _sources = iTunesApp.Sources;
                foreach (IITSource s in _sources)
                {
                    if (s.Kind == ITSourceKind.ITSourceKindIPod)
                    {
                        Transfer = true;

                        IITPlaylist l = s.Playlists.get_ItemByName(s.Name);

                        IITLibraryPlaylist lpl = l as IITLibraryPlaylist;

                        foreach (IITTrack track in list)
                        {
                            IITTrack itt = lpl.AddTrack(track);
                            if (itt == null)
                                _IIC.OnFactorisableError<ImpossibleToTransferMusicToIPod>(track.Name);
                        }

                        IITIPodSource ipod = s as IITIPodSource;
                        if (ipod != null)
                            ipod.UpdateIPod();

                        break;
                    }
                }

                if (Transfer == false)
                {
                    OnError(new iPodNotFound());
                    return;
                }

                _IIC.FireFactorizedEvents();
                OnProgress(new EndExport(AlbumToExport));
            }
            catch (Exception e)
            {
                
                Trace.WriteLine("Problem to connect to iPod " + e.ToString());

                if (e.Message == "The playlist is not modifiable.")
                    OnError(new ITunesIPodPlaylistreadonly());
                else
                    OnError(new ITunesNotResponding());
            }

        }
    }
}
