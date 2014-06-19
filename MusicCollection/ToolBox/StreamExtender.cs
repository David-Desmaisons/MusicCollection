using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MusicCollection.ToolBox
{
    

    static internal class StreamExtender
    {
        static internal bool Compare(this Stream stream1, Stream stream2)
        {

            if (stream1 == null)
                throw new Exception("Prohibited");

            if (stream2 == null)
                return false;

            if (((stream1.CanSeek) && (stream2.CanSeek)) && (stream1.Length != stream2.Length))
                return false;

            const int bufferSize = 2048 * 2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }
    }



}
