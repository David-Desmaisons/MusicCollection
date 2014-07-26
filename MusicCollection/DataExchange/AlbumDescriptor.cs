using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Threading;
using iTunesLib;

//using MusicBrainz;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Buffer;
using MusicCollection.Infra;
using MusicCollection.DataExchange.Cue;
using MusicCollection.WebServices.Amazon;
using MusicCollection.Implementation.Modifier;
using MusicCollection.WebServices;
using MusicCollection.ToolBox.Web;

namespace MusicCollection.DataExchange
{
    enum DataExportImportType
    {
        iTunes,
        CUE,
        FreeDB,
        WindowsPhone
    };


    public class AlbumDescriptor : NotifyCompleteAdapterNoCache, IFullEditableAlbumDescriptor
    {
        public AlbumDescriptor()
        {
            Name = string.Empty;
            RawIDs = DiscIDs.Empty;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Artist, Name);
        }

        #region static builders

        static internal AlbumDescriptor FromDiscogs(dynamic found, bool NeedCovers, IOAuthManager iOAuthManager, HttpContext mycontext, CancellationToken ict)
        {
            AlbumDescriptor res = NeedCovers ? new AlbumDescriptor() : new LoadingAlbumDescriptor();

            List<dynamic> Images = found.images as List<dynamic>;
            bool hasimages = (Images != null) && (Images.Count > 0);

            if (NeedCovers && !hasimages)
                return null;

            res.Artist = MusicCollection.Implementation.Artist.AuthorName((found.artists as List<dynamic>).Select(o => o.name as string).ToList());
            res.Year = found.year;

            if (found.genres.Count > 0)
                res.Genre = found.genres[0];

            res.Name = found.title;
            res.RawTrackDescriptors = (found.tracklist as List<dynamic>).Select((o, i) => new TrackDescriptor(res, o.title, i + 1, o.duration, o.position)).ToList();
            res.TracksNumber = (uint)res.TrackDescriptors.Count;
            if (hasimages && (iOAuthManager != null))
            {

                Func<dynamic, int, AImage> GetImages = (o, i) =>
                {
                    Thread.Sleep(1000);
                    string myuri = o.uri;
                    IHttpWebRequest request = InternetProvider.InternetHelper.CreateHttpRequest(myuri);
                    request.UserAgent = mycontext.UserAgent;
                    request.Headers.Add("Authorization", iOAuthManager.GenerateAuthzHeader(myuri, "GET"));
                    request.PreAuthenticate = true;
                    return new AImage(BufferFactory.GetBufferProviderFromHttpRequest(request), i);
                };

                if (NeedCovers)
                    res.RawImages = Images.Select(GetImages).CancelableToList(ict);
                else
                    (res as LoadingAlbumDescriptor).LoadAction = () => Images.Select(GetImages).ToList();
            }

            return res;
        }

        static internal AlbumDescriptor FromGraceNote(MusicCollection.WebServices.GraceNote.DTO.AlbumDto found, bool NeedCovers, CancellationToken ict)
        {
            if ( NeedCovers && (found.ArtworkDto == null))
                return null;

            AlbumDescriptor res = NeedCovers ? new AlbumDescriptor() : new LoadingAlbumDescriptor();

            res.Artist = found.Artist;
            res.Year = found.Year;

            if (found.Genre.Length > 0)
                res.Genre = found.Genre[0].Description;

            res.Name = found.Title;
            res.RawTrackDescriptors = (found.TrackDto==null) ? new List<TrackDescriptor>() : found.TrackDto.Select((o, i) => new TrackDescriptor(res, o.Title, o.Number)).ToList();
            res.TracksNumber = (uint)found.TrackCount;

            res.RawImages = new List<AImage>();
            
            if (found.ArtworkDto==null)
                return res;

            var Images = found.ArtworkDto.Where( a=> a.ArtworkType=="COVERART");
  
            Func<MusicCollection.WebServices.GraceNote.DTO.ArtworkDto,int,AImage> GetImages = (art,i) =>
            {
                string myuri = art.Location;
                IHttpWebRequest request = InternetProvider.InternetHelper.CreateHttpRequest(myuri);
                return new AImage(BufferFactory.GetBufferProviderFromHttpRequest(request), i);
            };

            if (NeedCovers)
                res.RawImages.AddCollection( Images.Select(GetImages).CancelableToList(ict));
            else
                (res as LoadingAlbumDescriptor).LoadAction = () => Images.Select(GetImages).ToList();

            return res;
        }



