﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MusicCollection.Fundation;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollection.Infra;
using System.Windows;
using System.Collections.Specialized;
using System.Diagnostics;
using MusicCollection.Utilies;

namespace MusicCollectionWPF.ViewModel
{
    public class AlbumEditorViewModel : ViewModelBase, IInformationEditor
    {
        private IModifiableAlbum _IModifiableAlbum;
        private IMusicSession _Session;
        public AlbumEditorViewModel(IMusicSession iMusicSession, IModifiableAlbum iModifiableAlbum)
        {
            _IModifiableAlbum = iModifiableAlbum;
            _Session = iMusicSession;

            ArtistSearchableFactory = new ArtistSearchableFactory(iMusicSession);

            GenreFactory = FactoryBuilder.Instanciate((n) => iMusicSession.GetGenreFactory().Create(n));

            Images = _IModifiableAlbum.Images;
            SelectedImages = new WrappedObservableCollection<IAlbumPicture>();
            SelectedTracks = new WrappedObservableCollection<IModifiableTrack>();

            if (Images.Count > 0)
                SelectedImage = Images[0];

            _Name = _IModifiableAlbum.Name;
            Authours = _IModifiableAlbum.Artists;
            _Year = _IModifiableAlbum.Year;
            _Genre = iMusicSession.GetGenreFactory().Get(_IModifiableAlbum.Genre);
            Genres = Register(iMusicSession.AllGenres.LiveOrderBy(global => global.FullName));

            Tracks = new WrappedObservableCollection<IModifiableTrack>(_IModifiableAlbum.Tracks.
                OrderBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber).ThenBy(t => t.Name));

            SetFrontCover = Register(RelayCommand.Instanciate<IAlbumPicture>(SetToFront, ial => (ial != null) && Images.IndexOf(ial) > 0));
            ToLast = Register(RelayCommand.Instanciate<IAlbumPicture>(SetToLast, ial => (ial != null) && Images.IndexOf(ial) != Images.Count - 1));

            SplitImage = Register(RelayCommand.Instanciate<IAlbumPicture>(DoSplitImage, ial => ial != null));
            RotateImageRight = Register(RelayCommand.Instanciate<IAlbumPicture>((al)=>DoRotateImage(al,true), ial => ial != null));
            RotateImageLeft = Register(RelayCommand.Instanciate<IAlbumPicture>((al) => DoRotateImage(al, false), ial => ial != null));
            DeleteImage = Register(RelayCommand.Instanciate<IAlbumPicture>(DoDeleteImage, ial => ial != null));
            ImageFromFile = RelayCommand.Instanciate(DoImageFromFile);
            PasteImage = RelayCommand.InstanciateStatic(DoPasteImage, CanExecuteImage);

            DeleteTrack = Register(RelayCommand.Instanciate<IModifiableTrack>(DoDeleteTrack, ial => ial != null));
            WindowOpenTrack = Register(RelayCommand.Instanciate<IModifiableTrack>(DoWindowOpenTrack, ial => ial != null));
            UpdateFromFileName = Register(RelayCommand.Instanciate<IModifiableTrack>(DoUpdateFromFileName, ial => ial != null));
            RemoveTrackNumber= Register(RelayCommand.Instanciate<IModifiableTrack>(DoRemoveTrackNumber, ial => ial != null));
            PreFixByArtistName = Register(RelayCommand.Instanciate<IModifiableTrack>(DoPreFixByArtistName, ial => ial != null));

            FindFromDB = RelayCommand.Instanciate(DoFindFromInternet);
            BrowseInternet = RelayCommand.Instanciate(FindOnInternet);
            OK = RelayCommand.Instanciate(DoCommit);
        }

        private void DoCommit()
        {
            _Continue = true;
            _IModifiableAlbum.Name = _Name;
            _IModifiableAlbum.Year = _Year;
            if (_Genre!=null) _IModifiableAlbum.Genre = _Genre.FullName;
            Window.Close();
        }

        private bool _Continue = false;
        public IAsyncCommiter GetCommiter()
        {
            if  (_Continue) 
                return this._IModifiableAlbum;

            _IModifiableAlbum.CancelChanges();
            return null;
        }

