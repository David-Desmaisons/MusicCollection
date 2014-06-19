using MusicCollection.Fundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    [Serializable]
    internal class ManualAparencySettings : IAparencyUserSettings
    {
        public int DisplaySizer { get; set; }

        public AlbumPresenter PresenterMode { get; set; }

        public string SplashScreenPath { get; set; }

        public string SplashScreenPath1 { get; set; }

        public void Save()
        {
        }

        public AlbumFieldType AlbumSortering{ get; set; }

        public bool AlbumSorterPolarity { get; set; }

        public SettingsManagement.PersistentColumns TrackGrid { get; set; }
    }
}
