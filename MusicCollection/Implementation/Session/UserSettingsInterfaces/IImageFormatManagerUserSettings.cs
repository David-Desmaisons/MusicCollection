using System;
namespace MusicCollection.Fundation
{
    public interface IImageFormatManagerUserSettings
    {
        uint ImageNumber { get; set; }
        bool ImageNumberLimit { get; set; }
        double ImageSizeMoLimit { get; set; }
    }
}
