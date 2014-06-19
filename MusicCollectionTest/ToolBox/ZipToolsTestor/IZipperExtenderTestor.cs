using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Maths;
using MusicCollection.ToolBox.ZipTools;

namespace MusicCollectionTest.ToolBox.ZipToolsTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class IZipperExtenderTestor
    {
        private IZipper _IZipper;
  

        [SetUp]
        public void SetUp()
        {
            _IZipper = Substitute.For<IZipper>();
        }

        [Test]
        public void Test_Zipp()
        {
            Dictionary<string, string> ser = new Dictionary<string, string>();
            ser.Add("1", "a");
            ser.Add("2", "b");
            _IZipper.SerializeSafe(ser, "huju", "uiui");

            _IZipper.Received().Zipp(Arg.Is<IEnumerable<string>>(s => s.SequenceEqual(new string[] { "1", "a", "2", "b" })), "huju", "uiui");
        }

        [Test]
        public void Test_ZippAsync()
        {
            Dictionary<string, string> ser = new Dictionary<string, string>();
            ser.Add("1", "a");
            ser.Add("2", "b");
            _IZipper.SerializeSafeAsync(ser, "huju", "uiui");

            _IZipper.Received().ZippAsync(Arg.Is<IEnumerable<string>>(s => s.SequenceEqual(new string[] { "1", "a", "2", "b" })), "huju", "uiui");
        }

        [Test]
        public void Test_UnSerializeSafe()
        {
            //arrange
            List<string> ls = new List<string>() { "a" };
            _IZipper.UnZipp("huju", "uiui").Returns(ls);

            IDictionary<string, string> res = _IZipper.UnSerializeSafe("huju", "uiui");

            res.Should().BeNull();
       }

        [Test]
        public void Test_UnSerializeSafe_Success()
        {
            //arrange
            List<string> ls = new List<string>() { "a","b","c","d" };
            _IZipper.UnZipp("huju", "uiui").Returns(ls);

            IDictionary<string, string> res = _IZipper.UnSerializeSafe("huju", "uiui");

            res.Should().NotBeNull();
            res.Should().HaveCount(2);
            res["a"].Should().Be("b");
            res["c"].Should().Be("d");
        }
    }
}
