using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MusicCollection.SettingsManagement;

namespace MusicCollection.Fundation
{
    
    public interface IMusicSettings
    {
        IMusicImporterExporterUserSettings MusicImporterExporter { get; }

        IUnrarUserSettings RarFileManagement { get; }

        IMaturityUserSettings CollectionFileSettings { get; }

        IImageFormatManagerUserSettings ImageFormatManagerUserSettings { get; }

        IWebUserSettings WebUserSettings { get; }

        IConverterUserSettings ConverterUserSettings { get; }

        IAparencyUserSettings AparencyUserSettings { get; }

        IiTunesUserSettings iTunesSetting { get;}

        IPathUserSettings PathUserSettings { get; }

        IEmailInformationSettings EmailInformationSettings { get; }

        void SessionEnds();
    }

    public static class IMusicSettingsExtension
    {
        public static IUIGridManagement GetIUIGridManagement(this  IMusicSettings iIMusicSettings )
        {
            return new UIGridManagement(iIMusicSettings);
        }
    }
}
