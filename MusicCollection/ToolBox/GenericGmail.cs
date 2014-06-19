using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace MusicCollection.ToolBox
{
    internal class EmailActor
    {
        internal string Name
        {
            get;
            private set;
        }

        internal string Adress
        {
            get;
            private set;
        }

        internal EmailActor(string iName,string iAdress)
        {
            Name = iName;
            Adress=iAdress;
        }
    }
   
    internal class GenericGmail
    {
        private const string _Host="smtp.gmail.com";
        private const string _Gmailadress="gmail.com";

        private EmailActor _From;
        internal EmailActor From
        {
            get { return _From; }
        }

        internal string Message
        {
            get;
            set;
        }

        internal string Title
        {
            get;
            set;
        }

        internal string Password
        {
            get;
            private set;
        }

 
        internal GenericGmail(EmailActor from,string iPassword)
        {
            _From = from;
            Password=iPassword;
        }

        internal static bool IsGmailAdress(string adress)
        {
            if (string.IsNullOrEmpty(adress))
                return false;

            return adress.EndsWith("gmail.com");
        }

        internal bool Send(EmailActor to)
        {
            if (!IsGmailAdress(_From.Adress))
                return false;

            MailAddress fromAddress = new MailAddress(From.Adress,From.Name);
            MailAddress toAddress = new MailAddress(to.Adress,to.Name);

            //Password

            using (SmtpClient smtp = new SmtpClient
             {
                 Host = _Host,
                 Port = 587,
                 EnableSsl = true,
                 DeliveryMethod = SmtpDeliveryMethod.Network,
                 UseDefaultCredentials = false,
                 Credentials = new NetworkCredential(fromAddress.Address, Password)
             })
            {

                try
                {
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = Title,
                        Body = Message
                    })
                    {
                        smtp.Send(message);

                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception while sending email " + e.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}

