using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;


namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    internal class Freedb : TestBase
    {

        [Test]
        public void Test()
        {
            List<string> res = new List<string>() ;
            using (StreamReader tr = new StreamReader(Path.Combine(MYIn,"FreedbAnswer.txt")))
            {
                string lign = null;
                while ((lign = tr.ReadLine()) != null)
                {
                    res.Add(lign);
                }
            }

            IFullAlbumDescriptor ifa= AlbumDescriptor.FromFreeDBInfo(res);
            AssertAlbumDescriptor(ifa,AlbumsOld[0][0],AlbumDescriptorCompareMode.AlbumandTrackMD);
        }

        [Test]
        public void Test_Pourri()
        {
             List<string> mypourri = new List<string>(){
                    "DISCID=270b8617",
                    "DTITLE=Franske Stemninger / Con Spirito",
                    "DYEAR=1981",
                    "DGENRE=Classical",
                    "TTITLE0=Mille regretz de vous abandonner",
                    "TTITLE1=c'est con mais je vais avoir besoin",
                    "TTITLE1= de vachement d'espace pour ce titre mega bon",
                    "TTITLE2=demain",
                    "TTITLE3=aujourd'hui"
             };

             IFullAlbumDescriptor ifa = AlbumDescriptor.FromFreeDBInfo(mypourri);
             AssertAlbumDescriptor(ifa, AlbumsOld[0][1], AlbumDescriptorCompareMode.AlbumandTrackMD);
        }
    }

}
