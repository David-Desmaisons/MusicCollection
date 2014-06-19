using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using NUnit;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;
using MusicCollection.DataExchange;
using MusicCollection.Fundation;


namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    public class DiscIDsTestor
    {

        [Test]
        public void Test()
        {
            DiscIDs target = DiscIDs.FromIDsAndHashes("iAsin", "MusicBrainz_ID", "iCDDBh", "iMusicBrainz_HashID");

            target.Asin.Should().Be("iAsin");
            target.CDDB.Should().Be("iCDDBh");
            target.MusicBrainzCDId.Should().Be("iMusicBrainz_HashID");
            target.MusicBrainzID.Should().Be("MusicBrainz_ID");
            target.CDDBQueryString.Should().BeNull();

            IDiscIDs t = target;

            t.IsEmpty.Should().BeFalse();

            t.Equals(null).Should().BeFalse();
            t.Equals(t).Should().BeTrue();

            target.Interface.Should().Be(t);

            DiscIDs target2 = DiscIDs.FromIDsAndHashes("iAsin", "MusicBrainz_ID", "iCDDBh", "iMusicBrainz_HashID");
            target.Equals(target2).Should().BeTrue();
            target.GetHashCode().Should().Be(target2.GetHashCode());

            DiscIDs target3 = DiscIDs.FromIDsAndHashes("Asin", "MusicBrainz_ID", "iCDDBh", "iMusicBrainz_HashID");
            target3.Equals(target2).Should().BeFalse();

            DiscIDs target4 = DiscIDs.FromIDsAndHashes("iAsin", "MusicBrainz_IDi", "iCDDBh", "iMusicBrainz_HashID");
            target4.Equals(target2).Should().BeFalse();

            DiscIDs target5 = DiscIDs.FromIDsAndHashes("iAsin", "MusicBrainz_ID", "iCDDBht", "iMusicBrainz_HashID");
            target5.Equals(target2).Should().BeFalse();

            DiscIDs target6 = DiscIDs.FromIDsAndHashes("iAsin", "MusicBrainz_ID", "iCDDBh", "iMusicBrainz_HashIDh");
            target6.Equals(target2).Should().BeFalse();

            target6.ToString().Should().NotBeNull();
        }

        [Test]
        public void Test_Empty()
        {
            DiscIDs target = DiscIDs.FromIDsAndHashes(null,null,null,null);
            
            IDiscIDs t = target;
            t.IsEmpty.Should().BeTrue();

            DiscIDs target2 = DiscIDs.FromIDsAndHashes(null, null, null, null);

            target.Equals(target2).Should().BeTrue();

        }
    }
}
