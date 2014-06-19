using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Buffer;
using System.IO;
using System.Windows.Media.Imaging;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class ImageUtilityTestor : TestBase
    {
        [Test]
        public void IBufferProvider_null()
        {
            IBufferProvider bp = null;
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().BeNull();
        }

        [Test]
        public void IBufferProvider_with_persistedPath()
        {
            IBufferProvider bp = Substitute.For<IBufferProvider>();
            bp.PersistedPath.Returns(this.GetFileInName("ford.jpg"));
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().NotBeNull();
        }

        private Stream GetStream()
        {
            Stream res = new FileStream(this.GetFileInName("ford.jpg"), FileMode.Open);
            return res;
        }

        private Stream GetRottenStream()
        {
            Stream res = new FileStream(this.GetFileInName("TextFile1.txt"), FileMode.Open);
            return res;
        }


        [Test]
        public void IBufferProvider_Fail_with_persistedPath()
        {
            IBufferProvider bp = Substitute.For<IBufferProvider>();
            bp.PersistedPath.Returns(this.GetFileInName("TextFile1.txt"));
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().BeNull();
        }

        [Test]
        public void IBufferProvider_with_buffer()
        {
            IBufferProvider bp = Substitute.For<IBufferProvider>();
            bp.PersistedPath.Returns((string)null);
            bp.GetBuffer().Returns(GetStream());
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().NotBeNull();
        }

        [Test]
        public void IBufferProvider_Fail_with_buffer()
        {
            IBufferProvider bp = Substitute.For<IBufferProvider>();
            bp.PersistedPath.Returns((string)null);
            bp.GetBuffer().Returns(GetRottenStream());
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().BeNull();
        }

        [Test]
        public void IBufferProvider_Fail_with_Null_buffer()
        {
            IBufferProvider bp = Substitute.For<IBufferProvider>();
            bp.PersistedPath.Returns((string)null);
            bp.GetBuffer().Returns((Stream)null);
            BitmapImage res = bp.ImageFromBuffer();
            res.Should().BeNull();
        }


        [Test]
        public void Rotate_Teste_Save()
        {
            using (Stream str = new FileStream(this.GetFileInName("ford.jpg"), FileMode.Open))
            {
                PictureChanger target = ImageUtility.ChangerFromStreamRotation(str, true);
                target.Should().NotBeNull();
                string outpath = this.GetFileOutNameRandom(".bin");
                target.Save(outpath);
                File.Exists(outpath).Should().BeTrue();

            }
        }

        [Test]
        public void Rotate_Teste()
        {
            using (Stream str = new FileStream(this.GetFileInName("ford.jpg"), FileMode.Open))
            {
                PictureChanger target = ImageUtility.ChangerFromStreamRotation(str, false);
                target.Should().NotBeNull();
                target.Dispose();
            }
        }

        [Test]
        public void Rotate_Teste_KO()
        {
            using (Stream str = this.GetRottenStream())
            {
                PictureChanger target = ImageUtility.ChangerFromStreamRotation(str, false);
                target.Should().BeNull();
                str.Close();
            }
        }

        [Test]
        public void ChangerFromStreamResize_Teste_KO()
        {
            using (Stream str = this.GetRottenStream())
            {
                PictureChanger target = ImageUtility.ChangerFromStreamResize(str, 10);
                target.Should().BeNull();
                str.Close();
            }
        }

        [Test]
        public void ChangerFromStream_Teste_KO()
        {
            PictureChanger target = ImageUtility.ChangerFromStream(null);
            target.Should().BeNull();
        }


    }
}
