using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.DataExchange;

namespace MusicCollection.Utilies.Edition
{
    internal class OriginedGroupedTrack
    {
        internal IList<Track> Tracks
        {
            get;
            private set;
        }

        internal Album OriginAlbum
        {
            get;
            private set;
        }

        internal bool Complete
        {
            get;
            private set;
        }

        internal AlbumTarget NewTarget
        {
            private set;
            get;
        }

        private bool? _IsTarget;
        internal bool IsTarget
        {
            get
            {
                if (_IsTarget == null)
                {
                    _IsTarget = (OriginAlbum.Name == NewTarget.AlbumName) && (OriginAlbum.Artists.SequenceEqual(NewTarget.Artists));
                }

                return (bool)_IsTarget;
            }
        }

        internal OriginedGroupedTrack(Album ial, IList<Track> iTracks, AlbumTarget at)
        {
            Tracks = iTracks;
            OriginAlbum = ial;
            Complete = ial.Tracks.Count == Tracks.Count;
            NewTarget = at;
        }
    }

    internal class AlbumTarget : IAlbumDescriptor
    {
        private AlbumTarget(IList<Artist> iArtists, string iAlbumName, string iGenre, int? iYear)
        {
            Artists = iArtists;
            AlbumName = iAlbumName;
            _Genre = iGenre;
            _Year = iYear;
        }

        private string _Genre
        {
            get;
            set;
        }

        private int? _Year
        {
            get;
            set;
        }

        internal bool HasChangedInAlbumName
        {
            get { return ((OrderedTrack.Count > 1) || (OrderedTrack[0].IsTarget == false)); }
        }

        #region IAlbumDescriptor

        //          return new AlbumDescriptor() { Name = AlbumName, Artist = Artist.AuthorName(Artists), Year = iYear ?? OrderedTrack[0].OriginAlbum.Year, Genre = iGenre ?? OrderedTrack[0].OriginAlbum.Genre };




        string IAlbumDescriptor.Artist
        {
            get { return Artist.AuthorName(Artists); }
        }


        string IAlbumDescriptor.Genre
        {
            get { return _Genre ?? OrderedTrack[0].OriginAlbum.Genre; }
        }


        //private bool _IDC = false;
        //private IDiscIDs _IDResult = null;

        IDiscIDs IAlbumDescriptor.IDs
        {
            get
            {
                return  DiscIDs.Empty;
            }
        }

        uint IAlbumDescriptor.TracksNumber
        {
            get { return 0; }
        }

        string IAlbumDescriptor.Name
        {
            get { return AlbumName; }
        }

        int IAlbumDescriptor.Year
        {
            get { return _Year ?? OrderedTrack[0].OriginAlbum.Year; }
        }

        #endregion


        //internal bool NewEntity
        //{
        //    get;
        //    private set;
        //}


        internal Album TrivialAlbum
        {
            get
            {
                return OrderedTrack.Where(ots => ots.IsTarget).Select(ot => ot.OriginAlbum).FirstOrDefault();
            }
        }

        public static IList<AlbumTarget> FromListAndTargets(IList<Track> iTracks, IList<Artist> Artists, string AlbumName, string itGenre = null, int? tYear = null)
        {
            var res = iTracks.ToLookup(t => t.RawAlbum).ToLookup(a => new AlbumTarget(Artists ?? a.Key.RawArtists, AlbumName ?? a.Key.Name, itGenre, tYear));
            return res.Apply(g => g.Key.OrderedTrack = g.Select((gg) => new OriginedGroupedTrack(gg.Key, gg.ToList(), g.Key)).ToList()).Select(g => g.Key).ToList();
        }

        internal IList<Artist> Artists
        {
            get;
            private set;
        }

        internal string AlbumName
        {
            get;
            private set;
        }

        internal Album AlbumNewAlbum
        {
            get;
            set;
        }

        internal List<OriginedGroupedTrack> OrderedTrack
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            AlbumTarget at = obj as AlbumTarget;

            if (at == null)
                return false;

            if (object.ReferenceEquals(this, at))
                return true;

            if (AlbumName != at.AlbumName)
                return false;

            if (object.ReferenceEquals(Artists, at.Artists))
                return true;

            if (Artists == null)
                return false;

            return Artists.SequenceEqual(at.Artists);
        }

        public override int GetHashCode()
        {
            int k = AlbumName.GetHashCode();

            Artists.Apply((a) => k = k ^ a.Name.GetHashCode());

            return k;

        }

    }
}