        static internal AlbumDescriptor FromAmazonItem(Item Amazonf, bool NeedCovers, CancellationToken ict)
        {
            AlbumDescriptor res = NeedCovers ? new AlbumDescriptor() : new LoadingAlbumDescriptor();

            ImageSet[] im = Amazonf.ImageSets;
            bool HasCover = ((im != null) && (im.Length > 0));

            if (NeedCovers && !HasCover)
                return null;

            res.Artist = MusicCollection.Implementation.Artist.AuthorName(Amazonf.ItemAttributes.Artist);
            res.Asin = Amazonf.ASIN;

            if (Amazonf.ItemAttributes.ReleaseDate != null)
            {
                int Year = 0;
                int.TryParse(Amazonf.ItemAttributes.ReleaseDate.Remove(4), out Year);
                res.Year = Year;
            }
            res.Genre = Amazonf.ItemAttributes.Genre;
            res.Name = Amazonf.ItemAttributes.Title;

            TracksDisc[] td = Amazonf.Tracks;
            res.RawTrackDescriptors = new List<TrackDescriptor>();
            int DN = (td != null) ? td.Length : 0;
            if (DN != 0)
            {
                for (int i = 0; i < DN; i++)
                {
                    res.RawTrackDescriptors.AddRange(td[i].Track.Select((o) => new TrackDescriptor(res, o.Value, int.Parse(o.Number), i + 1)));
                }
            }

            res.TracksNumber = (uint)res.RawTrackDescriptors.Count;

            if (HasCover)
            {
                if (NeedCovers)
                    res.RawImages = im.Cast<ImageSet>().Select((o, i) => new AImage(BufferFactory.GetBufferProviderFromURI(new Uri(o.LargeImage.URL)), i))
                        .CancelableToList(ict);
                else
                    (res as LoadingAlbumDescriptor).LoadAction = () => im.Cast<ImageSet>().Select((o, i) => new AImage(BufferFactory.GetBufferProviderFromURI(new Uri(o.LargeImage.URL)), i)).ToList();
            }

            return res;
        }

        static internal IFullAlbumDescriptor FromiTunes(IITAudioCDPlaylist icd)
        {
            AlbumDescriptor res = new AlbumDescriptor();

            res.Artist = icd.Artist;
            res.Year = icd.Year;
            res.Genre = icd.Genre;
            res.Name = icd.Name;

            IITTrackCollection td = icd.Tracks;
            if ((td != null) && (td.Count > 0))
            {
                res.RawTrackDescriptors = td.Cast<IITTrack>().
                    Select((o) => new TrackDescriptor(res, o.Name, o.TrackNumber) { DiscNumber = (uint)(o.DiscNumber) }).ToList();
            }
            else
            {
                res.RawTrackDescriptors = new List<TrackDescriptor>();
            }

            res.TracksNumber = (uint)res.RawTrackDescriptors.Count;

            return res;
        }

        static internal IAlbumDescriptor SimpleAlbumDescriptorFromAlbumModifier(IInternalAlbumModifier found)
        {
            var res = new AlbumDescriptor();
            res.Artist = MusicCollection.Implementation.Artist.AuthorName(found.Artists);
            res.Year = found.Year;
            res.Genre = found.Genre;
            res.Name = found.Name;
            res.RawIDs = found.CDIDs;
            return res;
        }


        static internal AlbumDescriptor FromCUESheet(string FileName)
        {
            AlbumBuilder cab = new AlbumBuilder(DataExportImportType.CUE);
            CUESheetAnalyser csa = new CUESheetAnalyser(FileName);
            AlbumDescriptor res = csa.Visit(cab);
            if (res != null)
                res.CUESheetFileName = FileName;
            return res;
        }

