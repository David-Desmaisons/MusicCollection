using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{
    [XmlRoot("URL")]
    public class ArtworkDto
    {
        [XmlAttribute("TYPE")]
        public string ArtworkType { get; set; }

        [XmlAttribute("HEIGHT")]
        public int Height { get; set; }

        [XmlText]
        public string Location { get; set; }

        [XmlAttribute("SIZE")]
        public string Size { get; set; }

        [XmlAttribute("WIDTH")]
        public int Width { get; set; }
    }
}

