using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualEmailInformationSettings : IEmailInformationSettings
    {
        public string EmailAdress {get;set;}
        public string Password { get; set; }
        public string EmailReceptor { get; set; }
    }
}
