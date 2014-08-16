using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.Itunes;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    public class iTunesCDInformationReader
    {

        [Test]
        public void Test()
        {
            var CDdrive = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom).FirstOrDefault();

            if ( (CDdrive==null) || ( !CDdrive.IsReady))
            { 
                Assert.Ignore("CD Not Inserted.  Omitting.");
            }


            iTunesCDInformationFinder tc = new iTunesCDInformationFinder();
            tc.Compute(null);
            var res = tc.FoundCDInfo;
            Assert.IsNotNull(res);
            Assert.AreEqual(res.WebProvider, MusicCollection.WebServices.WebProvider.iTunes);
        }
    }
}
