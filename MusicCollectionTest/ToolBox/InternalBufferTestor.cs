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
using MusicCollection.ToolBox.Buffer;
using System.IO;

namespace MusicCollectionTest.ToolBox
{

    internal static class IPersistentBufferProviderExtender
    {
        internal static IBufferProvider ShouldBeClonedOK(this IPersistentBufferProvider @this)
        {
            IBufferProvider res = @this.Clone(); 
            res.Should().NotBeNull();

            object.ReferenceEquals(res, @this).Should().BeFalse();
       
            @this.Compare(res).Should().BeTrue();

          

            return res;
           
        }

        internal static void ShouldBeCopiedOK(this IPersistentBufferProvider @this,string iPath)
        {
            bool res = @this.CopyTo(iPath);
            res.Should().BeTrue();

            File.Exists(iPath).Should().BeTrue();

            IPersistentBufferProvider bc = InternalBufferFactory.GetBufferProviderFromFile(iPath);
            bc.Should().NotBeNull();
            @this.Compare(bc).Should().BeTrue();

            @this.CopyTo("").Should().BeFalse();

        }

        internal static void ShouldBeSavedOK(this IPersistentBufferProvider @this, string iPath)
        {        
            @this.Save(null).Should().BeFalse();
            @this.Save(@"Z:\").Should().BeFalse();

            bool res = @this.Save(iPath);
            res.Should().BeTrue();
            @this.PersistedPath.Should().Be(iPath);

            File.Exists(iPath).Should().BeTrue();


            string Truncated = Path.Combine( Path.GetDirectoryName(iPath), Path.GetFileNameWithoutExtension(iPath));
            bool res2 = @this.Save(Truncated);
            res2.Should().BeTrue();
            string targetPath = Truncated + ".bin";
            @this.PersistedPath.Should().Be(targetPath);
            File.Exists(targetPath).Should().BeTrue();
        }
    }


    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    internal class InternalBufferTestor : TestBase
    {
        private string _Http_Path = @"http://upload.wikimedia.org/wikipedia/commons/0/09/Hua_Guofeng-1.jpg";
        private string _MyImage;
        //= "Hua_Guofeng-1.jpg";
        private string _MyImage2;
        private string _MyImage3;
        //= "Ford.jpg";
        //private string _User_Agent = @"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.17 Safari/537.36";
        private string _Deferer = @"http://en.wikipedia.org/wiki/File:Hua_Guofeng-1.jpg";




        [SetUp]
        public void SetUp()
        {
            _MyImage = this.GetFileInName( "Hua_Guofeng-1.jpg");
            _MyImage2 = this.GetFileInName("Ford.jpg");
            _MyImage3 = this.GetFileInName("Copy of ford.jpg");
        }


        private IPersistentBufferProvider GetHttpBuffer()
        {
            Uri path = new Uri(_Http_Path);
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromURI(path);
            target.Should().NotBeNull();
            return target;
        }

        private IPersistentBufferProvider GetPathBuffer()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            target.Should().NotBeNull();
            return target;
        }

        private IPersistentBufferProvider GetBufferBuffer(byte[] buffer)
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromArray(buffer);
            target.Should().NotBeNull();
            return target;
        }

        private IPersistentBufferProvider GetBufferBuffer()
        {
            using (var filestream = new FileStream(_MyImage,FileMode.Open))
            {
                using(var memoryStream = new MemoryStream())
                {
                    filestream.CopyTo(memoryStream);
                     byte[] buffer = memoryStream.ToArray();
                    return GetBufferBuffer(buffer);
                }
            }
        }

