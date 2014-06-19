using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IEmailInformationSettings
    {
        string EmailAdress { get; }

        string Password { get; }

        string EmailReceptor { get; }
    }
}
