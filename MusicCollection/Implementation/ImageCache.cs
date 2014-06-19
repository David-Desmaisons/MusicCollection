using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

using MusicCollection.Nhibernate.Blob;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Buffer;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{
    internal class ImageCache:IDisposable
    {
        private Album _Father;
        private IBufferProvider _Buffer;
        private AlbumImage _ImageCached;

        internal ImageCache(Album Father)
        {
            _Father = Father;
            Update();
        }

        internal ImageCache(Album Father, IBufferProvider iBp)
        {
            _Father = Father;
            _Buffer = iBp;
        }

      
        internal void AfterLoad(bool Updatedata)
        {
            AlbumImage ai = _Father.RawFrontImage;

            if (Updatedata && (_Buffer == null) && (ai != null))
            {
                if (!ai.IsBroken)
                    Update();
            }

            _ImageCached = _Father.RawFrontImage;
        }

        internal IBufferProvider Buffer
        {
            get
            {
                return _Buffer;
            }
        }

        private WeakReference _ImageSmall = null;
        private BitmapImage CachedImage
        {
            get
            {
                if (_ImageSmall == null)
                    return null;

                if (_ImageSmall.Target == null)
                {
                    _ImageSmall = null;
                    return null;
                }

                return (_ImageSmall.Target as BitmapImage);
            }
            set
            {
                if (value == null)
                    _ImageSmall = null;

                _ImageSmall = new WeakReference(value, false);
            }
        }

        internal BitmapImage Image
        { get { BitmapImage res = CachedImage; if (res != null) return res; res = _Buffer == null ? null : _Buffer.ImageFromBuffer(); CachedImage = res; return res; } }


        private const int _Dim = 280;
        static internal int Dimension
        {
            get { return _Dim; }
        }


        internal void Update(bool force=false)
        {
            AlbumImage newim = _Father.RawFrontImage;

            if (!force && (object.ReferenceEquals(_ImageCached, newim)))
                return;

            IPersistentBufferProvider ibp = newim == null ? null : newim.CreateJpgThumbnail(Dimension);
            _Buffer = ibp;
            if (ibp != null)
            { 
                ibp.DefaultExtension = ".jpg";
                //ibp.Save(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            }

            CachedImage = null;

            _ImageCached = newim;
        }

        public void Dispose()
        {
            CachedImage = null;
        }
    }
}
