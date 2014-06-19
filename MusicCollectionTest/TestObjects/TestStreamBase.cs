using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MusicCollectionTest.TestObjects
{
    internal class TestStreamBase : TestBase
    {
        protected byte[] GetFirstbuffer()
        {
            return new byte[5] { 0, 0, 0, 1, 1 };
        }

        protected byte[] GetSecondbuffer()
        {
            return new byte[1] { 0 };
        }

        protected byte[] GetThirdbuffer()
        {
            return new byte[5] { 1, 1, 1, 0, 0 };
        }

        protected Stream GetFirstStream()
        {
            return new MemoryStream(GetFirstbuffer());
        }

        protected Stream GetSecondStream()
        {
            return new MemoryStream(GetSecondbuffer());
        }

        protected Stream GetThirdStream()
        {
            return new MemoryStream(GetThirdbuffer());
        }
    }
}
