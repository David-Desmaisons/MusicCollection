using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;

using MusicCollection.ToolBox.Buffer ;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox;

namespace MusicCollection.DataExchange
{
    public interface ICompleteIMageInfo : IIMageInfo, IFileRelatedEntity
    {
    //    ICompleteIMageInfo Clone();
    }


    public class AImage : ICompleteIMageInfo
        // //,IFileRelatedEntity
    {
        private IBufferProvider _Buffer;
        private int _ID;

        public AImage()
        {
        }

        internal AImage(IBufferProvider ImageBuffer, int iid)
        {
            _Buffer = ImageBuffer;
            _ID = iid;
        }


        public AImage Clone()
        {
            return new AImage(this._Buffer, this._ID);
        }

   
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [XmlIgnore]
        public IBufferProvider ImageBuffer
        {
            get { return _Buffer; }
        }

        public string Path
        {
            get { return _Buffer.PersistedPath; }
            set { _Buffer = BufferFactory.GetBufferProviderFromFile(value); }
        }

        [XmlIgnore]
        public IFullAlbumDescriptor Album
        {
            set {}
        }

        [XmlIgnore]
        public BitmapImage Image
        {
            get { return _Buffer.ImageFromBuffer(); }
        }
    }
}
