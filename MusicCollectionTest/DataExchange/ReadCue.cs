using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

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
    internal class ReadCue:TestBase
    {
        private List<string> _Cues;


        [SetUp]
        public void SetUp()
        {
            _Cues = (from fif in new DirectoryInfo(MYIn).GetFiles() where ((fif.Extension == ".cue") && (fif.Name != "Pres and Teddy.cue")) select fif.FullName).ToList();
        }


        [Test]
        public void Test()
        {
            Console.WriteLine("Testing {0} albums",_Cues.Count);
            List<AlbumDescriptor> cuesheetsnv = _Cues.Select(s => AlbumDescriptor.FromCUESheet(s)).OrderBy(s => s.CUESheetFileName??string.Empty).ToList();
            List<CueSheet> cuesheets = _Cues.Select(s => new CueSheet(s)).OrderBy(cs=>cs.FileName).ToList();

            Assert.That(cuesheetsnv.Count, Is.EqualTo(cuesheets.Count));

            AssertAlbumDescriptors(cuesheetsnv, AlbumsOld[0], AlbumDescriptorCompareMode.AlbumandTrackMD);
            AssertAlbumDescriptors(cuesheets, AlbumsOld[0], AlbumDescriptorCompareMode.AlbumandTrackMD, true);

            for (int i = 0; i < cuesheetsnv.Count; i++)
            {
                AlbumDescriptor ad =cuesheetsnv[i];  
                CueSheet cs = cuesheets[i];

                ad.GetCueMinLengthInseconds().Should().BeGreaterThan(-1);

                ad.CheckCueConsistency().Should().BeTrue();

                Assert.That(ad.CUESheetFileName, Is.EqualTo(cs.FileName));
                Assert.That(ad.CUEFile, Is.EqualTo(cs.Tracks[0].DataFile.Filename));
                Assert.That(ad.RawTrackDescriptors.Count, Is.EqualTo(cs.Tracks.Length));
                for (int j = 0; j < ad.RawTrackDescriptors.Count; j++)
                {
                    CueTrack ct = cs.Tracks[j];
                    TrackDescriptor td = ad.RawTrackDescriptors[j];

                    if (ct.DataFile.Filename!=null)
                        td.CUEFile.Should().Be(ct.DataFile.Filename);

                    Assert.That(ct.Indices.Length, Is.AtMost(2));
                    Assert.That(ct.Indices.Length, Is.AtLeast(1));
                    Nullable<Index> i0=null;
                    Nullable<Index> i1=null;
                    if (ct.Indices.Length == 1)
                    {
                        Assert.That(ct.Indices[0].Number == 1);
                        i1 = ct.Indices[0];
                    }
                    else
                    {
                        Assert.That(ct.Indices[0].Number == 0);
                        Assert.That(ct.Indices[1].Number == 1);
                        i0 = ct.Indices[0];
                        i1 = ct.Indices[1];
                    }

                    Assert.That(((Index)i1).TotalFrames, Is.EqualTo(td.CueIndex01.TotalFrames));
                    Assert.That(((Index)i1).TotalSeconds, Is.EqualTo(td.CueIndex01.TotalSeconds));
                    Assert.That(((Index)i1).Seconds, Is.EqualTo(td.CueIndex01.Seconds));
                    Assert.That(((Index)i1).Frames, Is.EqualTo(td.CueIndex01.Frames));

                    if (td.CueIndex00 == null)
                        Assert.That(i0, Is.Null);
                    else
                    {
                        Assert.That(i0, Is.Not.Null);
                        Assert.That(((Index)i0).TotalFrames, Is.EqualTo(td.CueIndex00.TotalFrames));
                        Assert.That(((Index)i0).TotalSeconds, Is.EqualTo(td.CueIndex00.TotalSeconds));
                        Assert.That(((Index)i0).Seconds, Is.EqualTo(td.CueIndex00.Seconds));
                        Assert.That(((Index)i0).Frames, Is.EqualTo(td.CueIndex00.Frames));
                    }
                   
                        
                }
            }

        }     
        
        [Test]
        public void TestPourri()
        {
            string name = Path.Combine(MYIn,"Pres and Teddy.cue");
            AlbumDescriptor ad = AlbumDescriptor.FromCUESheet(name);

            Assert.That(ad.CheckCueConsistency(), Is.False);
        }

        [Test]
        public void TestDiscnumber()
        {
            string name = this.GetFileInName("The Complete Novus & Columbia Recordings.cue");
            AlbumDescriptor ad = AlbumDescriptor.FromCUESheet(name);
            ad.Should().NotBeNull();

            ad.TrackDescriptors.Count.Should().Be(2);

            foreach (var t in ad.TrackDescriptors)
            {
                t.DiscNumber.Should().Be(3);
            }
        }

        
    }
}
