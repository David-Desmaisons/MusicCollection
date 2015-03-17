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

        //private class AlbumCollectionProviderPlugger<T> : ICollectionProvider<IAlbum>
        //{
        //    private Func<T, IObservableCollection<IAlbum>> _Provider;
        //    private T _Element;
        //    public AlbumCollectionProviderPlugger(T iElement, Func<T, IObservableCollection<IAlbum>> iProvider)
        //    {
        //        _Provider = iProvider;
        //        _Element = iElement;
        //    }

        //    public IObservableCollection<IAlbum> Collection
        //    {
        //        get{return _Provider(_Element);}
        //    }
        //}

        //public static ICollectionProvider<IAlbum> GetAlbumCollection(this IMusicObject @this)
        //{
        //    return GetAlbumCollectionFromMusicObject((dynamic)@this);
        //}

        //private static ICollectionProvider<IAlbum> GetAlbumCollectionFromMusicObject(IMusicObject iotherobject)
        //{
        //    return null;
        //}

        //private static ICollectionProvider<IAlbum> GetAlbumCollectionFromMusicObject(IAlbum ial)
        //{
        //    return new AlbumCollectionProviderPlugger<IAlbum>(ial, al => al.SingleObservableCollection());
        //}

        //private static ICollectionProvider<IAlbum> GetAlbumCollectionFromMusicObject(ITrack itrack)
        //{
        //    return new AlbumCollectionProviderPlugger<ITrack>(itrack, al => itrack.Album.SingleObservableCollection());
        //}

        //private static ICollectionProvider<IAlbum> GetAlbumCollectionFromMusicObject(IGenre igenre)
        //{
        //    return new AlbumCollectionProviderPlugger<IGenre>(igenre, ar => ar.Albums);
        //}

        //private static ICollectionProvider<IAlbum> GetAlbumCollectionFromMusicObject(IArtist iartist)
        //{
        //    return new AlbumCollectionProviderPlugger<IArtist>(iartist, ar => ar.Albums);
        //}
    }
}
