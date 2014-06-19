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
using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    internal class SpaceCheckerTest : TestBase
    {
        public long _SpaceAvailable;
        [SetUp]
        public void SetUp()
        {
            _SpaceAvailable = FileInternalToolBox.AvailableFreeSpace("C:\\");
        }

        [Test]
        public void Test_0()
        {
            _SpaceAvailable = FileInternalToolBox.AvailableFreeSpace("C:\\");

            SpaceChecker target = new SpaceChecker("C:\\", 0);
           target.DiskName.Should().Be("C:\\");
           target.SizeAvailable.SizeInByte.Should().Be(_SpaceAvailable);
           target.Delta.SizeInByte.Should().Be(_SpaceAvailable);
           target.OK.Should().BeTrue();
           string ts = target.ToString();
           ts.Should().Contain("Remaining");
        }

        [Test]
        public void Test_1()
        {
            _SpaceAvailable = FileInternalToolBox.AvailableFreeSpace("C:\\");

            SpaceChecker target = new SpaceChecker("C:\\", 2 * _SpaceAvailable);
            target.DiskName.Should().Be("C:\\");
            target.SizeAvailable.SizeInByte.Should().Be(_SpaceAvailable);
            (-target.Delta).SizeInByte.Should().Be(_SpaceAvailable);
            target.OK.Should().BeFalse();
            string ts = target.ToString();
            ts.Should().Contain("Missing");
        }

        [Test]
        public void Test_3()
        {
            _SpaceAvailable = FileInternalToolBox.AvailableFreeSpace("C:\\");

            string path = this.GetFileInName("A - B.mp3");
            File.Exists(path).Should().BeTrue();

            SpaceChecker target = new SpaceChecker("C:\\",path.SingleItemCollection());
            //A - B.mp3 106KB 108.943 bytes 

            target.SizeAvailable.SizeInByte.Should().Be(_SpaceAvailable);
            target.SizeNeeded.SizeInByte.Should().Be(108943);
            target.Delta.SizeInByte.Should().Be(_SpaceAvailable - 108943);
            target.OK.Should().BeTrue();
            string ts = target.ToString();
            ts.Should().Contain("Remaining");
        }
    }
}
