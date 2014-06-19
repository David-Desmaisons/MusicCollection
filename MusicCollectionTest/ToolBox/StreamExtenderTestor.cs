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
    internal class StreamExtenderTestor : TestStreamBase
    {

        [Test]
        public void Stream_Different_Different_Size()
        {
            Stream f = GetFirstStream();
            Stream s = GetSecondStream();

            f.Compare(s).Should().BeFalse();

            f.Dispose();
            s.Dispose();
        }

        [Test]
        public void Stream_Different_Same_Size()
        {
            Stream f = GetFirstStream();
            Stream s = GetThirdStream();

            f.Compare(s).Should().BeFalse();

            f.Dispose();
            s.Dispose();
        }

        [Test]
        public void Stream_Same_Buffer()
        {
            Stream f = GetFirstStream();
            Stream s = GetFirstStream();

            f.Compare(s).Should().BeTrue();

            f.Dispose();
            s.Dispose();
        }

        [Test]
        public void Stream_Null()
        {
            Stream f = GetFirstStream();
            Stream ns = null;

            f.Compare(null).Should().BeFalse();

            Action sh = () => ns.Compare(f);
            sh.ShouldThrow<Exception>();

            f.Dispose();
        }

    }
}
