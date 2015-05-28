using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Diagnostics;

using TagLib;
using NHibernate;

using MusicCollection.Fundation;
using MusicCollection.FileImporter;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Implementation.Modifier;
using MusicCollection.Utilies;
using MusicCollection.ToolBox.Event;
using System.Threading.Tasks;

namespace MusicCollection.Implementation
{
    internal class AlbumModifier : NotifyCompleteListenerObject, IModifiableAlbum, IInternalAlbumModifier
    {
        #region Fields

        private Album _AM;
        private string _Name = null;
        private string _Genre = null;
        private int? _Year = null;
        private IIsolatedMofiableList<IAlbumPicture> _AlbumImages = null;
        private IIsolatedMofiableList<IModifiableTrack> _Tracks = null;
        private IIsolatedMofiableList<IArtist> _Artists = null;

        private IPicture[] _PictureTobeStored = null;
        private bool _Dirty = false;
        private bool _ImageDirty = false;
        private bool _TrackDirty = false;
        private bool _AuthorDirty = false;
        private IImportContext _IT;
        private bool _NeedToUpdateFile = false;

        private List<IArtist> _OldArtist;

        public string NewName { get { return _Name; } }
        public string NewGenre { get { return _Genre; } }
        public int? NewYear { get { return _Year; } }
        public bool IsImageDirty { get { return _ImageDirty; } }

        private bool _UnderTrans = false;

        #endregion

        public IImportContext Context { get { return _IT; } }

        public IMusicSession Session { get { return _AM.Session; } }

        public bool NeedToUpdateFile{ get { return _Dirty; } }
      
     
        private void UpdateDirtyStatus()
        {
            _NeedToUpdateFile = (_Year != null) || (_Artists.Changed) || (_Genre != null) || (_Name != null) || (_ImageDirty) || (_AuthorDirty);
            _Dirty = _NeedToUpdateFile || _TrackDirty;
        }

        public bool IsAlbumOnlyModified {  get { return _NeedToUpdateFile; } }

        internal bool SomeThingChanged
        {
            get
            {
                if (_Dirty)
                    return true;

                if (_Tracks == null)
                    return false;

                return _Tracks.MofifiableCollection.Any(t=>t.IsModified);
            }
        }

        #region ImagesManagement

        private void OnImagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _ImageDirty = true;
            UpdateDirtyStatus();
        }

        private IAlbumPicture AddImage(AlbumImage iap, int Index)
        {
            if ((iap == null) || (iap.IsBroken))
                return null;

            _AlbumImages.MofifiableCollection.Insert(Index, iap);
            return iap;
        }

        public IAlbumPicture AddAlbumPicture(string FileName, int Index)
        {
            return AddImage(AlbumImage.GetFromFile(_AM, FileName, Name), Index);
        }

        public IAlbumPicture AddAlbumPicture(BitmapSource BMS, int Index)
        {
            return AddImage(AlbumImage.GetAlbumPictureFromBitmapSource(_AM, BMS), Index);
        }

        public IAlbumPicture GetAlbumPictureFromUri(string uri, int Index, IHttpContextFurnisher Context)
        {
            return AddImage(AlbumImage.GetAlbumPictureFromUri(_AM, uri, Context), Index);
        }

        public IAlbumPicture RotateImage(int Index, bool angle)
        {
            IAlbumPicture old = Images[Index];
            IAlbumPicture newpic = old.CloneRotate(angle);

            if (newpic != null)
            {
                Images.Insert(Index, newpic);
                Images.RemoveAt(Index + 1);

                return newpic;
            }

            return old;
        }

        public IDiscIDs CDIDs
        {
            get { return _AM.CDIDs; }
        }

        public IEnumerable<IAlbumPicture> SplitImage(int Index)
        {
            IAlbumPicture ToBesplit = Images[Index];
            int initind = Index;

            Images.RemoveAt(Index);

            IEnumerable<IAlbumPicture> res = null;

            try
            {
                res = ToBesplit.Split().ToList();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem splitting image " + e.ToString());
                Images.Insert(initind, ToBesplit);
            }

            if (res==null)
            {
                yield return ToBesplit;
            }
            else
            {
                foreach (AlbumImage bt in res)
                {
                    yield return  AddImage(bt, Index++);
                }
            }
        }
        #endregion

