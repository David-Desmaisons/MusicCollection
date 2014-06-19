using System;

namespace MusicCollection.Fundation
{
    public interface ISessionEntityFinder
    {
        IEntityFinder<IAlbum> AlbumFinder { get; }
        IEntityFinder<IArtist> ArtistFinder { get; }
        IEntityFinder<ITrack> TrackFinder { get; }
    }
}
