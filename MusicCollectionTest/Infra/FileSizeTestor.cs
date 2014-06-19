using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Infra;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class FileSizeTestor
    {
        [Test]
        public void Test()
        {
            FileSize fs = FileSize.FromBytes(1024);
            fs.SizeInByte.Should().Be(1024);
            fs.SizeInKB.Should().Be(1);
            fs.ToString().Should().Contain("KB");
        }

        [Test]
        public void Test2()
        {
            FileSize fs = FileSize.FromKB(1024);
            fs.SizeInKB.Should().Be(1024);
            fs.SizeInMB.Should().Be(1);
            fs.ToString().Should().Contain("MB");
        }

        [Test]
        public void Test3()
        {
            FileSize fs = FileSize.FromMB(1024);
            fs.SizeInMB.Should().Be(1024);
            fs.SizeInGB.Should().Be(1);
            fs.ToString().Should().Contain("GB");
        }

        [Test]
        public void Test4()
        {
            FileSize fs = FileSize.FromGB(8);
            fs.SizeInGB.Should().Be(8);
        }

        [Test]
        public void Test5()
        {
            FileSize fs = FileSize.FromBytes(1073741824);
            fs.SizeInGB.Should().Be(1);
        }

        [Test]
        public void Test6()
        {
            FileSize fs = FileSize.FromBytes(1024);
            fs.SizeInKB.Should().Be(1);
            fs.SizeInByte.Should().Be(1024);

            FileSize fs2 = FileSize.FromBytes(25);
            fs2.SizeInByte.Should().Be(25);

            FileSize fs3 = FileSize.FromBytes(1000);
            fs3.SizeInByte.Should().Be(1000);

            FileSize add = fs + fs2;
            add.SizeInByte.Should().Be(1049);

            FileSize sub = add - fs3;
            sub.SizeInByte.Should().Be(49);


            FileSize fs4 = FileSize.FromBytes(50);
            FileSize sub2 = sub - fs4;
            sub2.SizeInByte.Should().Be(-1);
            (-sub2).SizeInByte.Should().Be(1);
        }
    }
}
