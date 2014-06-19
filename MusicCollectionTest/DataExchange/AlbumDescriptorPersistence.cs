using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.FileConverter;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    class AlbumDescriptorPersistence : TestBase
    {
        private List<string> _Cues;


        [SetUp]
        public void SetUp()
        {
            _Cues = (from fif in new DirectoryInfo(MYIn).GetFiles() where (fif.Extension == ".cue") select fif.FullName).ToList();
        }


        [Test]
        public void Test()
        {
            Console.WriteLine("Testing {0} albums", _Cues.Count);

            AssertAlbumDescriptors(_Cues.Select(s => AlbumDescriptor.FromCUESheet(s)), AlbumsOld[0], AlbumDescriptorCompareMode.AlbumandTrackMD);
            AssertAlbumDescriptors(_Cues.Select(s => new CueSheet(s)), AlbumsOld[0], AlbumDescriptorCompareMode.AlbumandTrackMD, true);

            string mpath = Path.Combine(MYOut, "t1.xml");

            AlbumDescriptor res = AlbumDescriptor.FromCUESheet(_Cues[0]);

            XmlSerializer xs = new XmlSerializer(typeof(TrackDescriptor));
            using (Stream s = File.Create(mpath))
                xs.Serialize(s, res.RawTrackDescriptors[0] );

            mpath = Path.Combine(MYOut, "a1.xml");
            xs = new XmlSerializer(typeof(AlbumDescriptor));
            using (Stream s = File.Create(mpath))
                xs.Serialize(s, res);


        }
    }
}