        internal AlbumModifier(Album iAM, bool resetCorruptedImage, IImportContext IT)
        {
            _AM = iAM;
            _IT = IT;
   
            _AlbumImages = iAM.RawImages;
            _AlbumImages.MofifiableCollection.CollectionChanged += OnImagesChanged;

            _Artists = iAM.GetArtistModifier();
            _Artists.MofifiableCollection.CollectionChanged += MofifiableCollection_CollectionChanged;
            _Artists.OnCommit += new EventHandler<EventArgs>(_Artists_OnCommit);

            _OldArtist = _Artists.MofifiableCollection.ToList();

            if (resetCorruptedImage)
            {
                Stack<int> IndexToRemove = new Stack<int>();
                for (int i=0;i<_AlbumImages.MofifiableCollection.Count;i++)
                {
                    if (_AlbumImages.MofifiableCollection[i] == null)
                        IndexToRemove.Push(i);
                }

                while (IndexToRemove.Count > 0)
                {
                    _AlbumImages.MofifiableCollection.RemoveAt(IndexToRemove.Pop());
                }

                foreach (IAlbumPicture pic in (from im in _AlbumImages.MofifiableCollection where im.IsBroken select im).ToList())
                {
                    Images.Remove(pic);
                }
            }
        }

        #region Artists

        void _Artists_OnCommit(object sender, EventArgs e)
        {          
            _Artists.MofifiableCollection.Where(ar => ((!_OldArtist.Contains(ar)))).Cast<Artist>().Apply(ar => ar.AddAlbum(this._AM, this._IT));
            _OldArtist.Where(ar => ((!_Artists.MofifiableCollection.Contains(ar)))).Cast<Artist>().Apply(ar => ar.RemoveAlbum(this._AM, this._IT));
        }

        public bool AuthorDirty
        {
            get { return _AuthorDirty; }
        }

        public void ForceAuthorReWrite()
        {
            _AuthorDirty = true;
            _NeedToUpdateFile = true;
        }

        public string ArtistName
        {
            get { return Artist.AuthorName(_Artists.MofifiableCollection); }
        }

