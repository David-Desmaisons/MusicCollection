using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    [XmlRoot("CLIENT")]
    public class Client
    {
        public Client()
        {
        }

        public Client(string clientId)
        {
            this.Id = clientId;
        }

        [XmlText]
        public string Id { get; set; }
    }
}

