using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Infra;
using System.Text.RegularExpressions;
using System.IO;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    class FileServiceTest
    {
        [Test]
        public void GetFileType_Test()
        {
            string.Empty.GetFileType().Should().Be(FileType.Unknown);
            "nn.xml".GetFileType().Should().Be(FileType.XML);
            "nn.jpg".GetFileType().Should().Be(FileType.Image);
            "nnc.cue".GetFileType().Should().Be(FileType.CueSheet);
            "neenc.mcc".GetFileType().Should().Be(FileType.Mcc);
            "lpl.mp3".GetFileType().Should().Be(FileType.Music);
            "lpl.flac".GetFileType().Should().Be(FileType.MusicToConv);
            "nj.rar".GetFileType().Should().Be(FileType.RarCompressed);
            "nj.txt".GetFileType().Should().Be(FileType.Txt);
             "nj.db".GetFileType().Should().Be(FileType.Trash);
        }

        [Test]
        public void test_const()
        {
            FileServices.WV.Should().Be(".wv");
            FileServices.FLAC.Should().Be(".flac");
            FileServices.MP3.Should().Be(".mp3");
            FileServices.AIFF.Should().Be(".aiff");
            FileServices.APE.Should().Be(".ape");
            FileServices.AAC.Should().Be(".aac");
            FileServices.M4A.Should().Be(".m4a");
            FileServices.MP4.Should().Be(".mp4");
            FileServices.OGG.Should().Be(".ogg");
            FileServices.WAV.Should().Be(".wav");
        }

          [Test]
        public void test_PotentialRemovable()
        {
            FileStatus.FileNotFoundDriverFoundFixed.PotentialRemovable().Should().BeFalse();
            FileStatus.FileExistsDriverFixed.PotentialRemovable().Should().BeFalse();
            FileStatus.FileExistsDriverRemovable.PotentialRemovable().Should().BeTrue();
            FileStatus.FileNotFoundDriverFoundRemovable.PotentialRemovable().Should().BeTrue();
            FileStatus.DriverNotFound.PotentialRemovable().Should().BeTrue();
        }

          [Test]
          public void test_GetFileStatus()
          {
              FileStatus res = FileServices.GetFileStatus(@"W:\toto.jpg");
              res.Should().Be(FileStatus.DriverNotFound);
          }

          private Match SelectString(string istring)
          {
              return Regex.Match(istring, @"^(\*\.\w{2,5};)*(\*\.\w{2,5})$");
          }


          [Test]
          public void test_GetRarFilesSelectString()
          {
              string res = FileServices.GetRarFilesSelectString();
              SelectString(res).Success.Should().BeTrue();
              res.Should().Contain("*.rar");
              res.Should().Contain("*.zip");
          }

       

          [Test]
          public void test_GetImagesFilesSelectString()
          {
              string res = FileServices.GetImagesFilesSelectString();
              SelectString(res).Success.Should().BeTrue();
              res.Should().Contain("*.jpeg");
              res.Should().Contain("*.png");
          }

          [Test]
          public void test_GetPathRoot()
          {
              string res = FileServices.GetPathRoot(@"a:\jjik\edeee");
              res.Should().Be(@"A:\");

              res = FileServices.GetPathRoot(@"a:");
              res.Should().Be(@"A:\");

              res = FileServices.GetPathRoot(@"W:\jji\");
              res.Should().Be(@"W:\");

              res = FileServices.GetPathRoot(@"jjik\edeee");
              res.Should().BeEmpty();

              res = FileServices.GetPathRoot(@"jjikedeee");
              res.Should().BeEmpty();

              res = FileServices.GetPathRoot(null);
              res.Should().BeEmpty();

              string rotten = new string(Path.GetInvalidPathChars());
              res = FileServices.GetPathRoot(rotten);
              res.Should().BeEmpty();
             

          }


        

    }
}
