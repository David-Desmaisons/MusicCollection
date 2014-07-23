using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("OPTION")]
    public class SearchOption
    {
        [XmlElement("PARAMETER")]
        public string Parameter { get; set; }

        [XmlElement("VALUE")]
        public string Value { get; set; }
    }
}

