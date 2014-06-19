using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardRarSettings : IUnrarUserSettings
    {
        internal StandardRarSettings()
        {
        }

        public bool AddPassword(string ipw)
        {
            if (string.IsNullOrEmpty(ipw))
                return false;

            if (Settings.Default.RarPasswords == null)
            {
                var res = new System.Collections.Specialized.StringCollection();
                res.Add(ipw);
                Settings.Default.RarPasswords = res;
                return true;
            }
            else
            {
                if (!Settings.Default.RarPasswords.Contains(ipw))
                {
                    Settings.Default.RarPasswords.Add(ipw);
                    return true;
                }
            }

            return false;
        }

       public IEnumerable<string> EnumerableRarPasswords
        {
            get
            {
                return (Settings.Default.RarPasswords == null) ? 
                    Enumerable.Empty<string>()
                    : Settings.Default.RarPasswords.Cast<string>().Where(s => !String.IsNullOrEmpty(s));
            }
        }

       public string[] RarPasswords
       {
           //get { return RarPasswords.ToArray(); }
           get { return EnumerableRarPasswords.ToArray(); }         
           set
           {
               Settings.Default.RarPasswords = new System.Collections.Specialized.StringCollection();
               if (value == null)
                   return;
               Settings.Default.RarPasswords.AddRange(value.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray());
           }
       }

        public bool AddUseRarPasswordToList
        {
            get { return Settings.Default.AddUseRarPasswordToList; }
            set { Settings.Default.AddUseRarPasswordToList = value; }
        }

        public Fundation.CompleteFileBehaviour RarZipFileAfterSuccessfullExtract
        {
            get { return Settings.Default.RarZipFileAfterSuccessfullExtract; }
            set { Settings.Default.RarZipFileAfterSuccessfullExtract = value; }
        }

        public Fundation.CompleteFileBehaviour RarZipFileAfterFailedExtract
        {
            get { return Settings.Default.RarZipFileAfterFailedExtract; }
            set { Settings.Default.RarZipFileAfterFailedExtract = value; }
        }

        public ConvertFileBehaviour RarExctractManagement
        {
            get { return Settings.Default.RarExctractManagement; }
            set { Settings.Default.RarExctractManagement = value; }
        }
    }
}
