using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollection.ToolBox;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.Web;


namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class FileInternalToolBoxTestor:TestBase
    {

         [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void SetDown()
        {
        }

        [Test]
        public void BasicTest()
        {
            string pp = Path.Combine(this.MYIn, "toto.txt");
            Assert.That(!File.Exists(pp));
            string res = FileInternalToolBox.CreateNewAvailableName(pp);
            Assert.That(res, Is.EqualTo(pp));

            res = FileInternalToolBox.CreateNewAvailableName(this.MYIn, "toto",".txt");
            Assert.That(res, Is.EqualTo(pp));

            string pp2 = Path.Combine(this.MYIn, "Dummy.txt");
            Assert.That(File.Exists(pp2));
            res = FileInternalToolBox.CreateNewAvailableName(this.MYIn, "Dummy", ".txt");
            Assert.That(res, Is.Not.EqualTo(pp2));

            string exres = Path.Combine(this.MYIn, "Dummy(1).txt");
            Assert.That(res, Is.EqualTo(exres));
            Assert.That(!File.Exists(exres));

            res = FileInternalToolBox.CreateNewAvailableName(pp2);
            Assert.That(res, Is.EqualTo(exres));

            string namepour = "totototototototototototototototototototototototototototototototototototototototototototototototottotottoototototootottototototoottotototoototootottototootototoototototototototototo.txt";

            string pp3 = Path.Combine(this.MYIn, namepour);
            res = FileInternalToolBox.CreateNewAvailableName(this.MYIn, namepour, ".txt");
            Assert.That(res, Is.Not.EqualTo(pp3));
            Assert.That(res.Length, Is.EqualTo(255));
            Assert.That(Path.GetExtension(res), Is.EqualTo(".txt"));
            Assert.That(Path.GetDirectoryName(res), Is.EqualTo(this.MYIn));

            using (FileStream fs = File.Create(res))
            {
                Assert.That(fs, Is.Not.Null);
            }

            Assert.That(File.Exists(res));
            string old = res;

            res = FileInternalToolBox.CreateNewAvailableName(this.MYIn, namepour, ".txt");
            Assert.That(res, Is.Not.EqualTo(pp3));
            Assert.That(res, Is.Not.EqualTo(old));
            Assert.That(res.Length, Is.EqualTo(255));
            Assert.That(Path.GetExtension(res), Is.EqualTo(".txt"));
            Assert.That(Path.GetDirectoryName(res), Is.EqualTo(this.MYIn));
            Assert.That(!File.Exists(res));



            File.Delete(old);

        }

        [Test]
        public void FileInternalToolBox_Test()
        {
              string res  = FileInternalToolBox.GetFileName("");
              res.Should().BeNull();

              res  = FileInternalToolBox.GetFileName(Path.GetTempPath());
              res.Should().BeNull();

              res = FileInternalToolBox.GetFileName(Path.Combine( Path.GetTempPath(), "to.ex" ));
              res.Should().BeNull();

              res = FileInternalToolBox.GetFileName(Path.Combine(Path.GetTempPath(), "to.ext"));
              res.Should().Be("to.ext");

              res = FileInternalToolBox.GetFileName(Path.Combine(Path.GetTempPath(), "to.jpeg"));
              res.Should().Be("to.jpeg");

      
        }

        [Test]
        public void GetAvailableCDDriver_Test()
        {
            IEnumerable<char> res = FileInternalToolBox.GetAvailableCDDriver();
            res.Should().Contain('D');
        }

        [Test]
        public void AvailableFreeSpace_Test()
        {
            long res = FileInternalToolBox.AvailableFreeSpace("Z:\\");
            res.Should().Be(0);

            res = FileInternalToolBox.AvailableFreeSpace("C:\\");
            res.Should().BePositive();
        }

        [Test]
        public void CreateFolder_Test()
        {
            string res = FileInternalToolBox.CreateFolder(this.MYOut, "DirInNew");
            res.Should().NotBeNull();
            res.Should().StartWith(this.MYOut);
            Path.GetFileName(res).Should().Be("DirInNew");

            DirectoryInfo dir = new DirectoryInfo(res);
            dir.Exists.Should().BeTrue();
            dir.Parent.FullName.Should().Be(this.MYOut);
            

            string res2 = FileInternalToolBox.CreateFolder(this.MYOut, "DirInNew");
            res2.Should().NotBeNull();
            res2.Should().StartWith(this.MYOut); 
            res2.Should().NotBe(res);
            DirectoryInfo dir2 = new DirectoryInfo(res2);
            dir2.Exists.Should().BeTrue();
            dir2.Parent.FullName.Should().Be(this.MYOut);
            Path.GetFileName(res2).Should().Be("DirInNew(1)");

           string res3 = FileInternalToolBox.CreateFolder(this.MYOut, "DirInNew");
           res3.Should().NotBeNull();
           res3.Should().StartWith(this.MYOut);
           res3.Should().NotBe(res);
           DirectoryInfo dir3 = new DirectoryInfo(res3);
           dir3.Exists.Should().BeTrue();
           dir3.Parent.FullName.Should().Be(this.MYOut);
           Path.GetFileName(res3).Should().Be("DirInNew(2)");
           
        }

        [Test]
        public void GetApplicationDirectoryName_Test()
        {
            string res = FileInternalToolBox.GetApplicationDirectoryName();
            Directory.Exists(res).Should().BeTrue();
            res.Should().EndWith("Music Collection");

        }
    }

    
}
