using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Un4seen.Bass.AddOn.Cd;
using TagLib.Id3v2;

namespace MusicCollection.Implementation
{
    public class DiscIDs : IDiscIDs
    {

        private string _Asin=null;
        //private string _ISRC = null;
        private string _MusicBrainz_ID = null;
        private string _MusicBrainz_HashID = null;
        private string _CDDB = null;

        internal DiscIDs(string iAsin, string iMusicBrainz_HashID, string iCDDB, string MusicBrainz_ID = null)
        {
            _Asin = iAsin;
            //_ISRC = iISRC;
            _MusicBrainz_HashID = iMusicBrainz_HashID;
            _MusicBrainz_ID = MusicBrainz_ID;
            _CDDB = iCDDB;
        }

        private DiscIDs()
        { 
        }

        internal IDiscIDs Interface
        {
            get
            {
                return this;
            }
        }

        internal string Asin
        {
            get { return _Asin; }
            set { _Asin = value; }
        }

        string IDiscIDs.Asin
        {
            get { return _Asin; }
        }

        //string IDiscIDs.ISRC
        //{
        //    get { return _ISRC; }
        //}

        string IDiscIDs.MusicBrainz_HashID
        {
            get { return _MusicBrainz_HashID; }
        }

        string IDiscIDs.CDDB
        {
            get { return _CDDB; }
        }

        internal string MusicBrainz_ID
        {
            get { return _MusicBrainz_ID; }
            set { _MusicBrainz_ID = value; }
        }

        string IDiscIDs.MusicBrainz_ID 
        {
            get { return _MusicBrainz_ID; }
        }

        static internal DiscIDs FromAsin(string Asin)
        {
            return new DiscIDs(Asin, null, null, null);
        }

        static internal DiscIDs FromCDDB(string CDDB)
        {
            return new DiscIDs(null, CDDB, null, null);
        }

        static internal DiscIDs FromMusicBrainz_ID(string MusicBrainz_ID)
        {
            return new DiscIDs(null, null, MusicBrainz_ID, null);
        }

        //static internal DiscIDs FromISRC(string ISRC)
        //{
        //    return new DiscIDs(null, null, null, ISRC);
        //}

        static internal DiscIDs FromBassInfo(int CDnumber)
        {
            string RawCDDB = BassCd.BASS_CD_GetID(CDnumber, BASSCDId.BASS_CDID_CDDB);
            string CDDB = (RawCDDB == null) ? null : RawCDDB.Replace(" ", "+");
            string MusicBrainz_HashID = BassCd.BASS_CD_GetID(CDnumber, BASSCDId.BASS_CDID_MUSICBRAINZ);
  
            return new DiscIDs(null,  MusicBrainz_HashID, CDDB);
        }

        static DiscIDs()
        { _Empty = new DiscIDs(); }

        private static DiscIDs _Empty = null;

        static internal DiscIDs Empty
        { get{return  _Empty; }}

        static internal DiscIDs FromFile(TagLib.File file)
        {
            string MusicBrainz_ID = file.Tag.MusicBrainzDiscId;
            string Asin = file.Tag.AmazonId;

            if ( !(string.IsNullOrEmpty(MusicBrainz_ID)) ||
                 !(string.IsNullOrEmpty(Asin)))

                return new DiscIDs(Asin, null, null, MusicBrainz_ID);

            return Empty;
  
        }




    }
}
