using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using SevenZip;

namespace MusicCollection.ToolBox.ZipTools.SevenZipSharp
{
    public static class SevenZipExtender
    {
        public static Task CompressStreamAnsync(this SevenZipCompressor @this,Stream instream,Stream outstream,string iPassword)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            EventHandler<EventArgs> OnEnded=null;
            OnEnded = (o, e) => 
            { tcs.TrySetResult(null); @this.CompressionFinished -= OnEnded; };
            @this.CompressionFinished+=OnEnded;

            @this.BeginCompressStream(instream, outstream, iPassword);
            return tcs.Task;
        }

        //public static Task ExtractFileAnsync(this SevenZipExtractor @this, string Filename, Stream outstream)
        //{
        //    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

        //    EventHandler<EventArgs> OnEnded=null;
        //    OnEnded = (o, e) => { tcs.TrySetResult(null); @this.ExtractionFinished -= OnEnded; };
        //    @this.ExtractionFinished+=OnEnded;

        //    @this.BeginExtractFile(Filename, outstream);
        //    return tcs.Task;
        //}
    }
}
