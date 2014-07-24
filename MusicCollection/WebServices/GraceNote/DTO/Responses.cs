using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
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

        static public Responses Deserialize(string xml)
        {
            Responses res = null;
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XmlSerializer serializer2 = new XmlSerializer(typeof(Responses));
            using (StringReader reader2 = new StringReader(document.OuterXml))
            {
                res = (Responses)serializer2.Deserialize(reader2);
            }
            return res;
        }

    }
}

