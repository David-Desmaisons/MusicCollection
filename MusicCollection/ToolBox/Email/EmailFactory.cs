using MusicCollection.Fundation;
using MusicCollection.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.ToolBox
{
    internal class EmailFactory : IEmailFactory
    {
        private const string _AdminName = "Music collection Administator";
        private const string _DestName = "David Desmaisons";
        private readonly EmailActor _GmailAdmin;
        private readonly EmailActor _GmailReceptor;
        private readonly string _Password;

        public EmailFactory(IEmailInformationSettings iEmailInformationManager)
        {
            _GmailAdmin = new EmailActor(_AdminName, iEmailInformationManager.EmailAdress);
            _GmailReceptor = new EmailActor(_DestName, iEmailInformationManager.EmailReceptor);

            _Password = iEmailInformationManager.Password;
        }


        public IEmail GetEmail()
        {
            return new AdminEmail(new GenericGmail(_GmailAdmin, _Password), _GmailReceptor);
        }
    }
}
