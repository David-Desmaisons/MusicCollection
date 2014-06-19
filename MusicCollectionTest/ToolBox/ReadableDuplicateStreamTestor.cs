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
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class ReadableDuplicateStreamTestor : TestStreamBase
    {

        private string _FS;
        private string _SS;
        private string _TS;

        [SetUp]
        public void S_U()
        {
            _FS = this.GetFileOutNameRandom(".bin");
            using (var fileStream = File.Create(_FS))
            {
                using (var fs = this.GetFirstStream())
                {
                    fs.CopyTo(fileStream);
                }
            }

            _SS = this.GetFileOutNameRandom(".bin");
            using (var fileStream = File.Create(_SS))
            {
                using (var fs = this.GetSecondStream())
                {
                    fs.CopyTo(fileStream);
                }
            }

            _TS = this.GetFileOutNameRandom(".bin");
            using (var fileStream = File.Create(_TS))
            {
                using (var fs = this.GetThirdStream())
                {
                    fs.CopyTo(fileStream);
                }
            }

          
        }

        [Test]
        public void TestBasicDuplicate_BadCall()
        {
            ReadableDuplicateStream target = null;

            Action fe = () => target = new ReadableDuplicateStream(_SS, 3, 0);
            fe.ShouldThrow<Exception>();

            Action fe2 = () => target = new ReadableDuplicateStream(_SS, -2, 2);
            fe2.ShouldThrow<Exception>();
        }

        [Test]
        public void TestBasicDuplicate_BadCall2()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_SS, 0, 5);

            Action fe = () => target.SetLength(90);
            fe.ShouldThrow<NotSupportedException>();

            Action fe2 = () => target.Write(new Byte[1]{0},0,1);
            fe2.ShouldThrow<NotSupportedException>();

            Action fe3 = () => target.WriteByte(0);
            fe3.ShouldThrow<NotSupportedException>();

            int res = 0;
            Action fe4 = () => res = target.ReadTimeout;
            fe3.ShouldThrow<NotSupportedException>();

            Action fe5 = () => res = target.WriteTimeout;
            fe5.ShouldThrow<Exception>();

            Action fe6 = () => target.ReadTimeout = 0;
            fe6.ShouldThrow<Exception>();

            Action fe7 = () => target.WriteTimeout = 0;
            fe7.ShouldThrow<Exception>();

            IAsyncResult resa = null;
            Action fe8 = () => resa = target.BeginWrite(new byte[5], 0, 1, null, null);
            fe8.ShouldThrow<Exception>();

            Action fe9 = () => target.EndWrite(resa);
            fe9.ShouldThrow<Exception>();        
        }

        [Test]
        public void TestBasicDuplicate()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_FS,0,5);
            target.CanWrite.Should().BeFalse();
            target.CanRead.Should().BeTrue();
            target.CanSeek.Should().BeTrue();
            target.Compare(this.GetFirstStream()).Should().BeTrue();
            Action ac = () => target.Flush();
            ac.ShouldNotThrow();

            target.Length.Should().Be(5);
        }

        [Test]
        public void TestBasicDuplicate_2()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_FS, 0, 5);
            target.CanWrite.Should().BeFalse();
            target.CanRead.Should().BeTrue();
            target.CanSeek.Should().BeTrue();
            target.Length.Should().Be(5);

            var cpt = EnumerableFactory.CreateList(6, i => target.ReadByte());
            cpt[5].Should().Be(-1);
            cpt.Take(5).Select(i=>(byte)i).Should().Equal(this.GetFirstbuffer());
        }

        [Test]
        public void TestBasicDuplicate_Decal()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_FS, 3, 4);
            target.CanWrite.Should().BeFalse();
            target.CanRead.Should().BeTrue();
            target.CanSeek.Should().BeTrue();
            target.Length.Should().Be(2);

            var cpt = EnumerableFactory.CreateList(3, i => target.ReadByte());
            cpt[2].Should().Be(-1);
            cpt.Take(2).Should().Equal(1,1);
        }

        [Test]
        public void TestBasicDuplicate_Async()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_FS, 1, 4);
            target.CanWrite.Should().BeFalse();
            target.CanRead.Should().BeTrue();
            target.CanSeek.Should().BeTrue();
            target.Length.Should().Be(4);

            byte[] buffer = new byte[4];
            IAsyncResult resas = target.BeginRead(buffer, 0, 4, null, null);
            int res = target.EndRead(resas);

            res.Should().Be(4);
            buffer.Should().Equal( (byte)0, (byte)0, (byte)1, (byte)1);
        }

        [Test]
        public void TestBasicDuplicate_Decal_Seek()
        {
            ReadableDuplicateStream target = new ReadableDuplicateStream(_FS, 2, 4);
            target.CanWrite.Should().BeFalse();
            target.CanRead.Should().BeTrue();
            target.CanSeek.Should().BeTrue();
            target.Length.Should().Be(3);

            var cpt = EnumerableFactory.CreateListWhile(() =>
            { int r = target.ReadByte(); return (r == -1) ? new Tuple<bool, byte>(false, 0) : new Tuple<bool, byte>(true,(byte)r); });

            cpt.Count.Should().Be(3);
            cpt.Should().Equal((byte)0, (byte)1, (byte)1);
            target.ReadByte().Should().Be(-1);

            target.Seek(1, SeekOrigin.Begin);
            var cpt2 = EnumerableFactory.CreateListWhile(() =>
            { int r = target.ReadByte(); return (r == -1) ? new Tuple<bool, byte>(false, 0) : new Tuple<bool, byte>(true, (byte)r); });
            cpt2.Should().Equal((byte)1, (byte)1);

            target.Seek(-3, SeekOrigin.End);
            var cpt3 = EnumerableFactory.CreateListWhile(() =>
            { int r = target.ReadByte(); return (r == -1) ? new Tuple<bool, byte>(false, 0) : new Tuple<bool, byte>(true, (byte)r); });
            cpt3.Should().Equal((byte)0, (byte)1, (byte)1);
        }

    }
}
