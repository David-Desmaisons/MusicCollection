using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using SevenZip;

using MusicCollection.Infra;
using MusicCollection.ToolBox.ZipTools.SevenZipSharp;


namespace MusicCollection.ToolBox.ZipTools
{
    public class SevenZipZipper : IZipper
    {
        public SevenZipZipper()
        {
        }

        private void CheckArguments(IEnumerable<string> iText, string iFileName)
        {
            if (iText == null)
                throw new ArgumentNullException();

            FileInfo fif = new FileInfo(iFileName);
            if (!fif.Directory.Exists)
                throw new ArgumentException();

            if (fif.Exists)
                fif.Delete();
        }

        public async Task<bool> ZippAsync(IEnumerable<string> iText, string iFileName, string iPassword)
        {
            CheckArguments(iText, iFileName);

            SevenZipCompressor sevenZipCompressor = new SevenZipCompressor()
            {
                DirectoryStructure = true,
                EncryptHeaders = true,
                DefaultItemName = "Default.txt"
            };

            try
            {
                using (var instream = new MemoryStream())
                {
                    using (var streamwriter = new StreamWriter(instream))
                    {
                        iText.Apply(t => streamwriter.WriteLine(t));

                        await streamwriter.FlushAsync();
                        instream.Position = 0;

                        using (Stream outstream = File.Create(iFileName))
                        {
                            await sevenZipCompressor.CompressStreamAnsync(instream, outstream, iPassword);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem zipping a text: {0}", e));
                return false;
            }

            return true;
        }


        public bool Zipp(IEnumerable<string> iText, string iFileName, string iPassword)
        {
            CheckArguments(iText, iFileName);

            SevenZipCompressor sevenZipCompressor = new SevenZipCompressor()
            {
                DirectoryStructure = true,
                EncryptHeaders = true,
                DefaultItemName = "Default.txt"
            };

            try
            {
                using (var instream = new MemoryStream())
                {
                    using (var streamwriter = new StreamWriter(instream) { AutoFlush = true })
                    {
                        iText.Apply(t => streamwriter.WriteLine(t));
                        instream.Position = 0;
                        using (Stream outstream = File.Create(iFileName))
                        {
                            sevenZipCompressor.CompressStream(instream, outstream, iPassword);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem zipping a text: {0}", e));
                return false;
            }

            return true;
        }

        public IEnumerable<string> UnZipp(string iFileName, string iPassword)
        {
            FileInfo fif = new FileInfo(iFileName);
            if (!fif.Exists)
                throw new ArgumentException();

            using (var instream = new MemoryStream())
            {
                using (SevenZipExtractor sze = new SevenZipExtractor(iFileName, iPassword))
                {
                    sze.ExtractFile(0, instream);
                    instream.Position = 0;

                    using (var streamreader = new StreamReader(instream))
                    {
                        while (streamreader.Peek() >= 0)
                        {
                            yield return streamreader.ReadLine();
                        }
                    }
                }
            }

        }


        public bool Check(string iFileName, string iPassword)
        {
            FileInfo fif = new FileInfo(iFileName);
            if (!fif.Exists)
                throw new ArgumentException();

            using (SevenZipExtractor sze = new SevenZipExtractor(iFileName, iPassword))
            {
                return sze.Check();
            }

        }

    }
}
