﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    internal class EntityFinder : ISessionEntityFinder
    {
        private MusicSessionImpl _IMS;
        internal EntityFinder(MusicSessionImpl ism)
        {
            _IMS = ism;
            AlbumFinder = new ItemFinder<IAlbum>(_IMS.AllAlbums, a => a.Name);
            //.NormalizedName);
            //ArtistFinder = new ItemFinder<IArtist>(_IMS.AllArtists, a => a.NormalizedName);
            TrackFinder = new ItemFinder<ITrack>(_IMS.AllTracks, (t) => t.Name);
            //.ToLower().WithoutAccent());
        }

        public IEntityFinder<IAlbum> AlbumFinder
        {
            get;
            private set;
        }

        public IEntityFinder<IArtist> ArtistFinder
        {
            get { return _IMS.ArtistFinder; }
            //get;
            //private set;
        }

        public IEntityFinder<ITrack> TrackFinder
        {
            get;
            private set;
        }

    }
}
