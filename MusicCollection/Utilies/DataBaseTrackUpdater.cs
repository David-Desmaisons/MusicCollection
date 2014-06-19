using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Nhibernate.Session;
using MusicCollection.Nhibernate.Mapping;

namespace MusicCollection.Utilies
{

    internal class DataBaseTrackUpdater : DataBaseUpdater<Track>
    {

        internal DataBaseTrackUpdater(Func<Track, DBOperation> OnTrack)
            : base(OnTrack)
        {
        }


        protected override IEnumerable<Track> GetAllElements(IMusicSession session)
        {
            return from al in session.AllAlbums from tr in al.Tracks select tr as Track;
        }
    }


    internal class DataBaseAlbumUpdater : DataBaseUpdater<Album>
    {

        internal DataBaseAlbumUpdater(Func<Album, DBOperation> OnTrack)
            : base(OnTrack)
        {
        }

        protected override IEnumerable<Album> GetAllElements(IMusicSession session)
        {
            return from al in session.AllAlbums select al as Album;
        }
    }

    internal class DataBaseArtistUpdater : DataBaseUpdater<Artist>
    {

        internal DataBaseArtistUpdater(Func<Artist, DBOperation> OnTrack)
            : base(OnTrack)
        {
        }

        protected override IEnumerable<Artist> GetAllElements(IMusicSession session)
        {
            return from ar in session.AllArtists select ar as Artist;//from al in session as Sess select al as Album;
        }
    } 
}
    

