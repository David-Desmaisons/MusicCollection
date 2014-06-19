using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualUnrarUserSettings : IUnrarUserSettings
    {
        public Fundation.CompleteFileBehaviour RarZipFileAfterSuccessfullExtract { get; set; }

        public Fundation.CompleteFileBehaviour RarZipFileAfterFailedExtract { get; set; }

        public Fundation.ConvertFileBehaviour RarExctractManagement { get; set; }

        private List<string> _RarPasswords = new List<string>();
        public IEnumerable<string> EnumerableRarPasswords
        {
            get { return _RarPasswords; }
        }

        public bool AddPassword(string ipw)
        {
            if (_RarPasswords.Contains(ipw))
                return false;

            _RarPasswords.Add(ipw);
            return true;
        }

        public bool AddUseRarPasswordToList { get; set; }


        public string[] RarPasswords
        {
            get
            {
                return _RarPasswords.ToArray();
            }
            set
            {
                _RarPasswords.Clear();
                if (value!=null)
                    _RarPasswords.AddRange(value);
            }
        }
    }
}
