﻿using MusicCollection.Fundation;
using MusicCollection.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Implementation.Session
{
    internal class StandardMusicExporterSettings : IMusicImporterExporterUserSettings
    {
        public MusicExportType LastExportType
        {
            get { return  Settings.Default.LastExportType;}
            set { Settings.Default.LastExportType = value; }
        }

        public MusicImportType LastImportType
        {
            get { return Settings.Default.LastImportType;}
            set { Settings.Default.LastImportType = value; }
        }

        public bool SynchronizeBrokeniTunes
        {
            get { return Settings.Default.SynchronizeBrokeniTunes; }
            set { Settings.Default.SynchronizeBrokeniTunes = value;  }
        }

        public string OutputPath
        {
            get
            {
                string res = Settings.Default.ExportOutputPath;
                if (string.IsNullOrEmpty(res))
                {
                    res = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    Settings.Default.ExportOutputPath = res;
                }

                return res;
            }
            set
            {
                Settings.Default.ExportOutputPath = value;
            }
        }

        public bool OpenCDDoorOnEndImport
        {
            get { return Settings.Default.OpenCDDoorOnEndImport; }
            set { Settings.Default.OpenCDDoorOnEndImport = value; }
        }


        public string PathMove
        {
            get
            {
                string res = Settings.Default.MoveOutputPath;
                if (string.IsNullOrEmpty(res))
                {
                    res = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    Settings.Default.MoveOutputPath = res;
                }

                return res;
            }
            set
            {
                Settings.Default.MoveOutputPath = value;
            }
        }
    }
}
