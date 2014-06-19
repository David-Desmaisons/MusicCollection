using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IUnrarUserSettings
    {
        CompleteFileBehaviour RarZipFileAfterSuccessfullExtract { get; set; }

        CompleteFileBehaviour RarZipFileAfterFailedExtract { get; set; }

        ConvertFileBehaviour RarExctractManagement { get; set; }

        IEnumerable<string> EnumerableRarPasswords { get; }

        string[] RarPasswords { get; set; }

        bool AddPassword(string ipw);

        bool AddUseRarPasswordToList { get; set; }
    }
}
