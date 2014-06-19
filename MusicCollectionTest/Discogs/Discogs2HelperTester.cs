using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;
using MusicCollection.WebServices;

using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using System.Threading;
using MusicCollection.Implementation.Session;

namespace MusicCollectionTest.Discogs
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Discogs2")]
    [NUnit.Framework.Category("InternetConnectionDependant")]
    internal class Discogs2HelperTester : TestBase
    {

        [SetUp]
        public void SU()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Assert.Ignore("Internet Acess Mandatory.  Omitting.");
            }
        }



        [Test]
        public void StringAlbumParserTester()
        {
            int i = 0;
            foreach(IFullAlbumDescriptor ifua in AlbumsOld[0])
            {
                Discogs2Finder dh = new Discogs2Finder(new StandardWebSettings());
                IWebQuery iwq = new WebQueryFactory(null).FromAlbumDescriptor(ifua);
                //iwq.NeedCoverArt = true;
                iwq.NeedCoverArt = false;
                var res = dh.Search(iwq, new CancellationToken ());
                if (i==0)
                {
                    res.Any().Should().BeTrue();
                }
                
                if (i == 1)
                {
                    List<Match<AlbumDescriptor>> reses = res.ToList();
                    Assert.That(reses.Count, Is.EqualTo(1));
                    Assert.That(reses[0].FindItem.Genre, Is.EqualTo("Jazz"));
                    Assert.That(reses[0].FindItem.Name, Is.EqualTo(ifua.Name));

                    //InternetFinder idf = new InternetFinder(Context.WebServicesManager);
                    //idf.Query = iwq;
                    //idf.Compute(true);

                    //List<WebMatch<IFullAlbumDescriptor>> ress = idf.Result.Found;
                    //Assert.That(ress.Count, Is.AtLeast(1));
                    //WebMatch<IFullAlbumDescriptor> discogs = ress.Where(o => o.WebProvider == "Discogs").FirstOrDefault();
                    //Assert.That(discogs, Is.Not.Null);
                    //AssertAlbumDescriptor(reses[0].FindItem, discogs.FindItem, AlbumDescriptorCompareMode.AlbumandTrackMD);
                }
                i++;
                dh.Dispose();
            }

 
        }
    }
}
