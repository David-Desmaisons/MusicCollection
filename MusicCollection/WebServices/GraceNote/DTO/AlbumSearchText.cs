using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("TEXT")]
    public class AlbumSearchText
    {
        public AlbumSearchText()
        {
        }

        public AlbumSearchText(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        [XmlAttribute("TYPE")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}

