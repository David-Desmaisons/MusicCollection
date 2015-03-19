using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionWPF.ViewModel
{
    public static class AlbumCollectionProvider
    {

        public static IObservableCollection<IAlbum> GetAlbumCollection(this IMusicObject @this)
        {
            if (@this==null)
                return null;

            return GetAlbumCollectionFromMusicObject((dynamic)@this);
        }

        private static IObservableCollection<IAlbum> GetAlbumCollectionFromMusicObject(IMusicObject iotherobject)
        {
            return null;
        }

        private static IObservableCollection<IAlbum> GetAlbumCollectionFromMusicObject(IAlbum ial)
        {
            return ial.SingleObservableCollection();
        }

        private static IObservableCollection<IAlbum> GetAlbumCollectionFromMusicObject(ITrack itrack)
        {
            return itrack.Album.SingleObservableCollection();
        }

        private static IObservableCollection<IAlbum> GetAlbumCollectionFromMusicObject(IGenre igenre)
        {
            return igenre.Albums;
        }

        private static IObservableCollection<IAlbum> GetAlbumCollectionFromMusicObject(IArtist iartist)
        {
            return iartist.Albums;
        }


        public static IObservableCollection<TrackView> GetTrackCollection(this IMusicObject @this)
        {
            if (@this == null)
                return null;

            return GetTrackCollectionFromMusicObject((dynamic)@this);
        }

        private static IObservableCollection<TrackView> GetTrackCollectionFromMusicObject(IMusicObject iotherobject)
        {
            return null;
        }

        private static IObservableCollection<TrackView> GetTrackCollectionFromMusicObject(IAlbum ial)
        {
            return ial.Tracks.LiveSelect(t => TrackView.GetTrackView(t));
        }

        private static IObservableCollection<TrackView> GetTrackCollectionFromMusicObject(ITrack itrack)
        {
            return TrackView.GetTrackView(itrack).SingleObservableCollection();
        }

        private static IObservableCollection<TrackView> GetTrackCollectionFromMusicObject(IGenre igenre)
        {
            return igenre.Albums.LiveSelectMany(al => al.Tracks).LiveSelect(t => TrackView.GetTrackView(t));
        }

        private static IObservableCollection<TrackView> GetTrackCollectionFromMusicObject(IArtist iartist)
        {
            return iartist.Albums.LiveSelectMany(al=>al.Tracks).LiveSelect(t=> TrackView.GetTrackView(t));
        }

     
    }
}
