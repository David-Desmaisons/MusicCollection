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

    //public class InternetFinderResultEventArgs : EventArgs
    //{
    //    public IWebResult Found
    //    {
    //        get;
    //        private set;
    //    }

    //    public IWebQuery Album
    //    {
    //        get;
    //        private set;
    //    }


    //    public InternetFinderResultEventArgs(IWebQuery Al, IWebResult iFound)
    //    {
    //        Found = iFound;
    //        Album = Al;
    //    }
    //}

    public class InternetFailed
    {
        public Exception Exception { get; private set; }

        public WebProvider? WebService { get; set; }

        internal string AdditionalInfo { get; private set; }

        public bool InternetIsDown
        {
            get { return ((_Broken) || (Exception != null)); }
        }

        public Nullable<HttpStatusCode> Code { get; private set; }

        public bool WebServiceNotResponding { get; private set; }

        private InternetFailed(Exception iException)
        {
            Exception = iException;
            Code = null;
        }

        private bool _Broken=false;

        private InternetFailed(bool iBroken)
        {
            _Broken = iBroken;
            Exception = null;
            Code = null;
        }

        private InternetFailed(string iAdditionalInfo,Nullable<HttpStatusCode> code)
        {
            WebServiceNotResponding = true;
            AdditionalInfo = iAdditionalInfo;
            Exception = null;
            Code = code;
        }

        private InternetFailed(List<InternetFailed> errors, string iAdditionalInfo)
        {
            WebServiceNotResponding = errors[0].WebServiceNotResponding;
            AdditionalInfo = iAdditionalInfo;
            Exception = errors[0].Exception;
            Code = errors[0].Code;
        }

        static internal InternetFailed InternetDown(Exception iException)
        {
            return new InternetFailed(iException);
        }

        static internal InternetFailed InternetDown()
        {
            return new InternetFailed(true);
        }

        static internal InternetFailed WebServiceDown(string AdditionalInfo,Nullable<HttpStatusCode> code = null)
        {
            return new InternetFailed(AdditionalInfo,code);
        }

        static internal InternetFailed PartialResult(List<InternetFailed> errors)
        {
            return new InternetFailed(errors,"Partial result found");
        }

    }
}
