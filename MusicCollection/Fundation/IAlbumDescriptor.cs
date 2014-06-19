using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;

using MusicCollection.Infra;
using MusicCollection.DataExchange;

namespace MusicCollection.Fundation
{
    public interface ITrackMetaDataDescriptor
    {
        IAlbumDescriptor AlbumDescriptor { get; }
        string Artist { get; }
        string Name { get; }

        uint TrackNumber { get; }

        uint DiscNumber { get; }

        TimeSpan Duration { get; }
    }

    public interface IIMageInfo
    {
        int ID { get; }
        IBufferProvider ImageBuffer { get; }
        BitmapImage Image{get;}
        
    }

    public interface IEditableTrackDescriptor : ITrackDescriptor
    {
        new string Name { get; set; }
        new uint DiscNumber { get; set; }
    }

    public interface ITrackDescriptor : ITrackMetaDataDescriptor
    {
        string Path { get; }
        string MD5 { get; }
        Stream MusicStream();
    }

    public interface IFullEditableAlbumDescriptor : IFullAlbumDescriptor
    {
        new string Name { set; get; }
        new string Artist { set; get; }
        new string Genre { set; get; }
        new Int32 Year { get; set; }

        void InjectImages(IAlbumDescriptor iad, bool MultiInject);

        List<IEditableTrackDescriptor> EditableTrackDescriptors { get; }
    }

    public interface IFullAlbumDescriptor : IAlbumDescriptor
    {
        List<ITrackMetaDataDescriptor> TrackDescriptors { get; }

        IList<IIMageInfo> Images { get; }

        IFullEditableAlbumDescriptor GetEditable();

        IEnumerable<IFullAlbumDescriptor> SplitOnDiscNumber();

        bool MatchTrackNumberOnDisk(int TN);

        bool HasImage();
    }

    public interface IAlbumDescriptor
    {
        string Artist { get; }

        string Genre { get; }

        IDiscIDs IDs { get; }

        uint TracksNumber { get; }

        string Name { get; }

        Int32 Year { get; }

    }

    public class IFullAlbumDescriptorWrapper
    {
        private LoadingAlbumDescriptor _Desc;
        public IFullAlbumDescriptorWrapper(IFullAlbumDescriptor desc)
        {
            _Desc = desc as LoadingAlbumDescriptor;
        }

        public void LoadAdditionalInfo()
        {
            if (_Desc != null)
                _Desc.Load();
        }
    }

    public static class IFullAlbumDescriptorExtension
    {
        public static void LoadImages(this IFullAlbumDescriptor desc)
        {
            new IFullAlbumDescriptorWrapper(desc).LoadAdditionalInfo();
        }
    }
}
