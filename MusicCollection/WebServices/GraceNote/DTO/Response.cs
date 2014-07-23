using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("RESPONSE")]
    public class Response
    {
        [XmlElement("ALBUM")]
        public AlbumDto[] AlbumDto { get; set; }

        [XmlElement("RANGE")]
        public RangeDto RangeDto { get; set; }

        [XmlAttribute("STATUS")]
        public string Status { get; set; }

        [XmlElement("USER")]
        public  User User { get; set; }
    }
}

