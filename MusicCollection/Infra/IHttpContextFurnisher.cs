using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace MusicCollection.Infra
{
    public interface IHttpContextFurnisher
    {
        HttpContext Context
        {
            get;
        }
    }
}
