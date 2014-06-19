using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MusicCollection.Fundation;

namespace MusicCollection.ToolBox
{
    internal class AdminEmail : IEmail
    {
        private readonly GenericGmail _Gmail;
        private readonly EmailActor _GmailReceptor;

         //musiccollectionadmin 
                
         //       music.collection.noreply

       
        //private const string _AdminName="Music collection Administator";
        //private const string _DestName= "David Desmaisons";

        //private static readonly EmailActor _GmailAdmin;
        //private static readonly EmailActor _GmailReceptor;

        //static AdminEmail()
        //{
        //    _AdminAdress = Properties.Settings.Default.AdministrativeEmail;
        //    _AdminPassword = Properties.Settings.Default.AdministrativeEmailPassword;
        //    _GmailAdmin = new EmailActor(_AdminName, _AdminAdress);
        //    _GmailReceptor = new EmailActor(_DestName, _Dest);
 
        //}

        internal AdminEmail(GenericGmail iGmail, EmailActor iGmailReceptor)
        {
            _Gmail = iGmail;
            _GmailReceptor = iGmailReceptor;
        } 

        public string Title
        {
            get { return _Gmail.Title; }
            set { _Gmail.Title=value; }
        }

        public string Message
        {
            get { return _Gmail.Message; }
            set { _Gmail.Message = value; }
        }

        //public AdminEmail()
        //{
        //    _Gmail = new GenericGmail(_GmailAdmin,_AdminPassword);
        //}

        public bool Send()
        {
            return _Gmail.Send(_GmailReceptor);
        }
    }
}
