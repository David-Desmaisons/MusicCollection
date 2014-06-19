using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.SettingsManagement;
using MusicCollection.Infra;
using  MusicCollection.ToolBox.Event;

namespace MusicCollection.Implementation
{
    internal class MusicRemover : IMusicRemover
    {

        private UISafeEvent<EventArgs> _Ended;
 

        public event EventHandler<EventArgs> Completed
        {
            add { _Ended.Event += value; }
            remove { _Ended.Event -= value; }
        }


        private void OnEnded(EventArgs EndEvent=null)
        {
            _Ended.Fire(EndEvent, true);
        }

        
        private IImportContext _Transaction;
        private bool? _PR;
        private IList<IAlbum> _AlbumToremove = new List<IAlbum>();
        private IList<ITrack> _TrackToremove = new List<ITrack>();

        private bool _UnderTrans = false;

        internal MusicRemover(IImportContext transaction)
        {
            _Transaction = transaction;
            _PR = transaction.DeleteManager.DeleteFileOnDeleteAlbum;
            _Ended = new UISafeEvent<EventArgs>(this);

        }

        static internal MusicRemover GetMusicRemover(IInternalMusicSession session)
        {
            return new MusicRemover(session.GetNewSessionContext());
        }

        public IList<IAlbum> AlbumtoRemove { get { return _AlbumToremove; } }

        public IList<ITrack> TrackRemove { get { return _TrackToremove; } }

        bool IMusicRemover.IsValid { get { return _PR != null; } }

        public bool? IncludePhysicalRemove { get { return _PR; } set { _PR = value; } }

        private IEnumerable<IObjectStateCycle> Removable
        {
            get
            {
                return AlbumtoRemove.Cast<IObjectStateCycle>().Concat(TrackRemove.Cast<IObjectStateCycle>());
            }
        }

        void IMusicRemover.Comit(bool Sync)
        {
            //Removable.Apply(al => al.State = ObjectState.Available);
  
            if (Sync)
                PrivateComit();
            else
            {
                Action Ac = PrivateComit;
                _UnderTrans = true;
                Ac.BeginInvoke(null, null);
            }
        }

        private void PrivateComit()
        {
            if (_PR == null)
                return;

            var otheralbums = _TrackToremove.Select(t=>t.Album).Distinct();
            var completaotheralbums = otheralbums.Where(oa=>oa.Tracks.All(t=>_TrackToremove.Contains(t)));


            IList<Album> allalbumtodelete = _AlbumToremove.Concat(completaotheralbums).Distinct().Cast<Album>().ToList();
            IList<Track> singleTrack = _TrackToremove.Where(t => !allalbumtodelete.Contains(t.Album)).Cast<Track>().ToList();

            using (_Transaction.SessionLock())
            {
                try
                {

                    FileCleaner fc = _Transaction.Folders.GetFileCleanerFromAlbums(allalbumtodelete, _Transaction.Folders.IsFileRemovable, null, true);
                    FileCleaner fc2 = _Transaction.Folders.GetFileCleanerFromTracks(singleTrack, _Transaction.Folders.IsFileRemovable, null, true);


                    using (IMusicTransaction imt = _Transaction.CreateTransaction())
                    {
                        foreach (Album al in allalbumtodelete)
                        {
                            _Transaction.AddForRemove(al);
                        }

                        foreach (Track te in singleTrack)
                        {
                            _Transaction.AddForRemove(te);
                        }

                        imt.Commit();
                    }

                    if (_PR == false)
                        return;

                    fc.Remove();
                    fc2.Remove();
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception during Remove " + e.ToString());
                }
                finally
                {
                    OnEnded();
                }

            }

            _UnderTrans = false;
            _AlbumToremove = null;
        }

        void IMusicRemover.Cancel()
        {
            if (_UnderTrans)
                return;

            _AlbumToremove = null;
        }

        void IDisposable.Dispose()
        {
            if (_UnderTrans)
                return;

            _AlbumToremove = null;
        }


    }
}
