﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MusicCollection.ToolBox.Web;
using MusicCollection.Implementation;

namespace MusicCollection.WebServices.MuzicBrainz
{
    public class MusicBrainzHttpCreator
    {
        private const string _BaseMusicBrainz = "http://musicbrainz.org/ws/2/";
        private const string _BaseCoverArchive = "http://coverartarchive.org/";

        private string _Base;
        private string _Category;
        private string _Final;
        private string _Value;

        private MusicBrainzHttpCreator(string iBase, string iCategory, string iFinal)
        {
            _Base = iBase;
            _Category = iCategory;
            _Final = iFinal;
        }

        internal IHttpWebRequest BuildRequest()
        {
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Buildstring);
            return Mature(InternetProvider.InternetHelper.CreateHttpRequest(Buildstring));
        }

        static public MusicBrainzHttpCreator ForCDIdSearch()
        {
            return new MusicBrainzHttpCreator(_BaseMusicBrainz, "discid/", "?inc=recordings+artist-credits&fmt=json");
        }

        static public MusicBrainzHttpCreator ForReleaseSearch()
        {
            return new MusicBrainzHttpCreator(_BaseMusicBrainz, "release/?query=", "&fmt=json");
        }

        static public MusicBrainzHttpCreator ForReleaseIdSearch()
        {
            return new MusicBrainzHttpCreator(_BaseMusicBrainz, "release/", "?inc=recordings+artist-credits&fmt=json");
        }

        static public MusicBrainzHttpCreator ForCoverArtSearch()
        {
            return new MusicBrainzHttpCreator(_BaseCoverArchive, "release/", string.Empty);
        }

        public MusicBrainzHttpCreator SetValue(string ivalue)
        {
            _Value = ivalue;
            return this;
        }

        private MusicBrainzHttpCreator SetItem(string Item, string ivalue)
        {
            if (_Value != null)
                _Value += @" AND ";

            _Value += string.Format(@"{0}:""{1}""", Item,ivalue);
            return this;
        }

        public MusicBrainzHttpCreator SetArtist(string ivalue)
        {
             char[] separators = new char[] { ',', ';', '&'};

             foreach (string Name in ivalue.Split(separators, StringSplitOptions.RemoveEmptyEntries))
             {
                 SetItem("artist", Name);
             }

             return this;
        }

        public MusicBrainzHttpCreator SetDiscName(string ivalue)
        {
            return SetItem("release", ivalue);
        }


        private string Buildstring
        {
            get { return string.Format("{0}{1}{2}{3}", _Base, _Category, _Value, _Final); }
        }

        private IHttpWebRequest Mature(IHttpWebRequest request)
        {
            request.Accept = "application/json";
            return request;
        }
    }
}
