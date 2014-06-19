using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollection.DataExchange;
using MusicCollection.Fundation;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    class DiscHashTest
    {
        [Test]
        public void BasicTest_MusicBrainzJsonInterpretor_FromMusicBrainzReleasesResult()
        {
            DiscHash target = DiscHash.InstanciateFromQuery("iMusicBrainz_HashID", "iCDDBqtttttttt");

            target.MusicBrainzHash.Should().Be("iMusicBrainz_HashID");
            target.CDDBQueryString.Should().Be("iCDDBqtttttttt");
            target.CDDB.Should().Be("iCDDBqtt");
        }

    }
}
