using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Diagnostics;

using TagLib;

using MusicCollection.Fundation;
using MusicCollection.Nhibernate.Blob;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Buffer;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    class AlbumImage : IAlbumPicture, IDisposable
    {
        private IBufferProvider _IBP;
        private PictureType _PT;
        private string _Description;
        private Album _Al;
        private bool? _Broken = null;
        private bool _IsDeleted = false;


        private int ID { get; set; }

        internal AlbumImage()
        { }

        internal AlbumImage(Album Al, string iDescription, IBufferProvider BP, PictureType PT)
        {
            _Description = iDescription;
            _IBP = BP;
            _PT = PT;
            _Al = Al;
        }

        public IAlbum Album
        {
            get
            {
                return _Al;
            }
        }

        public void Dispose()
        {
            if (!_IsDeleted)
            {
                _ImageSmall = null;
                _IBP.Dispose();
                _IsDeleted = true;
            }
        }

        public int Rank
        {
            get;
            internal set;
        }

        private Album Owner
        {
            get { return _Al; }
            set { _Al = value; }
        }

        string IAlbumPicture.Description
        {
            get { return _Description; }
        }


        IPicture IAlbumPicture.ConvertToIPicture()
        {

            IImportContext iic = _Al.Context;

            if (iic == null)
                throw new Exception("Session management");

            byte[] RD = null;

            using (Stream str = _IBP.GetBuffer())
            {
                RD = iic.ImageManager.GetImagetoEmbed(str);
            }

            if (RD == null)
                return null;

            IPicture IP = new Picture(new ByteVector(RD, RD.Length));
            IP.Description = _Description;
            IP.Type = _PT;

            return IP;
        }



        bool IAlbumPicture.CompareContent(IAlbumPicture Other)
        {
            AlbumImage autre = Other as AlbumImage;
            if (autre == null)
                return false;

            return _IBP.Compare(autre._IBP);
        }

        PictureType IAlbumPicture.PictureType
        {
            get { return _PT; }
        }

        public bool IsBroken
        {
            get
            {
                if (_Broken != null)
                    return (bool)_Broken;

                BitmapImage res = GetImage(1000);

                return (bool)_Broken;

            }
        }

        internal AlbumImage Clone(Album al)
        {
            return new AlbumImage(al, _Description, _IBP.Clone(), _PT);
        }

        private WeakReference<BitmapImage> _ImageSmall = null;

        private BitmapImage CachedMediumImage
        {
            get
            {
                if (_ImageSmall == null)
                    return null;

                BitmapImage res = null;

                if (!_ImageSmall.TryGetTarget(out res))
                {
                    _ImageSmall = null;
                    return null;
                }

                return res;
            }
            set
            {
                if (value == null)
                    _ImageSmall = null;

                _ImageSmall = new WeakReference<BitmapImage>(value, false);
            }
        }

        public BitmapImage MediumImage
        { get { BitmapImage res = CachedMediumImage; if (res != null) return res; res = PrivateGetImage(280); CachedMediumImage = res; return res; } }


        public BitmapImage GetImage(int DecodePixelWidth)
        { return PrivateGetImage(DecodePixelWidth); }

        internal long SizeOnDisk
        {
            get
            {
                return (_IBP == null) ? 0 : _IBP.Length;
            }
        }

        private BitmapImage PrivateGetImage(int? DecodePixelWidth)
        {
            if (_Broken == true)
                return null;

            if (_IBP == null)
            {
                _Broken = true;
                return null;
            }

            BitmapImage BMI = _IBP.ImageFromBuffer(DecodePixelWidth);
            _Broken = BMI == null;
            return BMI;
        }


        public IEnumerable<IAlbumPicture> Split()
        {

            using (Stream str = _IBP.GetBuffer())
            {
                using (Bitmap Im = new Bitmap(str))
                {
                    double ratio = (double)Im.Width / (double)Im.Height;
                    int candidat = Math.Max(1, (int)Math.Floor(ratio));
                    double percision = ratio - candidat;

                    int SplitIn = (candidat != 1) ? candidat : 2;


                    System.Drawing.GraphicsUnit point = System.Drawing.GraphicsUnit.Pixel;
                    RectangleF OriginalcropArea = Im.GetBounds(ref point);
                    float newwidth = (int)OriginalcropArea.Size.Width / SplitIn;

                    SizeF SizeImage = new SizeF(newwidth, OriginalcropArea.Size.Height);
                    PointF newP = OriginalcropArea.Location;
                    RectangleF cropArea = RectangleF.Empty;

                    for (int i = 0; i < SplitIn; i++)
                    {
                        if (i == SplitIn - 1)
                        {
                            SizeImage = new SizeF(OriginalcropArea.Size.Width - newP.X, OriginalcropArea.Size.Height);
                        }

                        cropArea = new RectangleF(newP, SizeImage);

                        using (Bitmap bm = Im.Clone(cropArea, Im.PixelFormat))
                        {
                            using (Stream ms = new MemoryStream())
                            {
                                bm.Save(ms, ImageFormat.Jpeg);
                                yield return new AlbumImage(_Al, _Description, BufferFactory.GetBufferProviderFromStream(ms), _PT);
                            }
                        }

                        newP = new PointF(newP.X + newwidth, newP.Y);
                    }

                }
            }


            yield break;
        }




        internal static AlbumImage GetFromIPicture(Album al, IPicture pict)
        {
            return new AlbumImage(al, pict.Description, BufferFactory.GetBufferProviderFromArray(pict.Data.Data), pict.Type);
        }


        internal static AlbumImage GetFromFile(Album al, string FileName, string Description = "Cover", PictureType Type = PictureType.FrontCover)
        {
            return new AlbumImage(al, Description, BufferFactory.GetBufferProviderFromFile(FileName), Type);
        }

        internal static AlbumImage GetFromBuffer(Album al, IBufferProvider ibp, string Description = "Cover", PictureType Type = PictureType.FrontCover)
        {
            return new AlbumImage(al, Description, ibp, Type);
        }


        internal static AlbumImage GetAlbumPictureFromUri(Album al, string uri, IHttpContextFurnisher Context, string Description = "Cover", PictureType iType = PictureType.FrontCover)
        {
            IBufferProvider ibp = BufferFactory.GetBufferProviderFromURI(new Uri(uri), Context);

            return (ibp == null) ? null : new AlbumImage(al, Description, ibp, iType);
        }


        internal static AlbumImage GetAlbumPictureFromBitmapSource(Album al, BitmapSource BMS, string Description = "Cover", PictureType iType = PictureType.FrontCover)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            using (Stream memoryStream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(BMS));
                encoder.Save(memoryStream);

                return new AlbumImage(al, Description, BufferFactory.GetBufferProviderFromStream(memoryStream), iType);
            }
        }

        internal string ExportTo(string Directory)
        {

            string res = null;

            try
            {
                using (Stream str = _IBP.GetBuffer())
                {
                    using (Converter iu = ImageUtility.FromStreamWithSameQuality(str))
                    {
                        String photolocation = FileInternalToolBox.CreateNewAvailableName(Path.Combine(Directory, ((this.Rank == 0) ? "Cover" : string.Format("{0}", Rank)) + ".jpg"));  //file name 
                        iu.SaveTo(photolocation);

                        res = photolocation;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem moving files " + e.ToString());
                return null;
            }

            return res;
        }


        internal string GetPath()
        {
            return _IBP.PersistedPath;
        }

        public IBufferProvider GetBuffer()
        { return _IBP; }

        private void CreateThumbnail(string Location)
        {
            using (Stream Original = _IBP.GetBuffer())
            {
                using (PictureChanger pc = ImageUtility.ChangerFromStreamResize(Original, 280))
                {
                    String photolocation = Path.Combine(Location, "Thumbnail_" + ((this.Rank == 0) ? "Cover" : string.Format("{0}", Rank)) + ".jpg");  //file name 
                    pc.Save(photolocation);
                }
            }

        }

        internal IPersistentBufferProvider CreateJpgThumbnail(int SizeInPixel)
        {
            if (_Broken == true)
                return null;

            using (Stream Original = _IBP.GetBuffer())
            {
                Stream output = ImageUtility.CreateJpgThumbnail(Original, SizeInPixel);
                if (output != null)
                    return InternalBufferFactory.GetBufferProviderFromStream(output);

                _Broken = true;

                return null;
            }

        }


        public IAlbumPicture CloneRotate(bool pos)
        {
            using (Stream Original = _IBP.GetBuffer())
            {
                using (PictureChanger pc = ImageUtility.ChangerFromStreamRotation(Original, pos))
                {
                    return pc == null ? null : new AlbumImage(_Al, _Description, BufferFactory.GetBufferProviderFromStream(pc.Stream), _PT);
                }
            }
        }

        private class AlbumImageContentComparer : IEqualityComparer<IAlbumPicture>
        {
            public bool Equals(IAlbumPicture x, IAlbumPicture y)
            {
                AlbumImage xx = x as AlbumImage;
                AlbumImage yy = y as AlbumImage;

                if (xx == null)
                    return (yy == null);

                if (yy == null)
                    return false;

                return xx._IBP.Compare(yy._IBP);
            }

            public int GetHashCode(IAlbumPicture obj)
            {
                AlbumImage xx = obj as AlbumImage;
                return xx == null ? 0 : xx._IBP.GetContentHashCode();
            }

            static private AlbumImageContentComparer _Comp;

            private AlbumImageContentComparer()
            {
            }

            static AlbumImageContentComparer()
            {
                _Comp = new AlbumImageContentComparer();
            }

            static internal AlbumImageContentComparer Comparer
            {
                get { return _Comp; }
            }


        }

        public IEqualityComparer<IAlbumPicture> Comparer { get { return AlbumImageContentComparer.Comparer; } }

    }



}
