using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("QUERY")]
    public class Query
    {
        public Query()
        {
            AlbumSearchText = new List<AlbumSearchText>();
            SearchOption = new List<SearchOption>();
        }

        public Query(string command):this()
        {
            this.Command = command; 
           
        }

        public Query(string command, Client client)
            : this(command)
        {
            this.Client = client;       
        }

        public Query AddSearch(string Type, string Value)
        {
            AlbumSearchText.Add(new AlbumSearchText(Type, Value));
            return this;
        }

        public Query AddOption(string Option, string Value)
        {
            SearchOption.Add(new SearchOption() { Parameter = Option, Value = Value });
            return this;
        }

        public void NeedFullCover()
        {
            this.AddOption(Option: "SELECT_EXTENDED", Value: "COVER")
             .AddOption(Option: "COVER_SIZE", Value: "XLARGE,LARGE,MEDIUM,SMALL,THUMBNAIL");
        }

        [XmlElement("TEXT")]
        public List<AlbumSearchText> AlbumSearchText { get; private set; }

        [XmlElement("CLIENT")]
        public Client Client { get; set; }

        [XmlAttribute("CMD")]
        public string Command { get; set; }

        [XmlElement("GN_ID")]
        public string GracenoteId { get; set; }

        [XmlElement("MODE")]
        public string Mode { get; set; }

        [XmlElement("TOC")]
        public Toc Toc { get; set; }   

        [XmlElement("RANGE")]
        public RangeDto RangeDto { get; set; }

        [XmlElement("OPTION")]
        public List<SearchOption> SearchOption { get; private set; }
    }
}