        static internal AlbumDescriptor FromFreeDBInfo(List<string> FileName)
        {
            AlbumBuilder cab = new AlbumBuilder(DataExportImportType.FreeDB);
            Freedb.FreedbParser csa = new Freedb.FreedbParser(FileName);

            AlbumDescriptor res = csa.Visit(cab);

            if (res == null)
                return null;

            //petite ratrap sur les fichiers corrompus 
            var duples = res.RawTrackDescriptors.ToLookup(tr => tr.TrackNumber);

            foreach (IGrouping<uint, TrackDescriptor> group in duples)
            {
                if (group.Count() == 1)
                    continue;

                var orderedgoups = group.OrderBy(td => res.RawTrackDescriptors.IndexOf(td)).ToList();
                TrackDescriptor survivor = orderedgoups[0];
                int count = orderedgoups.Count;
                for (int i = 1; i < count; i++)
                {
                    TrackDescriptor tobetr = orderedgoups[i];
                    survivor.Name += tobetr.Name;
                    res.RawTrackDescriptors.Remove(tobetr);
                }
            }

            return res;
        }

        private const string _Unknown = "Unknown Artist";

        static internal AlbumDescriptor CreateBasicFromCD(ICDInfoHandler dfih, IImportContext Context)
        {
            return CreateBasicFromCD(dfih, Context, _Unknown, Context.FindNewUnknownNameAlbumForArtist(_Unknown), true);
        }

        static internal AlbumDescriptor CreateBasicFromCD(ICDInfoHandler dfih, IImportContext Context, string Artist, string Name, bool Basic = false)
        {
            var res = new AlbumDescriptor();
            res.RawIDs = dfih.IDs;
            res.TracksNumber = (uint)dfih.TrackNumbers;

            res.Artist = Artist;
            res.Name = Name;

            res.RawTrackDescriptors = Enumerable.Range(0, dfih.TrackNumbers).Select(i => new TrackDescriptor(res, string.Format("Track {0}", i + 1), i + 1, dfih.Duration(i), res.Artist)).ToList();
            //if (!Basic)
            //    res.MergeTracksIDsFromCDInfos(dfih);

            return res;
        }

        #endregion

        #region Builder Album

        static internal AlbumDescriptor CopyAlbum(Album al, bool CopyImage = true)
        {
            return BuildForExport(al, true, CopyImage);
        }

        static internal AlbumDescriptor DuplicateFromAlbum(Album al)
        {
            return BuildForExport(al, false, false);
        }

        static internal AlbumDescriptor BuildForExport(Album al, bool Copyall, bool CopyImage)
        {
            var res = new AlbumDescriptor();

            res.Artist = al.Author;
            res.Genre = al.Genre;
            res.RawIDs = al.CDIDs;
            res.TracksNumber = al.Interface.TracksNumber;
            res.Name = al.Name;
            res.Year = al.Interface.Year;
            res.DateAdded = al.Interface.DateAdded;

            if (Copyall)
            {
                res.RawTrackDescriptors = (from tr in al.RawTracks select new TrackDescriptor(res, tr)).ToList();

                if (CopyImage)
                    res.RawImages = (from im in al.ModifiableImages select new AImage(BufferFactory.GetBufferProviderFromFile(im.GetPath()), im.Rank)).ToList();

                res.Thumbnail = al.ImageCachePath;
            }

            return res;
        }

        internal TrackDescriptor AddExportedTrack(MusicCollection.Implementation.Track track, string path)
        {
            if (track == null)
                return null;

            if ((RawTrackDescriptors != null) && (from t in RawTrackDescriptors select t.Path).Contains(track.Path))
                return null;

            if (RawTrackDescriptors == null)
                RawTrackDescriptors = new List<TrackDescriptor>();

            TrackDescriptor td = new TrackDescriptor(this, track, path);

            RawTrackDescriptors.Add(td);
            return td;
        }

        internal void AddImage(int iID, string iPath)
        {
            if (RawImages == null)
                RawImages = new List<AImage>();

            RawImages.Add(new AImage(BufferFactory.GetBufferProviderFromFile(iPath), iID));
        }

        #endregion

        #region Duplicators

        private AlbumDescriptor Clone(bool includtn = true)
        {
            AlbumDescriptor ad = BasicClone();
            ad.Artist = Artist;
            ad.Name = Name;
            ad.Year = Year;
            ad.Genre = Genre;
            ad.RawIDs = IDs;
            ad.TracksNumber = (includtn) ? TracksNumber : 0;

            if (RawImages != null)
            {
                ad.RawImages = RawImages.Select(ri => ri.Clone()).ToList();
            }

            return ad;
        }

        protected virtual AlbumDescriptor BasicClone()
        {
            return new AlbumDescriptor();
        }

