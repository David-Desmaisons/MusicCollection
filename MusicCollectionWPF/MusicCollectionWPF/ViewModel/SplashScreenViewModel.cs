using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel
{
    public class SplashScreenViewModel
    {
        private IMusicSplashScreenHelper _Imssh;
        public SplashScreenViewModel(IMusicSplashScreenHelper imssh)
        {
            _Imssh = imssh;
            Version = string.Format("V {0}", VersionInfo.GetVersionInfo().ToString());
        }

        public string Version { get; private set; }

        public string Path
        {
            get { return _Imssh.Path; }
        }

        public string Path1
        {
            get { return _Imssh.Path1; }
        }
    }
}