        #region Tracks
        private IEnumerable<IModifiableTrack> GetTracks(IModifiableTrack icontext)
        {
            if (icontext == null)
                return Enumerable.Empty<IModifiableTrack>();

            return SelectedTracks.Contains(icontext) ? SelectedTracks.ToList() : icontext.SingleItemCollection();
        }

        private void DoDeleteTrack(IModifiableTrack iModifiableTrack)
        {
            foreach (var imt in GetTracks(iModifiableTrack))
            {
                imt.Delete();
                Tracks.Remove(imt);
            }
        }

        private void DoRemoveTrackNumber(IModifiableTrack iModifiableTrack)
        {
            foreach (IModifiableTrack mt in GetTracks(iModifiableTrack))
            {
                StringTrackParser stp = new StringTrackParser(mt.Name, false);
                if (stp.FounSomething)
                {
                    mt.Name = stp.TrackName;
                    if ((mt.TrackNumber == 0) && (stp.TrackNumber != null))
                        mt.TrackNumber = (uint)stp.TrackNumber;
                }
            }
        }

        private void DoUpdateFromFileName(IModifiableTrack iModifiableTrack)
        {
            GetTracks(iModifiableTrack).Apply(mt=> mt.Name = System.IO.Path.GetFileNameWithoutExtension(mt.Path));
        }

        private void DoPreFixByArtistName(IModifiableTrack iModifiableTrack)
        {
            GetTracks(iModifiableTrack).Apply(
                imt => imt.Name = string.Format("{0}-{1}", imt.Father.Artists.GetDisplayName(), imt.Name));
        }

        private void DoWindowOpenTrack(IModifiableTrack iModifiableTrack)
        {
            FileServices.OpenExplorerWithSelectedFiles(GetTracks(iModifiableTrack)
                .Where(imt => imt.State != ObjectState.Removed).Select(tr => tr.Path).ToList());
        }
        #endregion

        #region Images

        private void DoImageFromFile()
        {
            var results = this.Window.ChooseFiles("Select Image",
                               "All Image Files | " + FileServices.GetImagesFilesSelectString(),_IModifiableAlbum.MainDirectory);

            int index = (SelectedImage == null) ? -1 : _IModifiableAlbum.Images.IndexOf(SelectedImage);

            var mynew = results.Select(filename => _IModifiableAlbum.AddAlbumPicture(filename, ++index)).ToList().FirstOrDefault();

            if (mynew != null)
                SelectedImage = mynew;
        }

        private IEnumerable<IAlbumPicture> GetImages(IAlbumPicture icontext)
        {
            if (icontext == null)
                return Enumerable.Empty<IAlbumPicture>();

            return SelectedImages.Contains(icontext) ? SelectedImages.ToList() : icontext.SingleItemCollection();
        }

        private void DoSplitImage(IAlbumPicture ifal)
        {
            foreach(IAlbumPicture ial in GetImages(ifal))
            { 
                int Index = Images.IndexOf(ial);
                if (Index == -1) return;
                //SelectedImage = _IModifiableAlbum.SplitImage(Index);
                SelectedImages.AddCollection(_IModifiableAlbum.SplitImage(Index));
            }
        }

        private void DoRotateImage(IAlbumPicture ifal,bool irigth)
        {
            foreach (IAlbumPicture ial in GetImages(ifal))
            {
                int Index = Images.IndexOf(ial);
                if (Index == -1) return;
                SelectedImages.Add(_IModifiableAlbum.RotateImage(Index, irigth));
            }
        }

        private void DoDeleteImage(IAlbumPicture ifal)
        {
            foreach (IAlbumPicture ial in GetImages(ifal))
            {
                int Index = Images.IndexOf(ial);
                if (Index == -1) return;

                _IModifiableAlbum.Images.RemoveAt(Index);

                int count = _IModifiableAlbum.Images.Count;
                if (count > 0)
                {
                    int imaindex = Math.Min(Math.Max(Index--, 0), count - 1);
                    SelectedImage = _IModifiableAlbum.Images[imaindex];
                }
            }
        }

