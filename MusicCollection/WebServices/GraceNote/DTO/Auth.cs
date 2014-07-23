using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{

    public class Auth
    {
        public Auth()
        {
        }

        public Auth(string clientId, string token)
        {
            this.Client = new Client(clientId);
            this.User = new User(token);
        }

        [XmlElement("CLIENT")]
        public Client Client { get; set; }

        [XmlElement("USER")]
        public User User { get; set; }
    }
}

