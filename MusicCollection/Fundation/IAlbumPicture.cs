using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using TagLib;
using System.Windows.Media.Imaging;

using MusicCollection.Infra;

namespace MusicCollection.Fundation
{

    public interface IAlbumPicture
    {
        string Description
        {
            get;
        }

        IPicture ConvertToIPicture();

        PictureType PictureType
        {
            get;
        }

        int Rank
        {
            get;
        }

        IAlbum Album
        {
            get;
        }

        bool CompareContent(IAlbumPicture Other);

        bool IsBroken { get; }

        BitmapImage GetImage(int DecodePixelWidth);

        BitmapImage MediumImage
        { get; }

        IEnumerable<IAlbumPicture> Split();

        IAlbumPicture CloneRotate(bool angle);

        IBufferProvider GetBuffer();

        IEqualityComparer<IAlbumPicture> Comparer { get; }
       
    }
}
