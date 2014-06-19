using MusicCollection.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Fundation
{
    public interface IAparencyUserSettings
    {
        int DisplaySizer { get; set; }

        AlbumPresenter PresenterMode { get; set; }

        string SplashScreenPath { get; set; }

        string SplashScreenPath1 { get; set; }

        AlbumFieldType AlbumSortering { get; set; }

        bool AlbumSorterPolarity { get; set; }

        PersistentColumns TrackGrid{get;set;}
        
        void Save();
    }
}
