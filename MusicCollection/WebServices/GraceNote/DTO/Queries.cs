using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("QUERIES")]
    public class Queries
    {
        public Queries()
        {
            Country = "usa";
            Lang = "eng";
        }

        [XmlElement("AUTH")]
        public Auth Auth { get; set; }

        [XmlElement("COUNTRY")]
        public string Country { get; set; }

        [XmlElement("LANG")]
        public string Lang { get; set; }

        [XmlElement("QUERY")]
        public Query Query { get; set; }

        public void Serialize(Stream outstream)
        {
            XmlSerializerNamespaces serializerNamespace = GetSerializerNamespace();
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(outstream, this, serializerNamespace);
        }

        private XmlSerializerNamespaces GetSerializerNamespace()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            return namespaces;
        }
    }
}

