using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.Implementation
{

    internal enum FindWay { ByName,ByAsin, ByMusicBrainzID ,ByHashes}


    internal class MatchAlbum : Match<Album>
    {
        
        internal FindWay Way
        {
            get;
            private set;
        }

        internal MatchAlbum(Album itrack, MatchPrecision iPrecision, FindWay iWay)
            : base(itrack, iPrecision)
        {
            Way = iWay;
        }


        public override string ToString()
        {
            return string.Format("Album: {0}, Way: {1} Precision: {2}",FindItem,Way,Precision);
        }
    }

}
