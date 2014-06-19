using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using MusicCollection.Fundation;
using MusicCollection.TaglibExtender;
using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using MusicCollection.Infra;

namespace MusicCollection.Implementation
{

    public class FileRelatedEntity
    {
        public FileRelatedEntity()
        { 
        }

        public FileRelatedEntity(string iPath)
        {
            Path = iPath;
        }

        private ExportAlbum _AD;
        [XmlIgnore]
        internal ExportAlbum Album
        {
            set { if (_AD == null) _AD = value; }
            get { return _AD; }
        }


        public string Path
        {
            get;
            set;
        }

        internal string Reconnect(string current, string original)
        {
            if (!Path.StartsWith(original))
                return null;

            string Rel = Path.Substring(original.Length);

            return System.IO.Path.Combine(current, Rel);

        }

        internal void UpdatePath(string idir, string original)
        {
            Path = Reconnect(idir, original);
        }
    }

    public class ExportTrack : FileRelatedEntity,ITrackDescriptor
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
            this.ISRCName = tr.ISRC == null ? null : tr.ISRC.Name;
            this.TrackNumber = tr.TrackNumber;
            this.Duration = tr.Duration;
            MD5 = tr.MD5HashKey;
            Rating = tr.Rating;
            this.DateAdded = tr.Interface.DateAdded;
            this.LastPLayed = tr.Interface.LastPLayed;
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
                string Dest = FileHelper.CreateMusicalFolder(new DiscInfoCue(Name,Artist));

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
                Console.WriteLine("Problem moving files {0}", e);
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
        IAlbumDescriptor ITrackDescriptor.AlbumDescriptor
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

        private bool _Broken=false;

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
                    _Broken=true;
                    Console.WriteLine(e);
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

        private ISRC _ISRC;
        [XmlIgnore]
        ISRC ITrackDescriptor.ISRC
        {
            get
            {
                if (_ISRC == null)
                {
                    using (TagLib.File f = File)
                    {
                        _ISRC = (f == null) ? null : f.ISRC();
                    }
                }

                return _ISRC;
            }
        }