        public IFullEditableAlbumDescriptor GetEditable()
        {
            AlbumDescriptor ad = Clone();
            if (RawTrackDescriptors != null)
            {
                ad.RawTrackDescriptors = RawTrackDescriptors.Select(tr => tr.Clone(ad)).ToList();
            }

            return ad;
        }

        public IEnumerable<IFullAlbumDescriptor> SplitOnDiscNumber()
        {
            ILookup<uint, TrackDescriptor> mysep = RawTrackDescriptors.ToLookup(tr => tr.DiscNumber);
            if (mysep.Count <= 1)
            {
                yield return this;
                yield break;
            }

            foreach (IGrouping<uint, TrackDescriptor> group in mysep)
            {
                AlbumDescriptor ad = Clone(false);
                ad.RawTrackDescriptors = group.Select(tr => tr.Clone(ad)).ToList();
                yield return ad;
            }

        }

        #endregion

        #region methods

        public virtual bool HasImage()
        {
            return ((RawImages != null) && (RawImages.Count > 0));
        }

        public bool MatchTrackNumberOnDisk(int TN)
        {
            if (RawTrackDescriptors.Count == 0)
                return TN == 0;

            return RawTrackDescriptors.ToLookup(tr => tr.DiscNumber).Where(g => g.Count() == TN).Any();
        }

        internal TrackDescriptor CreateChild()
        {
            TrackDescriptor res = new TrackDescriptor(this);
            RawTrackDescriptors.Add(res);
            return res;
        }

        internal void Mature(DataExportImportType dit)
        {
            if (DiscNumber > 0)
            {
                RawTrackDescriptors.Apply(t => t.DiscNumber = DiscNumber);
            }

            if (dit != DataExportImportType.FreeDB)
                return;

            RawTrackDescriptors.Apply(t => t.TrackNumber++);

            if (this.TracksNumber == 0)
            {
                this.TracksNumber = (uint)RawTrackDescriptors.Count;
            }
        }


        internal void MergeIDsFromCDInfos(ICDInfoHandler dif)
        {
            IDiscIDs dicsids = dif.IDs;

            if (dicsids != null)
            {
                if (CDDB == null) CDDB = dicsids.CDDB;
                if (Asin == null) Asin = dicsids.Asin;
                if (MusicBrainzID == null) MusicBrainzID = dicsids.MusicBrainzID;
                if (MusicBrainzCDId == null) MusicBrainzCDId = dicsids.MusicBrainzCDId;
            }
        }

        public void InjectImages(IAlbumDescriptor iad, bool AllowMultiInject)
        {
            var ad = iad as AlbumDescriptor;
            if (ad == null)
                return;

            if (iad == this)
                return;

            if (RawImages != null && !AllowMultiInject)
            {
                RawImages.Clear();
                RawImages = null;
            }

            if (ad.RawImages != null)
            {
                if (RawImages == null)
                    RawImages = new List<AImage>();
                RawImages.AddRange(ad.RawImages.Select(im => im.Clone()));
            }

        }

        internal double GetCueMinLengthInseconds()
        {
            if (this.RawTrackDescriptors.Count == 0)
                return -1;

            TimeCueIndexer last = this.RawTrackDescriptors.OrderBy(t => t.TrackNumber).Last().CueIndex01;
            return (last == null) ? -1 : (double)last.TotalSeconds;
        }

