using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.Fundation
{
    public enum IndividualMergeStategy { Never,Always,IfDummyValue};

    public enum MergeStategy { OnlyCover, CoverAndMissingData , CoverAndData};

    public interface IMergeStrategy
    {
        IndividualMergeStategy InjectAlbumImage {get;}
        IndividualMergeStategy AlbumMetaData { get; }
        IndividualMergeStategy TrackMetaData { get; }
    }

    public interface IMergeStrategyFactory
    {
        IMergeStrategy Standard { get; }
        IMergeStrategy OnlyIfDummy { get; }
        IMergeStrategy Get(MergeStategy iInjectAlbumImage);
        IMergeStrategy Get( IndividualMergeStategy iInjectAlbumImage,IndividualMergeStategy iAlbumMetaData,IndividualMergeStategy iTrackMetaData);
    }
}
