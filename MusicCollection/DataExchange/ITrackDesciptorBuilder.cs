using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.FileImporter;

namespace MusicCollection.DataExchange
{
    internal interface IAlbumDescriptorBuilderEngine
    {
        AlbumDescriptor Visit(IAlbumDescriptorBuilder iBuilder);
    }


    internal interface IAttributeObjectDescriber
    {
        void DescribeAttribute(string AttributeName, Func<string> AttributeValue);
    }

    internal interface IAttributeObjectBuilder<T> : IAttributeObjectDescriber where T : class
    {
        T Mature();
    }

    internal interface ITracksDescriptorBuilder
    {
        IAttributeObjectBuilder<TrackDescriptor> DescribeNewTrack();
    }

    internal interface IAlbumDescriptorBuilder : IAttributeObjectBuilder<AlbumDescriptor>, ITracksDescriptorBuilder
    {
    }

}
