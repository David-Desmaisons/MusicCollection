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

namespace MusicCollection.DataExchange
{
    public class ExportAlbum : IFullAlbumDescriptor
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

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}",Name,Artist,Genre,Year);
        }

      
        private ExportAlbum(Album al, bool Copyall)
        {
            _Al = al;

            Artist = al.Author;
            Genre = al.Genre;
            IDs = al.CDIDs;
            TracksNumber = al.Interface.TracksNumber;
            Name = al.Name;
            Year = al.Interface.Year;
            DateAdded = al.Interface.DateAdded;

            if (Copyall)
            {
                Tracks = (from tr in al.RawTracks select new ExportTrack(tr, this)).ToList();
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

        private DiscIDs _IDs;

        [XmlIgnore]
        public IDiscIDs IDs
        {
            get { if (_IDs == null) _IDs = DiscIDs.FromIDsAndHashes(Asin, MusicBrainzID, CDDB, MusicBrainzHash); return _IDs; }
            private set { _IDs = value as DiscIDs; }
        }

        private string _CDDB;
        public string CDDB
        {
            get { return _CDDB ?? (_IDs == null ? null : IDs.CDDB); }
            set { _CDDB = value; IDs = null; }
        }

        private string _Asin;
        public string Asin
        {
            get { return _Asin ?? (_IDs == null ? null : IDs.Asin); }
            set { _Asin = value; IDs = null; }
        }

        private string _MBzHash;
        public string MusicBrainzHash
        {
            get { return _MBzHash ?? (_IDs == null ? null : IDs.MusicBrainzCDId); }
            set { _MBzHash = value; IDs = null; }
        }

        private string _MBzID;
        public string MusicBrainzID
        {
            get { return _MBzID ?? (_IDs == null ? null : IDs.MusicBrainzID); }
            set { _MBzID = value; IDs = null; }
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
            set { if (_FRE == null)  _FRE = new FileRelatedEntity(); _FRE.Path = value; }
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
            //Genre = string.Empty;
        }

        internal void Import(IImportContext Context)
        {
            foreach (ExportTrack tr in Tracks)
            {
                tr.Import(Context);
            }
        }

        private IEnumerable<IFileRelatedEntity> FREs
        {
            get
            { return Tracks.Concat<IFileRelatedEntity>(Images).Concat(_FRE.SingleItemCollection<IFileRelatedEntity>()); }
        }


        internal void AfterImport(string iPath, string original)
        {
            bool KO = true;
            foreach (IFileRelatedEntity et in FREs)
            {
                et.Album = this;
                if (KO && File.Exists(et.Path))
                    KO = false;
            }

            if (KO)
            {
                bool TestReconnect = false;
                string Direc = iPath + @"\";
                foreach (IFileRelatedEntity et in FREs)
                {
                    if (File.Exists(et.Reconnect(Direc, original)))
                    {
                        TestReconnect = true;
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

        [XmlIgnore]
        List<ITrackMetaDataDescriptor> IFullAlbumDescriptor.TrackDescriptors { get { return (from t in Tracks select t).ToList<ITrackMetaDataDescriptor>(); } }

         [XmlIgnore]
        IList<IIMageInfo> IFullAlbumDescriptor.Images
        {
            get { return Images.Select<ImageInfo,IIMageInfo>(t=>t).ToList(); }
        }


         public IFullEditableAlbumDescriptor GetEditable()
         {
             throw new NotImplementedException();
         }


         public IEnumerable<IFullAlbumDescriptor> SplitOnDiscNumber()
         {
             throw new NotImplementedException();
         }


         public bool MatchTrackNumberOnDisk(int TN)
         {
             throw new NotImplementedException();
         }


         public bool HasImage()
         {
             return ((_Images!=null) && (_Images.Count>0));
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

        internal bool Export(string iDir, string ForceRoot = null)
        {
            _Path = FileInternalToolBox.CreateNewAvailableName(Path.Combine(iDir, "Albums.xml"));

            OriginalDir = ForceRoot ?? iDir;

            XmlSerializer xs = new XmlSerializer(typeof(ExportAlbums));
            using (Stream s = File.Create(_Path))
                xs.Serialize(s, this);

            return true;
        }

        private void Reroot(IDictionary<string, string> dic)
        {
            var res = from al in Albums from tr in al.Tracks select tr;
            foreach (ExportTrack tr in res)
            {
                string output = null;
                if (dic.TryGetValue(tr.Path, out output))
                {
                    tr.Path = output;
                }

            }
        }

        private void AfterImport(string iPath)
        {
            if (Albums == null)
                return;

            foreach (ExportAlbum eal in Albums)
            {
                eal.AfterImport(iPath, OriginalDir);
            }
        }

        static internal ExportAlbums ImportContainer(string iPath, bool Reconnect, string ForcePath = null, IDictionary<string, string> iReroot = null)
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

                if (Reconnect)
                    Als.AfterImport(ForcePath ?? Path.GetDirectoryName(iPath));

                return Als;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem reading XML " + e.ToString());
            }

            return null;
        }

        static internal IList<ExportAlbum> Import(string iPath, bool Reconnect, string iFP, IDictionary<string, string> Reroot)
        {
            ExportAlbums Als = ImportContainer(iPath, Reconnect, iFP, Reroot);

            return Als == null ? null : Als.Albums;
        }
    }
}
