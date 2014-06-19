using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Fundation;

namespace MusicCollection.Implementation
{
    class MergeStrategyFactory : IMergeStrategyFactory
    {
        internal MergeStrategyFactory()
        {
        }


        private class MergerStrategy : IMergeStrategy
        {
            internal MergerStrategy(IndividualMergeStategy iAl, IndividualMergeStategy iIm, IndividualMergeStategy iTr)
            {
                InjectAlbumImage = iIm;
                AlbumMetaData = iAl;
                TrackMetaData = iTr;
            }

            public IndividualMergeStategy InjectAlbumImage {get;private set;}

            public IndividualMergeStategy AlbumMetaData {get;private set;}

            public IndividualMergeStategy TrackMetaData { get; private set; }
        }


        public IMergeStrategy Standard
        {
            get { return new MergerStrategy(IndividualMergeStategy.IfDummyValue, IndividualMergeStategy.Always, IndividualMergeStategy.IfDummyValue); }
        }

        public IMergeStrategy OnlyIfDummy
        {
            get { return new MergerStrategy(IndividualMergeStategy.IfDummyValue, IndividualMergeStategy.IfDummyValue, IndividualMergeStategy.IfDummyValue); }
        }

        public IMergeStrategy Get( IndividualMergeStategy iAlbumMetaData, IndividualMergeStategy iInjectAlbumImage, IndividualMergeStategy iTrackMetaData)
        {
            return new MergerStrategy(iAlbumMetaData, iInjectAlbumImage, iTrackMetaData);
        }


        public IMergeStrategy Get(MergeStategy iInjectAlbumImage)
        {
            switch (iInjectAlbumImage)
            {
                case MergeStategy.OnlyCover:
                    return new MergerStrategy(IndividualMergeStategy.Never, IndividualMergeStategy.Always, IndividualMergeStategy.Never);

                case MergeStategy.CoverAndMissingData:
                    return new MergerStrategy(IndividualMergeStategy.IfDummyValue,IndividualMergeStategy.Always,  IndividualMergeStategy.IfDummyValue);

                case MergeStategy.CoverAndData:
                    return new MergerStrategy( IndividualMergeStategy.Always, IndividualMergeStategy.Always, IndividualMergeStategy.Always);
       
            }

            return null;
        }
    }
}