        private void SetToFront(IAlbumPicture ial)
        {
            if (ial == null)
                return;

            int Index = Images.IndexOf(ial);
            if (Index == -1)
                return;

            Images.Move(Index, 0);
        }

        private void SetToLast(IAlbumPicture ial)
        {
            if (ial == null)
                return;

            int Index = Images.IndexOf(ial);
            if (Index == -1)
                return;

            Images.Move(Index, Images.Count - 1);
        }

        private void DoPasteImage()
        {
            int c = (SelectedImage == null) ? 0 : _IModifiableAlbum.Images.IndexOf(SelectedImage);
            IAlbumPicture newapic = null;

            try
            {
                if (Clipboard.ContainsImage())
                {
                    newapic = _IModifiableAlbum.AddAlbumPicture(Clipboard.GetImage(), c);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    int Index = c;
                    Clipboard.GetFileDropList().Cast<string>().Where(f => FileServices.GetFileType(f) == FileType.Image)
                            .Apply(sn => _IModifiableAlbum.AddAlbumPicture(sn, Index++));
                    newapic = _IModifiableAlbum.Images[c];
                }

                SelectedImage = newapic;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem using clipboard: Memory issue?:" + ex.ToString());
            }
        }

        private bool CanExecuteImage()
        {
            if (Clipboard.ContainsImage())
                return true;

            if (!Clipboard.ContainsFileDropList())
                return false;

            try
            {
                StringCollection res = Clipboard.GetFileDropList();
                if (res == null)
                    return false;

                return res.Cast<string>().Any(f => FileServices.GetFileType(f) == FileType.Image);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem using clipboard:" + ex.ToString());
            }

            return false;
        }

        #endregion

        private void DoFindFromInternet()
        {
            WebAlbumSelectorViewModel wasvm = new WebAlbumSelectorViewModel(this._Session, this._IModifiableAlbum);
            Window.CreateFromViewModel(wasvm).ShowDialog();
        }

        private void FindOnInternet()
        {
            InternetFinderViewModel wasvm = new InternetFinderViewModel(this._IModifiableAlbum);
            Window.CreateFromViewModel(wasvm).Show();
        }

        //DiscNumber

        public IList<IModifiableTrack> Tracks { get; private set; }

        public IList<IGenre> Genres { get; private set; }


        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.Set(ref _Name, value); }
        }

        private int _Year;
        public int Year
        {
            get { return _Year; }
            set { this.Set(ref _Year, value); }
        }

        private IGenre _Genre;
        public IGenre Genre
        {
            get { return _Genre; }
            set { this.Set(ref _Genre, value); }
        }

        private IAlbumPicture _SelectedImage;
        public IAlbumPicture SelectedImage
        {
            get { return _SelectedImage; }
            set { this.Set(ref _SelectedImage, value); }
        }

        public ObservableCollection<IAlbumPicture> Images { get; private set; }

        public IFactory GenreFactory { get; private set; }

        public ArtistSearchableFactory ArtistSearchableFactory { get; private set; }

        public IList<IArtist> Authours { get; private set; }

        public WrappedObservableCollection<IAlbumPicture> SelectedImages { get; private set; }

        public WrappedObservableCollection<IModifiableTrack> SelectedTracks { get; private set; }

        public bool CanMoveImage { get { return this.Get<AlbumEditorViewModel, bool>(() => (t) => t.Images.Count > 1); } }

        public ICommand FindFromDB { get; private set; }
        public ICommand OK { get; private set; }
        public ICommand BrowseInternet { get; private set; }
        public ICommand SetFrontCover { get; private set; }
        public ICommand ToLast { get; private set; }
        public ICommand ImageFromFile { get; private set; }
        public ICommand SplitImage { get; private set; }
        public ICommand RotateImageRight { get; private set; }
        public ICommand RotateImageLeft { get; private set; }
        public ICommand DeleteImage { get; private set; }
        public ICommand PasteImage { get; private set; }
        public ICommand DeleteTrack { get; private set; }
        public ICommand WindowOpenTrack { get; private set; }
        public ICommand UpdateFromFileName { get; private set; }
        public ICommand RemoveTrackNumber { get; private set; }
        public ICommand PreFixByArtistName { get;private set; }

      
    }

}