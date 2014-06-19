using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.WebServices;

namespace MusicCollection.Fundation
{

    //public class InternetFinderEventArgs : EventArgs
    //{
    //    public string DBName
    //    {
    //        get;
    //        private set;
    //    }

    //    public IWebQuery Album
    //    {
    //        get;
    //        private set;
    //    }

    //    public InternetFinderEventArgs(string iDBName, IWebQuery Al)
    //        : base()
    //    {
    //        DBName = iDBName;
    //        Album = Al;
    //    }
    //}

    //public class OnInternetFinderProgressEventArgs : InternetFinderEventArgs
    //{
    //    public string DisPlayProgress
    //    {
    //        get;
    //        private set;
    //    }

    //    public override string ToString()
    //    {
    //        return (DBName==null) ? DisPlayProgress: string.Format("{0} : {1} Data Base",DisPlayProgress,DBName);
    //    }

    //    //public OnInternetFinderProgressEventArgs(string iProgress,string iDBName, IWebQuery Al):base(iDBName,Al)
    //    //{
    //    //    DisPlayProgress = iProgress;
    //    //}
    //}

    public class InternetFinderResultEventArgs : EventArgs
    {
        public IWebResult Found
        {
            get;
            private set;
        }

        public IWebQuery Album
        {
            get;
            private set;
        }


        public InternetFinderResultEventArgs(IWebQuery Al, IWebResult iFound)
        {
            Found = iFound;
            Album = Al;
        }
    }

    public class InternetFailedArgs : EventArgs
    {
        public Exception Exception
        {
            get;
            private set;
        }

        public WebProvider? WebService
        {
            get;
            set;
        }

        internal string AdditionalInfo
        {
            get;
            private set;
        }

        public bool InternetIsDown
        {
            get { return ((_Broken) || (Exception != null)); }
        }

        public Nullable<HttpStatusCode> Code
        {
            get;
            private set;
        }

        public bool WebServiceNotResponding
        {
            get;
            private set;
        }

 
        private InternetFailedArgs(Exception iException)
        {
            Exception = iException;
            Code = null;
        }

        private bool _Broken=false;

        private InternetFailedArgs(bool iBroken)
        {
            _Broken = iBroken;
            Exception = null;
            Code = null;
        }

        private InternetFailedArgs(string iAdditionalInfo,Nullable<HttpStatusCode> code)
        {
            WebServiceNotResponding = true;
            AdditionalInfo = iAdditionalInfo;
            Exception = null;
            Code = code;
        }

        private InternetFailedArgs(List<InternetFailedArgs> errors, string iAdditionalInfo)
        {
            WebServiceNotResponding = errors[0].WebServiceNotResponding;
            AdditionalInfo = iAdditionalInfo;
            Exception = errors[0].Exception;
            Code = errors[0].Code;
        }

        static internal InternetFailedArgs InternetDown(Exception iException)
        {
            return new InternetFailedArgs(iException);
        }

        static internal InternetFailedArgs InternetDown()
        {
            return new InternetFailedArgs(true);
        }

        static internal InternetFailedArgs WebServiceDown(string AdditionalInfo,Nullable<HttpStatusCode> code = null)
        {
            return new InternetFailedArgs(AdditionalInfo,code);
        }

        static internal InternetFailedArgs PartialResult(List<InternetFailedArgs> errors)
        {
            return new InternetFailedArgs(errors,"Partial result found");
        }

    }
}
