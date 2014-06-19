using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MusicCollection.ToolBox
{
   
    internal class ReadableDuplicateStream : Stream
    {
        private FileStream _FS=null;
        private long _Begin;
        private long _End;

        internal ReadableDuplicateStream(string path, long Begin, long End)
        {
            _FS = new FileStream(path, FileMode.Open, FileAccess.Read);
            if ((_FS.CanRead == false) || (_FS.CanSeek == false) || (Begin>End) || (Begin<0))
                throw new ArgumentException();

            if (End > _FS.Length -1)
                End = _FS.Length -1;

            _Begin = Begin;
            _End = End;
            _FS.Seek(Begin,SeekOrigin.Begin);
        }

        public override bool CanRead { get { return true; } }       
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return false; } }


        //
        // Summary:
        //     When overridden in a derived class, gets the length in bytes of the stream.
        //
        // Returns:
        //     A long value representing the length of the stream in bytes.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     A class derived from Stream does not support seeking.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Length { get { return _End - _Begin + 1; } }


//
        // Summary:
        //     When overridden in a derived class, gets or sets the position within the
        //     current stream.
        //
        // Returns:
        //     The current position within the stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support seeking.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Position 
        {
            get { return _FS.Position-_Begin; }
            set { if ((value > Length) || (value < 0)) throw new IOException(); _FS.Position = value  + _Begin; }
        
        }


        //
        // Summary:
        //     Gets or sets a value, in miliseconds, that determines how long the stream
        //     will attempt to read before timing out.
        //
        // Returns:
        //     A value, in miliseconds, that determines how long the stream will attempt
        //     to read before timing out.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.IO.Stream.ReadTimeout method always throws an System.InvalidOperationException.
         public override int ReadTimeout { get { throw new InvalidOperationException(); } set { throw new InvalidOperationException(); } }

        //
        // Summary:
        //     Gets or sets a value, in miliseconds, that determines how long the stream
        //     will attempt to write before timing out.
        //
        // Returns:
        //     A value, in miliseconds, that determines how long the stream will attempt
        //     to write before timing out.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.IO.Stream.WriteTimeout method always throws an System.InvalidOperationException.
         public override int WriteTimeout { get { throw new InvalidOperationException(); } set { throw new InvalidOperationException(); } }

       
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (Position + count > Length)
                count = (int)(Length - Position);

 
            return _FS.BeginRead(buffer, offset , count, callback, state);
        }

        
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            _FS.Close();
        }

        
        // Summary:
        //     Releases the unmanaged resources used by the System.IO.Stream and optionally
        //     releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only
        //     unmanaged resources.
        protected override void Dispose(bool disposing)
        {
            _FS.Dispose();
        }


        //
        // Summary:
        //     Waits for the pending asynchronous read to complete.
        //
        // Parameters:
        //   asyncResult:
        //     The reference to the pending asynchronous request to finish.
        //
        // Returns:
        //     The number of bytes read from the stream, between zero (0) and the number
        //     of bytes you requested. Streams return zero (0) only at the end of the stream,
        //     otherwise, they should block until at least one byte is available.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     asyncResult is null.
        //
        //   System.ArgumentException:
        //     asyncResult did not originate from a System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        //     method on the current stream.
        //
        //   System.IO.IOException:
        //     The stream is closed or an internal error has occurred.
        public override int EndRead(IAsyncResult asyncResult)
        {
            return _FS.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new ArgumentException();
        }
   
        public override void Flush()
        {
        }

        
        
        //
        // Summary:
        //     When overridden in a derived class, reads a sequence of bytes from the current
        //     stream and advances the position within the stream by the number of bytes
        //     read.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes. When this method returns, the buffer contains the specified
        //     byte array with the values between offset and (offset + count - 1) replaced
        //     by the bytes read from the current source.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin storing the data read
        //     from the current stream.
        //
        //   count:
        //     The maximum number of bytes to be read from the current stream.
        //
        // Returns:
        //     The total number of bytes read into the buffer. This can be less than the
        //     number of bytes requested if that many bytes are not currently available,
        //     or zero (0) if the end of the stream has been reached.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The sum of offset and count is larger than the buffer length.
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset or count is negative.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support reading.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position + count > Length)
                count = (int)(Length - Position);

            return _FS.Read(buffer, offset, count);
        }

        //
        // Summary:
        //     Reads a byte from the stream and advances the position within the stream
        //     by one byte, or returns -1 if at the end of the stream.
        //
        // Returns:
        //     The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The stream does not support reading.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override int ReadByte()
        {
            if (_FS.Position > _End)
                return -1;

            return _FS.ReadByte();
        }

        //
        // Summary:
        //     When overridden in a derived class, sets the position within the current
        //     stream.
        //
        // Parameters:
        //   offset:
        //     A byte offset relative to the origin parameter.
        //
        //   origin:
        //     A value of type System.IO.SeekOrigin indicating the reference point used
        //     to obtain the new position.
        //
        // Returns:
        //     The new position within the current stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support seeking, such as if the stream is constructed
        //     from a pipe or console output.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Seek(long offset, SeekOrigin origin)
        {
            long res = 0;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    res = _FS.Seek(offset + _Begin, SeekOrigin.Begin);
                    break;

                case SeekOrigin.End:
                    res = _FS.Seek(offset + 1 + _End, SeekOrigin.Begin);
                    break;

                case SeekOrigin.Current:
                    res = _FS.Seek(offset, SeekOrigin.Current);
                    break;

            }

            return res;
        }



        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
       
       
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
    }
}
