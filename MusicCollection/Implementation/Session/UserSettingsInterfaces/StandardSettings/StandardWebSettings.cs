using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardWebSettings : IWebUserSettings
    {


        public string GraceNoteAplicationID
        {
            get { return ConfigurationManager.AppSettings["GraceNoteAplicationID"]; }
        }

        public string GraceNoteDeviceID
        {
            get { return Settings.Default.GraceNoteDeviceID; }
            set { Settings.Default.GraceNoteDeviceID = value; }
        }

        public string DiscogsConsumerKey
        {
            get { return ConfigurationManager.AppSettings["DiscogsConsumerKey"]; }
        }

        public string DiscogsConsumerSecret
        {
            get { return ConfigurationManager.AppSettings["DiscogsConsumerSecret"]; }
        }

        public string AmazonaccessKeyId
        {
            get { return ConfigurationManager.AppSettings["AmazonaccessKeyId"]; }
        }

        public string AmazonsecretKey
        {
            get { return ConfigurationManager.AppSettings["AmazonsecretKey"]; }
        }

        public string FreedbServer
        {
            get{return Settings.Default.FreedbServer;}
            set{ Settings.Default.FreedbServer=value;}
        }

        public int DiscogsTimeOut
        {
            get{return Settings.Default.DiscogsTimeOut;}
            set{ Settings.Default.DiscogsTimeOut=value;}
        }

        public bool DiscogsActivated
        {
            get{return Settings.Default.DiscogsActivated;}
            set{ Settings.Default.DiscogsActivated=value;}
        }

        public bool AmazonActivated
        {
            get{return Settings.Default.AmazonActivated;}
            set{ Settings.Default.AmazonActivated=value;}
        }

        public string DiscogsToken
        {
            get{return Settings.Default.DiscogsToken;}
            set{ Settings.Default.DiscogsToken=value;}
        }

        public string DiscogsTokenSecret
        {
            get{return Settings.Default.DiscogsTokenSecret;}
            set{ Settings.Default.DiscogsTokenSecret=value;}
        }

        public List<string> FreedbServers
        {
            get { return Settings.Default.FreedbServers.Cast<string>().ToList(); }
        }


        public void Save()
        {
            Settings.Default.Save();
        }

        public string RoviSearchAPIKey
        {
            get { return ConfigurationManager.AppSettings["RoviSearchAPIKey"]; }
        }

        public string RoviSharedSecret
        {
            get { return ConfigurationManager.AppSettings["RoviSharedSecret"]; }
        }


        public string MuzicBrainzUser
        {
            get { return ConfigurationManager.AppSettings["MuzicBrainzUser"]; }
        }

        public string MuzicBrainzPassword
        {
            get { return ConfigurationManager.AppSettings["MuzicBrainzPassword"]; }
        }
    }
}
