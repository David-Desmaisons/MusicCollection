using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{
    public interface IDiscIDs
    {
        //10
        string Asin { get; }

        //36
        string MusicBrainzID { get; }

        //28
        string MusicBrainzCDId { get; }

        //8
        string CDDB { get; }

        string CDDBQueryString { get; }

        bool IsEmpty { get; }

        DiscHash RawHash { get; }

        //event EventHandler<ClassTimeChangeEvent<IDiscIDs>> Changes;
    }

    public class DiscHash
    {
        private string _CDDBQueryString = null;
        private string _CDDB = null;
        private string _MusicBrainz_HashID = null;

        internal DiscHash()
        {
        }

        static internal DiscHash InstanciateFromQuery(string iMusicBrainz_HashID, string iCDDBq)
        {
            if ((iMusicBrainz_HashID == null) && (iCDDBq == null))
                return null;

            return new DiscHash(iMusicBrainz_HashID, iCDDBq, null);
        }

        static internal DiscHash InstanciateFromHash(string iMusicBrainz_HashID, string iCDDBh)
        {
            if ((iMusicBrainz_HashID == null) && (iCDDBh == null))
                return null;

            return new DiscHash(iMusicBrainz_HashID, null, iCDDBh);
        }

        internal DiscHash(string MusicBrainz_HashID, string CDDBQueryString, string iCDDB)
        {
            _CDDBQueryString = CDDBQueryString;
            _MusicBrainz_HashID = MusicBrainz_HashID;
            _CDDB = iCDDB;
        }

        //28
        public string MusicBrainzHash
        {
            get { return _MusicBrainz_HashID; }
            private set { _MusicBrainz_HashID = value; }
        }


        public string CDDBQueryString
        {
            get { return _CDDBQueryString; }
            private set { _CDDBQueryString = value; }
        }

        //8
        public string CDDB
        {
            get
            {
                return _CDDB ?? (_CDDBQueryString == null ? null : _CDDBQueryString.Remove(8));
            }
        }

        public override string ToString()
        {
            return string.Format("CDDB:{0} - CDBB Query:{1} - MusicBrainzHash {2}", _CDDB, _CDDBQueryString, _MusicBrainz_HashID);
        }

        public override bool Equals(object obj)
        {
            DiscHash dh = obj as DiscHash;
            if (dh == null)
                return false;

            return ((dh.CDDB == CDDB) && (dh._MusicBrainz_HashID == _MusicBrainz_HashID));
        }

        public override int GetHashCode()
        {
            return ((CDDB == null) ? 0 : CDDB.GetHashCode()) ^ ((_MusicBrainz_HashID == null) ? 0 : _MusicBrainz_HashID.GetHashCode());
        }

        public bool IsEmpty { get { return ((CDDB == null) && (_MusicBrainz_HashID == null)); } }
    }
}
