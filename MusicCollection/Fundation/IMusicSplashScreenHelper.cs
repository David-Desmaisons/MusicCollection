using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public interface IMusicSplashScreenHelper
    {
        void GenerateIfNeccessary();

        string Path
        {
            get;
        }

        string Path1
        {
            get;
        }

        double Heigth
        {
            get;
        }

        double Width
        {
            get;
        }


    }
}
