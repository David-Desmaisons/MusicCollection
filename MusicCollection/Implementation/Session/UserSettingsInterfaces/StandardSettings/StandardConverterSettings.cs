using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardConverterSettings : IConverterUserSettings
    {
        public ConvertFileBehaviour FileCreatedByConvertion
        {
            get { return Settings.Default.FileCreatedByConvertion; }
            set { Settings.Default.FileCreatedByConvertion = value; }
        }

        public PartialFileBehaviour SourceFileUsedForConvertion
        {
            get { return Settings.Default.SourceFileUsedForConvertion; }
            set { Settings.Default.SourceFileUsedForConvertion = value; }
        }

        public PartialFileBehaviour ConvertedFileExtractedFromRar
        {
            get { return Settings.Default.ConvertedFileExtractedFromRar; }
            set { Settings.Default.ConvertedFileExtractedFromRar = value; }
        }

        public string BassUser
        {
            get { return ConfigurationManager.AppSettings["BassUser"]; }
        }

        public string BassPassword
        {
            get { return ConfigurationManager.AppSettings["BassPassword"]; }
        }

    }
}
