using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollection.Utilies
{
    public class TrackFileStatusLoader
    {
        public event EventHandler<EventArgs> StatusLoaded;

        private IMusicSession _IMS;
        public TrackFileStatusLoader(IMusicSession ims)
        {
            _IMS = ims;
        }

        private void BenchLoad()
        {
            IList<ITrack> tracks = _IMS.AllTracks.ToList();

            FileStatus fs;
            tracks.Apply(tr => fs = tr.FileExists);
        }

        private bool _Changed=false;
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs not)
        {
            _Changed = true;
        }

        private void SyncLoad()
        {
            _IMS.AllTracks.CollectionChanged += CollectionChanged;
            BenchLoad();

            if (_Changed)
            {
                BenchLoad();
            }

            _IMS.AllTracks.CollectionChanged -= CollectionChanged;

            EventHandler<EventArgs> sl = StatusLoaded;

            if (sl != null)
                sl(this, new EventArgs());
        }

        public void Load(bool Sync)
        {
            if (Sync)
            {
                SyncLoad();
            }
            else
            {
                Action a = SyncLoad;
                a.BeginInvoke(null, null);
            }

        }
    }
}
