using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MusicCollection.WebServices.GraceNote.DTO
{
    [XmlRoot("USER")]
    public class User
    {
        public User()
        {
        }

        public User(string token)
        {
            this.Token = token;
        }

        [XmlText]
        public string Token { get; set; }
    }
}

