using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using MusicCollection.Properties;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.Infra;


namespace MusicCollection.ToolBox
{
    internal class Converter : IDisposable
    {
        private EncoderParameters Parameters { get; set; }

        static Converter()
        {
            _JpegEncoder = GetEncoder(ImageFormat.Jpeg);
        }

        static private ImageCodecInfo _JpegEncoder;
        static private ImageCodecInfo JpegEncoder
        {
            get { return _JpegEncoder; }
        }

        static private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        private Stream _Str;


        internal Converter(Stream str, long Ratio)
        {
            _Str = str;
            Parameters = new EncoderParameters(1);
            Parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Ratio);
        }

        public void Dispose()
        {
        }


        internal void SaveTo(string Path)
        {
            using (Bitmap bm = new Bitmap(_Str))
            {
                bm.Save(Path, JpegEncoder, Parameters);
            }
        }

        internal byte[] RawData()
        {
            byte[] res = null;
            using (Bitmap bm = new Bitmap(_Str))
            {
                using (MemoryStream str = new MemoryStream())
                {
                    bm.Save(str, JpegEncoder, Parameters);
                    res = str.ToArray();
                }
            }
            return res;
        }
    }


    internal static class ImageUtility
    {

        static private Converter FromStreamAndSize(Stream str, long MoSize)
        {
            if (str == null)
                throw new Exception();

            if (MoSize <= 0)
                return new Converter(str, 100);

            if (str.CanSeek == false)
                throw new Exception("Input Error");

            long Length = str.Length;
            long Ratio = (Length == 0) ? 100 : Math.Max(5L, (MoSize * 100) / Length);

            return new Converter(str, Math.Min(100, Ratio));
        }

        static internal Converter FromStreamWithSameQuality(Stream str)
        {
            return FromStreamAndSize(str, 0);
        }

        internal static Converter FromStream(Stream str, double MoSize)
        {
            return FromStreamAndSize(str, FileSize.FromMB(MoSize).SizeInByte);
        }

        internal static PictureChanger ChangerFromStream(Stream str)
        {
            try
            {
                return new PictureChanger(str);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem managing image "+e.ToString());
                return null;
            }
        }

        internal static PictureChanger ChangerFromStreamRotation(Stream str, bool pos)
        {
            try
            {
                using (PictureChanger pc = ChangerFromStream(str))
                {
                    return pc == null ? null : pc.Rotate(pos);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem managing image"+ e.ToString());
                return null;
            }
        }

        internal static Stream CreateJpgThumbnail(Stream InStream, int desiredsize)
        {
            using (PictureChanger pc = ImageUtility.ChangerFromStreamResize(InStream, desiredsize))
            {
                if (pc != null)
                    return pc.Stream;           

                return null;
            }
        }

        internal static PictureChanger ChangerFromStreamResize(Stream str, int Size)
        {
            try
            {
                using (PictureChanger pc = ChangerFromStream(str))
                {
                    return pc == null ? null : pc.Clone(Size);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem managing image  " + e.ToString());
                return null;
            }
        }

        internal static void CreateMosaic(List<string> paths,int width,int heigth,int Dim,string outpath)
        {
            if (paths.Count < width * heigth)
                return;

            using (Bitmap m_Bitmap = new Bitmap(width * Dim, heigth * Dim))
            {
                Graphics g = Graphics.FromImage(m_Bitmap);

                for (int i = 0; i < heigth; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(paths[j + i * width]);
                        g.DrawImage(img, j * Dim, i * Dim, Dim, Dim);
                    }
                }

                m_Bitmap.Save(outpath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        internal static BitmapImage ImageFromPath(string Path)
        {
            try
            {
                BitmapImage BMI = new BitmapImage();

                BMI.BeginInit();
                BMI.CacheOption = BitmapCacheOption.OnLoad;
                BMI.UriSource = new Uri(Path);
                BMI.EndInit();
               
                BMI.Freeze();

                return BMI;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
        }

        internal static BitmapImage ImageFromBuffer(this Stream SM, int? DecodePixelWidth)
        {
            try
            {
                BitmapImage BMI = new BitmapImage();

                BMI.BeginInit();
                if (DecodePixelWidth != null)
                    BMI.DecodePixelWidth = (int)DecodePixelWidth;

                BMI.CacheOption = BitmapCacheOption.OnLoad;
                BMI.StreamSource = SM;
                BMI.EndInit();

                BMI.Freeze();

                return BMI;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
        }

        internal static BitmapImage ImageFromBuffer(this IBufferProvider ibp, int? DecodePixelWidth)
        {
            using (Stream SM = ibp.GetBuffer())
            {
                return SM==null? null : SM.ImageFromBuffer(DecodePixelWidth);
            }
        }

        internal static BitmapImage ImageFromBuffer(this IBufferProvider ibp)
        {
            if (ibp == null)
                return null;

            string path = ibp.PersistedPath;

            if (path != null)
                return ImageFromPath(path);

            using (Stream SM = ibp.GetBuffer())
            {
                return SM == null ? null : SM.ImageFromBuffer(null);
            }
        }
    }

    internal class PictureChanger : IDisposable
    {

        private Image _Image;

        internal PictureChanger(Stream str)
        {
            _Image = Image.FromStream(str);
        }

        private PictureChanger(Image iImage)
        {
            _Image = iImage;
        }

        internal void Save(string filename)
        {
            _Image.Save(filename);
        }

        internal Stream Stream
        {
            get
            {
                Stream res = new MemoryStream();

                _Image.Save(res, ImageFormat.Jpeg);

                return res;
            }
        }


        public void Dispose()
        {
            _Image.Dispose();
        }

        internal PictureChanger Clone(int DesiredSize)
        {
            try
            {
                int sourceWidth = _Image.Width;
                int sourceHeight = _Image.Height;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)DesiredSize / (float)sourceWidth);
                nPercentH = ((float)DesiredSize / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap b = new Bitmap(destWidth, destHeight);

                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    g.DrawImage(_Image, 0, 0, destWidth, destHeight);
                }

                return new PictureChanger(b);
            }

            catch (Exception e)
            {
                Trace.WriteLine("Problem duplicating an image" + e.ToString());
            }

            return null;
        }

        internal PictureChanger Rotate(bool pos)
        {
            try
            {
                Image returnBitmap = (Image)_Image.Clone();

                returnBitmap.RotateFlip(pos ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipNone);

                return new PictureChanger(returnBitmap);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem duplicating an image" + e.ToString());
            }

            return null;
        }

    }
}
