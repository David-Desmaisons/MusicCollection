using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra
{
    public class SilentException : Exception 
    {
        public SilentException(string iMessage)  : base(iMessage)
        {
        }

        public bool SendEmail {get;set;}
    }
}
