using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IEmail
    {
        string Title {get;set;}
       
        string Message {get;set;}

        bool Send();
    }
}