        void MofifiableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _AuthorDirty = true;
            _Dirty = true;
        }

        public IList<IArtist> Artists
        {
            get { return _Artists.MofifiableCollection; }
        }

        #endregion

        public string MainDirectory { get { return Path.GetDirectoryName(_AM.RawTracks.First().Path); } }

        private static class NameConstructor
        {
            private readonly static char[] Blank = new char[] { ' ' };
            internal static string Format(string Name)
            {
                return string.Format(@"%22{0}%22", string.Join("+", Name.Split(Blank, StringSplitOptions.RemoveEmptyEntries)));
            }
        }

        public string CreateSearchGoogleSearchString()
        {
            return string.Join("+%2B+", (from a in _Artists.MofifiableCollection select NameConstructor.Format(a.Name)).Union(NameConstructor.Format(Name).SingleItemCollection()));
        }

        public string Name
        {
            get
            {
                if (_Name != null) return _Name;

                return _AM.Interface.Name;
            }
            set
            {
                if (Set(ref _Name, value))
                    UpdateDirtyStatus();
            }
        }

        public string Genre
        {
            get
            {
                if (_Genre != null) return _Genre;

                return _AM.Genre;
            }
            set
            {
                if (Set(ref _Genre, value))
                    UpdateDirtyStatus();
            }
        }


        public int Year
        {
            get
            {
                if (_Year != null) return (int)_Year;

                return _AM.Interface.Year;
            }
            set
            {
                if (Set(ref _Year, value))
                    UpdateDirtyStatus();
            }
        }

        public ObservableCollection<IModifiableTrack> Tracks
        {
            get
            {
                if (_Tracks == null)
                {
                    _Tracks = _AM.GetTrackModifier(this);
                    _Tracks.MofifiableCollection.CollectionChanged += OnCollectionChanged;
                }

                return _Tracks.MofifiableCollection;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs ipchea)
        {
            _TrackDirty = true;
            UpdateDirtyStatus();
        }

        public ObservableCollection<IAlbumPicture> Images
        {
            get { return _AlbumImages.MofifiableCollection; }
        }

        public IAlbumPicture FrontImage
        {
            get {  return (Images.Count == 0) ? null : Images[0]; }
        }

        public IPicture[] PictureTobeStored
        {
            get { return _PictureTobeStored;}
            private set { _PictureTobeStored = value;}
        }

        public void Remove(TrackModifier tm)
        {
            _Tracks.MofifiableCollection.Remove(tm);
        }

        private bool RemoveFileIfNeccessary(IEnumerable<Track> FileToRemove, IProgress<ImportExportError> progress)
        {
            bool? delete = _IT.DeleteManager.DeleteFileOnDeleteAlbum;

            if (delete == false)
                return false;

            FileCleaner proprifier = Context.Folders.GetFileCleanerFromTracks(FileToRemove, _IT.Folders.IsFileRemovable, null, true);

            if (delete == null)
            {
                DeleteAssociatedFiles deaf = new DeleteAssociatedFiles(proprifier.Paths);
                progress.SafeReport(deaf);
                delete = (deaf.Continue == true);
            }

            if (delete == true)
                proprifier.Remove();

            return (delete == true);
        }

        private static bool AlbumMerge(Album Changing, AlbumModifier AM, IImportContext IT, IProgress<ImportExportError> progress)
        {
            try
            {
                AM._UnderTrans = true;

                if (!Changing.Images.SequenceEqual(AM._AM.Images, (im1, im2) => im1.CompareContent(im2)))
                {
                    int i = 0;
                    foreach (AlbumImage im in Changing.Images)
                    {
                        AM.Images.Insert(i++, im.Clone(AM._AM));
                    }
                }

                //forceons l'ecriture des infos albums pour les tracks migres
                AM.Name = AM.Name;
                AM.ForceAuthorReWrite();

                if (AM._AM.Genre == null)
                    AM.Genre = Changing.Genre;

                if (AM._AM.Interface.Year == 0)
                    AM.Year = Changing.Interface.Year;

                IList<Track> tracks = Changing.RawTracks.ToList();

                using (IMusicTransaction IMut = IT.CreateTransaction())
                {
                    foreach (Track tr in tracks)
                    {
                        AM.Tracks.Add(new TrackModifier(tr.CloneTo(IT, AM._AM), AM));
                    }

                    AM.TrivialCommit(IMut, progress);

                    IT.AddForRemove(Changing);

                    IMut.Commit();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Problem during Album Merge: " + e.ToString());
            }

            AM._UnderTrans = false;
            AM._AM.Context = null;

            return true;
        }

        public void ReinitImages()
        {
            if (Images.Count != 0)
                return;

            int i=0;
            foreach(AlbumImage ai in _AM.ImagesFromFile)
            {
                AddImage(ai,i++);
            }
        }


        private bool PrivateCommit(IProgress<ImportExportError> progress)
        {
            _UnderTrans = true;
            if (!SomeThingChanged)
            {
                _UnderTrans = false;
                _AM.Context = null;
                Dispose();
                return true;
            }

            bool res = false;

            Album toremove = OtherAlbumSameName;
            if (toremove != null)
            {
                AlbumModifier AM = toremove.GetModifiableAlbum(false, _IT);

                if (AM == null)
                {
                    _UnderTrans = false;
                    _AM.Context = null;
                    Dispose();
                    return false;
                }

                using (AM)
                {
                    OtherAlbumConfirmationNeededEventArgs Err = new OtherAlbumConfirmationNeededEventArgs(toremove);
                    progress.SafeReport(Err);
                    //OnError(Err);
                    if (!Err.Continue)
                    {
                        res = false;
                    }
                    else
                    {
                        //this.Author = null;
                        this.Name = null;
                        this._Artists.CancelChanges();

                        res = (SomeThingChanged) ? PrivateSimpleCommit(progress) : true;

                        if (res)
                        {
                            res = AlbumMerge(_AM, AM, _IT, progress);
                        }
                    }
                }
            }
            else res = PrivateSimpleCommit(progress);

            _UnderTrans = false;
            _AM.Context = null;
            Dispose();
            return res;
        }

        private bool TrivialCommit(IMusicTransaction IMut,IProgress<ImportExportError> progress)
        {
            bool res = true;

            if (Tracks.Count == 0)
            {
                IMut.ImportContext.AddForRemove(_AM);
                RemoveFileIfNeccessary(_AM.RawTracks, progress);
            }
            else
            {
                IMut.ImportContext.AddForUpdate(_AM);

                try
                {
                    List<TrackModifier> tmstr = new List<TrackModifier>();

                    _Tracks.OnBeforeChangedCommited += ((o, e) =>
                    {
                        if (e.What == NotifyCollectionChangedAction.Remove)
                            tmstr.Add(e.Who as TrackModifier);
                    });

                    _Tracks.CommitChanges();

                    if (tmstr.Count > 0)
                    {
                        var FileToRemove = from t in tmstr select t.Track;

                        foreach (Track tm in FileToRemove)
                        {
                            _IT.AddForRemove(tm);
                        }

                        RemoveFileIfNeccessary(FileToRemove, progress);
                    }

                    using (_AM.MusicSession.Albums.GetRenamerTransaction(_AM))
                    {
                        if (_AuthorDirty)
                        {
                            _Artists.CommitChanges();
                        }

                        if (NewName != null)
                            _AM.Name = NewName;
                    }

                    if (IsImageDirty)
                    {
                        _AlbumImages.CommitChanges();
                        int Count = _AM.Images.Count;
                        PictureTobeStored = (from imama in (from im in _AM.Images where (_IT.ImageManager.IsImageRankOKToEmbed(im.Rank, Count)) select im.ConvertToIPicture()) where imama != null select imama).ToArray();
                    }

                    bool Modifyres = (_AM.Modify(this));

                    if (Modifyres == false)
                    {
                        PictureTobeStored = null;
                        progress.SafeReport(new UnknownNameChangedEventArgs(_AM.ToString()));
                        //OnError(new UnknownNameChangedEventArgs(_AM.ToString()));
                        return false;
                    }
                }
                catch (Exception e)
                {
                    PictureTobeStored = null;
                    Trace.WriteLine(e);
                    progress.SafeReport(new UnknownNameChangedEventArgs(_AM.ToString()));
                    //OnError(new UnknownNameChangedEventArgs(_AM.ToString()));
                    return false;
                }

                PictureTobeStored = null;
            }

            return res;
        }


        private bool PrivateSimpleCommit(IProgress<ImportExportError> progress)
        {
            bool res = true;

            using (IMusicTransaction IMut = _IT.CreateTransaction())
            {
                res = TrivialCommit(IMut, progress);
                if (res)
                    IMut.Commit();
                else
                    IMut.Cancel();
            }

            return res;
        }

        public IAlbum OriginalAlbum { get { return _AM; } }

        public bool Commit(IProgress<ImportExportError> progress = null)
        {
            _AlbumImages.MofifiableCollection.CollectionChanged -= OnImagesChanged;
            return PrivateCommit(progress);
        }

        public Task<bool> CommitAsync(IProgress<ImportExportError> progress)
        {
            _AlbumImages.MofifiableCollection.CollectionChanged -= OnImagesChanged;
            _UnderTrans = true;

            return Task.Run(() => PrivateCommit(progress));
        }

        public void CancelChanges()
        {
            Dispose();
        }

        private Album OtherAlbumSameName
        {
            get
            {
                if ((_Name == null) && (_AuthorDirty == false))
                    return null;

                Album Al = Context.FindByName(Name, Artist.AuthorName(this._Artists.MofifiableCollection));
                return ((Al != null) && (Al != _AM)) ? Al : null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!_UnderTrans)
            {
                _AM.ResetChanges(this);
            }

            _AlbumImages.MofifiableCollection.CollectionChanged -= OnImagesChanged;

            if (_Tracks!=null)
                _Tracks.MofifiableCollection.CollectionChanged -= OnCollectionChanged;
        }

        public async Task MergeFromMetaDataAsync(IFullAlbumDescriptor iad, IMergeStrategy Strategy)
        {
            await Task.Run( () => MergeFromMetaData(iad, Strategy));
        }

        public void MergeFromMetaData(IFullAlbumDescriptor iad, IMergeStrategy Strategy)
        {
            if (Strategy.AlbumMetaData != IndividualMergeStategy.Never)
            {
                if ((Year == 0) || Strategy.AlbumMetaData == IndividualMergeStategy.Always)
                    Year = iad.Year;

                if (string.IsNullOrEmpty(Name) || Strategy.AlbumMetaData == IndividualMergeStategy.Always)
                    Name = iad.Name;

                if ((_Artists.MofifiableCollection.Count == 0) || (Strategy.AlbumMetaData == IndividualMergeStategy.Always))
                {
                    _Artists.MofifiableCollection.Clear();
                    _Artists.MofifiableCollection.AddCollection(this._IT.GetArtistFromName(iad.Artist));
                }

                if ((string.IsNullOrEmpty(Genre)) || (Strategy.AlbumMetaData == IndividualMergeStategy.Always))
                    Genre = iad.Genre;
            }

            if ((Strategy.InjectAlbumImage == IndividualMergeStategy.Always) || ((Strategy.InjectAlbumImage == IndividualMergeStategy.IfDummyValue) && Images.Count == 0))
            {
                int i = 0;
                if (iad.Images != null)
                {
                    foreach (IIMageInfo Im in iad.Images)
                    {
                        if (AddImage(AlbumImage.GetFromBuffer(_AM, Im.ImageBuffer), i) != null)
                            i++;
                    }
                }
            }

            if (Strategy.TrackMetaData == IndividualMergeStategy.Never)
                return;

            //var OrderedTracksInput = (from r in iad.TrackDescriptors orderby r.Duration.TotalMilliseconds ascending select r).ToList();
            //var OrderedTracks = (from r in Tracks orderby r.Duration.TotalMilliseconds ascending select r).ToList();

            var OrderedTracksInput = iad.TrackDescriptors.OrderBy(r => r.DiscNumber).ThenBy(r=>r.TrackNumber).ToList();
            var OrderedTracks = Tracks.OrderBy(r => r.DiscNumber).ThenBy(r=>r.TrackNumber).ToList();

            if (OrderedTracksInput.Count != OrderedTracks.Count)
                return;

            int j = 0;

            foreach (IModifiableTrack INT in OrderedTracks)
            {
                ITrackMetaDataDescriptor Tr = OrderedTracksInput[j++];

                if ((Strategy.TrackMetaData == IndividualMergeStategy.Always) || (new StringTrackParser(INT.Name, false).IsDummy))
                    INT.Name = Tr.Name;

                if ((Strategy.TrackMetaData == IndividualMergeStategy.Always) || string.IsNullOrEmpty(INT.Artist))
                    INT.Artist = Tr.Artist;

                if ((Strategy.TrackMetaData == IndividualMergeStategy.Always) || (INT.TrackNumber == 0))
                    INT.TrackNumber = Tr.TrackNumber;

                TimeSpan delta = (Tr.Duration - INT.Duration);
                Trace.WriteLine(delta);
            }
        }

        public IAlbumDescriptor GetAlbumDescriptor()
        {
            return AlbumDescriptor.SimpleAlbumDescriptorFromAlbumModifier(this);
        }
    }
}