        [XmlElement("ISRC")]
        public string ISRCName
        {
            get { return _ISRC == null ? null : _ISRC.Name; }
            set { _ISRC = ISRC.Fromstring(value); }
        }
    }

    public class ImageInfo : FileRelatedEntity
    {
        public ImageInfo()
        { }

        //public string Location
        //{
        //    get { return Path; }
        //    set { Path = value; }
        //}

        public int ID
        {
            get;
            set;
        }
    }

    public class ExportAlbum : IAlbumDescriptor
    {
        private Album _Al;
        static internal ExportAlbum CopyAlbum(Album al)
        {
            return new ExportAlbum(al, true);
        }

        static internal ExportAlbum DuplicateFromAlbum(Album al)
        {
            return new ExportAlbum(al, false);
        }

        private ExportAlbum(Album al, bool Copyall)
        {
            _Al = al;

            Artist = al.Author;
            Genre = al.Genre;
            IDs = al.IDs;
            TracksNumber = al.Interface.TracksNumber;
            Name = al.Name;
            Year = al.Interface.Year;
            DateAdded = al.Interface.DateAdded;

            if (Copyall)
            {
                Tracks = (from tr in al.CrudeTracks select new ExportTrack(tr, this)).ToList();
                Images = (from im in al.ModifiableImages select new ImageInfo() { ID = im.Rank, Path = im.GetPath() }).ToList();
                Thumbnail = al.ImageCachePath;
            }


        }

        internal void AddImage(int iID, string iPath)
        {
            if (_Images == null)
                _Images = new List<ImageInfo>();

            _Images.Add(new ImageInfo() { ID = iID, Path = iPath });
        }

        public string Artist
        {
            get;
            set;
        }

        public string Genre
        {
            get;
            set;
        }

        public DiscIDs IDs
        {
            get;
            set;
        }

        public uint TracksNumber
        {
            get;
            set;
        }

        public DateTime DateAdded
        {
            get;
            set;
        }

        private List<ImageInfo> _Images;
        public List<ImageInfo> Images
        {
            get { return _Images; }
            set { _Images = value; }
        }

        private FileRelatedEntity _FRE;

        public string Thumbnail
        {
            get { return _FRE == null ? null : _FRE.Path; }
            set { if (_FRE == null)  _FRE = new FileRelatedEntity();  _FRE.Path = value; }
        }


        internal void AddExportedTrack(Track track, string path)
        {
            if (track == null)
                return;

            if (track.RawAlbum != _Al)
                return;

            if ((Tracks != null) && (from t in Tracks select t.Track).ToList().Contains(track))
                return;

            if (Tracks == null)
                Tracks = new List<ExportTrack>();

            Tracks.Add(new ExportTrack(track, this, path));
        }

        internal ExportAlbum()
        {
        }

        internal void Import(IImportContext Context)
        {
            foreach(ExportTrack tr in Tracks)
            {
                tr.Import(Context);
            }
           
        }

        private IEnumerable<FileRelatedEntity> FREs
        {
            get
            { return Tracks.Concat<FileRelatedEntity>(Images).Concat(_FRE.SingleItemCollection<FileRelatedEntity>()); }
        }
      

        internal void AfterImport(string iPath,string original)
        {
            bool KO = true;
            foreach (FileRelatedEntity et in FREs)
            {
                et.Album = this;
                if ( KO && File.Exists(et.Path))
                    KO = false;
            }

            if ( KO)
            {
                bool TestReconnect = false;
                string Direc = iPath + @"\";
                foreach (FileRelatedEntity et in FREs)
                {
                    if (File.Exists(et.Reconnect(Direc, original)))
                    { 
                        TestReconnect=true;
                        break;
                    }

                }

                if (TestReconnect)
                {
                    foreach (FileRelatedEntity et in FREs)
                    {
                        et.UpdatePath(iPath, original);
                    }
                }
            }
        }

       


        public string Name
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }


        public List<ExportTrack> Tracks
        {
            get;
            set;
        }
    }


    public class ExportAlbums
    {
        public List<ExportAlbum> Albums
        {
            get;
            set;
        }

        public string OriginalDir
        {
            get;
            set;
        }

        internal void Import(IImportContext Context)
        {
            foreach (ExportAlbum tr in Albums)
            {
                tr.Import(Context);
            }

        }

        private string _Path;
        internal string CurrentPath
        {
            get { return _Path; }
        }

        internal bool Export(string iDir,string ForceRoot=null)
        {
            _Path = FileInternalToolBox.CreateNewAvailableName(Path.Combine(iDir, "Albums.xml"));

            OriginalDir = ForceRoot?? iDir;

            XmlSerializer xs = new XmlSerializer(typeof(ExportAlbums));
            using (Stream s = File.Create(_Path))
                xs.Serialize(s, this);

            return true;
        }

        private void Reroot(IDictionary<string,string> dic)
        {
            var res = from al in Albums from tr in al.Tracks select tr;
            foreach (ExportTrack tr in res)
            {
                string output=null;
                if (dic.TryGetValue(tr.Path, out output))
                {
                    tr.Path = output;
                }

            }
        }

        private void AfterImport(string iPath)
        {
            if (Albums==null)
                return;

            foreach(ExportAlbum eal in Albums)
            {
                eal.AfterImport(iPath,OriginalDir);
            }
        }

        static internal ExportAlbums ImportContainer(string iPath, bool Reconnect, string ForcePath = null, IDictionary<string, string> iReroot=null)
        {
            ExportAlbums Als = null;

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ExportAlbums));
                using (Stream s = File.OpenRead(iPath))
                    Als = xs.Deserialize(s) as ExportAlbums;

                if (Als == null)
                    return null;

                if (iReroot != null)
                    Als.Reroot(iReroot);

                if(Reconnect)
                    Als.AfterImport(ForcePath ?? Path.GetDirectoryName(iPath));

                return Als;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading XML {0}", e);
            }

            return null;
        }

        static internal IList<ExportAlbum> Import(string iPath, bool Reconnect,string iFP,IDictionary<string, string> Reroot)
        {
            ExportAlbums Als = ImportContainer(iPath, Reconnect, iFP, Reroot);

            return Als == null ? null : Als.Albums;
        }
    }
}
