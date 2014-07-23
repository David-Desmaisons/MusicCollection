using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("QUERY")]
    public class Query
    {
        public Query()
        {
        }

        public Query(string command)
        {
            this.Command = command;
        }

        public Query(string command, Client client)
            : this(command)
        {
            this.Client = client;
        }

        [XmlElement("TEXT")]
        public  AlbumSearchText[] AlbumSearchText { get; set; }

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
        public SearchOption[] SearchOption { get; set; }
    }
}

