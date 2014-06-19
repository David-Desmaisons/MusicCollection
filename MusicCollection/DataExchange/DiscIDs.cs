using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Un4seen.Bass.AddOn.Cd;
using TagLib.Id3v2;

using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollection.DataExchange
{

    public sealed class DiscIDs : IDiscIDs
    {
        private DiscHash _Hash = null;

        private DiscIDs()
        {
            //_IDChanger = new FiringSimpleChange<IDiscIDs>(this);
        }

        private string _Asin = null;
        private string _MusicBrainz_ID = null;
        //private FiringSimpleChange<IDiscIDs> _IDChanger;


        private DiscIDs(string iAsin, string MusicBrainz_ID, string iCDDB, string iMusicBrainz_HashID, bool isQuery)
            : this()
        {
            _Hash = isQuery ? DiscHash.InstanciateFromQuery(iMusicBrainz_HashID, iCDDB) : DiscHash.InstanciateFromHash(iMusicBrainz_HashID, iCDDB);
            _Asin = iAsin;
            _MusicBrainz_ID = MusicBrainz_ID;
        }

        //10
        public string Asin
        {
            get { return _Asin; }
        }

        //36 
        public string MusicBrainzID
        {
            get { return _MusicBrainz_ID; }
        }

        public string CDDBQueryString
        {
            get { return _Hash == null ? null : _Hash.CDDBQueryString; }
        }

        public string MusicBrainzCDId
        {
            get { return _Hash == null ? null : _Hash.MusicBrainzHash; }
        }

        public string CDDB
        {
            get { return _Hash == null ? null : _Hash.CDDB; }
        }



      
        public DiscHash RawHash { get { return _Hash; } }

        internal IDiscIDs Interface
        {
            get
            {
                return this;
            }
        }

        bool IDiscIDs.IsEmpty { get { return ((RawHash == null || RawHash.IsEmpty) && (Asin == null) && (MusicBrainzID == null)); } }

        static internal DiscIDs Empty
        { get { return new DiscIDs(); } }


        static internal DiscIDs FromIDsAndHashes(string iAsin, string MusicBrainz_ID, string iCDDBh, string iMusicBrainz_HashID)
        {
            return new DiscIDs(iAsin, MusicBrainz_ID, iCDDBh, iMusicBrainz_HashID, false);
        }

        static internal DiscIDs FromBassInfo(int CDnumber)
        {
            string RawCDDB = BassCd.BASS_CD_GetID(CDnumber, BASSCDId.BASS_CDID_CDDB);
            string CDDB = (RawCDDB == null) ? null : RawCDDB.Replace(" ", "+");
            string MusicBrainz_HashID = BassCd.BASS_CD_GetID(CDnumber, BASSCDId.BASS_CDID_MUSICBRAINZ);

            return new DiscIDs(null, null, CDDB, MusicBrainz_HashID, true);
        }

        static internal DiscIDs FromFile(TagLib.File file)
        {
            string MusicBrainz_ID = file.Tag.MusicBrainzDiscId;
            string Asin = file.Tag.AmazonId;

            if (!(string.IsNullOrEmpty(MusicBrainz_ID)) ||
                 !(string.IsNullOrEmpty(Asin)))

                return new DiscIDs(Asin, MusicBrainz_ID, null, null, false);

            return Empty;

        }

        static bool IsNullOrEmpty(IDiscIDs id)
        {
            return ((id == null) || (id.IsEmpty));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            DiscIDs o = obj as DiscIDs;
            if (o == null)
                return false;

            if (IsNullOrEmpty(this))
                return IsNullOrEmpty(o);

            if (Asin!=o.Asin)
                return false;

            if (MusicBrainzID!=o.MusicBrainzID)
                return false;

            if (RawHash == null)
                return o.RawHash == null;

            return RawHash.Equals(o.RawHash);

        }

        public override int GetHashCode()
        {
            int key = (Asin==null) ? 0 : Asin.GetHashCode();
            key = key ^ ((MusicBrainzID == null) ? 0 : MusicBrainzID.GetHashCode());
            key = key ^ ((RawHash == null) ? 0 : RawHash.GetHashCode());

            return key;
        }

        public override string ToString()
        {
            return string.Format("Asin:{0} - MusicBrainzID:{1} - RawHash {2}", Asin, MusicBrainzID,RawHash);
        }
    }
}
