using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.Fundation;
//using MusicCollection.Properties;
using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    internal class SplashScreenGenerator : IMusicSplashScreenHelper
    {
        private const uint _RegenerateSplashScreen=30;
        private IInternalMusicSession _MSI;
        private int _Heigth=1;
        private int _Width=16;
        private int _Width1 = 4;
        private string _Path=null;
        private string _Path1 = null;

        internal SplashScreenGenerator(IInternalMusicSession msi)
        {
            _MSI = msi;
        }

        internal int Capacity
        {
            get { return _Heigth * _Width; }
        }

        public double Heigth
        {
            get { return _Heigth * ImageCache.Dimension; }
        }

        public double Width
        {
            get { return _Width * ImageCache.Dimension;  }
        }

        private string GenerateSave(List<string> albim, int H, int W)
        {
            List<string> res = albim.Randomize(H*W);
            string newpath = _MSI.Folders.GetPrivatePath() + ".jpg";
            ImageUtility.CreateMosaic(res, W, H, ImageCache.Dimension, newpath);
            return newpath;
        }

        public void GenerateIfNeccessary()
        {
            try
            {
                //ulong Count = Settings.Default.SessionOpenCount;
                //Settings.Default.SessionOpenCount = Count + 1;

                var albim = (from al in _MSI.Albums let all = al as Album where all.HasImage let im = all.ImageCachePath where (im != null) select im).ToList();
           
                
                if (albim.Count < Capacity)
                {
                    ////Settings.Default.Save();
                    //_MSI.Setting.AparencyUserSettings.Save();
                    return;
                }

              
                //string oldpath = Settings.Default.SplashScreenPath;
                //string oldpath1 = Settings.Default.SplashScreenPath1;

                //Settings.Default.SplashScreenPath = GenerateSave(albim,_Heigth, _Width);
                //Settings.Default.SplashScreenPath1 = GenerateSave(albim, _Heigth, _Width1);             
                //Settings.Default.Save();

                string oldpath = _MSI.Setting.AparencyUserSettings.SplashScreenPath;
                string oldpath1 = _MSI.Setting.AparencyUserSettings.SplashScreenPath1;

                _MSI.Setting.AparencyUserSettings.SplashScreenPath = GenerateSave(albim, _Heigth, _Width);
                _MSI.Setting.AparencyUserSettings.SplashScreenPath1 = GenerateSave(albim, _Heigth, _Width1);
                _MSI.Setting.AparencyUserSettings.Save();

                if (!string.IsNullOrEmpty(oldpath1))
                    FileExtender.RevervibleFileDelete(oldpath1, false);

                if (!string.IsNullOrEmpty(oldpath))
                    FileExtender.RevervibleFileDelete(oldpath, false);

                //_Path = Settings.Default.SplashScreenPath;
                //_Path1 = Settings.Default.SplashScreenPath1;
                _Path  = _MSI.Setting.AparencyUserSettings.SplashScreenPath;
                _Path1 =  _MSI.Setting.AparencyUserSettings.SplashScreenPath1;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem during SplachScreen generation {0}",e));
            }
          
        }

        public string Path
        {
            get 
            {
                //string or = Settings.Default.SplashScreenPath;
                string or = _MSI.Setting.AparencyUserSettings.SplashScreenPath;
                if ((_Path == null) && (!string.IsNullOrEmpty(or)))
                {
                    string np = FileInternalToolBox.CreateNewAvailableName(System.IO.Path.Combine(_MSI.Folders.Temp, System.IO.Path.GetFileName(or)));
                    System.IO.File.Copy(or, np);
                    _Path = np;
                }
                return _Path; 
            }
        }

        public string Path1
        {
            get
            {
                //string or = Settings.Default.SplashScreenPath1;
                string or = _MSI.Setting.AparencyUserSettings.SplashScreenPath1;
                if ((_Path1 == null) && (!string.IsNullOrEmpty(or)))
                {
                    string np = FileInternalToolBox.CreateNewAvailableName(System.IO.Path.Combine(_MSI.Folders.Temp, System.IO.Path.GetFileName(or)));
                    System.IO.File.Copy(or, np);
                    _Path1 = np;
                }
                return _Path1;
            }
        }
    }
}
