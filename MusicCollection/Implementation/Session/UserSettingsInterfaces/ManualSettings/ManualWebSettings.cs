using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    class ManualWebSettings : IWebUserSettings
    {
        public string FreedbServer
        {
            get;
            set;
        }

        public List<string> FreedbServers
        {
            get;
            set;
        }

        public int DiscogsTimeOut
        {
            get;
            set;
        }

        public bool DiscogsActivated
        {
            get;
            set;
        }

        public bool AmazonActivated
        {
            get;
            set;
        }

        public string DiscogsConsumerKey
        {
            get;
            set;
        }

        public string DiscogsConsumerSecret
        {
            get;
            set;
        }

        public string DiscogsToken
        {
            get;
            set;
        }

        public string DiscogsTokenSecret
        {
            get;
            set;
        }

        public string AmazonaccessKeyId
        {
            get;
            set;
        }

        public string AmazonsecretKey
        {
            get;
            set;
        }

        public void Save()
        {
        }
    }
}
