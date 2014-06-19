using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;


namespace MusicCollection.DataExchange
{
    internal class AlbumBuilder : DataExchanger<AlbumDescriptor>, IAlbumDescriptorBuilder
    {
        private DataExportImportType _DIT;

        internal AlbumBuilder(DataExportImportType dit)
        {
            _DIT = dit;
        }

        IAttributeObjectBuilder<TrackDescriptor> ITracksDescriptorBuilder.DescribeNewTrack()
        {
            return new TrackBuilder(_DIT, () => Object.CreateChild());
        }

        public void DescribeAttribute(string AttributeName, Func<string> AttributeValue)
        {
            base.DescribeAttribute(AttributeName, AttributeValue, _DIT);
        }

        public AlbumDescriptor Mature()
        {
            Object.Mature(_DIT);
            return Object;
        }
    }

    internal class ITunesTracksBuilder : ITracksDescriptorBuilder
    {
        internal ITunesTracksBuilder()
        {
        }

        IAttributeObjectBuilder<TrackDescriptor> ITracksDescriptorBuilder.DescribeNewTrack()
        {
            return new TrackBuilder(DataExportImportType.iTunes);
        }
    }


    internal class TrackBuilder : DataExchanger<TrackDescriptor>, IAttributeObjectBuilder<TrackDescriptor>
    {
        private DataExportImportType _DIT;

        internal TrackBuilder(DataExportImportType dit, Func<TrackDescriptor> fact=null):base(fact)
        {
            _DIT = dit;
        }

        public void DescribeAttribute(string Attribute, Func<string> Value)
        {
            base.DescribeAttribute(Attribute, Value, _DIT);
        }

        TrackDescriptor IAttributeObjectBuilder<TrackDescriptor>.Mature()
        {
            Object.Mature();
            return Object;
        }
    }
}
