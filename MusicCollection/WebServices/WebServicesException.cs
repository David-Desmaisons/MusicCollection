using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;

namespace MusicCollection.WebServices
{
    internal class WebServicesException : Exception
    {
        internal WebServicesException(InternetFailed iEvent):base("MusicCollection WebServices Exception",iEvent.Exception)
        {
            Event = iEvent;
        }

        internal InternetFailed Event
        {
            get;private set;
        }
    }
}
