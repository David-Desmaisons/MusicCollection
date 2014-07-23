using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("GENRE")]
    public class Genre
    {
        [XmlText]
        public string Description { get; set; }

        [XmlAttribute("ID")]
        public int Id { get; set; }

        [XmlAttribute("NUM")]
        public int Number { get; set; }
    }
}

