using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using System.Threading.Tasks;
using System.Threading;

namespace MusicCollection.Utilies
{
    public class TrackFileStatusLoader : IAsyncLoad
    {
        private IMusicSession _IMS;
        public TrackFileStatusLoader(IMusicSession ims)
        {
            _IMS = ims;
        }

        private bool BenchLoad(CancellationToken iCancellationRequestToken)
        {
            IList<ITrack> tracks = _IMS.AllTracks.ToList();

            FileStatus fs;
            return tracks.Apply(tr => fs = tr.FileExists, iCancellationRequestToken);
        }

        private bool _Changed=false;
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs not)
        {
            _Changed = true;
        }

        private bool SyncLoad(CancellationToken iCancellationRequestToken)
        {
            _IMS.AllTracks.CollectionChanged += CollectionChanged;
            if (!BenchLoad(iCancellationRequestToken))
                return false;

            while (_Changed)
            {
                _Changed = false;
                if (!BenchLoad(iCancellationRequestToken))
                    return false;
            }

            _IMS.AllTracks.CollectionChanged -= CollectionChanged;

            return true;
        }

        public Task<bool> LoadAsync(CancellationToken iCancellationRequestToken)
        {
            return Task.Run(() => SyncLoad(iCancellationRequestToken));
        }

    }
}
