using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using MusicCollection.Fundation;
using MusicCollection.DataExchange.Cue;
using MusicCollection.Implementation;
using MusicCollection.FileImporter;
using MusicCollection.Infra;

namespace MusicCollection.DataExchange
{
    public class TrackDescriptor : NotifyCompleteAdapterNoCache, IEditableTrackDescriptor, IFileRelatedEntity
    {
        #region Constructors

        public TrackDescriptor()
        {
            Name = string.Empty;
            Artist = string.Empty;
            TrackNumber = 0;
        }

        internal TrackDescriptor(AlbumDescriptor ad, string iname, int tracknumber)
        {
            _Father = ad;
            Name = iname;
            TrackNumber = (uint)tracknumber;
        }

   
        internal TrackDescriptor(AlbumDescriptor ad, string iname, int tracknumber, int iDiskNumber)
        {
            _Father = ad;
            Name = iname;
            TrackNumber = (uint)tracknumber;
            DiscNumber = (uint)iDiskNumber;
        }

        internal TrackDescriptor(AlbumDescriptor ad, string iname, int tracknumber, TimeSpan iDuration, string artits = null, int iDiskNumber = 0)
        {
            _Father = ad;
            Artist = artits;
            Name = iname;
            Duration = iDuration;
            TrackNumber = (uint)tracknumber;
            DiscNumber = (uint)iDiskNumber;
        }

        internal TrackDescriptor(AlbumDescriptor ad)
        {
            _Father = ad;
        }

        internal TrackDescriptor(AlbumDescriptor ad,Track tr)
        {
            _Father = ad;

            this.Artist = tr.Artist;
            this.Name = tr.Name;
            this.Path = tr.Path;
            this.TrackNumber = tr.TrackNumber;
            this.Duration = tr.Duration;
            this.MD5 = tr.MD5HashKey;
            this.Rating = tr.Rating;
            this.DateAdded = tr.Interface.DateAdded;
            this.LastPLayed = tr.Interface.LastPlayed;
            this.PlayCount = tr.Interface.PlayCount;
            this.DiscNumber = tr.Interface.DiscNumber;
        }

        internal TrackDescriptor(AlbumDescriptor ad,Track tr, string iPath): this(ad, tr)
        {
            Path = iPath;
        }

        internal TrackDescriptor(AlbumDescriptor ad, string iname, int tracknumber, string iDuration,string iAmazonTrackPosition)
        {
            _Father = ad;
            Name = iname;
            TimeSpan res;
            if (TimeSpan.TryParse(iDuration, out res))
            {
                Duration = iDuration.Count(o => o == ':') == 2 ? res : new TimeSpan(0, res.Hours, res.Minutes);
            }
            Regex rg = new Regex(@"(?<DiscNumber>(\d+))(-|\.)(?<TrackNumber>(\d+))");
            
            var match = rg.Match(iAmazonTrackPosition);
            if (match.Success)
            {
                DiscNumber = uint.Parse(match.Groups["DiscNumber"].Value);
                TrackNumber = uint.Parse(match.Groups["TrackNumber"].Value);
            }
            else
            {
                TrackNumber = (uint)tracknumber;
            }
        }

        internal TrackDescriptor Clone(AlbumDescriptor newfather)
        {
            TrackDescriptor res = new TrackDescriptor
            {
                _Father = newfather,
                Artist = Artist,
                Name = Name,
                Path = Path,
                TrackNumber = TrackNumber,
                Duration = Duration,
                MD5 = MD5,
                Rating = Rating,
                DateAdded = DateAdded,
                LastPLayed = LastPLayed,
                PlayCount = PlayCount,
                DiscNumber = DiscNumber
            };

            return res;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3}", TrackNumber, Name, _Father, DiscNumber);
        }

        static internal ITracksDescriptorBuilder GetItunesBuilder()
        {
            return new ITunesTracksBuilder();
        }

        #region Properties

        private AlbumDescriptor _Father;
        private AlbumDescriptor FatherAlbum
        {
            get
            {
                if (_Father == null)
                {
                    _Father = new AlbumDescriptor();
                }

                return _Father;
            }
        }

        [XmlIgnore]
        public IAlbumDescriptor AlbumDescriptor
        {
            get { return _Father; }
        }

