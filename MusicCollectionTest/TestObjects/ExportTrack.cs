using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.ToolBox.Buffer;

namespace MusicCollection.DataExchange
{
    public class ExportTrack : FileRelatedEntity, ITrackDescriptor, IFileRelatedEntity
    {
        private Track _Tr;

        internal ExportTrack()
        {
        }

        internal ExportTrack(Track tr, ExportAlbum eal)
        {
            _Tr = tr;

            Album = eal;

            this.Artist = tr.Artist;
            this.Name = tr.Name;
            this.Path = tr.Path;
            //this.ISRCName = tr.ISRC == null ? null : tr.ISRC.Name;
            this.TrackNumber = tr.TrackNumber;
            this.Duration = tr.Duration;
            MD5 = tr.MD5HashKey;
            Rating = tr.Rating;
            this.DateAdded = tr.Interface.DateAdded;
            this.LastPLayed = tr.Interface.LastPlayed;
            this.PlayCount = tr.Interface.PlayCount;
        }

        internal ExportTrack(Track tr, ExportAlbum eal, string iPath)
            : this(tr, eal)
        {
            Path = iPath;
        }

        internal Track Track
        {
            get { return _Tr; }
        }

        public uint TrackNumber
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}",this.TrackNumber,this.Name,this.Album);
        }

        public uint DiscNumber { get; set; }

        public DateTime? DateAdded
        {
            get;
            set;
        }

        public DateTime? LastPLayed
        {
            get;
            set;
        }

        public int PlayCount
        {
            get;
            set;
        }



        internal void Import(IImportContext Context)
        {
            try
            {
                string Dest = Context.Folders.CreateMusicalFolder(new DiscInfoCue(Name, Artist));

                if (!Context.XMLManager.RemoveFileOriginal)
                {
                    System.IO.File.Copy(Path, Dest);
                }
                else
                {
                    System.IO.File.Move(Path, Dest);
                }

                Path = Dest;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem moving files " + e.ToString());
                Context.OnFactorisableError<UnableToCopyFile>(Path);
            }
        }


        [XmlElement("Duration")]
        public long DurationWA
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get;
            set;
        }

        public uint Rating
        {
            get;
            set;
        }



        [XmlIgnore]
        IAlbumDescriptor ITrackMetaDataDescriptor.AlbumDescriptor
        {
            get { return Album; }
        }




        public string Artist
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        private bool _Broken = false;

        [XmlIgnore]
        private TagLib.File File
        {
            get
            {
                if (_Broken)
                    return null;

                TagLib.File file = null;
                try
                {
                    file = TagLib.File.Create(Path);
                }
                catch (Exception e)
                {
                    _Broken = true;
                    Trace.WriteLine(e);
                }
                return file;
            }
        }

        public Stream MusicStream()
        {
            using (TagLib.File f = File)
            {
                return (f == null) ? null : f.RawMusicStream();
            }
        }



        private string _MD5;
        public string MD5
        {
            get
            {
                if (_MD5 == null)
                {
                    using (TagLib.File f = File)
                    {
                        _MD5 = (f == null) ? null : f.MD5();
                    }
                }
                return _MD5;
            }
            set
            {
                _MD5 = value;
            }
        }

        //private ISRC _ISRC;
        //[XmlIgnore]
        //ISRC ITrackMetaDataDescriptor.ISRC
        //{
        //    get
        //    {
        //        if (_ISRC == null)
        //        {
        //            using (TagLib.File f = File)
        //            {
        //                _ISRC = (f == null) ? null : f.ISRC();
        //            }
        //        }

        //        return _ISRC;
        //    }
        //}

        //[XmlElement("ISRC")]
        //public string ISRCName
        //{
        //    get { return _ISRC == null ? null : _ISRC.Name; }
        //    set { _ISRC = ISRC.Fromstring(value); }
        //}
    }

    public class ImageInfo : FileRelatedEntity, IIMageInfo
    {
        public ImageInfo()
        { }


        public int ID
        {
            get;
            set;
        }


        public IBufferProvider ImageBuffer
        {
            get { return BufferFactory.GetBufferProviderFromFile(Path); }
        }


        public System.Windows.Media.Imaging.BitmapImage Image
        {
            get { return ImageBuffer.ImageFromBuffer(); }
        }


        public IIMageInfo Clone()
        {
            throw new NotImplementedException();
        }
    }


}
