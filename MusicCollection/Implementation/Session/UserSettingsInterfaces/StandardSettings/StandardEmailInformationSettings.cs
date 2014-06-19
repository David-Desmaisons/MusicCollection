using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    class StandardEmailInformationSettings : IEmailInformationSettings
    {
        public string EmailAdress
        {
            get { return  ConfigurationManager.AppSettings["AdministrativeEmail"]; }
        }

        public string Password
        {
            get { return  ConfigurationManager.AppSettings["AdministrativeEmailPassword"]; }
        }

        public string EmailReceptor
        {
            get { return  ConfigurationManager.AppSettings["AdministrativeEmailReceptor"]; }
        }
    }
}
