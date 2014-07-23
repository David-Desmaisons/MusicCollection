using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{
 
    public class ArtistEra
    {
        [XmlText]
        public string Description { get; set; }

        [XmlElement("ID")]
        public int Id { get; set; }

        [XmlElement("ORD")]
        public int Order { get; set; }
    }
}

