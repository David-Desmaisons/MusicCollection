using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("RESPONSES")]
    public class Responses
    {
        [XmlElement("MESSAGE")]
        public string Message { get; set; }

        [XmlElement("RESPONSE")]
        public Response Response { get; set; }
    }
}