        private string _Artist;
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Artist")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "PERFORMER")]
        public string Artist
        {
            get { return _Artist; }
            set { Set(ref _Artist, value); }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "INDEX 00")]
        public TimeCueIndexer CueIndex00
        {
            get;
            private set;
        }

        private string _cf;
        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "FILE")]
        public string CUEFile
        {
            get {return _cf ?? _Father.CUEFile;}
            private set { _cf = value; }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "INDEX 01")]
        public TimeCueIndexer CueIndex01
        {
            get;
            private set;
        }

        private string _Name;
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "TTITLE")] //TTITLE
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Name")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "TITLE")]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Name")]
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value); }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Album Artist")]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Artist")]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Album Artist")]
        public string Album_Artist
        {
            get { return FatherAlbum.Artist; }
            set { FatherAlbum.Artist = value; }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Album")]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Album")]
        public string Album_Name
        {
            get { return FatherAlbum.Name; }
            set { FatherAlbum.Name = value; }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Track Count")]
        public uint TracksCount
        {
            get { return FatherAlbum.TracksNumber; }
            set { FatherAlbum.TracksNumber = value; }
        }

        [XmlIgnore]
        
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Year")]
        public int Year
        {
            get { return FatherAlbum.Year; }
            set { FatherAlbum.Year = value; }
        }

        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Year")]
        public Nullable<long> NullableYear
        {
            get { return FatherAlbum.Year == 0 ? new Nullable<long>() : FatherAlbum.Year; }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Genre")]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Genre")]
        public string Genre
        {
            get { return  FatherAlbum.Genre; }
            set { FatherAlbum.Genre = value; }
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Location")]
        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Path")]
        public string ItunesLocation
        {
            set
            {
                if (Path != null)
                    throw new Exception("Program error");

                string res = new Uri(value).LocalPath;
                const string LocalHost = "\\\\localhost\\";
                if (res.StartsWith(LocalHost))
                    res = res.Remove(0, LocalHost.Length);
                Path = res.Replace("&amp;", "&");
            }
            get
            {
                return Path;
            }

        }

        internal void Mature()
        {
            if ( (string.IsNullOrEmpty(FatherAlbum.Artist)) && (!(string.IsNullOrEmpty(Artist)) ))
                FatherAlbum.Artist=Artist;
        }

        public string Path
        {
            get;
            set;
        }

        private uint _TrackNumber;
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Track Number")]       
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "TRACK")]
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "TNUMBER")]
        public uint TrackNumber
        {
            get { return _TrackNumber; }
            set { Set(ref _TrackNumber, value); }
        }

        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Track Number")]
        public int TrackNumberInt
        {
            get { return (int)TrackNumber; }
        }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get;
            internal set;
        }

        [XmlElement("Duration")]
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Total Time")]
        public Int32 TotalTime
        {
            set { Duration = TimeSpan.FromMilliseconds(value); }
            get { return (Int32)Duration.TotalMilliseconds; }
        }

        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Total Time")]
        public long TotalTimeLOng
        {
            get { return (long)Duration.TotalMilliseconds; }
        }

        public DateTime? DateAdded
        {
            get;
            set;
        }

        private uint _DiscNumber;
        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Disc Number")]
        public uint DiscNumber
        {
            get { return _DiscNumber; }
            set { Set(ref _DiscNumber, value); }
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

        [MusicObjectAttributeMapping(DataExportImportType.iTunes, "Rating")]
        public uint BigRating
        {
            get { return Rating * 20; }
            set { Rating = value / 20; }
        }

        [MusicObjectAttributeMapping(DataExportImportType.WindowsPhone, "Rating")]
        public int IntRating
        {
            get { return (int)Rating; }
        }


        public uint Rating
        {
            get;
            set;
        }

        #endregion

        #region IFileRelatedEntity

        [XmlIgnore]
        public IFullAlbumDescriptor Album
        {
            set { _Father = value as AlbumDescriptor; }
        }

        #endregion

        #region XML Import

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
        #endregion

        #region FileRelated

        private TagLib.File File
        {
            get
            {
                TagLib.File file = null;
                if (Path == null)
                    return file;

                try
                {
                    file = TagLib.File.Create(Path);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                return file;
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
                        _MD5 =  f.MD5();
                    }
                }

                return _MD5;
            }
            set
            {
                _MD5 = value;
            }
        }

        public Stream MusicStream()
        {
            using (TagLib.File f = File)
            {
                if (f == null)
                    return null;

                return f.RawMusicStream();
            }
        }


        #endregion
    }
}