        public bool CheckCueConsistency()
        {
            var tracks = this.RawTrackDescriptors.OrderBy(t => t.TrackNumber);

            TimeCueIndexer curr = null;

            foreach (TrackDescriptor t in tracks)
            {
                if (t.CueIndex01 == null)
                    return false;

                if (curr != null)
                {
                    TimeCueIndexer first = t.CueIndex00 ?? t.CueIndex01;
                    if (curr >= first)
                        return false;

                    curr = t.CueIndex01;
                }

                if (t.CueIndex00 != null)
                {
                    if (t.CueIndex00 >= t.CueIndex01)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Import/Export XML methods

        private IEnumerable<IFileRelatedEntity> FREs
        {
            get
            {
                return this.RawTrackDescriptors.Concat<IFileRelatedEntity>(RawImages).Concat(_Thumbnail.SingleItemCollection<IFileRelatedEntity>());
            }
        }

        internal void AfterImport(bool reconnect, string iPath, string original)
        {
            if (string.IsNullOrEmpty(original))
                return;

            if (original[original.Length - 1] != '\\')
                original += @"\";

            bool KO = true;
            foreach (IFileRelatedEntity et in FREs)
            {
                et.Album = this;
                if (reconnect && KO && File.Exists(et.Path))
                    KO = false;
            }

            if ((!KO) || (!reconnect))
                return;


            string Direc = iPath + @"\";
            if (FREs.Any(et => File.Exists(et.Reconnect(Direc, original))))
            //if (FREs.Any(et => File.Exists(et.Reconnect(iPath, original))))
            {
                FREs.Apply(et => et.UpdatePath(iPath, original));
            }
        }

        internal void Import(IImportContext Context)
        {
            RawTrackDescriptors.Apply(tr => tr.Import(Context));
        }

        #endregion

        #region Property


        private FileRelatedEntity _Thumbnail;
        public string Thumbnail
        {
            get { return _Thumbnail == null ? null : _Thumbnail.Path; }
            set { if (_Thumbnail == null)  _Thumbnail = new FileRelatedEntity(value); }
        }

        private List<TrackDescriptor> _TrackDescriptors;
        [XmlElement("Track")]
        public List<TrackDescriptor> RawTrackDescriptors
        {
            get { if (_TrackDescriptors == null) _TrackDescriptors = new List<TrackDescriptor>(); return _TrackDescriptors; }
            set { _TrackDescriptors = value; }
        }

        [XmlIgnore]
        public List<ITrackMetaDataDescriptor> TrackDescriptors
        {
            get { return RawTrackDescriptors.ToList<ITrackMetaDataDescriptor>(); }
        }

        [XmlIgnore]
        public List<IEditableTrackDescriptor> EditableTrackDescriptors
        {
            get { return RawTrackDescriptors.ToList<IEditableTrackDescriptor>(); }
        }

        private string _Artist;
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "DTITLE0")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "PERFORMER")]
        public string Artist
        {
            get { return _Artist; }
            set { Set(ref _Artist, value); }
        }


        public DateTime DateAdded
        {
            get;
            set;
        }

        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "FILE")]
        public string CUEFile
        {
            get;
            set;
        }

        private string _Genre;
        //private string _Genre;
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "DGENRE")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "GENRE")]
        public string Genre
        {
            get { return _Genre; }
            set { this.Set(ref _Genre, value); }
        }

        [XmlIgnore]
        public IDiscIDs IDs
        {
            get { return RawIDs; }
        }

        [XmlIgnore]
        public string CUESheetFileName
        {
            private set;
            get;
        }

        private IDiscIDs RawIDs
        {
            set
            {
                CDDB = (value == null) ? null : value.CDDB;
                Asin = (value == null) ? null : value.Asin;
                MusicBrainzID = (value == null) ? null : value.MusicBrainzID;
                MusicBrainzCDId = (value == null) ? null : value.MusicBrainzCDId;
            }
            get { return DiscIDs.FromIDsAndHashes(Asin, MusicBrainzID, CDDB, MusicBrainzCDId); }
        }

        private uint _TracksNumber;
        public uint TracksNumber
        {
            get { return _TracksNumber; }
            set { Set(ref _TracksNumber, value); }
        }

        private string _Name;
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "DTITLE1")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "TITLE")]
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value); }
        }


        private int _Year;
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "DYEAR")]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "DATE")]
        public int Year
        {
            get { return _Year; }
            set { Set(ref _Year, value); }
        }

        private uint _DiscNumber;
        [XmlIgnore]
        [MusicObjectAttributeMapping(DataExportImportType.CUE, "DISCNUMBER")]
        public uint DiscNumber
        {
            get { return _DiscNumber; }
            set { Set(ref _DiscNumber, value); }
        }

        [XmlIgnore]
        public IList<IIMageInfo> Images
        {
            get { return (RawImages == null) ? null : RawImages.Select<ICompleteIMageInfo, IIMageInfo>(t => t).ToList(); }
        }

        [XmlElement("Images")]
        public List<AImage> RawImages
        {
            get;
            set;
        }

        [MusicObjectAttributeMapping(DataExportImportType.CUE, "DISCID")]
        [MusicObjectAttributeMapping(DataExportImportType.FreeDB, "DISCID")]
        public string CDDB
        {
            get;
            set;
        }

        public string Asin
        {
            get;
            set;
        }

        public string MusicBrainzCDId
        {
            get;
            set;
        }

        public string MusicBrainzID
        {
            get;
            set;
        }

        #endregion
    }
}
