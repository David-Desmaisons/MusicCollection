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
        internal WebServicesException(InternetFailedArgs iEvent):base("MusicCollection WebServices Exception",iEvent.Exception)
        {
            Event = iEvent;
        }

        internal InternetFailedArgs Event
        {
            get;private set;
        }
    }
}
