﻿using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardAparencySettings : NotifyCompleteAdapterNoCache, IAparencyUserSettings
    {

        internal StandardAparencySettings()
        {
            _PresenterMode = Settings.Default.PresenterMode;
        }
        
        public int DisplaySizer
        {
            get { return Settings.Default.DisplaySizer; }
            set 
            { 
                if (value == Settings.Default.DisplaySizer) 
                    return; 
                Settings.Default.DisplaySizer = value; 
            }
        }

        private AlbumPresenter _PresenterMode;
        public AlbumPresenter PresenterMode
        {
            get { return _PresenterMode; }
            set { if (Set(ref _PresenterMode,value)) Settings.Default.PresenterMode = value; }
        }

        public string SplashScreenPath
        {
            get { return Settings.Default.SplashScreenPath; }
            set { Settings.Default.SplashScreenPath = value; }
        }

        public string SplashScreenPath1
        {
            get { return Settings.Default.SplashScreenPath1; }
            set { Settings.Default.SplashScreenPath1 = value; }
        }

        public AlbumFieldType AlbumSortering
        {
            get { return Settings.Default.AlbumSortering; }
            set { Settings.Default.AlbumSortering = value; }
        }

        public bool AlbumSorterPolarity
        {
            get { return Settings.Default.AlbumSorterPolarity; }
            set { Settings.Default.AlbumSorterPolarity = value; }
        } 
     
        public void Save()
        {
            Settings.Default.Save();
        }

        public SettingsManagement.PersistentColumns TrackGrid
        {
            get { return Settings.Default.TrackGrid; }
            set { Settings.Default.TrackGrid = value; }
        }
    }
}
