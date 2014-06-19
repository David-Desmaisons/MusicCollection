using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MusicCollection.Implementation;
using MusicCollection.Properties;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;

namespace MusicCollection.SettingsManagement
{
    internal class ImageManagerImpl : ImageFormatManager
    {
        private double _ImageSizeMoLimit;
        private uint _ImageNumber;
        private bool _ImageNumberLimit;


        //internal ImageManagerImpl(IImportContext Ms)
        //{
        //    _ImageSizeMoLimit = Settings.Default.ImageSizeMoLimit;
        //    _ImageNumber = Settings.Default.ImageNumberLimit;
        //    _ImageNumberLimit = Settings.Default.ImageNumber;
        //}

        //internal ImageManagerImpl( double iImageSizeMoLimit,uint iImageNumber, bool iImageNumberLimit)
        //{
        //    _ImageSizeMoLimit = iImageSizeMoLimit;
        //    _ImageNumber = iImageNumber;
        //    _ImageNumberLimit = iImageNumberLimit;
        //}

        internal ImageManagerImpl(IImageFormatManagerUserSettings iImageFormatManagerUserSettings)
        {
            _ImageSizeMoLimit = iImageFormatManagerUserSettings.ImageSizeMoLimit;
            _ImageNumber = iImageFormatManagerUserSettings.ImageNumber;
            _ImageNumberLimit = iImageFormatManagerUserSettings.ImageNumberLimit;
        }


        public byte[] GetImagetoEmbed(System.IO.Stream ImageImput)
        {
            byte[] RD = null;


            try
            {
                using (Converter iu = ImageUtility.FromStream(ImageImput, _ImageSizeMoLimit))
                {
                    RD = iu.RawData();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem creating image" + e.ToString());
            }

            return RD;
        }

        public bool IsImageRankOKToEmbed(int Rank, int Total)
        {
            if (_ImageNumberLimit == false)
                return true;

            if (Total < _ImageNumber)
                return true;

            if (Rank == 0)
                return _ImageNumber > 0;

            if (Rank == Total - 1)
                return _ImageNumber > 1;

            return (Rank <= (_ImageNumber - 2));

        }
    }
}
