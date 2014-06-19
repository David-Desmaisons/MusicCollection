using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Maths;
using MusicCollection.ToolBox.ZipTools;
using System.IO;
using MusicCollectionTest.Integrated.Tools;


namespace MusicCollectionTest.ToolBox.ZipToolsTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class UnZipZippSevenSharpTestor : IntegratedBase
    {
        private IZipper _IZipper;

        [SetUp]
        public void S_U()
        {
            _IZipper = new SevenZipZipper();
        }

        private IEnumerable<string> MyStrings()
        {
            yield return "FirstOne";
            yield return "SecondOne";
            yield return "FirstOne";
            yield return "LastOne";
        }

        private string Password
        {
            get { return "SuperMario"; }
        }


        [Test]
        public void Test_Async()
        {
            string myfile = GetFileOutName("mydummy.7z");
            var task = _IZipper.ZippAsync(MyStrings(), myfile, Password);
            task.Wait();
            bool rs = task.Result;
            rs.Should().BeTrue();
            FileInfo fi = new FileInfo(myfile);
            fi.Exists.Should().BeTrue();


            _IZipper.Check(myfile, "titi").Should().BeFalse();
            _IZipper.Check(myfile, Password).Should().BeTrue();

            var res = _IZipper.UnZipp(myfile, Password).ToList();
            res.Should().Equal(MyStrings());
        }

        [Test]
        public void Test_Sync()
        {
            string myfile = GetFileOutName("mydummy2.7z");
            bool rs = _IZipper.Zipp(MyStrings(), myfile, Password);
            rs.Should().BeTrue();
            FileInfo fi = new FileInfo(myfile);
            fi.Exists.Should().BeTrue();

            var res = _IZipper.UnZipp(myfile, Password).ToList();
            res.Should().Equal(MyStrings());
        }

        [Test]
        public void Test_AlreadyExist()
        {
            string myfile = GetFileOutName("mydummy3.7z");

            using (File.Create(myfile)){ }

            bool rs = _IZipper.Zipp(MyStrings(), myfile, Password);
            rs.Should().BeTrue();
            FileInfo fi = new FileInfo(myfile);
            fi.Exists.Should().BeTrue();

            var res = _IZipper.UnZipp(myfile, Password).ToList();
            res.Should().Equal(MyStrings());
        }

        [Test]
        public void Test_Exception()
        {
            string myfile = @"C:\totoddldl\ext.7z";

            bool rs = false;
            Action act = () => rs = _IZipper.Zipp(MyStrings(), myfile, Password);
            act.ShouldThrow<ArgumentException>();

            myfile = null;
            act = () => rs = _IZipper.Zipp(MyStrings(), myfile, Password);
            act.ShouldThrow<ArgumentNullException>();

            myfile = GetFileOutName("mydummyftg.7z");
            act = () => rs = _IZipper.Zipp(null, myfile, Password);
            act.ShouldThrow<ArgumentNullException>();

            act = () => rs = _IZipper.Check(myfile, Password);
            act.ShouldThrow<ArgumentException>();

            IEnumerable<string> res = null;
            act = () => res = _IZipper.UnZipp(@"C:\totoddldl\ext.7z", Password).ToList();
            act.ShouldThrow<ArgumentException>();

        }


    }
}