        private IPersistentBufferProvider GetStreamBuffer(Stream iStream)
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromStream(iStream);
            target.Should().NotBeNull();
            return target;
        }

        private IPersistentBufferProvider GetStreamBuffer()
        {
            using (var filestream = new FileStream(_MyImage, FileMode.Open))
            {
                return GetStreamBuffer(filestream);
            }
        }

        [Test]
        public void Test_Http_Download_Null_Path()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromURI(null);
            target.Should().BeNull();
        }

        private void CheckInternet()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Assert.Ignore("Internet Acess Mandatory.  Omitting.");
            }
        }

        [Test]
        public void Test_Http_Download_Valid_Path()
        {
            CheckInternet();

            Uri path = new Uri(_Http_Path);
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromURI(path);
            target.Should().NotBeNull();

            target.IsOK.Should().BeTrue();
            target.IsPersistent.Should().BeFalse();

            target.Length.Should().Be(22833);
            target.GetContentHashCode().Should().Be(-1148293485);

            target.Dispose();


        }

        [Test]
        public void Test_Http_Download_Valid_Path_With_Internet_Context()
        {
            CheckInternet();

            IHttpContextFurnisher fake = Substitute.For<IHttpContextFurnisher>();
            fake.Context.Returns(new HttpContext(FileServices.UserAgent, _Deferer));

            Uri path = new Uri(_Http_Path);
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromURI(path,fake);
            target.Should().NotBeNull();

            var happened = fake.Received().Context;

            target.IsOK.Should().BeTrue();
            target.IsPersistent.Should().BeFalse();

            target.Length.Should().Be(22833);
            target.GetContentHashCode().Should().Be(-1148293485);

            target.Dispose();
        }

        [Test]
        public void Test_InValid_Path()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromFile("bb");
            target.Should().NotBeNull();

            target.IsOK.Should().BeFalse();
            target.IsPersistent.Should().BeFalse();
            target.RawData.Should().BeNull();


            IBufferProvider T2 = target.Clone();
            T2.Should().NotBeNull();
            T2.PersistedPath.Should().Be("bb");


            target.Save(this.GetFileOutNameRandom(".jpg")).Should().BeFalse();
            target.PersistedPath.Should().Be("bb");

            target.CopyTo(this.GetFileOutNameRandom(".jpg")).Should().BeFalse();

            T2.Dispose();
            target.Dispose();
        }

        [Test]
        public void Test_Valid_Path()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            target.Should().NotBeNull();

            target.IsOK.Should().BeTrue();
            target.IsPersistent.Should().BeFalse();
            target.PersistedPath.Should().Be(_MyImage);
            target.Length.Should().Be(22833);
            target.RawData.Length.Should().Be(22833);
            target.GetContentHashCode().Should().Be(-1148293485);

            target.Dispose();
        }

        [Test]
        public void Test_Valid_Path_2()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromFile(_MyImage2);
            target.Should().NotBeNull();

            target.IsOK.Should().BeTrue();
            target.IsPersistent.Should().BeFalse();
            target.PersistedPath.Should().Be(_MyImage2 );
            target.Length.Should().Be(2467);
            target.RawData.Length.Should().Be(2467);
            target.GetContentHashCode().Should().Be(520163002);

            target.Dispose();
    
        }

        [Test]
        public void Test_Compare_Entity()
        {
            IPersistentBufferProvider target = InternalBufferFactory.GetBufferProviderFromFile(_MyImage2);
            target.Should().NotBeNull();

            IPersistentBufferProvider target2 = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            target2.Should().NotBeNull();

            IPersistentBufferProvider sametarget2 = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            sametarget2.Should().NotBeNull();

            IPersistentBufferProvider reallysametarget2 = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            reallysametarget2.Should().NotBeNull();

            IPersistentBufferProvider target3 = InternalBufferFactory.GetBufferProviderFromFile(_MyImage3);
            target3.Should().NotBeNull();

            target.Compare(target2).Should().BeFalse();
            target.Compare(target3).Should().BeTrue();
            target2.Compare(target).Should().BeFalse();
            target3.Compare(target).Should().BeTrue();

            reallysametarget2.Compare(sametarget2).Should().BeTrue();
            reallysametarget2.Equals(sametarget2).Should().BeTrue();

            target.Compare(null).Should().BeFalse();
            target.Compare(target).Should().BeTrue();
            target.Compare(Substitute.For<IBufferProvider>()).Should().BeFalse();
            sametarget2.Compare(target2).Should().BeTrue();

            byte[] b = target.RawData;
            byte[] b2 = target2.RawData;
            byte[] b3 = target3.RawData;

            b.Should().Equal(b3);
            b.Should().NotEqual(b2);

            target.Dispose();
            target2.Dispose();
            target3.Dispose();
            reallysametarget2.Dispose();
        }


        [Test]
        public void Test_Compare_Entity_2()
        {
            
            byte[] Buffer = new byte[2] { 0, 0 };
            IPersistentBufferProvider verydiff = InternalBufferFactory.GetBufferProviderFromArray(Buffer);
            verydiff.Should().NotBeNull();

            IPersistentBufferProvider same = InternalBufferFactory.GetBufferProviderFromFile(_MyImage);
            same.Should().NotBeNull();

            IPersistentBufferProvider diff = InternalBufferFactory.GetBufferProviderFromFile(_MyImage2);
            diff.Should().NotBeNull();

            IPersistentBufferProvider samebutbuffer = GetBufferBuffer();
            IPersistentBufferProvider samebutstream = GetStreamBuffer();

            verydiff.Compare(same).Should().BeFalse();
            verydiff.Compare(samebutbuffer).Should().BeFalse();
            same.Compare(verydiff).Should().BeFalse();
            samebutbuffer.Compare(verydiff).Should().BeFalse();

            samebutstream.Compare(samebutbuffer).Should().BeTrue();
            samebutbuffer.Compare(samebutstream).Should().BeTrue();

            samebutbuffer.Compare(samebutbuffer).Should().BeTrue();
            samebutbuffer.Compare(null).Should().BeFalse();

            samebutstream.Compare(same).Should().BeTrue();
            same.Compare(samebutstream).Should().BeTrue();

            samebutstream.Equals(same).Should().BeFalse();
            same.Equals(samebutstream).Should().BeFalse();

            verydiff.Dispose();
            same.Dispose();
            diff.Dispose();
            samebutbuffer.Dispose();
            samebutstream.Dispose();

        }

        [Test]
        public void BufferPathCloneOK()
        {
            IPersistentBufferProvider target = this.GetPathBuffer();
            IBufferProvider res = target.ShouldBeClonedOK();
            res.Dispose();
            target.Dispose();
        }

        [Test]
        public void BufferHttpCloneOK()
        {
            IPersistentBufferProvider target = this.GetHttpBuffer();
            IBufferProvider res = target.ShouldBeClonedOK();
            res.Dispose();
            target.Dispose();
        }

        [Test]
        public void BufferStreamCloneOK()
        {
            IPersistentBufferProvider target = this.GetStreamBuffer();
            IBufferProvider res = target.ShouldBeClonedOK();
            res.Dispose();
            target.Dispose();
        }

        [Test]
        public void BufferBufferCloneOK()
        {
            IPersistentBufferProvider target = this.GetBufferBuffer();
            IBufferProvider res = target.ShouldBeClonedOK();
            res.Should().Be(target);
            res.Dispose();
            target.Dispose();
        }


        [Test]
        public void BufferBufferBasic()
        {
            IPersistentBufferProvider target = this.GetBufferBuffer();
            target.Length.Should().Be(22833);
            target.RawData.Length.Should().Be(22833);
            target.GetContentHashCode().Should().Be(-1148293485);
            target.PersistedPath.Should().BeNull();

            target.Dispose();
        }

        [Test]
        public void BufferStreamBasic()
        {
            IPersistentBufferProvider target = this.GetStreamBuffer();
            target.Length.Should().Be(22833);
            target.RawData.Length.Should().Be(22833);
            target.GetContentHashCode().Should().Be(-1148293485);
            target.PersistedPath.Should().BeNull();

            target.Dispose();
        }


        [Test]
        public void BufferPathCopiedOK()
        {
            IPersistentBufferProvider target = this.GetPathBuffer();
            target.ShouldBeCopiedOK(this.GetFileOutNameRandom(".jpg"));
            target.Dispose();
        }

      
        [Test]
        public void BufferBufferCopiedOK()
        {
            IPersistentBufferProvider target = this.GetBufferBuffer();
            target.ShouldBeCopiedOK(this.GetFileOutNameRandom(".jpg"));
            target.Dispose();
        }

        [Test]
        public void BufferPathSavedOK()
        {
            IPersistentBufferProvider target = this.GetPathBuffer();
            target.ShouldBeSavedOK(this.GetFileOutNameRandom(".jpg"));
            target.Dispose();
        }


        [Test]
        public void BufferBufferSavedOK()
        {
            IPersistentBufferProvider target = this.GetBufferBuffer();
            target.ShouldBeSavedOK(this.GetFileOutNameRandom(".jpg"));
            target.Dispose();
        }


    }
}
