using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Specialized;

using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;
using MusicCollection.ToolBox.Collection.Observable;

namespace MusicCollection.Implementation
{
    internal class AlbumCollection : ROCollection<IAlbum>, IFullObservableCollection<IAlbum>, IInvariant
    {

        private StrongCacheCollection<Tuple<String, String>, Album> _AlbumsByName = null;
        private StrongCacheCollection<String, Album> _AlbumsByAsin = null;
        private StrongCacheCollection<String, Album> _AlbumsByMusicBrainzID = null;

        private LookUpImpl<DiscHash, Album> _AlbumsByCDHash = null;
        private MusicSessionImpl _Session;

        internal AlbumCollection(MusicSessionImpl Session):base(1000)
        {
            _Session = Session;

            _AlbumsByName = new StrongCacheCollection<Tuple<String, String>, Album>((a => new Tuple<string, string>(a.NormalizedName, a.Author)));

            _AlbumsByCDHash = new LookUpImpl<DiscHash, Album>((a => a.Hashes),1000);
            _AlbumsByAsin = new StrongCacheCollection<String, Album>((a => a.Asin));
            _AlbumsByMusicBrainzID = new StrongCacheCollection<String, Album>((a => a.MusicBrainzID));
         }

        internal void CoreUnRegister(Album Album)
        {
            _AlbumsByName.Remove(Album);
            _AlbumsByCDHash.Remove(Album);
            _AlbumsByAsin.Remove(Album);
            _AlbumsByMusicBrainzID.Remove(Album);
            base.RealRemove(Album);
        }

        internal void CoreRegister(Album Album)
        {
            _AlbumsByName.Register(Album);
            _AlbumsByCDHash.Add(Album);
            _AlbumsByAsin.Register(Album);
            _AlbumsByMusicBrainzID.Register(Album);
        }

        private class AlbumRenamer : IDisposable
        {
            private AlbumCollection _Father;
            private Album _Album;

            internal AlbumRenamer(AlbumCollection iFather, Album iAlbum)
            {
                _Father = iFather;
                _Album = iAlbum;
                _Father._AlbumsByName.Remove(_Album);
            }

            public void Dispose()
            {
                _Father._AlbumsByName.Register(_Album);
            }
        }

        internal IDisposable GetRenamerTransaction(Album ialbum)
        {
            return new AlbumRenamer(this, ialbum);
        }


            
  
        internal IEnumerable<MatchAlbum> FindByCDHashes(DiscHash DH)
        {
            if (DH==null)
                return Enumerable.Empty<MatchAlbum>();

            return from al in _AlbumsByCDHash[DH] select new MatchAlbum(al, MatchPrecision.Suspition, FindWay.ByHashes);
        }

        internal IEnumerable<MatchAlbum> FindAlbums(IAlbumDescriptor iad)
        {
            Album res = _AlbumsByAsin.Find(iad.IDs.Asin);
            if (res != null)
            {
                yield return new MatchAlbum(res, MatchPrecision.Exact,FindWay.ByAsin);
                yield break;
            }

            res = _AlbumsByMusicBrainzID.Find(iad.IDs.MusicBrainzID);
            if (res != null)
            {
                yield return new MatchAlbum(res, MatchPrecision.Exact,FindWay.ByMusicBrainzID);
                yield break;
            }

            res= FindByName(iad.Name, Artist.AuthorName(Artist.GetArtistFromName(iad.Artist, _Session)));
            if (res != null)
            {
                yield return new MatchAlbum(res, MatchPrecision.Exact,FindWay.ByName);
                yield break;
            }

            foreach (MatchAlbum ma in this.FindByCDHashes(iad.IDs.RawHash))
            {
                yield return ma;
            }
            yield break;

        }


        public bool Invariant
        {
            get
            {
                int C = _AlbumsByName.Count;
                if (C < _AlbumsByCDHash.Count)
                    return false;

                if (C < _AlbumsByAsin.Count)
                    return false;

                if (C < _AlbumsByMusicBrainzID.Count)
                    return false;

                if (C != base.Count)
                    return false;

                return true;
            }
        }


        internal Album FindByName(string AlbumName, string Artist)
        {
            return _AlbumsByName.Find(new Tuple<string, string>(AlbumName.Normalized(), Artist));
        }

        internal void RegisterPublish(Album Album)
        {
            CoreRegister(Album);
            Album.MusicSession = _Session;
            Publish(Album);
        }

        internal void Publish(Album Album)
        {
            base.RealAdd(Album);
        }



    }
}
